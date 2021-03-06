﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

using IRAP.Global;
using IRAP.Client.Global;
using IRAP.Client.User;
using IRAP.Client.SubSystem;
using IRAP.Entities.MES;
using IRAP.Entities.UTS;
using IRAP.Entities.MDM;
using IRAP.Entity.Kanban;
using IRAP.Entity.SSO;
using IRAP.Entity.MES;
using IRAP.WCF.Client.Method;

namespace IRAP.Client.GUI.MESPDC
{
    public partial class frmTroubleShooting_Base : IRAP.Client.Global.GUI.frmCustomFuncBase
    {
        private string className =
            MethodBase.GetCurrentMethod().DeclaringType.FullName;

        private List<SymbolInspecting> symbols = new List<SymbolInspecting>();
        private List<OperTypeInfo> repairActions = new List<OperTypeInfo>();
        private List<SubWIPIDCode_TroubleShooting> repairWIPIDs = new List<SubWIPIDCode_TroubleShooting>();

        private DataTable dtRepairItems = null;
        private DataTable dtSymbols = null;
        private DataTable dtRepairActions = null;
        private DataTable dtRepairWIPs = null;
        private DataTable dtInspectStatus = null;
        private List<FailureMode> failureModes = new List<FailureMode>();
        private List<DefectRootCause> defectRootCauses = new List<DefectRootCause>();
        private List<LeafSetEx> failureNatures = new List<LeafSetEx>();
        private List<LeafSetEx> failureDuties = new List<LeafSetEx>();
        private List<LeafSetEx> repairModes = new List<LeafSetEx>();
        private int currentProductLeaf = 0;
        private int currentWorkUnitLeaf = 0;
        /// <summary>
        /// 来源工位
        /// </summary>
        private int srcT107LeafID = 0;
        /// <summary>
        /// 来源工序
        /// </summary>
        private int srcT216LeafID = 0;
        /// <summary>
        /// 检查失效或测试失败行集事实表分区键
        /// </summary>
        private long rsFactPK = 0;
        /// <summary>
        /// 检查失效或测试失败事实编号
        /// </summary>
        private long linkedFactID = 0;
        /// <summary>
        /// 测试通道数
        /// </summary>
        private int numOfTestChannels = 0;
        /// <summary>
        /// 当前待维修产品的生产工单号
        /// </summary>
        private string pwoNo = "";

        private string caption = "";
        private string cultureName = "";

