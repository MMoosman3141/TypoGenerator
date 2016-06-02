using NemMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.IO;

namespace TypoGenerator {
	public class TypoGeneratorVM : NotifyPropertyChanged {
		private SpellingFile _wordFile;
		private PatternFile _ruleFile;
		private static Random _rnd = new Random();
		private Command _newWordFileCommand;
		private Command _addWordCommand;
		private Command _deleteWordCommand;
		private Command _saveWordFileCommand;
		private Command _saveWordFileAsCommand;
		private Command _loadWordFileCommand;
		private Command _exportToCsvCommand;
		private Command _editRulesCommand;
		private Command _exitCommand;
		private Command _showAboutCommand;

		public string Title {
			get {
				string title = "Typo Generator";
				if (!string.IsNullOrWhiteSpace(WordFile.Filename))
					title = $"{title}  ({Path.GetFileName(WordFile.Filename)})";
				if (WordFile.Changed)
					title = $"{title}*";

				return title;
			}
		}
		public SpellingFile WordFile {
			get {
				return _wordFile;
			}
			set {
				SpellingFile prev = _wordFile;

				if (SetProperty(ref _wordFile, value)) {
					if(prev != null)
						prev.PropertyChanged -= WordFile_PropertyChanged;

					if(value != null)
						_wordFile.PropertyChanged += WordFile_PropertyChanged;
				}
			}
		}

