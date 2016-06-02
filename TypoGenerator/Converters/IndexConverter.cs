﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypoGenerator.Converters {
	public class IndexConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (int)value + 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (int)value - 1;
		}
	}
}