        public frmTroubleShooting_Base()
        {
            InitializeComponent();

            cultureName = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2).ToUpper();
            if (Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2) == "en")
                caption = "System tips";
            else
                caption = "系统信息";
        }

        public void FormComponentsInit()
        {
            if (Options.SelectStation != null)
            {
                currentWorkUnitLeaf = Options.SelectStation.T107LeafID;
            }
            else
            {
                currentWorkUnitLeaf = 0;
            }
            if (Options.SelectProduct != null)
            {
                currentProductLeaf = Options.SelectProduct.T102LeafID;
            }
            else
            {
                currentProductLeaf = 0;
            }

            if (dtRepairWIPs != null)
            {
                grdProducts.DataSource = dtRepairWIPs;

                grdvProducts.OptionsBehavior.Editable = true;
                grdvProducts.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            }

            if (dtRepairItems != null)
            {
                grdRepairItems.DataSource = dtRepairItems;
            }

            if (dtSymbols != null)
            {
                GetSymbols();
                risluSymbol.DataSource = dtSymbols;
                risluSymbol.DisplayMember = "ItemName";
                risluSymbol.ValueMember = "ItemLeaf";
                risluSymbol.NullText = "";
            }

            GetRepairActions();
            riluRepairAction.DataSource = repairActions;
            riluRepairAction.DisplayMember = "OpTypeName";
            riluRepairAction.ValueMember = "OpType";
            riluRepairAction.NullText = "";

            GetDefectRootCauses();
            riluDefectRootCauses.DataSource = defectRootCauses;
            riluDefectRootCauses.NullText = "";

            GetFailureNature();
            riluFailureNature.DataSource = failureNatures;
            riluFailureNature.NullText = "";

            GetFailureDuties();
            riluFailureDuty.DataSource = failureDuties;
            riluFailureDuty.NullText = "";

            GetRepairModes();
            riluRepairMode.DataSource = repairModes;
            riluRepairMode.NullText = "";
        }

        /// <summary>
        /// 创建临时表
        /// </summary>
        private DataTable InitializeDataTable(string tableName)
        {
            DataTable dt = new DataTable(tableName);
            switch (tableName.ToLower())
            {
                case "symbols":
                    dt.Columns.Add("Ordinal", typeof(int));
                    dt.Columns.Add("ItemLeaf", typeof(int));
                    dt.Columns.Add("ItemName", typeof(string));
                    dt.Columns.Add("AlternateItemName", typeof(string));
                    dt.Columns.Add("ItemType", typeof(int));            // ItemLeaf 的 TreeID
                    break;
                case "repairactions":
                    dt.Columns.Add("OpType", typeof(int));
                    dt.Columns.Add("OpTypeName", typeof(string));
                    break;
                case "repairwips":
                    dt.Columns.Add("PartNumber", typeof(string));
                    dt.Columns.Add("WIPCode", typeof(string));
                    dt.Columns.Add("InspectStatus", typeof(string));
                    dt.Columns.Add("RepairStatus", typeof(int));
                    break;
                case "inspectstatus":
                    dt.Columns.Add("InspectName", typeof(string));
                    dt.Columns.Add("InspectStatusID", typeof(int));
                    break;
            }
            return dt;
        }

        private void GetSymbols()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                if (CurrentOptions.Instance.OptionOne == null ||
                    CurrentOptions.Instance.OptionTwo == null)
                {
                    WriteLog.Instance.Write("还没有配置工位列表和产品列表", strProcedureName);
                    IRAPMessageBox.Instance.ShowErrorMessage("还没有配置工位列表和产品列表", caption);
                    return;
                }

                int errCode = 0;
                string errText = "";

                IRAPMDMClient.Instance.ufn_GetList_Symbols_Inspecting(
                    IRAPUser.Instance.CommunityID,
                    IRAPUser.Instance.SysLogID,
                    CurrentOptions.Instance.OptionTwo.T102LeafID,
                    0,
                    ref symbols,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(string.Format("({0}){1}", errCode, errText), strProcedureName);

                if (errCode == 0)
                {
                    dtSymbols.Clear();
                    foreach (SymbolInspecting symbol in symbols)
                    {
                        if (symbol.T110LeafID != 0)
                        {
                            dtSymbols.Rows.Add(
                                new object[]
                                {
                                    symbol.Ordinal,
                                    symbol.T110LeafID,
                                    symbol.T110Code,
                                    symbol.ComponentCode,
                                    "110",
                                });
                        }
                        else
                        {
                            dtSymbols.Rows.Add(
                                new object[]
                                {
                                    symbol.Ordinal,
                                    symbol.ComponentLeafID,
                                    symbol.ComponentCode,
                                    "",
                                    symbol.ComponentTreeID,
                                });
                        }
                    }
                }
                else
                {
                    IRAPMessageBox.Instance.ShowErrorMessage(errText, caption);
                }
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private DataTable GetSymbols(int pwoCategoryLeaf, List<SymbolInspecting> symbols)
        {
            DataTable rlt = InitializeDataTable("Symbols");
            foreach (SymbolInspecting symbol in symbols)
            {
                if (symbol.PWOCategoryLeaf == pwoCategoryLeaf)
                    if (symbol.T110LeafID != 0)
                    {
                        rlt.Rows.Add(
                            new object[]
                            {
                                    symbol.Ordinal,
                                    symbol.T110LeafID,
                                    symbol.T110Code,
                                    symbol.ComponentCode,
                                    "110",
                            });
                    }
                    else
                    {
                        rlt.Rows.Add(
                            new object[]
                            {
                                    symbol.Ordinal,
                                    symbol.ComponentLeafID,
                                    symbol.ComponentCode,
                                    "",
                                    symbol.ComponentTreeID,
                            });
                    }
            }
            return rlt;
        }

        /// <summary>
        /// 获取指定工位的失效模式清单
        /// </summary>
        /// <param name="srcWorkUnitLeaf"></param>
        private void GetFailureModes(int srcWorkUnitLeaf)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            if (CurrentOptions.Instance.OptionOne == null ||
                CurrentOptions.Instance.OptionTwo == null)
            {
                WriteLog.Instance.Write("还没有配置工位列表和产品列表", strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage("还没有配置工位列表和产品列表", caption);
                return;
            }

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                IRAPMDMClient.Instance.ufn_GetList_FailureModes(
                    IRAPUser.Instance.CommunityID,
                    currentProductLeaf,
                    srcWorkUnitLeaf,
                    IRAPUser.Instance.SysLogID,
                    ref failureModes,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(string.Format("({0}){1}", errCode, errText), strProcedureName);
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        public void GetDefectRootCauses()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            if (CurrentOptions.Instance.OptionOne == null ||
                CurrentOptions.Instance.OptionTwo == null)
            {
                WriteLog.Instance.Write("还没有配置工位列表和产品列表", strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage("还没有配置工位列表和产品列表", caption);
                return;
            }

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                IRAPMDMClient.Instance.ufn_GetList_DefectRootCauses(
                    IRAPUser.Instance.CommunityID,
                    IRAPUser.Instance.SysLogID,
                    currentProductLeaf,
                    0,
                    ref defectRootCauses,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(string.Format("({0}){1}", errCode, errText), strProcedureName);
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private List<DefectRootCause> GetFailureSrcOperation(
            int pwoCategoryLeaf, 
            List<DefectRootCause> sources)
        {
            List<DefectRootCause> rlt = new List<DefectRootCause>();
            foreach (DefectRootCause causes in sources)
            {
                if (causes.PWOCategoryLeaf == pwoCategoryLeaf)
                    rlt.Add(causes.Clone());
            }
            return rlt;
        }

        public void GetFailureNature()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                IRAPKBClient.Instance.sfn_AccessibleLeafSetEx(
                    IRAPUser.Instance.CommunityID,
                    183,
                    IRAPUser.Instance.ScenarioIndex,
                    IRAPUser.Instance.SysLogID,
                    ref failureNatures,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format(
                        "({0}){1}", errCode, errText), 
                    strProcedureName);

                for (int i = failureNatures.Count - 1; i >= 0; i--)
                {
                    if (failureNatures[i].LeafStatus != 0)
                        failureNatures.RemoveAt(i);
                }
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        public void GetFailureDuties()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                IRAPKBClient.Instance.sfn_AccessibleLeafSetEx(
                    IRAPUser.Instance.CommunityID,
                    184,
                    IRAPUser.Instance.ScenarioIndex,
                    IRAPUser.Instance.SysLogID,
                    ref failureDuties,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText), 
                    strProcedureName);

                for (int i = failureDuties.Count - 1; i >= 0; i--)
                {
                    if (failureDuties[i].LeafStatus != 0)
                        failureDuties.RemoveAt(i);
                }
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        public void GetRepairModes()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                IRAPKBClient.Instance.sfn_AccessibleLeafSetEx(
                    IRAPUser.Instance.CommunityID,
                    119,
                    1,
                    IRAPUser.Instance.SysLogID,
                    ref repairModes,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText), 
                    strProcedureName);
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void GetRepairActions()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                IRAPUTSClient.Instance.sfn_OperTypes(
                    11,
                    IRAPUser.Instance.LanguageID,
                    ref repairActions,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText), 
                    strProcedureName);

                if (errCode == 0)
                {
                    dtRepairActions.Clear();
                    foreach (OperTypeInfo action in repairActions)
                    {
                        dtRepairActions.Rows.Add(new object[]
                            {
                                action.OpType,
                                action.OpTypeName,
                            });
                    }
                }
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void RefreshRepairItemColumns()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";
                List<TSFormCtrl> columns = new List<TSFormCtrl>();
                MenuInfo menuInfo = (MenuInfo)this.Tag;

                IRAPMESTSClient.Instance.ufn_GetList_TSFormCtrl(
                    IRAPUser.Instance.CommunityID,
                    menuInfo.ItemID,
                    IRAPConst.Instance.IRAP_PROGLANGUAGEID,
                    IRAPUser.Instance.SysLogID,
                    ref columns,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText), 
                    strProcedureName);
                if (errCode == 0)
                {
                    foreach (TSFormCtrl column in columns)
                    {
                        grdvRepairItems.Columns[column.Ordinal].Visible = column.IsValid;
                    }
                }
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void GetSubWIPIDCodesWithBarCode(string wipPattern)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            repairWIPIDs.Clear();
            if (Options.SelectStation == null ||
                Options.SelectProduct == null)
            {
                WriteLog.Instance.Write("还没有配置产品列表和工位列表", strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage("还没有配置产品列表和工位列表", caption);
                return;
            }

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";
                List<SubWIPIDCodes_TS> subIDs = new List<SubWIPIDCodes_TS>();

                IRAPMESTSClient.Instance.ufn_GetList_SubWIPIDCodes_TS(
                    IRAPUser.Instance.CommunityID,
                    wipPattern,
                    currentProductLeaf,
                    currentWorkUnitLeaf,
                    IRAPUser.Instance.SysLogID,
                    ref subIDs,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText), 
                    strProcedureName);
                if (errCode == 0)
                {
                    foreach (SubWIPIDCodes_TS subID in subIDs)
                    {
                        SubWIPIDCode_TroubleShooting repairWIPIDCode = new SubWIPIDCode_TroubleShooting();
                        repairWIPIDCode.CopyFrom(subID);
                        repairWIPIDs.Add(repairWIPIDCode);
                    }
                }
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private WIPIDCode GetBarCodeInfo()
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WIPIDCode rlt = new WIPIDCode();

            if (Options.SelectStation == null ||
                Options.SelectProduct == null)
            {
                WriteLog.Instance.Write("还没有配置产品列表和工位列表", strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage("还没有配置产品列表和工位列表", caption);
                return null;
            }

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                IRAPMESClient.Instance.ufn_GetInfo_WIPIDCode(
                    IRAPUser.Instance.CommunityID,
                    edtBarCode.Text.Trim(),
                    currentProductLeaf,
                    currentWorkUnitLeaf,
                    false,
                    IRAPUser.Instance.SysLogID,
                    ref rlt,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText), 
                    strProcedureName);

                if (errCode != 0)
                {
                    IRAPMessageBox.Instance.ShowErrorMessage(errText, caption);
                }

                return rlt;
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
                return null;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void Clear()
        {
            repairWIPIDs.Clear();
            dtRepairWIPs.Clear();
            dtRepairItems.Clear();

            lblStatus.Text = "";

            grdRepairItems.DataSource = null;
            grdvRepairItems.OptionsBehavior.Editable = false;
            grdvRepairItems.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            grdRepairItems.UseEmbeddedNavigator = false;

            btnShowTestData.Enabled = false;
            btnBarCodeConf.Enabled = false;
        }

        private void AfterOptionChanged(object sender, EventArgs e)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                edtBarCode.Text = "";
                Clear();
                edtBarCode.Focus();

                FormComponentsInit();
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void GetFailuresOfNGProduct(SubWIPIDCode_TroubleShooting subWIPIDCode_TroubleShooting)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";
                List<FailuresOfNGProduct> failures = new List<FailuresOfNGProduct>();

                IRAPMESTSClient.Instance.ufn_GetList_FailuresOfNGProduct(
                    IRAPUser.Instance.CommunityID,
                    currentProductLeaf,
                    0,
                    subWIPIDCode_TroubleShooting.SubWIPIDCode,
                    rsFactPK,
                    subWIPIDCode_TroubleShooting.LinkedFactID,
                    IRAPUser.Instance.SysLogID,
                    ref failures,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(string.Format("({0}){1}", errCode, errText), strProcedureName);
                if (errCode == 0)
                {
                    foreach (FailuresOfNGProduct failure in failures)
                    {
                        subWIPIDCode_TroubleShooting.TSItems.Add(new SubWIPIDCode_TSItem()
                        {
                            T101LeafID = failure.T101LeafID,
                            T110LeafID = failure.T110LeafID,
                            T118LeafID = failure.T118LeafID,
                            FailurePointCount = failure.CntDefect,
                            T216LeafID = failure.T216LeafID,
                            T183LeafID = failure.T183LeafID,
                            T184LeafID = failure.T184LeafID,
                            IsInspectItem = true,
                        });
                    }
                    subWIPIDCode_TroubleShooting.PutListIntoDataTable();
                }
                else
                {
                    subWIPIDCode_TroubleShooting.TSItemsDT.Clear();
                    IRAPMessageBox.Instance.ShowErrorMessage(errText, caption);
                }
            }
            catch (Exception error)
            {
                subWIPIDCode_TroubleShooting.TSItemsDT.Clear();
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void frmTroubleShooting_Activated(object sender, EventArgs e)
        {
            Options.Visible = true;
            Options.ShowSwitchButton(true);

            FormComponentsInit();
        }

        private void edtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            if (e.KeyCode == Keys.Enter)
            {
                Clear();

                if (Options.SelectStation == null ||
                    Options.SelectProduct == null)
                {
                    WriteLog.Instance.Write("还没有配置产品列表和工位列表", strProcedureName);
                    IRAPMessageBox.Instance.ShowErrorMessage("还没有配置产品列表和工位列表", caption);
                    return;
                }

                if (edtBarCode.Text.Trim() == "")
                {
                    return;
                }

                WriteLog.Instance.WriteBeginSplitter(strProcedureName);
                try
                {
                    int errCode = 0;
                    string errText = "";
                    int productLeaf = currentProductLeaf;
                    string wipPattern = "";
                    string barCode = edtBarCode.Text.Trim();

                    WriteLog.Instance.Write("不良在制品路由校验", strProcedureName);
                    IRAPMESTSClient.Instance.usp_PokaYoke_TroubleShooting(
                        IRAPUser.Instance.CommunityID,
                        ref productLeaf,
                        currentWorkUnitLeaf,
                        edtBarCode.Text.Trim(),
                        IRAPUser.Instance.SysLogID,
                        out pwoNo,
                        out wipPattern,
                        out srcT107LeafID,
                        out srcT216LeafID,
                        out rsFactPK,
                        out linkedFactID,
                        out numOfTestChannels,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}", errCode, errText), 
                        strProcedureName);
                    WriteLog.Instance.Write(
                        string.Format(
                            "得到返回值：ProductLeaf={0}|WIPPartten={1}|SrcT107LeafID={2}|" +
                            "SrcT216LeafID={3}|RSFactPK={4}|LinkedFactID={5}" +
                            "NumTestChannels={6}|PWONo={7}",
                            productLeaf,
                            wipPattern,
                            srcT107LeafID,
                            srcT216LeafID,
                            rsFactPK,
                            linkedFactID,
                            numOfTestChannels,
                            pwoNo),
                        strProcedureName);

                    btnShowTestData.Enabled = (rsFactPK != 0 && linkedFactID != 0);

                    if (errCode == 0)
                    {
                        lblStatus.Text = errText;
                        lblStatus.ForeColor = Color.Blue;

                        #region 当前产品和扫描条码的产品是否一致？如果不一致则自动切换，如果切换失败，则报错
                        if (currentProductLeaf != productLeaf)
                        {
                            Options.RefreshOptionTwo(productLeaf);
                            if (Options.SelectProduct.T102LeafID != productLeaf)
                            {
                                WriteLog.Instance.Write("产品切换失败！", strProcedureName);
                                IRAPMessageBox.Instance.ShowErrorMessage(
                                    "产品切换失败，当前被扫描的在制品不能在本工位进行维修！", 
                                    caption);
                                return;
                            }
                            else
                            {
                                currentProductLeaf = productLeaf;

                                edtBarCode.Text = barCode;
                                lblStatus.Text = errText;
                                lblStatus.ForeColor = Color.Blue;
                            }
                        }
                        #endregion

                        #region 获取该条码的子板信息
                        GetSubWIPIDCodesWithBarCode(wipPattern);
                        #endregion

                        #region 获取失效模式
                        GetFailureModes(srcT107LeafID);
                        risluFailureMode.DataSource = failureModes;
                        risluFailureMode.DisplayMember = "FailureName";
                        risluFailureMode.ValueMember = "FailureLeaf";
                        risluFailureMode.NullText = "";
                        #endregion

                        foreach (SubWIPIDCode_TroubleShooting repairWIPID in repairWIPIDs)
                        {
                            repairWIPID.PWONo = pwoNo;

                            dtRepairWIPs.Rows.Add(new object[]
                                        {
                                            repairWIPID.PartNumber,
                                            repairWIPID.SubWIPIDCode,
                                            repairWIPID.LinkedFactID == 0 ? 0 : 1,
                                            repairWIPID.RepairStatus,
                                        });
                        }
                        grdvProducts.BestFitColumns();

                        btnBarCodeConf.Enabled = true;
                    }
                    else
                    {
                        lblStatus.Text = errText;
                        lblStatus.ForeColor = Color.Red;
                        edtBarCode.SelectAll();
                        edtBarCode.Focus();
                        return;
                    }
                }
                finally
                {
                    WriteLog.Instance.WriteEndSplitter(strProcedureName);
                }
            }
        }

        private void frmTroubleShooting_Load(object sender, EventArgs e)
        {
            #region 刷新维修情况中的项目
            RefreshRepairItemColumns();
            #endregion

            dtRepairItems = InitializeDataTable("RepairItems");
            dtSymbols = InitializeDataTable("Symbols");
            dtRepairActions = InitializeDataTable("RepairActions");
            dtRepairWIPs = InitializeDataTable("RepairWIPs");
            dtInspectStatus = InitializeDataTable("InspectStatus");

            dtInspectStatus.Rows.Add(new object[] { "正常", 0 });
            dtInspectStatus.Rows.Add(new object[] { "待修", 1 });
            riluInspectStatus.DataSource = dtInspectStatus;
            riluInspectStatus.NullText = "";

            switch (IRAPUser.Instance.CommunityID)
            {
                case 60013:
                    grdclmnT118LeafID.Caption = "不良现象";
                    grdclmnT216LeafID.Visible = false;
                    grdclmnT183LeafID.Caption = "责任区分";
                    grdclmnT184LeafID.Caption = "责任部门";
                    grdclmnTrackReferenceValue.Visible = false;
                    break;
            }

            Options.OptionChanged += AfterOptionChanged;

            FormComponentsInit();
        }

        private void grdvProducts_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            #region 从选择的子板维修项中加载维修项目
            int index = grdvProducts.GetFocusedDataSourceRowIndex();
            if (index >= 0 && index < repairWIPIDs.Count)
            {
                #region 获取检查失效项
                if (repairWIPIDs[index].LinkedFactID != 0 && 
                    repairWIPIDs[index].TSItemsDT.Rows.Count <= 0)
                {
                    GetFailuresOfNGProduct(repairWIPIDs[index]);
                }
                #endregion

                grdRepairItems.DataSource = repairWIPIDs[index].TSItemsDT;
                for (int i = 0; i < grdvRepairItems.Columns.Count; i++)
                {
                    if (grdvRepairItems.Columns[i].Visible)
                        grdvRepairItems.Columns[i].BestFit();
                }

                risluSymbol.DataSource = 
                    GetSymbols(
                        repairWIPIDs[index].PWOCategoryLeaf, 
                        symbols);
                riluDefectRootCauses.DataSource = 
                    GetFailureSrcOperation(
                        repairWIPIDs[index].PWOCategoryLeaf, 
                        defectRootCauses);

                grdvRepairItems.OptionsBehavior.Editable = true;
                grdvRepairItems.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                grdRepairItems.UseEmbeddedNavigator = true;
            }
            else
            {
                grdvRepairItems.OptionsBehavior.Editable = false;
                return;
            }
            #endregion
        }

        private void grdvRepairItems_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            DataRow dr = grdvRepairItems.GetDataRow(e.RowHandle);
            dr["ItemLeafID"] = 0;
            dr["ItemLeafTreeID"] = 0;
            dr["T118LeafID"] = 0;
            dr["FailurePointCount"] = 0;
            dr["T216LeafID"] = 0;
            dr["T183LeafID"] = 0;
            dr["T184LeafID"] = 0;
            dr["TrackReferenceValue"] = "";
            dr["IsInspectItem"] = false;
            grdvRepairItems.LayoutChanged();
        }

        private void btnBarCodeConf_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < repairWIPIDs.Count; i++)
            {
                if ((int)dtRepairWIPs.Rows[i]["RepairStatus"] <= 0)
                {
                    XtraMessageBox.Show(
                        "还有子在制品没有选择“维修状态”",
                        caption,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                repairWIPIDs[i].RepairStatus = (int)dtRepairWIPs.Rows[i]["RepairStatus"];
            }

            int destT216LeafID = 0;

            #region 如果维修状态非全4或全5，则要求选择目标工序
            {
                int intFailuredCount = 0;
                foreach (SubWIPIDCode_TroubleShooting item in repairWIPIDs)
                {
                    if (item.RepairStatus == 4 || item.RepairStatus == 5)
                        intFailuredCount++;
                }

                if (intFailuredCount < repairWIPIDs.Count)
                {
                    destT216LeafID =
                        Dialogs.frmSelectDestT216LeafID.Instance.DestT216LeafID(
                            IRAPUser.Instance.CommunityID,
                            currentProductLeaf,
                            currentWorkUnitLeaf,
                            srcT107LeafID,
                            GenerateT119LeafListInRepairItems(repairWIPIDs),
                            IRAPUser.Instance.SysLogID);
                    if (destT216LeafID < 0)
                        return;
                }
            }
            #endregion

            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);
            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                foreach (SubWIPIDCode_TroubleShooting item in repairWIPIDs)
                {
                    item.GetListFromDataTable();
                }

                IRAPMESTSClient.Instance.usp_SaveFact_TroubleShooting(
                    IRAPUser.Instance.CommunityID,
                    IRAPUser.Instance.UserCode,
                    currentProductLeaf,
                    currentWorkUnitLeaf,
                    CurrentOptions.Instance.OptionOne.T216LeafID,
                    CurrentOptions.Instance.OptionOne.T216Code,
                    destT216LeafID,
                    repairWIPIDs,
                    IRAPUser.Instance.SysLogID,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(string.Format("({0}){1}", errCode, errText), strProcedureName);
                if (errCode == 0)
                {
                    Clear();
                    edtBarCode.Text = "";
                    edtBarCode.Focus();
                    return;
                }
                else
                {
                    IRAPMessageBox.Instance.ShowErrorMessage(errText, caption);
                }
            }
            catch (Exception error)
            {
                WriteLog.Instance.Write(error.Message, strProcedureName);
                IRAPMessageBox.Instance.ShowErrorMessage(error.Message, caption);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void grdvProducts_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            grdvProducts_RowClick(grdvProducts, null);
        }

        private void frmTroubleShooting_Enter(object sender, EventArgs e)
        {
            if (Options.SelectStation != null)
            {
                currentWorkUnitLeaf = Options.SelectStation.T107LeafID;
            }
            else
            {
                currentWorkUnitLeaf = 0;
            }
            if (Options.SelectProduct != null)
            {
                currentProductLeaf = Options.SelectProduct.T102LeafID;
            }
            else
            {
                currentProductLeaf = 0;
            }
        }

        private void frmTroubleShooting_FormClosed(object sender, FormClosedEventArgs e)
        {
            Options.OptionChanged -= AfterOptionChanged;
        }

        private void btnShowTestData_Click(object sender, EventArgs e)
        {
            using (Dialogs.frmRSFactListTestData formTestData = 
                new Dialogs.frmRSFactListTestData())
            {
                formTestData.ShowDialog(rsFactPK, linkedFactID, numOfTestChannels);
            }
        }

        private void riteFailureMode_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (e.Value is int)
            {
                foreach (FailureMode fMode in failureModes)
                {
                    if (fMode.FailureLeaf == (int)e.Value)
                    {
                        e.DisplayText = fMode.FailureCode;
                        break;
                    }
                }
            }

            e.DisplayText = "";
        }

        private void riteFailureMode_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
            if (e.Value is string)
            {
                foreach (FailureMode fMode in failureModes)
                {
                    if (fMode.FailureCode == e.Value.ToString())
                    {
                        e.Value = fMode.FailureLeaf;
                        e.Handled = true;

                        return;
                    }
                }
                e.Value = 0;
            }
            else
            {
                e.Value = 0;
            }
        }

        private void grdvRepairItems_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            GridView view = sender as GridView;
            GridColumn colT119LeafID = view.Columns["T119LeafID"];
            GridColumn colT110LeafID = view.Columns["ItemLeafID"];
            GridColumn colT10xLeafID = view.Columns["T10xLeafID"];
            GridColumn colItemTreeID = view.Columns["ItemLeafTreeID"];
            GridColumn colSKUID1 = view.Columns["SKUID1"];
            GridColumn colSKUID2 = view.Columns["SKUID2"];

            int t119LeafID = (int)view.GetRowCellValue(e.RowHandle, colT119LeafID);
            int t110LeafID = (int)view.GetRowCellValue(e.RowHandle, colT110LeafID);
            int t101LeafID = 0;
            int itemTreeID = 0;

            foreach (SymbolInspecting symbol in symbols)
            {
                if (symbol.T110LeafID == t110LeafID)
                {
                    t101LeafID = symbol.ComponentLeafID;
                    view.SetRowCellValue(e.RowHandle, colT10xLeafID, t101LeafID);

                    itemTreeID = 110;
                    view.SetRowCellValue(e.RowHandle, colItemTreeID, itemTreeID);

                    view.UpdateCurrentRow();
                    break;
                }
                if (symbol.ComponentLeafID == t110LeafID)
                {
                    t101LeafID = symbol.ComponentLeafID;
                    view.SetRowCellValue(e.RowHandle, colT10xLeafID, t101LeafID);

                    itemTreeID = symbol.ComponentTreeID;
                    view.SetRowCellValue(e.RowHandle, colItemTreeID, itemTreeID);

                    view.UpdateCurrentRow();
                    break;
                }
            }

            if (t119LeafID == 10700)
            {
                WriteLog.Instance.WriteBeginSplitter(strProcedureName);
                try
                {
                    int errCode = 0;
                    string errText = "";
                    string skuID = "";

                    #region 获取更换后的 SKUID
                    IRAPMESTSClient.Instance.ufn_GetFIFOSKUIDinTSSite(
                        IRAPUser.Instance.CommunityID,
                        CurrentOptions.Instance.OptionOne.T107LeafID,
                        t101LeafID,
                        IRAPUser.Instance.SysLogID,
                        ref skuID,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}", errCode, errText),
                        strProcedureName);
                    if (errCode != 0)
                    {
                        XtraMessageBox.Show(
                            errText,
                            caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        view.SetColumnError(colT110LeafID, errText);
                        e.Valid = false;

                        return;
                    }
                    else
                    {
                        if (skuID == "")
                        {
                            XtraMessageBox.Show(
                                "维修库位没有可用库存，请先进行维修领料！",
                                caption,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            view.SetColumnError(colT110LeafID, "维修库位没有可用库存，请先进行维修领料！");
                            e.Valid = false;

                            return;
                        }
                        else
                        {
                            view.SetRowCellValue(e.RowHandle, colSKUID2, skuID);
                            view.UpdateCurrentRow();
                        }
                    }
                    #endregion

                    #region 获取原物料的 SKUID
                    string wipCode =
                    grdvProducts.GetFocusedRowCellValue(
                        grdvProducts.Columns["WIPCode"]).ToString();

                    IRAPMESTSClient.Instance.ufn_GetMaterialSKUIDBySymbol(
                        IRAPUser.Instance.CommunityID,
                        wipCode,
                        t110LeafID,
                        t101LeafID,
                        IRAPUser.Instance.SysLogID,
                        ref skuID,
                        out errCode,
                        out errText);
                    WriteLog.Instance.Write(
                        string.Format("({0}){1}", errCode, errText),
                        strProcedureName);
                    if (errCode != 0)
                    {
                        XtraMessageBox.Show(
                            errText,
                            caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        view.SetColumnError(colT110LeafID, errText);
                        e.Valid = false;

                        return;
                    }
                    else
                    {
                        view.SetRowCellValue(e.RowHandle, colSKUID1, skuID);
                        view.UpdateCurrentRow();
                    }
                    #endregion
                }
                finally
                {
                    WriteLog.Instance.WriteEndSplitter(strProcedureName);
                }
            }
            else
            {
                view.SetRowCellValue(e.RowHandle, colSKUID1, "");
                view.SetRowCellValue(e.RowHandle, colSKUID2, "");
                view.UpdateCurrentRow();
            }

            e.Valid = true;
        }

        private void grdvRepairItems_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }
        /// <summary>
        /// 根据维修项目列表生成维修模式叶标识列表字符串
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>

        private string GenerateT119LeafListInRepairItems(List<SubWIPIDCode_TroubleShooting> items)
        {
            string sb = "";
            foreach (SubWIPIDCode_TroubleShooting item in items)
            {
                item.GetListFromDataTable();
                foreach (SubWIPIDCode_TSItem tsItem in item.TSItems)
                {
                    if (sb.IndexOf(tsItem.T119LeafID.ToString()) >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (sb != "")
                        {
                            sb += ",";
                        }
                        sb += tsItem.T119LeafID.ToString();
                    }
                }
            }

            return sb;
        }
    }
}
