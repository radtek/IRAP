﻿using DevExpress.XtraEditors;
using IRAP.Client.Global;
using IRAP.Client.SubSystem;
using IRAP.Client.User;
using IRAP.Entities.MDM;
using IRAP.Global;
using IRAP.WCF.Client.Method;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IRAP.Client.SubSystem
{
    public partial class frmWIShow : Form
    {
        WIScreen _screen = null;

        public frmWIShow()
        {
            InitializeComponent();

            ShowInTaskbar = false;

            if (Screen.AllScreens.Length != 1)
            {

            }
        }
        internal frmWIShow(WIScreen screen) : this()
        {
            _screen = screen;
        }

        private void ShowOnMonitor()
        {
            DesktopBounds = _screen.Bounds;
            WindowState = FormWindowState.Maximized;
        }

        private void frmWIShow_Load(object sender, EventArgs e)
        {
            ShowOnMonitor();
        }

        private void picWI_MouseDown(object sender, MouseEventArgs e)
        {
            picWI.Visible = false;
            lblMessage.Visible = true;
        }

        private void picWI_MouseUp(object sender, MouseEventArgs e)
        {
            lblMessage.Visible = false;
            picWI.Visible = true;
        }
    }

    internal class WIScreen
    {
        public WIScreen()
        {
            Detected = false;
        }

        public int ScreenIndex { get; set; }
        public string DeviceName { get; set; }
        public long SysLogID { get; set; }
        /// <summary>
        ///  看板屏幕是否检测到
        /// </summary>
        public bool Detected { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Show { get; set; }
        public Rectangle Bounds { get; set; }

        public override string ToString()
        {
            return DeviceName;
        }

        public static WIScreen CreateInstance()
        {
            if (Screen.AllScreens.Length <= 1)
            {
                return null;
            }

            WIScreen rlt = new WIScreen()
            {
                ScreenIndex = 1,
                Detected = true,
                Bounds = Screen.AllScreens[1].Bounds,
            };

            return rlt;
        }
    }

    public class WIShow
    {
        private static WIShow _instance = null;
        private frmWIShow showWI = null;
        private Image defaultImage = null;

        public static WIShow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WIShow();
                }
                return _instance;
            }
        }

        private WIShow() { }

        private bool DownloadWIFile(string localPath, WIFile wi)
        {
            try
            {
                //TWaitting.Instance.ShowWaitForm("登录FTP服务器...");
                FTPHelper client = new FTPHelper(wi.Host, wi.UserName, wi.UserPass);

                try
                {
                    if (client.Download($"{wi.RemotePath}{wi.WIFileName}", localPath))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception error)
                {
                    throw error;
                }
            }
            finally
            {
                //TWaitting.Instance.CloseWaitForm();
            }
        }

        private Image LoadPicture(string localPath)
        {
            Image rlt = null;
            if (File.Exists(localPath))
            {
                try
                {
                    FileStream fs = new FileStream(localPath, FileMode.Open, FileAccess.Read);
                    int byteLength = (int)fs.Length;
                    byte[] fileBytes = new byte[byteLength];
                    fs.Read(fileBytes, 0, byteLength);
                    fs.Close();

                    rlt = Image.FromStream(new MemoryStream(fileBytes));
                }
                catch
                {
                    rlt = null;
                }
            }
            return rlt;
        }

        public void ShowWI(int t102LeafID, int t1216LeafID)
        {
            if (Screen.AllScreens.Length >= 2)
            {
                if (showWI == null)
                {
                    showWI = new frmWIShow(WIScreen.CreateInstance());
                    defaultImage = showWI.picWI.Image.Clone() as Image;
                }

                showWI.DesktopBounds = Screen.AllScreens[1].Bounds;

                try
                {
                    int errCode = 0;
                    string errText = "";
                    WIFile wiFile = new WIFile();
                    IRAPMDMClient.Instance.ufn_GetInfo_WIFIle(
                        IRAPUser.Instance.CommunityID,
                        t102LeafID,
                        t1216LeafID,
                        IRAPUser.Instance.SysLogID,
                        ref wiFile,
                        out errCode,
                        out errText);
                    if (errCode != 0)
                    {
                        XtraMessageBox.Show(
                            errText,
                            "出错啦",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        showWI.picWI.Image = null;
                    }
                    else
                    {
                        if (wiFile.WIFileName == "")
                        {
                            showWI.lblMessage.Text =
                                "电子作业指导书文件名空白，该作业指导书是否还未上传";
                            showWI.lblMessage.Visible = true;
                            showWI.picWI.Visible = false;
                        }
                        else
                        {
                            string basePath = $"{AppDomain.CurrentDomain.BaseDirectory}WI\\";
                            if (!Directory.Exists(basePath))
                            {
                                Directory.CreateDirectory(basePath);
                            }

                            string path = $"{basePath}{wiFile.WIFileName}";
                            if (!File.Exists(path))
                            {
                                try
                                {
                                    if (!DownloadWIFile(path, wiFile))
                                    {
                                        showWI.lblMessage.Text =
                                            $"本地和远程都未找到电子作业指导书[{wiFile}]文件";
                                        //showWI.lblMessage.Visible = true;
                                        //showWI.picWI.Visible = false;

                                        showWI.picWI.Image = defaultImage;

                                        return;
                                    }
                                }
                                catch (Exception error)
                                {
                                    showWI.lblMessage.Text = error.Message;
                                    //showWI.lblMessage.Visible = true;
                                    //showWI.picWI.Visible = false;
                                    showWI.picWI.Image = defaultImage;
                                    return;
                                }
                            }

                            showWI.picWI.Image = LoadPicture(path);
                            if (showWI.picWI.Image == null)
                            {
                                showWI.lblMessage.Text =
                                    $"无法加载图片文件[{path}]，该文件内容为" +
                                    "空或者无法识别图片文件格式";
                                //showWI.lblMessage.Visible = true;
                                //showWI.picWI.Visible = false;
                                showWI.picWI.Image = defaultImage;
                            }
                            else
                            {
                                showWI.lblMessage.Text = "图片加载成功";
                                showWI.lblMessage.Visible = false;
                                showWI.picWI.Visible = true;
                            }
                        }
                    }
                }
                finally
                {
                    showWI.Show();
                }
            }
        }

        public void HideWI()
        {
            if (showWI != null)
            {
                showWI.Hide();
            }
        }
    }
}
