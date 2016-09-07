using NemMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TypoGenerator {
	[Flags]
	public enum PatternMatchType {
		None,
		FullWord,
		WordStart,
		WordEnd,
		WordMedial,
		Anywhere
	}

	public class MisspellPattern : NotifyPropertyChanged {
		private int _index;
		private string _pattern;
		private PatternMatchType _matchType;
		private string _replacement;
		private static Random _rnd = new Random();

		public int Index {
			get {
				return _index;
			}
			set {
				SetProperty(ref _index, value);
			}
		}
		public string Pattern {
			get {
				return _pattern;
			}
			set {
				SetProperty(ref _pattern, value);
			}
		}
		public PatternMatchType MatchType {
			get {
				return _matchType;
			}
			set {
				SetProperty(ref _matchType, value);
			}
		}
		public string Replacement {
			get {
				return _replacement;
			}
			set {
				SetProperty(ref _replacement, value);
			}
		}

		public bool IsComplete() {
			if (string.IsNullOrWhiteSpace(Pattern))
				return false;

			if (_matchType == PatternMatchType.None)
				return false;

			if (string.IsNullOrWhiteSpace(Replacement))
				return false;

			return true;
		}

		public string GetRegexPattern() {
			string regexPattern;
			if (MatchType == PatternMatchType.FullWord) {
				regexPattern = $"^{Pattern}$";
			} else if (MatchType == PatternMatchType.WordStart) {
				regexPattern = $"^{Pattern}";
			} else if (MatchType == PatternMatchType.WordEnd) {
				regexPattern = $"{Pattern}$";
			} else if (MatchType == PatternMatchType.WordMedial) {
				regexPattern = $"(?!^){Pattern}(?!$)";
			} else if (MatchType == PatternMatchType.Anywhere) {
				regexPattern = Pattern;
			} else {
				regexPattern = ".*";
			}

			return regexPattern;
		}

		public string GetMisspelling(string word) {
			string regexPattern = GetRegexPattern();

			string replace = Replacement.ToLower() == "{delete}" ? "" : Replacement.ToLower();

			string misspelling;

			int tryCount = 0;
			do {
				switch (MatchType) {
					case PatternMatchType.FullWord:
					case PatternMatchType.WordStart:
					case PatternMatchType.WordEnd:
						misspelling = Regex.Replace(word, regexPattern, replace, RegexOptions.IgnoreCase);
						break;
					case PatternMatchType.WordMedial:
					case PatternMatchType.Anywhere:
					default:
						Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

						MatchCollection matches = regex.Matches(word);
						int matchIndex = matches[_rnd.Next(0, matches.Count)].Index;

						misspelling = regex.Replace(word, replace, 1, matchIndex);

						break;
				}
				tryCount++;
			} while (misspelling == word && tryCount <= 10);

			if (misspelling == word)
				return null;

			return misspelling;

		}
	}
}
