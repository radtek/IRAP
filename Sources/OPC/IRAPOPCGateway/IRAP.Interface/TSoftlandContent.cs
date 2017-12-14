﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace IRAP.Interface
{
    public class TSoftlandContent
    {
        protected TSoftlandHead head = new TSoftlandHead();
        protected TSoftlandBody bodyRequest = null;
        protected TSoftlandBody bodyResponse = null;
        protected TSoftlandLog logRequest = null;
        protected TSoftlandLog logResponse = null;

        public TSoftlandHead Head
        {
            get { return head; }
        }

        private void ResolveHead(XmlNode node)
        {
            try
            {
                head.Resolve(node);
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                throw error;
            }
        }

        protected virtual void ResolveBody(XmlNode node)
        {

        }

        protected virtual void ResolveLog(XmlNode node)
        {

        }

        /// <summary>
        /// 解析 XML 串
        /// </summary>
        /// <param name="xmlString"></param>
        public void Resolve(string xmlString)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(xmlString);
            }
            catch (Exception error)
            {
                string errText =
                    string.Format(
                        "在解析 XML 串的时候发生错误：[{0}]",
                        error.Message);
                Debug.WriteLine(errText);

                throw new Exception(errText);
            }

            XmlNode headNode = xml.SelectSingleNode("Softland/Head");
            if (headNode == null)
            {
                string errText = "XML 串不符合 WebAPI 不符合接口规范定义，没有找到 Softland/Head 节点";
                Debug.WriteLine(errText);
                throw new Exception(errText);
            }
            else
            {
                ResolveHead(headNode);
            }

            XmlNode bodyNode = xml.SelectSingleNode("Softland/Body");
            if (bodyNode == null)
            {
                string errText = "XML 串不符合 WebAPI 不符合接口规范定义，没有找到 Softland/Body 节点";
                Debug.WriteLine(errText);
                throw new Exception(errText);
            }
            else
                ResolveBody(bodyNode);

            XmlNode logNode = xml.SelectSingleNode("Softland/Log");
            if (logNode != null)
                ResolveLog(logNode);
        }

        public string GenerateResponseContent()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode root = xml.CreateElement("Softland");
            xml.AppendChild(root);

            root.AppendChild(xml.ImportNode(head.Generate(), true));

            if (bodyResponse != null)
                root.AppendChild(xml.ImportNode(bodyResponse.Generate(), true));
            if (logResponse != null)
                root.AppendChild(xml.ImportNode(logResponse.Generate(), true));

            return xml.OuterXml;            
        }
    }

    public class TSoftlandHead : TCustomXMLArea
    {
        public string ExCode { get; set; }
        public string CorrelationID { get; set; }
        public string CommunityID { get; set; }
        public string SysLogID { get; set; }
        public string UserCode { get; set; }
        public string AgencyLeaf { get; set; }
        public string RoleLeaf { get; set; }
        public string StationID { get; set; }
        public string UnixTime { get; set; }

        public void Resolve(XmlNode head)
        {
            ExCode = GetNodeInnerText(head, "ExCode");
            CorrelationID = GetNodeInnerText(head, "CorrelationID");
            CommunityID = GetNodeInnerText(head, "CommunityID");
            SysLogID = GetNodeInnerText(head, "SysLogID");
            UserCode = GetNodeInnerText(head, "UserCode");
            AgencyLeaf = GetNodeInnerText(head, "AgencyLeaf");
            RoleLeaf = GetNodeInnerText(head, "RoleLeaf");
            StationID = GetNodeInnerText(head, "StationID");
            UnixTime = GetNodeInnerText(head, "UnixTime");
        }

        public XmlNode Generate()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode head = xmlDoc.CreateElement("Head");
            AddXMLLeaf(xmlDoc, head, "ExCode", ExCode);
            AddXMLLeaf(xmlDoc, head, "CorrelationID", CorrelationID);
            AddXMLLeaf(xmlDoc, head, "CommunityID", CommunityID);
            AddXMLLeaf(xmlDoc, head, "SysLogID", SysLogID);
            AddXMLLeaf(xmlDoc, head, "UserCode", UserCode);
            AddXMLLeaf(xmlDoc, head, "AgencyLeaf", AgencyLeaf);
            AddXMLLeaf(xmlDoc, head, "RoleLeaf", RoleLeaf);
            AddXMLLeaf(xmlDoc, head, "StationID", StationID);
            AddXMLLeaf(xmlDoc, head, "UnixTime", UnixTime);

            return head;
        }
    }

    public abstract class TSoftlandBody : TCustomXMLArea
    {
        protected abstract XmlNode GenerateUserDefineNode();

        public virtual XmlNode Generate()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode node = xml.CreateElement("Body");

            node.AppendChild(xml.ImportNode(GenerateUserDefineNode(), true));

            return node;
        }
    }

    public abstract class TSoftlandLog : TCustomXMLArea
    {
        protected abstract XmlNode GenerateUserDefineNode();

        public virtual XmlNode Generate()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode node = xml.CreateElement("Log");

            node.AppendChild(xml.ImportNode(GenerateUserDefineNode(), true));

            return node;
        }
    }
}