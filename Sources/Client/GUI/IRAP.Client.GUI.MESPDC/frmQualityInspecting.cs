﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using DevExpress.XtraEditors;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraTab;

using IRAP.Global;
using IRAP.Client.Global.Enums;
using IRAP.Client.User;
using IRAP.Entities.MDM;
using IRAP.Entities.MES;
using IRAP.Entities.SSO;
using IRAP.Entities.IRAP;
using IRAP.WCF.Client.Method;

namespace IRAP.Client.GUI.MESPDC
{
    public partial class frmQualityInspecting : IRAP.Client.Global.GUI.frmCustomFuncBase
    {
        private string className =
            MethodBase.GetCurrentMethod().DeclaringType.FullName;

        private STB006 currentOperator = null;
        private List<Entities.EntityEquipmentInfo> equipments = 
            new List<Entities.EntityEquipmentInfo>();
        private List<BatchByEquipment> batchs = new List<BatchByEquipment>();
        private List<BatchPWOInfo> pwos = new List<BatchPWOInfo>();
        private List<InspectionItem> inspectionItems = new List<InspectionItem>();

        private DataTable dtInspection = new DataTable();

        public frmQualityInspecting()
        {
            InitializeComponent();
        }

        private void GetStations()
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
                List<WIPStation> stations = new List<WIPStation>();

                equipments.Clear();
                IRAPMDMClient.Instance.ufn_GetList_WIPStationsOfAHost(
                    IRAPUser.Instance.CommunityID,
                    IRAPUser.Instance.SysLogID,
                    ref stations,
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
                }
                else
                {
                    foreach (WIPStation station in stations)
                    {
                        equipments.Add(
                            new Entities.EntityEquipmentInfo()
                            {
                                Ordinal = station.Ordinal,
                                T107LeafID = station.T107LeafID,
                                T107EntityID = station.T107EntityID,
                                T107Code = station.T107Code,
                                T107AltCode = station.T107AltCode,
                                T107Name = station.T107Name,
                                T134LeafID = station.T134LeafID,
                                T134EntityID = station.T134EntityID,
                                T134Code = station.T134Code,
                                T134AltCode = station.T134AltCode,
                                T216LeafID = station.T216LeafID,
                                T216EntityID = station.T216EntityID,
                                T216Code = station.T216Code,
                                T216AltCode = station.T216AltCode,
                                T216Name = station.T216Name,
                                T133LeafID = station.T133LeafID,
                                T133EntityID = station.T133EntityID,
                                T133Code = station.T133Code,
                                T133AltCode = station.T133AltCode,
                            });
                    }
                }
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void ClearAll()
        {
            batchs.Clear();
            grdvBatchNos.UpdateCurrentRow();

            pwos.Clear();
            grdvPWOs.UpdateCurrentRow();
        }

        private void RefreshCtrlInForm()
        {
            grdBatchNos.RefreshDataSource();
            grdPWOs.RefreshDataSource();

            if (currentOperator == null)
            {
                cboWorkUnit.Enabled = false;
                splitContainerControl2.Enabled = false;
                vgrdInspectParams.Enabled = false;
                btnPWONew.Enabled = false;
                btnPWOModify.Enabled = false;
                btnPWORemove.Enabled = false;
                btnSaveParams.Enabled = false;
            }
            else
            {
                cboWorkUnit.Enabled = true;
                if (cboWorkUnit.SelectedItem != null)
                {
                    splitContainerControl2.Enabled = true;
                }
                if (grdvPWOs.GetFocusedDataSourceRowIndex() >= 0)
                {
                    vgrdInspectParams.Enabled = true;
                    btnPWONew.Enabled = true;
                    btnPWOModify.Enabled = true;
                    btnPWORemove.Enabled = true;
                    btnSaveParams.Enabled = true;
                }
                else
                {
                    vgrdInspectParams.Enabled = false;
                    btnPWONew.Enabled = false;
                    btnPWOModify.Enabled = false;
                    btnPWORemove.Enabled = false;
                    btnSaveParams.Enabled = false;
                }
            }
        }

