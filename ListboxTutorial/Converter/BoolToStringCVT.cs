// <copyright file="BoolToStringCVT.cs" company="$registerdorganization$">
// Copyright (c) 2012 . All Right Reserved
// </copyright>
// <author>xxchen</author>
// <email></email>
// <date>2012-07-23</date>
// <summary>A value converter for WPF and Silverlight data binding</summary>
using System;
using System.Windows.Data;

namespace NoteBook.Converter
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class BoolToStringCVT : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
                return string.Empty;
            var strs = parameter.ToString().Split(new[] {',', ';', '.', ':', '\'', '/'});
            var condition = (bool) value;
            if (strs.Length < 2)
                return condition?parameter.ToString():string.Empty;
            return condition ? strs[0] : strs[1];
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
