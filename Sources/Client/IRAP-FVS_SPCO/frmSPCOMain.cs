﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Configuration;
using System.Management;
using System.Reflection;
using System.Xml;

using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

using IRAP.Global;
using IRAP.Entity.SSO;
using IRAP.Entity.MDM;
using IRAP.WCF.Client.Method;

namespace IRAP_FVS_SPCO
{
    public partial class frmSPCOMain : XtraForm
    {
        private string className =
            MethodBase.GetCurrentMethod().DeclaringType.FullName;

        public delegate void RedrawingSPCChartMethod(
            XtraTabControl tabControl, 
            string t107Code, 
            string pwoNo, 
            int t47LeafID,
            int t216LeafID,
            int t133LeafID,
            int t20LeafID);

        private const int WS_SHOWMAXIMIZE = 3;

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private string brokerUri = "";
        private string queueName = "";
        private string macAddress = "";
        private StationLogin stationUser = null;
        private List<WIPStationProductionStatus> workUnits = new List<WIPStationProductionStatus>();
        private List<XtraUserControl> ucCharts = new List<XtraUserControl>();
        /// <summary>
        /// 应用服务器的当前时间
        /// </summary>
        private DateTime serverTime = DateTime.Now;

        public frmSPCOMain()
        {
            InitializeComponent();

            #region 从程序配置文件中读取 ActiveMQ 配置信息
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["ActiveMQ_URI"] != null)
                brokerUri = config.AppSettings.Settings["ActiveMQ_URI"].Value;
            if (brokerUri.Trim() == "")
                brokerUri = "failover:tcp://192.168.57.208:61616";

            if (config.AppSettings.Settings["ActiveMQ_QueueName"] != null)
                queueName = config.AppSettings.Settings["ActiveMQ_QueueName"].Value;
            if (queueName.Trim() == "")
                queueName = "firstQueue";
            #endregion

            #region
            if (config.AppSettings.Settings["Virtual Station Used"] != null)
            {
                if (config.AppSettings.Settings["Virtual Station Used"].Value.ToUpper() == "TRUE")
                {
                    if (config.AppSettings.Settings["Virtual Station"] != null)
                    {
                        macAddress = config.AppSettings.Settings["Virtual Station"].Value;
                    }
                }
            }
            #endregion

            if (macAddress.Trim() == "")
                GetMacAddress();
        }

        #region 自定义函数
        private void HandleRunningInstance()
        {
            Process current = Process.GetCurrentProcess();

            IntPtr hWnd = GetForegroundWindow();
            if (hWnd != current.MainWindowHandle)
            {
                ShowWindowAsync(current.MainWindowHandle, WS_SHOWMAXIMIZE);
                SetForegroundWindow(current.MainWindowHandle);
            }
        }