        private void GetBatchsFromEquipment(Entities.EntityEquipmentInfo station)
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

                batchs.Clear();
                IRAPMESClient.Instance.ufn_GetList_BatchByEquipment(
                    IRAPUser.Instance.CommunityID,
                    station.T133LeafID,
                    "IQ",
                    IRAPUser.Instance.SysLogID,
                    ref batchs,
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
                }
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private STB006 GetUserInfoWithIDCode(string idCode)
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
                List<STB006> users = new List<STB006>();

                IRAPUserClient.Instance.mfn_GetList_Users(
                    IRAPUser.Instance.CommunityID,
                    "",
                    idCode,
                    ref users,
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
                    return null;
                }
                else
                {
                    if (users.Count == 0)
                    {
                        XtraMessageBox.Show(
                            string.Format(
                                "未找到[{0}]的用户",
                                idCode),
                            caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return null;
                    }
                    else
                    {
                        return users[0];
                    }
                }
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        /// <summary>
        /// 根据生产容器批次号获取工单信息列表
        /// </summary>
        /// <param name="batchNumber">生产容器批次号</param>
        /// <returns></returns>
        private List<BatchPWOInfo> GetPWOWithBatchNo(string batchNumber)
        {
            List<BatchPWOInfo> rlt = new List<BatchPWOInfo>();

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

                IRAPMESClient.Instance.ufn_GetList_BatchPWONo(
                    IRAPUser.Instance.CommunityID,
                    batchNumber,
                    IRAPUser.Instance.SysLogID,
                    ref rlt,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText),
                    strProcedureName);
                if (errCode != 0)
                {
                    XtraMessageBox.Show(
                        errText,
                        "",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = rlt.Count - 1; i >= 0; i--)
                    {
                        if (rlt[i].QCStatus != 0)
                            rlt.RemoveAt(i);
                    }
                }
            }
            catch (Exception error)
            {
                string errMsg =
                    string.Format(
                        "获取工单信息列表时发生错误：[{0}]",
                        error.Message);
                WriteLog.Instance.Write(errMsg, strProcedureName);

                XtraMessageBox.Show(
                    errMsg,
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }

            return rlt;
        }

        /// <summary>
        /// 生成质量检验值临时表
        /// </summary>
        /// <param name="items"></param>
        private void InitInspectionItemsGrid(List<InspectionItem> items)
        {
            dtInspection.Clear();
            dtInspection.Columns.Clear();

            vgrdInspectParams.Rows.Clear();

            foreach (InspectionItem item in items)
            {
                string colName = string.Format("Column{0}", item.Ordinal);
                DataColumn dc = dtInspection.Columns.Add(colName, typeof(string));
                dc.Caption = item.T20Name;

                EditorRow row = new EditorRow();
                row.Properties.Caption = item.T20Name;
                row.Properties.FieldName = colName;
                vgrdInspectParams.Rows.Add(row);
            }

            //for (int i = 0; i < dtInspection.Columns.Count; i++)
            //{
            //    List<PPParamValue> values = items[i].ResolveDataXML();
            //    for (int j = 0; j < values.Count; j++)
            //    {
            //        DataRow dr = null;
            //        if (dtInspection.Rows.Count < j + 1)
            //        {
            //            dr = dtInspection.NewRow();
            //            dtInspection.Rows.Add(dr);
            //        }
            //        else
            //        {
            //            dr = dtInspection.Rows[j];
            //        }

            //        dr[i] = values[j].Metric01;
            //    }
            //}

            vgrdInspectParams.DataSource = dtInspection;
            vgrdInspectParams.BestFit();
        }

