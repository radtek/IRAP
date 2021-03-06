﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

using OPCAutomation;

using IRAP.Interface.OPC;
using IRAP.BL.OPCGateway.Interfaces;
using IRAP.BL.OPCGateway.Global;
using IRAP.BL.OPCGateway.Global.Entities;
using IRAP.OPC.Entity;
using IRAP.OPC.Library;

namespace IRAP.BL.OPCGateway.Actions
{
    internal class TGetDevices : IWebAPIAction
    {
        private TGetDevicesContent content = new TGetDevicesContent();

        public TGetDevices(string xmlContent)
        {
            try
            {
                content.ResolveRequest(xmlContent);
            }
            catch (Exception error)
            {
                foreach (DictionaryEntry de in error.Data)
                {
                    if (de.Key.ToString().ToUpper() == "ERRCODE")
                        content.Response.ErrCode = de.Value.ToString();
                    if (de.Key.ToString().ToUpper() == "ERRTEXT")
                        content.Response.ErrText = de.Value.ToString();
                }
            }
        }

        public string DoAction()
        {
            if (content.Request != null)
            {
                content.Response.ErrCode = "999999";
                content.Response.ErrText = "功能正在开发中......";

                try
                {
                    if (content.Request.ExCode == "GetDevices")
                    {
                        TKepwareServer server =
                            TKepwareServers.Instance.Servers.ByAddress(
                                content.Request.KepServAddr);

                        if (server == null)
                        {
                            server =
                                new TKepwareServer(
                                    content.Request.KepServAddr,
                                    content.Request.KepServName);
                            TKepwareServers.Instance.Servers.Add(server);
                        }

                        for (int i = 0; i < server.Devices.Count; i++)
                        {
                            content.Response.AddDeviceDetail(
                                new TGetDevicesRspDetail()
                                {
                                    Ordinal = i + 1,
                                    KepServerAddr = server.Devices[i].KepServAddr,
                                    KepServerName = server.Devices[i].KepServName,
                                    KepServerChannel = server.Devices[i].KepServChannel,
                                    KepServerDevice = server.Devices[i].KepServDevice,
                                });
                        }

                        content.Response.ErrCode = "0";
                        content.Response.ErrText = "设备列表获取完成";
                    }
                    else
                    {
                        content.Response.ErrCode = "900000";
                        content.Response.ErrText = "报文体中的交易代码和报文头中的交易代码不一致";
                    }
                }
                catch (Exception error)
                {
                    content.Response.ErrText = string.Format("系统抛出的错误：[{0}]", error.Message);
                    foreach (DictionaryEntry de in error.Data)
                    {
                        if (de.Key.ToString().ToUpper() == "ERRCODE")
                            content.Response.ErrCode = de.Value.ToString();
                        if (de.Key.ToString().ToUpper() == "ERRTEXT")
                            content.Response.ErrText = de.Value.ToString();
                    }
                }
            }

            return content.GenerateResponseContent();
        }
    }
}
