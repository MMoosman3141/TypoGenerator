using NemMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TypoGenerator {
	public class SpellingFile : NotifyPropertyChanged {
		private bool _changed;
		private string _filename;
		private ObservableCollection<Word> _words;

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
		public ObservableCollection<Word> Words {
			get {
				return _words;
			}
			private set {
				ObservableCollection<Word> prev = _words;

				if (SetProperty(ref _words, value)) {
					if (prev != null)
						prev.CollectionChanged -= Words_CollectionChanged;

					if (value != null)
						_words.CollectionChanged += Words_CollectionChanged;
				}
			}
		}

		public SpellingFile() {
			Changed = false;
			Filename = null;
			Words = new ObservableCollection<Word>();
		}

		public void AddWord(ObservableCollection<MisspellPattern> patterns) {
			Word newWord = new Word();
			newWord.Index = Words.Count;
			newWord.Patterns = patterns;
			newWord.PropertyChanged += Word_PropertyChanged;

			Words.Add(newWord);
		}

		public void RemoveWord(int index) {
			if (Words.Count > 0 && Words.Count >= index) {
				Words[index].PropertyChanged -= Word_PropertyChanged;
				Words.RemoveAt(index);

				for (int wordIndex = 0; wordIndex < Words.Count; wordIndex++)
					Words[wordIndex].Index = wordIndex;
			}
		}

		private void Words_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			Changed = true;
		}
		private void Word_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			Changed = true;
		}

		public static SpellingFile OpenFile(string filename) {
			XmlSerializer serializer = new XmlSerializer(typeof(SpellingFile));

			SpellingFile file;

			using (StreamReader streamReader = new StreamReader(filename)) {
				file = (SpellingFile)serializer.Deserialize(streamReader);
			}

			file.Filename = filename;
			file.Changed = false;

			return file;
		}
		public static void SaveFile(string filename, SpellingFile spellingFile) {
			spellingFile.Filename = filename;

			XmlSerializer serializer = new XmlSerializer(typeof(SpellingFile));

			using (StreamWriter streamWriter = new StreamWriter(filename)) {
				serializer.Serialize(streamWriter, spellingFile);
			}

			spellingFile.Changed = false;
		}
	}
}