        private void InitConsumer(string filterString)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            try
            {
                IConnectionFactory factory = new ConnectionFactory(brokerUri);
                IConnection connection = factory.CreateConnection();
                connection.ClientId = Guid.NewGuid().ToString();
                connection.Start();

                ISession session = connection.CreateSession();
                IMessageConsumer consumer =
                    session.CreateConsumer(
                        new ActiveMQQueue(queueName),
                        string.Format("filter='{0}'", filterString));
                consumer.Listener += new MessageListener(consumer_Listener);
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(
                    string.Format(
                        "创建ActiveMQ侦听失败：{0}",
                        error.Message), 
                    strProcedureName);
            }
        }

        private void consumer_Listener(IMessage message)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            ITextMessage msg = (ITextMessage)message;

            // 收到消息后，将当前应用激活，并置于最前面
            HandleRunningInstance();

            if (msg.Text.Trim() != "")
            {
                string text = msg.Text.Trim();
                XmlDocument xmlDoc = new XmlDocument();

                WriteLog.Instance.WriteBeginSplitter(strProcedureName);
                try
                {
                    xmlDoc.LoadXml(text);

                    XmlNode node = xmlDoc.SelectSingleNode("Content/Message");
                    if (node != null)
                    {
                        string pwoNo = "";
                        string t107Code = "";
                        int t47LeafID = 0;
                        int t216LeafID = 0;
                        int t133LeafID = 0;
                        int t20LeafID = 0;
                        DateTime sendTime = DateTime.Now;

                        if (node.Attributes["T107Code"] != null)
                            t107Code = node.Attributes["T107Code"].Value;
                        else
                            return;
                        //if (node.Attributes["PWONo"] != null)
                        //    pwoNo = node.Attributes["PWONo"].Value;
                        //else
                        //    return;
                        //if (node.Attributes["T47LeafID"] != null)
                        //    t47LeafID = int.Parse(node.Attributes["T47LeafID"].Value);
                        //else
                        //    return;
                        //if (node.Attributes["T216LeafID"] != null)
                        //    t216LeafID = int.Parse(node.Attributes["T216LeafID"].Value);
                        //else
                        //    return;
                        //if (node.Attributes["T133LeafID"] != null)
                        //    t133LeafID = int.Parse(node.Attributes["T133LeafID"].Value);
                        //else
                        //    return;
                        //if (node.Attributes["T20LeafID"] != null)
                        //    t20LeafID = int.Parse(node.Attributes["T20LeafID"].Value);
                        //else
                        //    return;
                        if (node.Attributes["SendDateTime"] != null)
                            sendTime = DateTime.Parse(node.Attributes["SendDateTime"].Value);
                        else
                            sendTime = DateTime.Now;

                        if ((DateTime.Now - sendTime).TotalSeconds <= 180)
                        {
                            // 切换到消息中指定工位的显示面板
                            object[] parameters = new object[7];
                            parameters[0] = tcMain;
                            parameters[1] = t107Code;
                            parameters[2] = pwoNo;
                            parameters[3] = t47LeafID;
                            parameters[4] = t216LeafID;
                            parameters[5] = t133LeafID;
                            parameters[6] = t20LeafID;

                            BeginInvoke(new RedrawingSPCChartMethod(RedrawingSPCChart), parameters);
                        }
                    }
                }
                catch (Exception error)
                {
                    WriteLog.Instance.Write(error.Message, strProcedureName);
                }
                finally
                {
                    WriteLog.Instance.WriteEndSplitter(strProcedureName);
                }
            }
        }

        private void RedrawingSPCChart(
            XtraTabControl tabControl,
            string t107Code,
            string pwoNo,
            int t47LeafID,
            int t216LeafID,
            int t133LeafID,
            int t20LeafID)
        {
            foreach (XtraTabPage page in tabControl.TabPages)
            {
                if (page.Tag == null)
                    continue;

                WIPStationProductionStatus workUnit = page.Tag as WIPStationProductionStatus;
                if (workUnit.T107Code == t107Code)
                {
                    if (tabControl.SelectedTabPage != page)
                        tabControl.SelectedTabPage = page;
                    else
                        tcMain_SelectedPageChanged(
                            tcMain,
                            new TabPageChangedEventArgs(
                                tabControl.SelectedTabPage,
                                page));
                    //foreach (Control control in page.Controls)
                    //{
                    //    if (control is ucRainBowChart)
                    //    {
                    //        ucRainBowChart chart = control as ucRainBowChart;
                    //        chart.DrawChart(stationUser, workUnit, pwoNo, t216LeafID, t133LeafID, t20LeafID);
                    //        return;
                    //    }
                    //    if (control is ucXBarRChart)
                    //    {
                    //        ucXBarRChart chart = control as ucXBarRChart;
                    //        chart.DrawChart(stationUser, workUnit, pwoNo, t20LeafID);
                    //        return;
                    //    }
                    //}
                }
            }
        }

        /// <summary>
        /// 获取当前真实的 MAC 地址
        /// </summary>
        protected virtual void GetMacAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                try
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        macAddress = mo["MacAddress"].ToString();
                        macAddress = macAddress.Replace(":", "");
                        break;
                    }
                }
                catch 
                {
                    mo.Dispose();
                }
            }
        }

        /// <summary>
        /// Pad 登录
        /// </summary>
        private StationLogin PadLogin(
            ref int errCode,
            ref string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                StationLogin stationUser = new StationLogin();

                IRAPUserClient.Instance.sfn_GetInfo_StationLogin(
                    macAddress != "" ? macAddress : "60010MDV1101001",
                    ref stationUser,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText),
                    strProcedureName);

                if (errCode == 0)
                    return stationUser;
                else
                    return null;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 获取当前站点的所有工位
        /// </summary>
        private List<WIPStationProductionStatus> GetWorkUnits(
            ref int errCode,
            ref string errText)
        {
            string strProcedureName =
                 string.Format(
                     "{0}.{1}",
                     className,
                     MethodBase.GetCurrentMethod().Name);
            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                List<WIPStationProductionStatus> rlt =
                    new List<WIPStationProductionStatus>();

                IRAPMDMClient.Instance.ufn_GetList_WIPStationProductionStatus(
                    stationUser.CommunityID,
                    stationUser.SysLogID,
                    0,
                    ref rlt,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("{0}.{1}", errCode, errText),
                    strProcedureName);
                if (errCode == 0)
                    return rlt;
                else
                    return null;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void HideErrorMessage()
        {
            if (lblErrorMessage.Visible)
                lblErrorMessage.Visible = false;
            if (!xtraScrollableControl.Visible)
                xtraScrollableControl.Visible = true;

            lblErrorMessage.Text = "";
        }

        private void ShowErrorMessage(string msg)
        {
            lblErrorMessage.Text = msg;

            if (!lblErrorMessage.Visible)
                lblErrorMessage.Visible = true;
            if (xtraScrollableControl.Visible)
                xtraScrollableControl.Visible = false;
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void frmSPCOMain_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            Application.DoEvents();

            int errCode = 0;
            string errText = "";

            #region 获取服务器的当前时间，并设置当前的系统时间
            IRAPSystemClient.Instance.sfn_GetServerDateTime(
                ref serverTime,
                out errCode,
                out errText);
            if (errCode == 0)
            {
                SetSystemDateTime.SetSystemTime(serverTime);
            }
            #endregion

            if (stationUser == null || stationUser.SysLogID <= 0)
            {
                #region 获取当前站点的登录信息
                stationUser = PadLogin(ref errCode, ref errText);
                if (errCode != 0)
                {
                    ShowErrorMessage(errText);
                    return;
                }
                else
                {
                    HideErrorMessage();
                }
                #endregion
            }

            if (stationUser != null)
            {
                #region 获取当前站点监管的工位列表，并根据工位列表生成控制图
                if (stationUser.SysLogID > 0)
                {
                    if (!xtraScrollableControl.Visible)
                        xtraScrollableControl.Visible = true;

                    if (workUnits.Count == 0)
                    {
                        btnClose.Visible = false;
                        try
                        {
                            tcMain.TabPages.Clear();
                            workUnits = GetWorkUnits(ref errCode, ref errText);

                            if (errCode == 0)
                            {
                                HideErrorMessage();

                                foreach (WIPStationProductionStatus workUnit in workUnits)
                                {
#if DEBUG
                                    //workUnit.T47LeafID = 373564;
#endif

                                    XtraTabPage page = tcMain.TabPages.Add();
                                    page.Text = workUnit.T107Name;
                                    page.Tag = workUnit;

                                    ucUncontrolChart chartNone = new ucUncontrolChart();
                                    chartNone.Dock = DockStyle.Fill;
                                    chartNone.Parent = page;

#if !DEBUG
                                    InitConsumer(workUnit.T107Code);
#endif

                                    ucCharts.Add(chartNone);
                                }

                                if (tcMain.TabPages.Count > 0)
                                {
                                    tcMain_SelectedPageChanged(
                                        tcMain,
                                        new TabPageChangedEventArgs(
                                            null,
                                            tcMain.TabPages[0]));
                                }
                            }
                            else
                            {
                                ShowErrorMessage(errText);
                            }
                        }
                        finally
                        {
                            btnClose.Visible = true;
                        }
                    }
                }
#endregion
            }
        }

        private void tcMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            if (e.Page != null && e.Page.Tag != null && e.Page.Tag is WIPStationProductionStatus)
            {
                string strProcedureName =
                    string.Format(
                        "{0}.{1}",
                        className,
                        MethodBase.GetCurrentMethod().Name);

                WIPStationProductionStatus workUnit = e.Page.Tag as WIPStationProductionStatus;
                int pageIndex = tcMain.TabPages.IndexOf(e.Page);

                int errCode = 0;
                string errText = "";
                List<WIPStationSPCMonitor> datas = new List<WIPStationSPCMonitor>();

//#if DEBUG
//                datas.Add(
//                    new WIPStationSPCMonitor()
//                    {
//                        T47LeafID = 373564,
//                        PWONo_InExecution = "34PFF1CCQY60628003",
//                        T216LeafID = 5258697,
//                        T133LeafID = 2155684,
//                        T20LeafID = 352942,
//                    });
//#else

                WriteLog.Instance.WriteBeginSplitter(strProcedureName);
                try
                {
                    IRAPMDMClient.Instance.ufn_GetList_WIPStationSPCMonitor(
                        stationUser.CommunityID,
                        stationUser.SysLogID,
                        workUnit.T107Code,
                        ref datas,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}", errCode, errText),
                        strProcedureName);
                    if (errCode != 0)
                    {
                        XtraMessageBox.Show(
                            errText,
                            "系统信息",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                finally
                {
                    WriteLog.Instance.WriteEndSplitter(strProcedureName);
                }
//#endif

                int t47LeafID = 0;
                if (datas.Count > 0)
                {
                    t47LeafID = datas[0].T47LeafID;
                }

                if (t47LeafID == 0)
                {
                    if (!(ucCharts[pageIndex] is ucUncontrolChart))
                    {
                        tcMain.TabPages[pageIndex].Controls.Remove(ucCharts[pageIndex]);
                        ucCharts[pageIndex] = null;

                        ucCharts[pageIndex] =
                            new ucUncontrolChart()
                            {
                                Dock = DockStyle.Fill,
                                Parent = e.Page,
                            };
                    }
                }
                else
                {
                    switch (t47LeafID)
                    {
                        case 373564:
                            if (!(ucCharts[pageIndex] is ucRainBowChart))
                            {
                                tcMain.TabPages[pageIndex].Controls.Remove(ucCharts[pageIndex]);
                                ucCharts[pageIndex] = null;

                                ucRainBowChart chartRainBow =
                                    new ucRainBowChart()
                                    {
                                        Dock = DockStyle.Fill,
                                        Parent = e.Page,
                                    };

                                ucCharts[pageIndex] = chartRainBow;
                            }

                            (ucCharts[pageIndex] as ucRainBowChart).DrawChart(
                                stationUser,
                                workUnit,
                                datas[0].PWONo_InExecution,
                                t47LeafID,
                                datas[0].T216LeafID,
                                datas[0].T133LeafID,
                                datas[0].T20LeafID);

                            break;
                        case 373565:
                            if (!(ucCharts[pageIndex] is ucXBarRChart))
                            {
                                tcMain.TabPages[pageIndex].Controls.Remove(ucCharts[pageIndex]);
                                ucCharts[pageIndex] = null;

                                ucXBarRChart chartXbarR =
                                    new ucXBarRChart()
                                    {
                                        Dock = DockStyle.Fill,
                                        Parent = e.Page,
                                    };

                                ucCharts[pageIndex] = chartXbarR;
                            }

                            (ucCharts[pageIndex] as ucXBarRChart).DrawChart(
                                stationUser,
                                workUnit,
                                datas[0].PWONo_InExecution,
                                //datas[0].T216LeafID,
                                //datas[0].T133LeafID,
                                datas[0].T20LeafID);
                            break;
                    }
                }
            }
        }
    }
}