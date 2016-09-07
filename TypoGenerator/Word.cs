using NemMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TypoGenerator {
	public class Word : NotifyPropertyChanged	{
		private int _index;
		private string _properWord;
		private ObservableCollection<string> _misspellings;
		private ObservableCollection<MisspellPattern> _patterns;
		//private List<string> _usedPatterns;
		private Command _generateMisspellingsCommand;
		private Command _generateMisspellingCommand;
		private static Random _rnd = new Random();

		public int Index {
			get {
				return _index;
			}
			set {
				SetProperty(ref _index, value);
			}
		}
		public string ProperWord {
			get {
				return _properWord;
			}
			set {
				SetProperty(ref _properWord, value, new Command[] { GenerateMisspellingsCommand});
			}
		}
		public ObservableCollection<string> Misspellings {
			get {
				return _misspellings;
			}
			set {
				SetProperty(ref _misspellings, value, new Command[] { GenerateMisspellingsCommand });
			}
		}
		[XmlIgnore]
		public ObservableCollection<MisspellPattern> Patterns {
			get {
				return _patterns;
			}
			set {
				SetProperty(ref _patterns, value);
			}
		}
		//public List<string> UsedPatterns {
		//	get {
		//		return _usedPatterns;
		//	}
		//	set {
		//		SetProperty(ref _usedPatterns, value);
		//	}
		//}
		[XmlIgnore]
		public Command GenerateMisspellingsCommand {
			get {
				return _generateMisspellingsCommand;
			}
			private set {
				SetProperty(ref _generateMisspellingsCommand, value);
			}
		}
		[XmlIgnore]
		public Command GenerateMisspellingCommand {
			get {
				return _generateMisspellingCommand;
			}
			set {
				SetProperty(ref _generateMisspellingCommand, value);
			}
		}

		public Word() {
			Misspellings = new ObservableCollection<string>();
			//UsedPatterns = new List<string>();

			GenerateMisspellingsCommand = new Command(GenerateMisspellings, CanGenerateMisspellings);
			GenerateMisspellingCommand = new Command(GenerateMisspelling);
		}

		private bool CanGenerateMisspellings() {
			if (string.IsNullOrWhiteSpace(ProperWord))
				return false;

			if (ProperWord.Length < 3)
				return false;

			if (Misspellings == null)
				return false;

			return true;
		}

		private void GenerateMisspellings() {
			//UsedPatterns.Clear();
			Misspellings.Clear();

			GenerateMisspelling(0);
			GenerateMisspelling(1);
			GenerateMisspelling(2);
		}
		private void GenerateMisspelling(object indexObj) {
			int index = (int)indexObj;

			while (Misspellings.Count - 1 < index) {
				Misspellings.Add("");
			}

			List<MisspellPattern> availablePatterns = Patterns.Where(item => Regex.IsMatch(ProperWord, item.GetRegexPattern())).ToList();

			if (availablePatterns.Count == 0) {
				Misspellings[index] = "{NA}";
				return;
			}

			string misspelling;
			int tryCount = 0;
			do {
				int patternIndex;
				if (availablePatterns.Count == 1)
					patternIndex = 0;
				else
					patternIndex = _rnd.Next(0, availablePatterns.Count);

				MisspellPattern pattern = availablePatterns[patternIndex];

				misspelling = pattern.GetMisspelling(ProperWord) ?? "{NA}";
				tryCount++;
			} while ((misspelling == "{NA}" || Misspellings.Contains(misspelling)) && tryCount <= 10);

			if (Misspellings.Contains(misspelling))
				misspelling = "{NA}";

			Misspellings[index] = misspelling;
		}
	}
}
