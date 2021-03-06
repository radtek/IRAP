﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

using IRAP.Global;
using IRAP.Entities.MES;

namespace IRAP.WCF.Client.Method
{
    public class IRAPMESBatchClient
    {
        private static IRAPMESBatchClient _instance = null;

        public static IRAPMESBatchClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IRAPMESBatchClient();
                return _instance;
            }
        }

        private string className =
            MethodBase.GetCurrentMethod().DeclaringType.FullName;

        private IRAPMESBatchClient() { }

        /// <summary>
        /// 获取指定工序、工位和设备的当前生产状态
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="t107LeafID">工位叶标识</param>
        /// <param name="t216LeafID">工序叶标识</param>
        /// <param name="t133LeafID">设备叶标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        public void ufn_GetInfo_SmeltBatchProduct(
            int communityID,
            int t107LeafID,
            int t216LeafID,
            int t133LeafID,
            long sysLogID,
            ref SmeltBatchProductionInfo data,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                data = null;

                #region 将函数调用参数加入 HashTable 中
                Hashtable hashParams = new Hashtable();

                hashParams.Add("communityID", communityID);
                hashParams.Add("t107LeafID", t107LeafID);
                hashParams.Add("t216LeafID", t216LeafID);
                hashParams.Add("t133LeafID", t133LeafID);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    string.Format(
                        "调用 ufn_GetInfo_SmeltBatchProduct 函数，参数：" +
                        "CommunityID={0}|T107LeafID={1}|T216LeafID={2}|" +
                        "T133LeafID={3}|SysLogID={4}",
                        communityID, t107LeafID, t216LeafID, t133LeafID, sysLogID),
                    strProcedureName);
                #endregion

                #region 执行存储过程或者函数
                using (WCFClient client = new WCFClient())
                {

                    object rlt =
                        client.WCFRESTFul(
                            "IRAP.BL.MES.dll",
                            "IRAP.BL.MES.BatchSystem_Asimco",
                            "ufn_GetInfo_SmeltBatchProduct",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}", errCode, errText),
                        strProcedureName);

                    if (errCode == 0)
                    {
                        List<SmeltBatchProductionInfo> datas = 
                            rlt as List<SmeltBatchProductionInfo>;
                        if (datas != null && datas.Count > 0)
                            data = datas[0];
                    }
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                errCode = -1001;
                errText = error.Message;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="batchNumber">熔炼容次号</param>
        /// <param name="sysLogID">语言标识</param>
        public void ufn_GetList_SmeltBatchPWONo(
            int communityID,
            string batchNumber,
            long sysLogID,
            ref List<OrderInfo> datas,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                if (datas == null)
                    datas = new List<OrderInfo>();
                else
                    datas.Clear();

                #region 将函数调用参数加入 HashTable 中
                Hashtable hashParams = new Hashtable();

                hashParams.Add("communityID", communityID);
                hashParams.Add("batchNumber", batchNumber);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    string.Format(
                        "调用 ufn_GetList_SmeltBatchPWONo 函数， 参数：" +
                        "CommunityID={0}|BatchNumber={1}|SysLogID={2}",
                        communityID, batchNumber, sysLogID),
                    strProcedureName);
                #endregion

                #region 执行存储过程或者函数
                using (WCFClient client = new WCFClient())
                {

                    object rlt =
                        client.WCFRESTFul(
                            "IRAP.BL.MES.dll",
                            "IRAP.BL.MES.BatchSystem_Asimco",
                            "ufn_GetList_SmeltBatchPWONo",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}", errCode, errText),
                        strProcedureName);

                    if (errCode == 0)
                        datas = rlt as List<OrderInfo>;
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                errCode = -1001;
                errText = error.Message;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 变更正在生产中的炉次绑定生产工单
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="batchNumber">熔次号</param>
        /// <param name="opType">操作类别：A-新增；D-删除</param>
        /// <param name="pwoNo">生产工单号</param>
        /// <param name="moNumber">制造订单号</param>
        /// <param name="moLineNo">制造订单行号</param>
        /// <param name="t102Code">产品物料号</param>
        /// <param name="linkedFactID">关联事实号</param>
        /// <param name="t131Code">材质</param>
        /// <param name="qty">计划数量</param>
        /// <param name="plateNo">型板号</param>
        /// <param name="batchLot">容次号</param>
        /// <param name="machineModelling">造型机台号</param>
        /// <param name="foldNumber">叠数</param>
        /// <param name="sysLogID">系统登录标识</param>
        public void usp_SaveChangeFact_SmeltBatchProduction(
            int communityID,
            string batchNumber,
            string opType,
            string pwoNo,
            string moNumber,
            int moLineNo,
            string t102Code,
            long linkedFactID,
            string t131Code,
            long qty,
            string plateNo,
            string batchLot,
            string machineModelling,
            int foldNumber,
            long sysLogID,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                #region 将函数调用参数加入 HashTable 中
                Hashtable hashParams = new Hashtable();
                hashParams.Add("communityID", communityID);
                hashParams.Add("batchNumber", batchNumber);
                hashParams.Add("opType", opType);
                hashParams.Add("pwoNo", pwoNo);
                hashParams.Add("moNumber", moNumber);
                hashParams.Add("moLineNo", moLineNo);
                hashParams.Add("t102Code", t102Code);
                hashParams.Add("linkedFactID", linkedFactID);
                hashParams.Add("t131Code", t131Code);
                hashParams.Add("qty", qty);
                hashParams.Add("plateNo", plateNo);
                hashParams.Add("batchLot", batchLot);
                hashParams.Add("machineModelling", machineModelling);
                hashParams.Add("foldNumber", foldNumber);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    string.Format(
                        "调用 usp_SaveChangeFact_SmeltBatchProduction，参数：" +
                        "CommunityID={0}|BatchNumber={1}|OpType={2}|" +
                        "PWONo={3}|MONumber={4}|MOLineNo={5}|T102Code={6}|" +
                        "LinkedFactID={7}|T131Code={8}|Qty={9}|PlateNo={10}" +
                        "BatchLot={11}|MachineModelling={12}|FoldNumber={13}" +
                        "SysLogID={14}",
                        communityID, batchNumber, opType, pwoNo, moNumber,
                        moLineNo, t102Code, linkedFactID, t131Code,
                        qty, plateNo, batchLot, machineModelling,
                        foldNumber, sysLogID),
                    strProcedureName);
                #endregion

                #region 调用应用服务过程，并解析返回值
                using (WCFClient client = new WCFClient())
                {
                    object rlt = client.WCFRESTFul(
                        "IRAP.BL.MES.dll",
                        "IRAP.BL.MES.BatchSystem_Asimco",
                        "usp_SaveChangeFact_SmeltBatchProduction",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = -1001;
                errText = error.Message;
                WriteLog.Instance.Write(errText, strProcedureName);
                WriteLog.Instance.Write(error.StackTrace, strProcedureName);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 以保存的熔炼批次检验数据删除
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="t216LeafID">产品叶标识</param>
        /// <param name="t107LeafID">工位叶标识</param>
        /// <param name="batchNumber">容次号</param>
        /// <param name="opType">操作标识：D-删除；U-修改(暂不支持)</param>
        /// <param name="factID">保存的工艺参数数据事实编号</param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        /// <returns></returns>
        public void usp_SaveFact_SmeltBatchMethodCancel(
            int communityID,
            int t216LeafID,
            int t107LeafID,
            string batchNumber,
            string opType,
            long factID,
            long sysLogID,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                #region 将函数调用参数加入 HashTable 中
                Hashtable hashParams = new Hashtable();
                hashParams.Add("communityID", communityID);
                hashParams.Add("t216LeafID", t216LeafID);
                hashParams.Add("t107LeafID", t107LeafID);
                hashParams.Add("batchNumber", batchNumber);
                hashParams.Add("opType", opType);
                hashParams.Add("factID", factID);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    string.Format(
                        "调用 usp_SaveFact_SmeltBatchMethodCancel，参数：" +
                        "CommunityID={0}|T216LeafID={1}|T107LeafID={2}|"+
                        "BatchNumber={3}|OpType={4}|FactID={5}|" +
                        "SysLogID={6}",
                        communityID, t216LeafID, t107LeafID, batchNumber, 
                        opType, factID, sysLogID),
                    strProcedureName);
                #endregion

                #region 调用应用服务过程，并解析返回值
                using (WCFClient client = new WCFClient())
                {
                    object rlt = client.WCFRESTFul(
                        "IRAP.BL.MES.dll",
                        "IRAP.BL.MES.BatchSystem_Asimco",
                        "usp_SaveFact_SmeltBatchMethodCancel",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = -1001;
                errText = error.Message;
                WriteLog.Instance.Write(errText, strProcedureName);
                WriteLog.Instance.Write(error.StackTrace, strProcedureName);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 批次生产开始
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="t216LeafID">产品叶标识</param>
        /// <param name="t107LeafID">工位叶标识</param>
        /// <param name="operatorCode">操作工代码</param>
        /// <param name="t131LeafID">产品类别叶标识</param>
        /// <param name="rsFactXML">
        /// 工单信息列表
        /// [RSFact]
        ///     [RF25
        ///         Ordinal=""
        ///         T102LeafID=""
        ///         T216LeafID=""
        ///         WIPCode=""
        ///         LotNumber=""
        ///         Texture=""
        ///         PWONo=""
        ///         BatchLot=""
        ///         Qty=""
        ///         Scale="" /]
        /// [/RSFact]
        /// </param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        public void usp_SaveFact_BatchProductionStart(
            int communityID,
            int t216LeafID,
            int t107LeafID,
            string operatorCode,
            int t131LeafID,
            string rsFactXML,
            long sysLogID,
            out string batchNo,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            batchNo = "";

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                #region 将函数参数加入 Hashtable 中
                Hashtable hashParams = new Hashtable();
                hashParams.Add("communityID", communityID);
                hashParams.Add("t216LeafID", t216LeafID);
                hashParams.Add("t107LeafID", t107LeafID);
                hashParams.Add("t131LeafID", t131LeafID);
                hashParams.Add("operatorCode", operatorCode);
                hashParams.Add("rsFactXML", rsFactXML);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    string.Format(
                        "执行存储过程 usp_SaveFact_BatchProductionStart，输入参数：" +
                        "CommunityID={0}|T216LeafID={1}|T107LeafID={2}|" +
                        "T131LeafID={3}|OperatorCode={4}|RSFactXML={5}|SysLogID={6}",
                        communityID, t216LeafID, t107LeafID, t131LeafID, operatorCode,
                        rsFactXML, sysLogID),
                    strProcedureName);
                #endregion

                #region 调用应用服务过程，并解析返回值
                using (WCFClient client = new WCFClient())
                {
                    object rlt = client.WCFRESTFul(
                        "IRAP.BL.MES.dll",
                        "IRAP.BL.MES.WorkOrder",
                        "usp_SaveFact_BatchProductionStart",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);
                    if (errCode == 0)
                    {
                        if (rlt is Hashtable)
                        {
                            Hashtable rltHash = (Hashtable)rlt;
                            try
                            {
                                HashtableTools.Instance.GetValue(
                                    rltHash,
                                    "BatchNumber",
                                    out batchNo);
                            }
                            catch (Exception error)
                            {
                                errCode = -1003;
                                errText = error.Message;
                                WriteLog.Instance.Write(errText, strProcedureName);
                                return;
                            }
                        }
                        else
                        {
                            errCode = -1002;
                            errText = "应用服务 usp_SaveFact_BatchProductionStart 返回的不是 Hashtable！";
                            WriteLog.Instance.Write(errText, strProcedureName);
                        }
                    }
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                errCode = -1001;
                errText = error.Message;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 批次生产开始
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="t216LeafID">产品叶标识</param>
        /// <param name="t107LeafID">工位叶标识</param>
        /// <param name="operatorCode">操作工代码</param>
        /// <param name="t131LeafID">产品类别叶标识</param>
        /// <param name="rsFactXML">
        /// 工单信息列表
        /// [RSFact]
        ///     [RF25
        ///         Ordinal=""
        ///         T102LeafID=""
        ///         T216LeafID=""
        ///         WIPCode=""
        ///         LotNumber=""
        ///         Texture=""
        ///         PWONo=""
        ///         BatchLot=""
        ///         Qty=""
        ///         Scale=""
        ///         Remark="" /]
        /// [/RSFact]
        /// </param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        public void usp_SaveFact_BatchProductionStart_QuenchAndTemper(
            int communityID,
            int t216LeafID,
            int t107LeafID,
            string operatorCode,
            int t131LeafID,
            string rsFactXML,
            long sysLogID,
            ref string batchNo,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                #region 将函数参数加入 Hashtable 中
                Hashtable hashParams = new Hashtable();
                hashParams.Add("communityID", communityID);
                hashParams.Add("t216LeafID", t216LeafID);
                hashParams.Add("t107LeafID", t107LeafID);
                hashParams.Add("t131LeafID", t131LeafID);
                hashParams.Add("operatorCode", operatorCode);
                hashParams.Add("rsFactXML", rsFactXML);
                hashParams.Add("batchNo", batchNo);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    "执行存储过程 usp_SaveFact_BatchProductionStart_QuenchAndTemper，输入参数：" +
                    $"CommunityID={communityID}|T216LeafID={t216LeafID}|" +
                    $"T107LeafID={t107LeafID}|T131LeafID={t131LeafID}|" +
                    $"OperatorCode={operatorCode}|RSFactXML={rsFactXML}|" +
                    $"BatchNumber={batchNo}|SysLogID={sysLogID}",
                    strProcedureName);
                #endregion

                #region 调用应用服务过程，并解析返回值
                using (WCFClient client = new WCFClient())
                {
                    object rlt = client.WCFRESTFul(
                        "IRAP.BL.MES.dll",
                        "IRAP.BL.MES.BatchSystem_Asimco",
                        "usp_SaveFact_BatchProductionStart_QuenchAndTemper",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);
                    if (errCode == 0)
                    {
                        if (rlt is Hashtable)
                        {
                            Hashtable rltHash = (Hashtable)rlt;
                            try
                            {
                                HashtableTools.Instance.GetValue(
                                    rltHash,
                                    "BatchNumber",
                                    out batchNo);
                            }
                            catch (Exception error)
                            {
                                errCode = -1003;
                                errText = error.Message;
                                WriteLog.Instance.Write(errText, strProcedureName);
                                return;
                            }
                        }
                        else
                        {
                            errCode = -1002;
                            errText = "应用服务 usp_SaveFact_BatchProductionStart_QuenchAndTemper 返回的不是 Hashtable！";
                            WriteLog.Instance.Write(errText, strProcedureName);
                        }
                    }
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                errCode = -1001;
                errText = error.Message;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 批次生产开始
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="t216LeafID">产品叶标识</param>
        /// <param name="t107LeafID">工位叶标识</param>
        /// <param name="operatorCode">操作工代码</param>
        /// <param name="batchNumber">炉次号</param>
        /// <param name="t131LeafID">产品类别叶标识</param>
        /// <param name="rsFactXML">
        /// 工单信息列表
        /// [RSFact]
        ///     [RF25
        ///         Ordinal=""
        ///         T102LeafID=""
        ///         T216LeafID=""
        ///         WIPCode=""
        ///         LotNumber=""
        ///         Texture=""
        ///         PWONo=""
        ///         BatchLot=""
        ///         Qty=""
        ///         Scale="" /]
        /// [/RSFact]
        /// </param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        public void usp_SaveFact_BatchProductionStart_N(
            int communityID,
            int t216LeafID,
            int t107LeafID,
            string operatorCode,
            string batchNumber,
            int t131LeafID,
            string rsFactXML,
            long sysLogID,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                #region 将函数参数加入 Hashtable 中
                Hashtable hashParams = new Hashtable();
                hashParams.Add("communityID", communityID);
                hashParams.Add("t216LeafID", t216LeafID);
                hashParams.Add("t107LeafID", t107LeafID);
                hashParams.Add("t131LeafID", t131LeafID);
                hashParams.Add("operatorCode", operatorCode);
                hashParams.Add("batchNumber", batchNumber);
                hashParams.Add("rsFactXML", rsFactXML);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    string.Format(
                        "执行存储过程 usp_SaveFact_BatchProductionStart_N，输入参数：" +
                        "CommunityID={0}|T216LeafID={1}|T107LeafID={2}|" +
                        "T131LeafID={3}|OperatorCode={4}|BatchNumber={5}" +
                        "|RSFactXML={6}|SysLogID={7}",
                        communityID, t216LeafID, t107LeafID, t131LeafID, operatorCode,
                        batchNumber, rsFactXML, sysLogID),
                    strProcedureName);
                #endregion

                #region 调用应用服务过程，并解析返回值
                using (WCFClient client = new WCFClient())
                {
                    object rlt = client.WCFRESTFul(
                        "IRAP.BL.MES.dll",
                        "IRAP.BL.MES.WorkOrder",
                        "usp_SaveFact_BatchProductionStart_N",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                errCode = -1001;
                errText = error.Message;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }


        /// <summary>
        /// 修改批次管理系统中的生产过程参数
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="linkedFactID">保存关联事实号</param>
        /// <param name="t216LeafID">产品叶标识</param>
        /// <param name="t107LeafID">工位叶标识</param>
        /// <param name="batchNumber">炉次号</param>
        /// <param name="rsFactXML">
        /// 工单信息列表
        /// [RSFact]
        ///     [RF25
        ///         Ordinal=""
        ///         T20LeafID=""
        ///         ParameterName=""
        ///         LowLimit=""
        ///         Criterion=""
        ///         HighLimit=""
        ///         Scale=""
        ///         UnitOfMeasure=""
        ///         Metric01="" /]
        /// [/RSFact]
        /// </param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        public void usp_SaveFact_BatchMethodParams_Modify(
            int communityID,
            long linkedFactID,
            int t216LeafID,
            int t107LeafID,
            string batchNumber,
            string rsFactXML,
            long sysLogID,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                #region 将函数调用参数加入 HashTable 中
                Hashtable hashParams = new Hashtable();
                hashParams.Add("communityID", communityID);
                hashParams.Add("linkedFactID", linkedFactID);
                hashParams.Add("t216LeafID", t216LeafID);
                hashParams.Add("t107LeafID", t107LeafID);
                hashParams.Add("batchNumber", batchNumber);
                hashParams.Add("rsFactXML", rsFactXML);
                hashParams.Add("sysLogID", sysLogID);
                WriteLog.Instance.Write(
                    "调用 usp_SaveFact_BatchMethodParams_Modify，输入参数：" +
                    $"CommunityID={communityID}|LinkedFactID={linkedFactID}|"+
                    $"T216LeafID={t216LeafID}|T107LeafID={t107LeafID}|"+
                    $"BatchNumber={batchNumber}|RSFactXML={rsFactXML}|"+
                    $"SysLogID={sysLogID}",
                    strProcedureName);
                #endregion

                #region 调用应用服务过程，并解析返回值
                using (WCFClient client = new WCFClient())
                {
                    object rlt = client.WCFRESTFul(
                        "IRAP.BL.MES.dll",
                        "IRAP.BL.MES.BatchSystem_Asimco",
                        "usp_SaveFact_BatchMethodParams_Modify",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = -1001;
                errText = error.Message;
                WriteLog.Instance.Write(errText, strProcedureName);
                WriteLog.Instance.Write(error.StackTrace, strProcedureName);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        public void usp_CancelDeliverMaterual(
            int communityID,
            long pwoIssuingFactID,
            out int errCode,
            out string errText)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                #region 将函数调用参数加入 HashTable 中
                Hashtable hashParams = new Hashtable();
                hashParams.Add("communityID", communityID);
                hashParams.Add("pwoIssuingFactID", pwoIssuingFactID);
                WriteLog.Instance.Write(
                    "调用 usp_CancelDeliverMaterual，输入参数：" +
                    $"CommunityID={communityID}|PWOIssuingFactID={pwoIssuingFactID}",
                    strProcedureName);
                #endregion

                #region 调用应用服务过程，并解析返回值
                using (WCFClient client = new WCFClient())
                {
                    object rlt = client.WCFRESTFul(
                        "IRAP.BL.MES.dll",
                        "IRAP.BL.MES.BatchSystem_Asimco",
                        "usp_CancelDeliverMaterual",
                        hashParams,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = -1001;
                errText = error.Message;
                WriteLog.Instance.Write(errText, strProcedureName);
                WriteLog.Instance.Write(error.StackTrace, strProcedureName);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }
    }
}