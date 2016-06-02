using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypoGenerator.Converters {
	public class PatternTypeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string matchType;

			switch ((PatternMatchType)value) {
				case PatternMatchType.WordStart:
					matchType = "Start of Word";
					break;
				case PatternMatchType.WordEnd:
					matchType = "End of Word";
					break;
				case PatternMatchType.WordMedial:
					matchType = "Middle of Word";
					break;
				case PatternMatchType.Anywhere:
					matchType = "Anywhere";
					break;
				case PatternMatchType.FullWord:
				default:
					matchType = "Full Word";
					break;
			}

			return matchType;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			PatternMatchType matchType;

			switch ((string)value) {
				case "Start of Word":
					matchType = PatternMatchType.WordStart;
					break;
				case "End of Word":
					matchType = PatternMatchType.WordEnd;
					break;
				case "Middle of Word":
					matchType = PatternMatchType.WordMedial;
					break;
				case "Anywhere":
					matchType = PatternMatchType.Anywhere;
					break;
				case "Full Word":
				default:
					matchType = PatternMatchType.FullWord;
					break;
			}

			return matchType;
		}
	}
}