		private void WordFile_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == nameof(WordFile.Changed) || e.PropertyName == nameof(WordFile.Filename))
				RaisePropertyChanged(() => Title);
		}

		public PatternFile RuleFile {
			get {
				return _ruleFile;
			}
			set {
				SetProperty(ref _ruleFile, value);
			}
		}
		public Command NewWordFileCommand {
			get {
				return _newWordFileCommand;
			}
			private set {
				SetProperty(ref _newWordFileCommand, value);
			}
		}
		public Command AddWordCommand {
			get {
				return _addWordCommand;
			}
			private set {				
				SetProperty(ref _addWordCommand, value);
			}
		}
		public Command DeleteWordCommand {
			get {
				return _deleteWordCommand;
			}
			private set {
				SetProperty(ref _deleteWordCommand, value);
			}
		}
		public Command SaveWordFileCommand {
			get {
				return _saveWordFileCommand;
			}
			private set {
				SetProperty(ref _saveWordFileCommand, value);
			}
		}
		public Command SaveWordFileAsCommand {
			get {
				return _saveWordFileAsCommand;
			}
			private set {
				SetProperty(ref _saveWordFileAsCommand, value);
			}
		}
		public Command LoadWordFileCommand {
			get {
				return _loadWordFileCommand;
			}
			private set {
				SetProperty(ref _loadWordFileCommand, value);
			}
		}
		public Command ExportToCsvCommand {
			get {
				return _exportToCsvCommand;
			}
			private set {
				SetProperty(ref _exportToCsvCommand, value);
			}
		}
		public Command EditRulesCommand {
			get {
				return _editRulesCommand;
			}
			private set {
				SetProperty(ref _editRulesCommand, value);
			}
		}
		public Command ExitCommand {
			get {
				return _exitCommand;
			}
			private set {
				SetProperty(ref _exitCommand, value);
			}
		}
		public Command ShowAboutCommand {
			get {
				return _showAboutCommand;
			}
			private set {
				SetProperty(ref _showAboutCommand, value);
			}
		}

		public TypoGeneratorVM() {
			WordFile = new SpellingFile();
			if (File.Exists("misspellingPatterns.xml")) {
				RuleFile = PatternFile.OpenFile("misspellingPatterns.xml");
			} else {
				RuleFile = new PatternFile();
				RuleFile.Filename = "misspellingPatterns.xml";
			}

			NewWordFileCommand = new Command(NewWordFile);

			AddWordCommand = new Command(AddWord);
			DeleteWordCommand = new Command(DeleteWord);

			SaveWordFileCommand = new Command(SaveWordFile);
			SaveWordFileAsCommand = new Command(SaveWordFileAs);
			LoadWordFileCommand = new Command(LoadWordFile);

			ExportToCsvCommand = new Command(ExportToCsv, CanExportToCsv);

			EditRulesCommand = new Command(EditRules);

			ExitCommand = new Command(ExitApplication);

			ShowAboutCommand = new Command(ShowAbout);
		}

		private void NewWordFile() {
			WordFile = new SpellingFile();

			RaisePropertyChanged(() => Title);
			ExportToCsvCommand.RaiseCanExecuteChanged();
		}

		private void AddWord() {
			WordFile.AddWord(RuleFile.Patterns);

			ExportToCsvCommand.RaiseCanExecuteChanged();
		}
		private void DeleteWord(object index) {
			WordFile.RemoveWord((int)index);

			ExportToCsvCommand.RaiseCanExecuteChanged();
		}

		private void SaveWordFile() {
			if (string.IsNullOrWhiteSpace(WordFile?.Filename))
				SaveWordFileAs();
			else
				SpellingFile.SaveFile(WordFile.Filename, WordFile);
		}
		private void SaveWordFileAs() {			
			SaveFileDialog dlgSave = new SaveFileDialog();
			dlgSave.DefaultExt = "xml";
			dlgSave.Filter = "XML Files|*.xml|All Files|*.*";

			if (dlgSave.ShowDialog() == true) {
				Mouse.OverrideCursor = Cursors.Wait;

				SpellingFile.SaveFile(dlgSave.FileName, WordFile);

				Mouse.OverrideCursor = null;
			}


		}
		private void LoadWordFile() {
			OpenFileDialog dlgOpen = new OpenFileDialog();
			dlgOpen.DefaultExt = "xml";
			dlgOpen.Filter = "XML Files|*.xml|All Files|*.*";

			if (dlgOpen.ShowDialog() == true) {
				Mouse.OverrideCursor = Cursors.Wait;

				WordFile = SpellingFile.OpenFile(dlgOpen.FileName);

				foreach (Word word in WordFile.Words)
					word.Patterns = RuleFile.Patterns;

				Mouse.OverrideCursor = null;
			}

			RaisePropertyChanged(() => Title);
			ExportToCsvCommand.RaiseCanExecuteChanged();
		}

		private void ExportToCsv() {
			SaveFileDialog dlgSave = new SaveFileDialog();
			dlgSave.DefaultExt = "csv";
			dlgSave.Filter = "CSV Files|*.csv|All Files|*.*";

			if (dlgSave.ShowDialog() == true) {
				Mouse.OverrideCursor = Cursors.Wait;

				List<string> rows = new List<string>();

				rows.Add("Number,Candidate 1,Candidate 2,Candidate 2,Candidate 3");
				foreach (Word word in WordFile.Words) {
					int index = word.Index + 1;

					List<string> allWords = new List<string>();
					allWords.Add(word.ProperWord);
					allWords.AddRange(word.Misspellings);

					string row = $"{index},{string.Join(",", allWords.OrderBy(item => _rnd.Next(int.MaxValue)))}";

					rows.Add(row);
				}

				bool retry = false;
				do {
					try {
						File.WriteAllLines(dlgSave.FileName, rows, Encoding.UTF8);
					} catch (IOException) {
						MessageBoxResult msgResult = MessageBox.Show($"The file, {dlgSave.FileName}, seems to be in use.  Please ensure the file is closed and click OK to try again.", "File In Use", MessageBoxButton.OKCancel);
						if (msgResult == MessageBoxResult.OK)
							retry = true;
					}
				} while (retry);

				Mouse.OverrideCursor = null;
			}
		}
		private bool CanExportToCsv() {
			if (WordFile.Words?.Count == 0)
				return false;

			return true;
		}

		private void EditRules() {
			PatternEditorMV patternEditor = new PatternEditorMV(RuleFile);
			if (patternEditor.ShowDialog() == false)
				RuleFile = PatternFile.OpenFile(RuleFile.Filename);

			foreach (Word word in WordFile.Words)
				word.Patterns = RuleFile.Patterns;
		}

		private void ExitApplication(object window) {
			((Window)window).Close();
		}

		private void ShowAbout() {
			About aboutWindow = new About();
			aboutWindow.ShowDialog();
		}

	}
}
