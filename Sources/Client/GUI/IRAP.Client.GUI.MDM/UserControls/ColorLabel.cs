﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace IRAP.Client.GUI.MDM {
    public partial class ColorLabel : XtraUserControl {
        public ColorLabel() {
            InitializeComponent();
        }

        [Browsable(true)]
        [Description("文本"), Category("文本"), DefaultValue("")]
        public string Text {
            get { return this.labelMessage.Text; }
            set { this.labelMessage.Text = value; }
        }

        [Browsable(true)]
        public Color Color {
            get { return this.panelColor.BackColor; }
            set { this.panelColor.BackColor = value; }
        }
    }
}
