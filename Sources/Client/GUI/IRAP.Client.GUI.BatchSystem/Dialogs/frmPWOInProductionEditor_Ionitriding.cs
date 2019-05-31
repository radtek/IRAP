﻿using DevExpress.XtraEditors;
using IRAP.Client.Global.Enums;
using IRAP.Client.User;
using IRAP.Entities.MES;
using IRAP.Global;
using IRAP.WCF.Client.Method;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace IRAP.Client.GUI.BatchSystem.Dialogs
{
    public partial class frmPWOInProductionEditor_Ionitriding : IRAP.Client.Global.frmCustomBase
    {
        private static string className =
            MethodBase.GetCurrentMethod().DeclaringType.FullName;

        private EditStatus editStatus = EditStatus.Browse;

        private int t134LeafID = 0;
        private int t216LeafID = 0;
        private int t131LeafID = 0;
        private List<EntityBatchPWO> datas = null;
        private EntityBatchPWO data = null;

        public frmPWOInProductionEditor_Ionitriding()
        {
            InitializeComponent();
        }

        public frmPWOInProductionEditor_Ionitriding(
            EditStatus status,
            int t134LeafID,
            int t216LeafID,
            int t131LeafID,
            List<EntityBatchPWO> pwos,
            ref EntityBatchPWO pwo) :
            this()
        {
            editStatus = status;

            this.t134LeafID = t134LeafID;
            this.t216LeafID = t216LeafID;
            this.t131LeafID = t131LeafID;
            datas = pwos;
            data = pwo;

            switch (editStatus)
            {
                case EditStatus.New:
                    Text = "新增";

                    break;
                case EditStatus.Edit:
                    Text = "修改";

                    edtPWONo.Text = data.PWONo;
                    edtProductNo.Text = data.T102Code;
                    edtProductName.Text = data.T102Name;
                    edtBatchNo.Text = data.LotNumber;
                    edtTextureCode.Text = data.Texture;
                    edtQuantity.Value = Convert.ToDecimal(data.Quantity);

                    edtPWONo.Enabled = false;

                    break;
            }
        }

        /// <summary>
        /// 获取材质
        /// </summary>
        /// <param name="materialCode">物料号（原材料/半成品/产成品）</param>
        /// <returns>string</returns>
        private string GetTextureCodeFromMaterialCode(
            string materialCode,
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
                string rlt = "";

                IRAPMDMClient.Instance.ufn_GetMaterialTextureCodeFromERP_FourthShift(
                    IRAPUser.Instance.CommunityID,
                    materialCode,
                    ref rlt,
                    out errCode,
                    out errText);
                WriteLog.Instance.Write(
                    string.Format("({0}){1}", errCode, errText),
                    strProcedureName);

                return rlt;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private string GeneratePokaYokeXML(List<EntityBatchPWO> pwos, OpenPWOInfoEx pwo)
        {
            string rlt = "";
            for (int i = 0; i < pwos.Count; i++)
            {
                rlt +=
                    string.Format(
                        "<RowSet><Ordinal>{0}</Ordinal>" +
                        "<T102LeafID>{1}</T102LeafID>" +
                        "<T216LeafID>{2}</T216LeafID></RowSet>\n",
                        i + 1,
                        pwos[i].T102LeafID,
                        t216LeafID);
            }
            rlt +=
                string.Format(
                    "<RowSet><Ordinal>{0}</Ordinal>" +
                    "<T102LeafID>{1}</T102LeafID>" +
                    "<T216LeafID>{2}</T216LeafID></RowSet>\n",
                    pwos.Count + 1,
                    pwo.T102LeafID,
                    t216LeafID);
            rlt = string.Format("<Param>\n{0}</Param>", rlt);

            return rlt;
        }

        private void edtPWONo_Validating(object sender, CancelEventArgs e)
        {
            if (edtPWONo.Text == "")
            {
                data = new EntityBatchPWO();

                edtProductNo.Text = "";
                edtProductName.Text = "";
                edtBatchNo.Text = "";
                edtTextureCode.Text = "";
                edtQuantity.Value = 0;

                e.Cancel = false;
                return;
            }

            string strProcedureName =
                string.Format(
                    "{0}.{1}",
                    className,
                    MethodBase.GetCurrentMethod().Name);

            foreach (EntityBatchPWO pwo in datas)
            {
                if (pwo.PWONo == edtPWONo.Text)
                {
                    XtraMessageBox.Show(
                        string.Format(
                            "生产工单[{0}]已经存在，不能重复增加！",
                            pwo.PWONo),
                        "系统信息",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    edtPWONo.Text = "";
                    e.Cancel = true;
                    return;
                }
            }

            WriteLog.Instance.WriteBeginSplitter(strProcedureName);
            try
            {
                int errCode = 0;
                string errText = "";

                #region 根据输入的工单号查找工单
                OpenPWOInfoEx openPWO = null;
                IRAPMESClient.Instance.ufn_GetInfo_OpenPWO(
                    IRAPUser.Instance.CommunityID,
                    edtPWONo.Text,
                    IRAPUser.Instance.SysLogID,
                    ref openPWO,
                    out errCode,
                    out errText);
                if (errCode != 0)
                {
                    XtraMessageBox.Show(
                        errText,
                        "系统信息",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    edtPWONo.Text = "";
                    e.Cancel = true;
                    return;
                }
                #endregion

                #region 获取当前工单的材质
                string textureCode = GetTextureCodeFromMaterialCode(
                    openPWO.ProductNo,
                    out errCode,
                    out errText);
                if (errCode != 0)
                {
                    XtraMessageBox.Show(
                        string.Format(
                            "获取生产工单[{0}]的材质时发生错误：[{1}]",
                            openPWO.PWONo,
                            errText),
                        "系统信息",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    edtPWONo.Text = "";
                    e.Cancel = true;
                    return;
                }
                #endregion

                #region 调用存储过程校验当前工单的工艺参数是否和其它工单一致
                string pokaYokeXML = GeneratePokaYokeXML(datas, openPWO);
                IRAPMESClient.Instance.usp_PokaYoke_ParamConsistency(
                    IRAPUser.Instance.CommunityID,
                    t131LeafID,
                    pokaYokeXML,
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
                        "系统信息",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    edtPWONo.Text = "";
                    e.Cancel = true;
                    return;
                }
                #endregion

                if (data == null)
                    data = new EntityBatchPWO();
                data.PWONo = openPWO.PWONo;
                data.T102Code = openPWO.ProductNo;
                data.T102Name = openPWO.ProductName;
                data.T102LeafID = openPWO.T102LeafID;
                data.LotNumber = openPWO.LotNumber;
                data.Texture = textureCode;

                edtProductNo.Text = data.T102Code;
                edtProductName.Text = data.T102Name;
                edtBatchNo.Text = data.LotNumber;
                edtTextureCode.Text = data.Texture;
            }
            finally
            {
                WriteLog.Instance.WriteEndSplitter(strProcedureName);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (edtPWONo.Text == "")
            {
                XtraMessageBox.Show(
                    "工单号不能空白！",
                    "系统信息",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                edtPWONo.Focus();
                return;
            }

            if (edtQuantity.Value <= 0)
            {
                XtraMessageBox.Show(
                    "请输入当前工单的生产数量！",
                    "系统信息",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                edtQuantity.Focus();
                return;
            }

            data.Quantity = Convert.ToInt32(edtQuantity.Value);

            DialogResult = DialogResult.OK;
        }
    }
}
