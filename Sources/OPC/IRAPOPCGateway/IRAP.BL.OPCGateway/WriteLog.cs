﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;

using System.IO;

namespace IRAP.BL.OPCGateway
{
    public class WriteLog
    {
        private static WriteLog _instance = null;
        private static object _lockStatus = new object();
        private Guid localGuid = Guid.NewGuid();

        public static WriteLog Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WriteLog();
                return _instance;
            }
        }

        private WriteLog()
        {
            logPath = string.Format(@"{0}Log\",
                                    AppDomain.CurrentDomain.BaseDirectory);
            AttributeFileName = FILE_ATTRIBUTE_WRITELOG;
            if (!System.IO.Directory.Exists(logPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }
                catch 
                {
                    canCreateLogFile = false;
                }
            }
        }

        #region 属性定义
        public const string FILE_ATTRIBUTE_WRITELOG = "IRAP.ini";

        /// <summary>
        /// 写日志的属性配置文件名
        /// </summary>
        string attributeFileName = "";
        /// <summary>
        /// 日志文件所在文件夹
        /// </summary>
        string logPath = "";
        /// <summary>
        /// 日志文件名前缀
        /// </summary>
        string writeLogFileName = "IRAP";
        /// <summary>
        /// 能否创建日志文件标志
        /// </summary>
        bool canCreateLogFile = true;

        public string AttributeFileName
        {
            get { return attributeFileName; }
            set
            {
                attributeFileName = string.Format("{0}{1}",
                                                  AppDomain.CurrentDomain.BaseDirectory,
                                                  value);                                                  
            }
        }

        public bool IsWriteLog
        {
            get
            {
                if (ConfigurationManager.AppSettings["WriteLog"] != null)
                    return ConfigurationManager.AppSettings["WriteLog"].ToString().ToUpper() == "TRUE";
                else
                    return false;
            }
            set
            {
                Configuration config =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config.AppSettings.Settings["WriteLog"].Value == null)
                    config.AppSettings.Settings.Add("WriteLog", true.ToString());
                else
                    config.AppSettings.Settings["WriteLog"].Value = value.ToString();

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        public string WriteLogFileName
        {
            get { return writeLogFileName; }
            set { writeLogFileName = value; }
        }
        #endregion

        #region 方法定义
        public void Write(Guid id, string msg, string modeName = "COMM")
        {
            if (IsWriteLog && canCreateLogFile)
            {
                lock (_lockStatus)
                {
                    DeleteObsoleteFiles();

                    string strLogFileName = string.Format("{0}{1}_{2}.log",
                                                          logPath,
                                                          writeLogFileName,
                                                          DateTime.Now.ToString("yyyy-MM-dd"));

                    StreamWriter sw = null;
                    if (File.Exists(strLogFileName))
                    {
                        while (true)
                        {
                            try
                            {
                                sw = File.AppendText(strLogFileName);
                                break;
                            }
                            catch
                            {
                                Thread.Sleep(10);
                            }
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            try
                            {
                                sw = File.CreateText(strLogFileName);
                                break;
                            }
                            catch
                            {
                                Thread.Sleep(10);
                            }
                        }
                    }

                    try
                    {
                        if (msg.Trim() == "")
                        {
                            sw.WriteLine("");
                        }
                        else
                        {
                            sw.WriteLine(string.Format("{0} : [{1}][{2}][{3}]",
                                                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                                       id.ToString(),
                                                       modeName,
                                                       msg));
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (sw != null)
                        {
                            sw.Close();
                        }
                    }
                }
            }
        }

        public void Write(string logFileName, string msg, string modeName = "COMM")
        {
            WriteLogFileName = logFileName;
            Write(localGuid, msg, modeName);
        }

        public void WriteBeginSplitter(string modeName)
        {
            string msg = " BEGIN ";
            msg = msg.PadLeft(msg.Length + 15, '=');
            msg = msg.PadRight(msg.Length + 15, '=');

            Write(localGuid, msg, modeName);
        }

        public void WriteBeginSplitter(string logFileName, string modeName)
        {
            WriteLogFileName = logFileName;
            WriteBeginSplitter(modeName);
        }

        public void WriteEndSplitter(string modeName)
        {
            string msg = " END ";
            msg = msg.PadLeft(msg.Length + 15, '=');
            msg = msg.PadRight(msg.Length + 15, '=');

            Write(localGuid, msg, modeName);
        }

        public void WriteEndSplitter(string logFileName, string modeName)
        {
            WriteLogFileName = logFileName;
            WriteEndSplitter(modeName);
        }

        private void DeleteObsoleteFiles()
        {
            string[] fileNames = Directory.GetFiles(logPath, "*.log");
            for (int i = 0; i < fileNames.Length - 1; i++)
            {
                FileInfo fi = new FileInfo(fileNames[i]);
                if (fi.LastWriteTime <= DateTime.Now.AddMonths(-1))
                    File.Delete(fileNames[i]);
            }
        }
        #endregion
    }
}
