﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanMGMT.Converter
{
    public class ProjectIDToNameConverter : System.Windows.Data.IValueConverter//此接口有以下两个方法
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";
            return BLL.ProjectBLL.Instance.GetProjectNameByID((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}