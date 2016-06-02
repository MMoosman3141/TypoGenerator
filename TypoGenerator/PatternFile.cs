using NemMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;

namespace TypoGenerator {
	public class PatternFile : NotifyPropertyChanged {
		private bool _changed;
		private string _filename;
		private ObservableCollection<MisspellPattern> _patterns;

		[XmlIgnore]
		public bool Changed {
			get {
				return _changed;
			}
			set {
				SetProperty(ref _changed, value);
			}
		}
		[XmlIgnore]
		public string Filename {
			get {
				return _filename;
			}
			set {
				SetProperty(ref _filename, value);
			}
		}
		public ObservableCollection<MisspellPattern> Patterns {
			get {
				return _patterns;
			}
			private set {
				ObservableCollection<MisspellPattern> prev = _patterns;

				if (SetProperty(ref _patterns, value)) {
					if (prev != null)
						prev.CollectionChanged -= Patterns_CollectionChanged;

					if(value != null)
						_patterns.CollectionChanged += Patterns_CollectionChanged;
				}
			}
		}

		public PatternFile() {
			Changed = false;
			Filename = null;
			Patterns = new ObservableCollection<MisspellPattern>();
		}

		public void AddPattern() {
			MisspellPattern pattern = new MisspellPattern();
			pattern.Index = Patterns.Count;
			pattern.PropertyChanged += Pattern_PropertyChanged;
			pattern.MatchType = PatternMatchType.FullWord;

			Patterns.Add(pattern);
		}

		public void RemovePattern(int index) {
			if (Patterns.Count > 0 && Patterns.Count >= index) {
				Patterns[index].PropertyChanged -= Pattern_PropertyChanged;
				Patterns.RemoveAt(index);

				for (int patternIndex = 0; patternIndex < Patterns.Count; patternIndex++)
					Patterns[patternIndex].Index = patternIndex;
			}
		}

		private void Patterns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			Changed = true;
		}
		private void Pattern_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			Changed = true;
		}

		public static PatternFile OpenFile(string filename) {
			XmlSerializer serializer = new XmlSerializer(typeof(PatternFile));

			PatternFile file;

			using (StreamReader streamReader = new StreamReader(filename)) {
				file = (PatternFile)serializer.Deserialize(streamReader);
			}

			file.Filename = filename;
			file.Changed = false;

			return file;
		}
		public static void SaveFile(string filename, PatternFile patternFile) {
			XmlSerializer serializer = new XmlSerializer(typeof(PatternFile));

			using (StreamWriter streamWriter = new StreamWriter(filename)) {
				serializer.Serialize(streamWriter, patternFile);
			}

			patternFile.Changed = false;
		}
	}
}
