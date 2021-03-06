﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Collections;

using IRAP.Global;
using IRAPShared;
using IRAPORM;
using IRAP.Entities.SCES;
using IRAP.Entities.MES.Tables;
using IRAP.Entities.MDM.Tables;
using IRAP.Entities.SCES.Tables;

namespace IRAP.BL.SCES
{
    public class IRAPSCES : IRAPBLLBase
    {
        private static string className =
            MethodBase.GetCurrentMethod().DeclaringType.FullName;

        public IRAPSCES()
        {
            WriteLog.Instance.WriteLogFileName =
                MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        }

        /// <param name="communityID">社区标识</param>
        /// <param name="dstT173LeafID">目标仓储地点叶标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>List[ProductionWorkOrder]</returns>
        public IRAPJsonResult ufn_GetList_ProductionWorkOrdersToDeliverMaterial(
            int communityID,
            int dstT173LeafID,
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
                List<ProductionWorkOrder> datas = new List<ProductionWorkOrder>();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@DstT173LeafID", DbType.Int32, dstT173LeafID));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                WriteLog.Instance.Write(
                    string.Format(
                        "调用函数 IRAPSCES..ufn_GetList_ProductionWorkOrdersToDeliverMaterial，" +
                        "参数：CommunityID={0}|DstT173LeafID={1}|SysLogID={2}",
                        communityID,
                        dstT173LeafID,
                        sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                try
                {
                    using (IRAPSQLConnection conn = new IRAPSQLConnection())
                    {
                        string strSQL =
                                "SELECT * " +
                                "FROM IRAPSCES..ufn_GetList_ProductionWorkOrdersToDeliverMaterial(" +
                                "@CommunityID, @DstT173LeafID, @SysLogID) " +
                                "ORDER BY ScheduledStartTime";
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<ProductionWorkOrder> lstDatas =
                            conn.CallTableFunc<ProductionWorkOrder>(strSQL, paramList);
                        datas = lstDatas.ToList();
                        errCode = 0;
                        errText = string.Format("调用成功！共获得 {0} 条记录", datas.Count);
                        WriteLog.Instance.Write(errText, strProcedureName);
                    }
                }
                catch (Exception error)
                {
                    errCode = 99000;
                    errText = string.Format(
                        "调用 IRAPSCES..ufn_GetList_ProductionWorkOrdersToDeliverMaterial 函数发生异常：{0}",
                        error.Message);
                    WriteLog.Instance.Write(errText, strProcedureName);
                }
                #endregion

                return Json(datas);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <param name="communityID">社区标识</param>
        /// <param name="subMaterialCode">子项物料号</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>List[PWOToDeliverByMaterialCode]</returns>
        public IRAPJsonResult ufn_GetList_PWOToDeliverByMaterialCode(
            int communityID,
            string subMaterialCode,
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
                List<PWOToDeliverByMaterialCode> datas = 
                    new List<PWOToDeliverByMaterialCode>();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@MaterialCode", DbType.String, subMaterialCode));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                WriteLog.Instance.Write(
                    string.Format(
                        "调用函数 IRAPSCES..ufn_GetList_PWOToDeliverByMaterialCode，" +
                        "参数：CommunityID={0}|MaterialCode={1}|SysLogID={2}",
                        communityID,
                        subMaterialCode,
                        sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                try
                {
                    using (IRAPSQLConnection conn = new IRAPSQLConnection())
                    {
                        string strSQL =
                                "SELECT * " +
                                "FROM IRAPSCES..ufn_GetList_PWOToDeliverByMaterialCode(" +
                                "@CommunityID, @MaterialCode, @SysLogID) " +
                                "ORDER BY ScheduledStartTime, FactID";
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<PWOToDeliverByMaterialCode> lstDatas =
                            conn.CallTableFunc<PWOToDeliverByMaterialCode>(strSQL, paramList);
                        datas = lstDatas.ToList();
                        errCode = 0;
                        errText = string.Format("调用成功！共获得 {0} 条记录", datas.Count);
                        WriteLog.Instance.Write(errText, strProcedureName);
                    }
                }
                catch (Exception error)
                {
                    errCode = 99000;
                    errText = string.Format(
                        "调用 IRAPSCES..ufn_GetList_PWOToDeliverByMaterialCode 函数发生异常：{0}",
                        error.Message);
                    WriteLog.Instance.Write(errText, strProcedureName);
                }
                #endregion

                return Json(datas);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <param name="communityID">社区标识</param>
        /// <param name="dstT173LeafID">目标仓储地点叶标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>List[ProductionWorkOrder]</returns>
        public IRAPJsonResult ufn_GetList_ProductionWorkOrdersToDeliverMaterial_N(
            int communityID,
            int dstT173LeafID,
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
                List<ProductionWorkOrderEx> datas = new List<ProductionWorkOrderEx>();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@DstT173LeafID", DbType.Int32, dstT173LeafID));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                WriteLog.Instance.Write(
                    string.Format(
                        "调用函数 IRAPSCES..ufn_GetList_ProductionWorkOrdersToDeliverMaterial_N，" +
                        "参数：CommunityID={0}|DstT173LeafID={1}|SysLogID={2}",
                        communityID,
                        dstT173LeafID,
                        sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                try
                {
                    using (IRAPSQLConnection conn = new IRAPSQLConnection())
                    {
                        string strSQL =
                                "SELECT * " +
                                "FROM IRAPSCES..ufn_GetList_ProductionWorkOrdersToDeliverMaterial_N(" +
                                "@CommunityID, @DstT173LeafID, @SysLogID) " +
                                "ORDER BY ScheduledStartTime, FactID";
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<ProductionWorkOrderEx> lstDatas =
                            conn.CallTableFunc<ProductionWorkOrderEx>(strSQL, paramList);
                        datas = lstDatas.ToList();
                        errCode = 0;
                        errText = string.Format("调用成功！共获得 {0} 条记录", datas.Count);
                        WriteLog.Instance.Write(errText, strProcedureName);
                    }
                }
                catch (Exception error)
                {
                    errCode = 99000;
                    errText = string.Format(
                        "调用 IRAPSCES..ufn_GetList_ProductionWorkOrdersToDeliverMaterial_N 函数发生异常：{0}",
                        error.Message);
                    WriteLog.Instance.Write(errText, strProcedureName);
                }
                #endregion

                return Json(datas);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }


        /// <summary>
        /// 获取工单物料配送指令单
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="pwoIssuingFactID">工单签发事实编号</param>
        /// <param name="sysLogID">系统登录标识</param>
        public IRAPJsonResult ufn_GetList_MaterialToDeliverForPWO(
            int communityID, 
            long pwoIssuingFactID, 
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
                List<MaterialToDeliver> datas = new List<MaterialToDeliver>();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@PWOIssuingFactID", DbType.Int64, pwoIssuingFactID));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                WriteLog.Instance.Write(
                    string.Format(
                        "调用函数 IRAPSCES..ufn_GetList_MaterialToDeliverForPWO，" +
                        "参数：CommunityID={0}|PWOIssuingFactID={1}|SysLogID={2}",
                        communityID,
                        pwoIssuingFactID,
                        sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                try
                {
                    using (IRAPSQLConnection conn = new IRAPSQLConnection())
                    {
                        string strSQL =
                                "SELECT * " +
                                "FROM IRAPSCES..ufn_GetList_MaterialToDeliverForPWO(" +
                                "@CommunityID, @PWOIssuingFactID, @SysLogID)";
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<MaterialToDeliver> lstDatas =
                            conn.CallTableFunc<MaterialToDeliver>(strSQL, paramList);
                        datas = lstDatas.ToList();
                        errCode = 0;
                        errText = string.Format("调用成功！共获得 {0} 条记录", datas.Count);
                        WriteLog.Instance.Write(errText, strProcedureName);
                    }
                }
                catch (Exception error)
                {
                    errCode = 99000;
                    errText = string.Format(
                        "调用 IRAPSCES..ufn_GetList_MaterialToDeliverForPWO 函数发生异常：{0}",
                        error.Message);
                    WriteLog.Instance.Write(errText, strProcedureName);
                }
                #endregion

                return Json(datas);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 打印生产工单流转卡，并生成工单原辅材料配送临时记录
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="transactNo">申请到的交易号</param>
        /// <param name="pwoIssuingFactID">工单签发事实编号</param>
        /// <param name="srcT173LeafID">发料库房叶标识</param>
        /// <param name="dstT173LeafID">目标超市叶标识</param>
        /// <param name="dstT106LeafID_Recv">目标超市收料库位叶标识</param>
        /// <param name="dstT1LeafID_Recv">目标超市物料员部门叶标识</param>
        /// <param name="pickUpSheetXML">备料清单XML</param>
        /// <param name="sysLogID">系统登录标识</param>
        public IRAPJsonResult usp_PrintVoucher_PWOMaterialDelivery(
            int communityID,
            long transactNo,
            long pwoIssuingFactID,
            int srcT173LeafID,
            int dstT173LeafID,
            int dstT106LeafID_Recv,
            int dstT1LeafID_Recv,
            string pickUpSheetXML,
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
                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@TransactNo", DbType.Int64, transactNo));
                paramList.Add(new IRAPProcParameter("@PWOIssuingFactID", DbType.Int64, pwoIssuingFactID));
                paramList.Add(new IRAPProcParameter("@SrcT173LeafID", DbType.Int32, srcT173LeafID));
                paramList.Add(new IRAPProcParameter("@DstT173LeafID", DbType.Int32, dstT173LeafID));
                paramList.Add(new IRAPProcParameter("@DstT106LeafID_Recv", DbType.Int32, dstT106LeafID_Recv));
                paramList.Add(new IRAPProcParameter("@DstT1LeafID_Recv", DbType.Int32, dstT1LeafID_Recv));
                paramList.Add(new IRAPProcParameter("@PickUpSheetXML", DbType.String, pickUpSheetXML));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrCode",
                        DbType.Int32,
                        ParameterDirection.Output,
                        4));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrText",
                        DbType.String,
                        ParameterDirection.Output,
                        400));
                WriteLog.Instance.Write(
                    string.Format("执行存储过程 IRAPSCES..usp_PrintVoucher_PWOMaterialDelivery，参数：" +
                        "CommunityID={0}|TransactNo={1}|PWOIssuingFactID={2}|SrcT173LeafID={3}|" +
                        "DstT173LeafID={4}|DstT106LeafID_Recv={5}|DstT1LeafID_Recv={6}|" +
                        "PickUpSheetXML={7}|SysLogID={8}",
                        communityID, transactNo, pwoIssuingFactID, srcT173LeafID, dstT173LeafID,
                        dstT106LeafID_Recv, dstT1LeafID_Recv, pickUpSheetXML, sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                using (IRAPSQLConnection conn = new IRAPSQLConnection())
                {
                    IRAPError error = 
                        conn.CallProc(
                            "IRAPSCES..usp_PrintVoucher_PWOMaterialDelivery", 
                            ref paramList);
                    errCode = error.ErrCode;
                    errText = error.ErrText;
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);

                    Hashtable rtnParams = new Hashtable();
                    if (errCode == 0)
                    {
                        foreach (IRAPProcParameter param in paramList)
                        {
                            if (param.Direction == ParameterDirection.InputOutput || 
                                param.Direction == ParameterDirection.Output)
                            {
                                if (param.DbType == DbType.Int32 && param.Value == DBNull.Value)
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), 0);
                                else
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), param.Value);
                            }
                        }
                    }

                    foreach (DictionaryEntry entry in rtnParams)
                    {
                        WriteLog.Instance.Write(
                            string.Format(
                                "[{0}]=[{1}]",
                                entry.Key,
                                entry.Value),
                            strProcedureName);
                    }

                    return Json(rtnParams);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = 99000;
                errText = string.Format("调用 IRAPSCES..usp_PrintVoucher_PWOMaterialDelivery 时发生异常：{0}", error.Message);
                return Json(new Hashtable());
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <param name="communityID">社区标识</param>
        /// <param name="pwoIssuingFactID">工单签发事实编号</param>
        /// <param name="sysLogID">系统登录标识</param>
        public IRAPJsonResult ufn_GetList_ProductionFlowCard(
            int communityID, 
            long pwoIssuingFactID, 
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
                List<ProductionFlowCard> datas = new List<ProductionFlowCard>();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@PWOIssuingFactID", DbType.Int64, pwoIssuingFactID));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                WriteLog.Instance.Write(
                    string.Format(
                        "调用函数 IRAPSCES..ufn_GetList_ProductionFlowCard，" +
                        "参数：CommunityID={0}|PWOIssuingFactID={1}|SysLogID={2}",
                        communityID,
                        pwoIssuingFactID,
                        sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                try
                {
                    using (IRAPSQLConnection conn = new IRAPSQLConnection())
                    {
                        string strSQL =
                                "SELECT * " +
                                "FROM IRAPSCES..ufn_GetList_ProductionFlowCard(" +
                                "@CommunityID, @PWOIssuingFactID, @SysLogID)";
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<ProductionFlowCard> lstDatas =
                            conn.CallTableFunc<ProductionFlowCard>(strSQL, paramList);
                        datas = lstDatas.ToList();
                        errCode = 0;
                        errText = string.Format("调用成功！共获得 {0} 条记录", datas.Count);
                        WriteLog.Instance.Write(errText, strProcedureName);
                    }
                }
                catch (Exception error)
                {
                    errCode = 99000;
                    errText = string.Format(
                        "调用 IRAPSCES..ufn_GetList_ProductionFlowCard 函数发生异常：{0}",
                        error.Message);
                    WriteLog.Instance.Write(errText, strProcedureName);
                }
                #endregion

                return Json(datas);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="materialCode">物料代码</param>
        /// <param name="customerCode">物料标签代码</param>
        /// <param name="shipToParty">发运地点</param>
        /// <param name="qtyInStore">物料计划量</param>
        /// <param name="dateCode">物料生产日期</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        /// <returns></returns>
        public IRAPJsonResult usp_PokaYoke_PallPrint(
            int communityID,
            string materialCode,
            string customerCode,
            string shipToParty,
            string qtyInStore,
            string dateCode,
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
                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@MaterialCode", DbType.String, materialCode));
                paramList.Add(new IRAPProcParameter("@CustomerCode", DbType.String, customerCode));
                paramList.Add(new IRAPProcParameter("@ShipToParty", DbType.String, shipToParty));
                paramList.Add(new IRAPProcParameter("@QtyInStore", DbType.String, qtyInStore));
                paramList.Add(new IRAPProcParameter("@DateCode", DbType.String, dateCode));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrCode",
                        DbType.Int32,
                        ParameterDirection.Output,
                        4));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrText",
                        DbType.String,
                        ParameterDirection.Output,
                        400));
                WriteLog.Instance.Write(
                    string.Format("执行存储过程 IRAPSCES..usp_PokaYoke_PallPrint，参数：" +
                        "CommunityID={0}|MaterialCode={1}|CustomerCode={2}|"+
                        "ShipToParty={3}|QtyInStore={4}|DateCode={5}|SysLogID={6}",
                        communityID, materialCode, customerCode, shipToParty,
                        qtyInStore, dateCode, sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                using (IRAPSQLConnection conn = new IRAPSQLConnection())
                {
                    IRAPError error =
                        conn.CallProc(
                            "IRAPSCES..usp_PokaYoke_PallPrint",
                            ref paramList);
                    errCode = error.ErrCode;
                    errText = error.ErrText;
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);

                    Hashtable rtnParams = new Hashtable();
                    if (errCode == 0)
                    {
                        foreach (IRAPProcParameter param in paramList)
                        {
                            if (param.Direction == ParameterDirection.InputOutput ||
                                param.Direction == ParameterDirection.Output)
                            {
                                if (param.DbType == DbType.Int32 && param.Value == DBNull.Value)
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), 0);
                                else
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), param.Value);
                            }
                        }
                    }