        private string GenerateRSFactXML()
        {
            string rlt = "";

            for (int i = 0; i < dtInspection.Rows.Count; i++)
            {
                int rowNo = i + 1;
                for (int j = 0; j < dtInspection.Columns.Count; j++)
                {
                    rlt +=
                        string.Format(
                            "<RF6_2 RowNum=\"{0}\" Ordinal=\"{1}\" " +
                            "T20LeafID=\"{2}\" LowLimit=\"\" " +
                            "Criterion=\"\" HighLimit=\"\" UnitOfMeasure=\"\" " +
                            "Metric01=\"{3}\" />",
                            rowNo,
                            inspectionItems[j].Ordinal,
                            inspectionItems[j].T20LeafID,
                            dtInspection.Rows[i][j].ToString());
                }
            }

            rlt = string.Format("<RSFact>{0}</RSFact>", rlt);
            return rlt;
        }

        private void frmQualityInspecting_Load(object sender, EventArgs e)
        {
            GetStations();
            equipments.Sort(new Entities.EntityEquipmentInfo_CompareByT133AltCode());
            foreach (Entities.EntityEquipmentInfo station in equipments)
            {
                cboWorkUnit.Properties.Items.Add(station);
            }

            grdBatchNos.DataSource = batchs;
            grdPWOs.DataSource = pwos;
        }

        private void frmQualityInspecting_Shown(object sender, EventArgs e)
        {
            GetStations();

            equipments.Sort(new Entities.EntityEquipmentInfo_CompareByT133AltCode());
            foreach (Entities.EntityEquipmentInfo station in equipments)
            {
                XtraTabPage page = new XtraTabPage();
                page.Text =
                    string.Format(
                        "[{0}]{1}",
                        station.T133AltCode,
                        station.T107Name);

                UserControls.ucBatchQualityInspecting inspect = 
                    new UserControls.ucBatchQualityInspecting(station);
                inspect.Dock = DockStyle.Fill;

                page.Controls.Add(inspect);

                tcMain.TabPages.Add(page);

                inspect.ClearAll();
            }
        }

        private void edtOperatorCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (edtOperatorCode.Text.Trim() != "")
                {
                    currentOperator = GetUserInfoWithIDCode(edtOperatorCode.Text.Trim());
                    if (currentOperator != null)
                    {
                        edtOperatorCode.Text =
                            string.Format(
                                "{0}[{1}]",
                                currentOperator.UserName,
                                currentOperator.UserCode);
                    }
                    else
                    {
                        edtOperatorCode.Text = "";
                    }
                }
                else
                {
                    edtOperatorCode.Text = "";
                    currentOperator = null;
                }