                    foreach (DictionaryEntry entry in rtnParams)
                    {
                        WriteLog.Instance.Write(
                            string.Format(
                                "[{0}]=[{1}]",
                                entry.Key,
                                entry.Value),
                            strProcedureName);
                    }

                    return Json(rtnParams);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = 99000;
                errText = string.Format("调用 IRAPSCES..usp_PokaYoke_PallPrint 时发生异常：{0}", error.Message);
                return Json(new Hashtable());
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 获取生产工单发料的未结配送指令清单，包括：
        /// ⒈ 已排产但尚未配送的
        /// ⒉ 已配送但尚未接收的 
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="dstT173LeafID">目标仓储地点叶标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        public IRAPJsonResult ufn_GetList_UnclosedDeliveryOrdersForPWO(
            int communityID,
            int dstT173LeafID,
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
                List<UnclosedDeliveryOrdersForPWO> datas = 
                    new List<UnclosedDeliveryOrdersForPWO>();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@DstT173LeafID", DbType.Int32, dstT173LeafID));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                WriteLog.Instance.Write(
                    string.Format(
                        "调用函数 IRAPSCES..ufn_GetList_UnclosedDeliveryOrdersForPWO，" +
                        "参数：CommunityID={0}|DstT173LeafID={1}|SysLogID={2}",
                        communityID,
                        dstT173LeafID,
                        sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                try
                {
                    using (IRAPSQLConnection conn = new IRAPSQLConnection())
                    {
                        string strSQL =
                                "SELECT * " +
                                "FROM IRAPSCES..ufn_GetList_UnclosedDeliveryOrdersForPWO(" +
                                "@CommunityID, @DstT173LeafID, @SysLogID)";
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<UnclosedDeliveryOrdersForPWO> lstDatas =
                            conn.CallTableFunc<UnclosedDeliveryOrdersForPWO>(strSQL, paramList);
                        datas = lstDatas.ToList();
                        errCode = 0;
                        errText = string.Format("调用成功！共获得 {0} 条记录", datas.Count);
                        WriteLog.Instance.Write(errText, strProcedureName);
                    }
                }
                catch (Exception error)
                {
                    errCode = 99000;
                    errText = string.Format(
                        "调用 IRAPSCES..ufn_GetList_UnclosedDeliveryOrdersForPWO 函数发生异常：{0}",
                        error.Message);
                    WriteLog.Instance.Write(errText, strProcedureName);
                }
                #endregion

                return Json(datas);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 生产工单配送流转卡打印后实际配送操作前撤销
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="af482PK">辅助事实分区键</param>
        /// <param name="pwoIssuingFactID">工单签发事实编号</param>
        /// <param name="sysLogID">系统登录标识</param>
        public IRAPJsonResult usp_UndoPrintVoucher_PWOMaterialDelivery(
            int communityID,
            long af482PK,
            long pwoIssuingFactID,
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
                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@AF482PK", DbType.Int64, af482PK));
                paramList.Add(new IRAPProcParameter("@PWOIssuingFactID", DbType.Int64, pwoIssuingFactID));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrCode",
                        DbType.Int32,
                        ParameterDirection.Output,
                        4));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrText",
                        DbType.String,
                        ParameterDirection.Output,
                        400));
                WriteLog.Instance.Write(
                    string.Format("执行存储过程 IRAPSCES..usp_UndoPrintVoucher_PWOMaterialDelivery，参数：" +
                        "CommunityID={0}|AF482PK={1}|PWOIssuingFactID={2}|" +
                        "SysLogID={3}",
                        communityID, af482PK, pwoIssuingFactID, sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                using (IRAPSQLConnection conn = new IRAPSQLConnection())
                {
                    IRAPError error =
                        conn.CallProc(
                            "IRAPSCES..usp_UndoPrintVoucher_PWOMaterialDelivery",
                            ref paramList);
                    errCode = error.ErrCode;
                    errText = error.ErrText;
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);

                    Hashtable rtnParams = new Hashtable();
                    if (errCode == 0)
                    {
                        foreach (IRAPProcParameter param in paramList)
                        {
                            if (param.Direction == ParameterDirection.InputOutput ||
                                param.Direction == ParameterDirection.Output)
                            {
                                if (param.DbType == DbType.Int32 && param.Value == DBNull.Value)
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), 0);
                                else
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), param.Value);
                            }
                        }
                    }

                    foreach (DictionaryEntry entry in rtnParams)
                    {
                        WriteLog.Instance.Write(
                            string.Format(
                                "[{0}]=[{1}]",
                                entry.Key,
                                entry.Value),
                            strProcedureName);
                    }

                    return Json(rtnParams);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = 99000;
                errText = string.Format("调用 IRAPSCES..usp_UndoPrintVoucher_PWOMaterialDelivery 时发生异常：{0}", error.Message);
                return Json(new Hashtable());
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 根据制造订单号和制造订单行号，获取工单信息
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="dstT173LeafID">目标仓储地点</param>
        /// <param name="moNumber">制造订单号</param>
        /// <param name="moLineNo">制造订单行号</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>ReprintPWO</returns>
        public IRAPJsonResult mfn_GetInfo_PWOToReprint(
            int communityID,
            int dstT173LeafID,
            string moNumber,
            int moLineNo,
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
                ReprintPWO rlt = new ReprintPWO();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                //paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                //paramList.Add(new IRAPProcParameter("@AF482PK", DbType.Int64, af482PK));
                //paramList.Add(new IRAPProcParameter("@PWOIssuingFactID", DbType.Int64, pwoIssuingFactID));
                //paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                //paramList.Add(
                //    new IRAPProcParameter(
                //        "@ErrCode",
                //        DbType.Int32,
                //        ParameterDirection.Output,
                //        4));
                //paramList.Add(
                //    new IRAPProcParameter(
                //        "@ErrText",
                //        DbType.String,
                //        ParameterDirection.Output,
                //        400));
                //WriteLog.Instance.Write(
                //    string.Format("执行存储过程 IRAPSCES..usp_UndoPrintVoucher_PWOMaterialDelivery，参数：" +
                //        "CommunityID={0}|AF482PK={1}|PWOIssuingFactID={2}|" +
                //        "SysLogID={3}",
                //        communityID, af482PK, pwoIssuingFactID, sysLogID),
                //    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                using (IRAPSQLConnection conn = new IRAPSQLConnection())
                {
                    long partitioningKey = communityID * 1000000000000 + DateTime.Now.Year;

                    string strSQL =
                        string.Format(
                            "SELECT * " +
                            "FROM IRAPMES..AuxFact_PWOIssuing " +
                            "WHERE PartitioningKey={0} " +
                            " AND MONumber='{1}' AND MOLineNo={2}",
                            partitioningKey,
                            moNumber,
                            moLineNo);
                    WriteLog.Instance.Write(strSQL, strProcedureName);

                    IList<AuxFact_PWOIssuing> lstOrders =
                        conn.CallTableFunc<AuxFact_PWOIssuing>(strSQL, paramList);
                    if (lstOrders.Count == 0)
                    {
                        errCode = -1;
                        errText = string.Format("未找到[{0}]-[{1}]的制造订单", moNumber, moLineNo);
                        WriteLog.Instance.Write(errText, strProcedureName);
                        return Json(new ReprintPWO());
                    }
                    rlt.PWONo = lstOrders[0].WFInstanceID;
                    rlt.PlannedStartDate = lstOrders[0].ScheduledStartTime;
                    rlt.PlannedCloseDate = lstOrders[0].ScheduledCloseTime;
                    rlt.MONumber = lstOrders[0].MONumber;
                    rlt.MOLineNo = lstOrders[0].MOLineNo;
                    rlt.LotNumber = lstOrders[0].LotNumber;

                    #region 获取制造订单的配料信息
                    strSQL =
                        string.Format(
                            "SELECT * " +
                            "FROM IRAPSCES..ufn_GetList_MaterialToDeliverForPWO(" +
                            "{0}, {1}, {2})",
                            communityID,
                            lstOrders[0].FactID,
                            sysLogID);
                    WriteLog.Instance.Write(strSQL, strProcedureName);

                    IList<MaterialToDeliver> lstMaterials =
                        conn.CallTableFunc<MaterialToDeliver>(strSQL, paramList);
                    if (lstMaterials.Count == 0)
                    {
                        errCode = -1;
                        errText = string.Format("未找到[{0}]-[{1}]制造订单的配料信息", moNumber, moLineNo);
                        WriteLog.Instance.Write(errText, strProcedureName);
                        return Json(new ReprintPWO());
                    }
                    rlt.T173Code = lstMaterials[0].T173Code;
                    rlt.T173Name = lstMaterials[0].T173Name;
                    rlt.AtStoreLocCode = lstMaterials[0].AtStoreLocCode;
                    rlt.DstWorkShopCode = lstMaterials[0].DstWorkShopCode;
                    rlt.DstWorkShopDesc = lstMaterials[0].DstWorkShopDesc;
                    rlt.SuggestedQuantityToPick = lstMaterials[0].SuggestedQuantityToPick.ToString();
                    rlt.UnitOfMeasure = lstMaterials[0].UnitOfMeasure;
                    rlt.T131Code = lstMaterials[0].T131Code;
                    rlt.ActualQtyDecompose = lstMaterials[0].ActualQtyDecompose;
                    rlt.MaterialCode = lstMaterials[0].MaterialCode;
                    rlt.MaterialDesc = lstMaterials[0].MaterialDesc;
                    rlt.ActualQuantityToDeliver = lstMaterials[0].ActualQuantityToDeliver.ToString();
                    rlt.DstT106Code = lstMaterials[0].DstT106Code;
                    #endregion

                    #region 获取制造订单的 T134 父节点信息
                    strSQL =
                        string.Format(
                            "SELECT * " +
                            "FROM IRAPMDM..stb057 WHERE PartitioningKey={0} " +
                            "AND NodeID={1}",
                            communityID * 10000 + 134,
                            lstOrders[0].T134NodeID);
                    WriteLog.Instance.Write(strSQL, strProcedureName);

                    IList<Stb057> lstT134Nodes =
                        conn.CallTableFunc<Stb057>(strSQL, paramList);
                    if (lstT134Nodes.Count > 0)
                    {
                        rlt.T134Name =
                            string.Format(
                                "{0}-[{1}]",
                                lstT134Nodes[0].NodeName,
                                lstT134Nodes[0].Code);
                    }
                    #endregion

                    #region 获取工单的计划产量以及产品信息
                    strSQL =
                        string.Format(
                            "SELECT * " +
                            "FROM IRAPMES..TempFact_OLTP " +
                            "WHERE PartitioningKey={0} " +
                            "AND WFInstanceID='{1}' " +
                            "AND OpID=482 AND OpType=1",
                            DateTime.Now.Year * 1000000000000 + communityID * 10000 + 482,
                            rlt.PWONo);
                    WriteLog.Instance.Write(strSQL, strProcedureName);

                    IList<FixedFact_MES> lstFixedFacts =
                        conn.CallTableFunc<FixedFact_MES>(strSQL, paramList);
                    if (lstFixedFacts.Count != 0)
                    {
                        rlt.PlannedQuantity = lstFixedFacts[0].Metric01;
                        rlt.ProductNo = lstFixedFacts[0].Code01;

                        int t102LeafID = lstFixedFacts[0].Leaf01;
                        strSQL =
                            string.Format(
                                "SELECT * " +
                                "FROM IRAPMDM..stb058 " +
                                "WHERE PartitioningKey={0} " +
                                "AND LeafID={1}",
                                communityID * 10000 + 102,
                                t102LeafID);
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<Stb058> lst058 =
                            conn.CallTableFunc<Stb058>(strSQL, paramList);
                        if (lst058.Count > 0)
                        {
                            rlt.ProductDesc = lst058[0].NodeName;
                        }
                    }
                    #endregion

                    #region 如果社区标识是 60030，则需要获取 GateWayWC
                    if (communityID == 60030)
                    {
                        strSQL =
                            string.Format(
                                "SELECT * " +
                                "FROM IRAPSCES..ITEM_INFO " +
                                "WHERE T102LeafID={0}",
                                lstMaterials[0].LeafID);
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<ITEM_INFO> lstItems =
                            conn.CallTableFunc<ITEM_INFO>(strSQL, paramList);
                        if (lstItems.Count > 0)
                        {
                            rlt.GateWayWC = lstItems[0].GATEWAY_WC;
                        }
                    }
                    #endregion

                    errCode = 0;
                    errText = "获取成功";
                }
                #endregion

                return Json(rlt);
            }
            catch (Exception error)
            {
                errCode = 99000;
                errText = string.Format("调用 mfn_GetInfo_PWOToReprint 时发生异常：{0}", error.Message);
                WriteLog.Instance.Write(errText, strProcedureName);
                return Json(new ReprintPWO());
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 获取指定期间指定库房工单物料配送历史记录
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="t173LeafID">仓储地点叶标识</param>
        /// <param name="beginDT">开始日期时间</param>
        /// <param name="endDT">结束日期时间</param>
        public IRAPJsonResult ufn_GetFactList_RMTransferForPWO(
            int communityID,
            int t173LeafID,
            DateTime beginDT,
            DateTime endDT,
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
                List<RMTransferForPWO> datas = new List<RMTransferForPWO>();

                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@T173LeafID", DbType.Int32, t173LeafID));
                paramList.Add(new IRAPProcParameter("@BeginDT", DbType.DateTime2, beginDT));
                paramList.Add(new IRAPProcParameter("@EndDT", DbType.DateTime2, endDT));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                WriteLog.Instance.Write(
                    string.Format(
                        "调用函数 IRAPSCES..ufn_GetFactList_RMTransferForPWO，参数：" +
                        "CommunityID={0}|T173LeafID={1}|BeginDT={2}|" +
                        "EndDT={3}|SysLogID={4}",
                        communityID,
                        t173LeafID,
                        beginDT.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        endDT.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                try
                {
                    using (IRAPSQLConnection conn = new IRAPSQLConnection())
                    {
                        string strSQL =
                                "SELECT * " +
                                "FROM IRAPSCES..ufn_GetFactList_RMTransferForPWO(" +
                                "@CommunityID, @T173LeafID, @BeginDT, @EndDT, @SysLogID)";
                        WriteLog.Instance.Write(strSQL, strProcedureName);

                        IList<RMTransferForPWO> lstDatas =
                            conn.CallTableFunc<RMTransferForPWO>(strSQL, paramList);
                        datas = lstDatas.ToList();
                        errCode = 0;
                        errText = string.Format("调用成功！共获得 {0} 条记录", datas.Count);
                        WriteLog.Instance.Write(errText, strProcedureName);
                    }
                }
                catch (Exception error)
                {
                    errCode = 99000;
                    errText = string.Format(
                        "调用 IRAPSCES..ufn_GetFactList_RMTransferForPWO 函数发生异常：{0}",
                        error.Message);
                    WriteLog.Instance.Write(errText, strProcedureName);
                }
                #endregion

                return Json(datas);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 修改生产工单配送数量
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="factID">生产工单事实编号</param>
        /// <param name="actualQtyToDeliver">配送数量</param>
        /// <param name="subTreeID">子项物料树标识</param>
        /// <param name="subLeafID">子项物料叶标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        /// <returns></returns>
        public IRAPJsonResult usp_SaveFact_PWODeliveryQty(
            int communityID,
            long factID,
            long actualQtyToDeliver,
            int subTreeID,
            int subLeafID,
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
                #region 创建数据库调用参数组，并赋值
                IList<IDataParameter> paramList = new List<IDataParameter>();
                paramList.Add(new IRAPProcParameter("@CommunityID", DbType.Int32, communityID));
                paramList.Add(new IRAPProcParameter("@FactID", DbType.Int64, factID));
                paramList.Add(new IRAPProcParameter("@ActualQtyToDeliver", DbType.Int64, actualQtyToDeliver));
                paramList.Add(new IRAPProcParameter("@SubTreeID", DbType.Int32, subTreeID));
                paramList.Add(new IRAPProcParameter("@SubLeafID", DbType.Int32, subLeafID));
                paramList.Add(new IRAPProcParameter("@SysLogID", DbType.Int64, sysLogID));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrCode",
                        DbType.Int32,
                        ParameterDirection.Output,
                        4));
                paramList.Add(
                    new IRAPProcParameter(
                        "@ErrText",
                        DbType.String,
                        ParameterDirection.Output,
                        400));
                WriteLog.Instance.Write(
                    string.Format("执行存储过程 IRAPSCES..usp_SaveFact_PWODeliveryQty，参数：" +
                        "CommunityID={0}|FactID={1}|ActualQtyToDelivery={2}|SubTreeID={3}|"+
                        "SubLeafID={4}|SysLogID={5}",
                        communityID, factID, actualQtyToDeliver, subTreeID, subLeafID, sysLogID),
                    strProcedureName);
                #endregion

                #region 执行数据库函数或存储过程
                using (IRAPSQLConnection conn = new IRAPSQLConnection())
                {
                    IRAPError error =
                        conn.CallProc(
                            "IRAPSCES..usp_SaveFact_PWODeliveryQty",
                            ref paramList);
                    errCode = error.ErrCode;
                    errText = error.ErrText;
                    WriteLog.Instance.Write(
                        string.Format(
                            "({0}){1}",
                            errCode,
                            errText),
                        strProcedureName);

                    Hashtable rtnParams = new Hashtable();
                    if (errCode == 0)
                    {
                        foreach (IRAPProcParameter param in paramList)
                        {
                            if (param.Direction == ParameterDirection.InputOutput ||
                                param.Direction == ParameterDirection.Output)
                            {
                                if (param.DbType == DbType.Int32 && param.Value == DBNull.Value)
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), 0);
                                else
                                    rtnParams.Add(param.ParameterName.Replace("@", ""), param.Value);
                            }
                        }
                    }

                    foreach (DictionaryEntry entry in rtnParams)
                    {
                        WriteLog.Instance.Write(
                            string.Format(
                                "[{0}]=[{1}]",
                                entry.Key,
                                entry.Value),
                            strProcedureName);
                    }

                    return Json(rtnParams);
                }
                #endregion
            }
            catch (Exception error)
            {
                errCode = 99000;
                errText = string.Format("调用 IRAPSCES..usp_SaveFact_PWODeliveryQty 时发生异常：{0}", error.Message);
                return Json(new Hashtable());
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }
    }
}