                RefreshCtrlInForm();
            }
        }

        private void cboWorkUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearAll();

            if (cboWorkUnit.SelectedItem != null)
            {
                Entities.EntityEquipmentInfo station = 
                    cboWorkUnit.SelectedItem as Entities.EntityEquipmentInfo;
                GetBatchsFromEquipment(station);

                grdBatchNos.DataSource = batchs;
                grdvBatchNos.UpdateCurrentRow();
                grdvBatchNos.BestFitColumns();
            }

            RefreshCtrlInForm();
        }

        private void grdvBatchNos_FocusedRowObjectChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventArgs e)
        {
            grdPWOs.DataSource = null;

            int idx = grdvBatchNos.GetFocusedDataSourceRowIndex();
            if (idx >= 0 && idx < batchs.Count)
            {
                BatchByEquipment batch = batchs[idx];
                pwos = GetPWOWithBatchNo(batch.BatchNumber);
                grdPWOs.DataSource = pwos;
            }
            else
            {
                pwos.Clear();
                grdPWOs.DataSource = null;
            }

            grdPWOs.RefreshDataSource();
            grdvPWOs.BestFitColumns();
        }

        private void grdvPWOs_FocusedRowObjectChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventArgs e)
        {
            int idx = grdvPWOs.GetFocusedDataSourceRowIndex();
            if (idx >=0 && idx < pwos.Count)
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

                    IRAPMDMClient.Instance.ufn_GetList_InspectionItems(
                        IRAPUser.Instance.CommunityID,
                        pwos[idx].T102LeafID,
                        ((Entities.EntityEquipmentInfo)cboWorkUnit.SelectedItem).T216LeafID,
                        pwos[idx].PWONo,
                        pwos[idx].BatchNumber,
                        IRAPUser.Instance.SysLogID,
                        ref inspectionItems,
                        out errCode,
                        out errText);
                    if (errCode != 0)
                    {
                        XtraMessageBox.Show(
                            errText,
                            "",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    WriteLog.Instance.WriteEndSplitter(strProcedureName);
                }
            }
            else
            {
                inspectionItems.Clear();
            }

            InitInspectionItemsGrid(inspectionItems);
        }

        private void btnPWONew_Click(object sender, EventArgs e)
        {
            if (dtInspection.Columns.Count < 0)
            {
                XtraMessageBox.Show(
                    "当前生产工单的在制品没有配置检验项",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (Dialogs.frmItemsEditor formEditor =
                new Dialogs.frmItemsEditor(
                    EditStatus.New,
                    splitContainerControl1.Panel2.Text,
                    dtInspection,
                    -1))
            {
                if (formEditor.ShowDialog() == DialogResult.OK)
                {
                    vgrdInspectParams.RefreshDataSource();
                }
            }
        }

        private void btnPWOModify_Click(object sender, EventArgs e)
        {
            if (dtInspection.Columns.Count < 0)
            {
                XtraMessageBox.Show(
                    "当前生产工单的在制品没有配置检验项",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            int idx = vgrdInspectParams.FocusedRecord;
            if (idx < 0)
            {
                XtraMessageBox.Show(
                    "当前没有需要修改的检验项值",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (Dialogs.frmItemsEditor formEditor =
                new Dialogs.frmItemsEditor(
                    EditStatus.Edit,
                    splitContainerControl1.Panel2.Text,
                    dtInspection,
                    idx))
            {
                if (formEditor.ShowDialog() == DialogResult.OK)
                {
                    vgrdInspectParams.RefreshDataSource();
                }
            }
        }

        private void btnPWORemove_Click(object sender, EventArgs e)
        {
            int idx = vgrdInspectParams.FocusedRecord;
            if (idx >= 0)
            {
                if (XtraMessageBox.Show(
                    string.Format(
                        "是否要删除选择的第[{0}]组参数值？",
                        idx + 1),
                    "",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    dtInspection.Rows.RemoveAt(idx);

                    vgrdInspectParams.RefreshDataSource();
                }
            }
        }

        private void btnSaveParams_Click(object sender, EventArgs e)
        {
            if (dtInspection.Columns.Count < 0)
            {
                XtraMessageBox.Show(
                    "当前生产工单的在制品没有配置检验项",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (dtInspection.Rows.Count < 0)
            {
                XtraMessageBox.Show(
                    "当前生产工单还没有输入检验值",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

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
                long factID = 0;

                int idx = grdvPWOs.GetFocusedDataSourceRowIndex();
                if (idx >= 0 && idx < pwos.Count)
                    factID = pwos[idx].FactID;

                IRAPMESClient.Instance.usp_SaveFact_BatchManualInspecting(
                    IRAPUser.Instance.CommunityID,
                    pwos[idx].FactID,
                    pwos[idx].T102LeafID,
                    ((Entities.EntityEquipmentInfo)cboWorkUnit.SelectedItem).T107LeafID,
                    pwos[idx].BatchNumber,
                    pwos[idx].LotNumber,
                    pwos[idx].PWONo,
                    1,
                    GenerateRSFactXML(),
                    IRAPUser.Instance.SysLogID,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText),
                    strProcedureName);
                if (errCode != 0)
                {
                    XtraMessageBox.Show(
                        errText,
                        "",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                        errText,
                        "",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    cboWorkUnit_SelectedIndexChanged(null, null);
                }
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void tcMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            XtraTabPage page = e.Page;
            foreach (Control ctrl in page.Controls)
            {
                if (ctrl is UserControls.ucBatchQualityInspecting)
                {
                    UserControls.ucBatchQualityInspecting inspect =
                        (UserControls.ucBatchQualityInspecting)ctrl;
                    inspect.ClearAll();

                    return;
                }
            }
        }
    }
}
