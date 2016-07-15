using NemMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TypoGenerator {
	public class PatternEditorVM : NotifyPropertyChanged {
		private PatternFile _ruleFile;
		private ObservableCollection<string> _matchTypes;
		private ReadOnlyObservableCollection<string> _roMatchTypes;

		private Command _addPatternCommand;
		private Command _deletePatternCommand;
		private Command _savePatternFileCommand;
		private Command _exitScreenCommand;

		public PatternFile RuleFile {
			get {
				return _ruleFile;
			}
			set {
				SetProperty(ref _ruleFile, value);
			}
		}
		public ReadOnlyObservableCollection<string> MatchTypes {
			get {
				return _roMatchTypes;
			}
			private set {
				SetProperty(ref _roMatchTypes, value);
			}
		}

		public Command AddPatternCommand {
			get {
				return _addPatternCommand;
			}
			private set {
				SetProperty(ref _addPatternCommand, value);
			}
		}
		public Command DeletePatternCommand {
			get {
				return _deletePatternCommand;
			}
			set {
				SetProperty(ref _deletePatternCommand, value);
			}
		}
		public Command SavePatternFileCommand {
			get {
				return _savePatternFileCommand;
			}
			private set {
				SetProperty(ref _savePatternFileCommand, value);
			}
		}
		public Command ExitScreenCommand {
			get {
				return _exitScreenCommand;
			}
			private set {
				SetProperty(ref _exitScreenCommand, value);
			}
		}

		public PatternEditorVM() {
			_matchTypes = new ObservableCollection<string>();
			_matchTypes.Add("Full Word");
			_matchTypes.Add("Start of Word");
			_matchTypes.Add("End of Word");
			_matchTypes.Add("Middle of Word");
			_matchTypes.Add("Anywhere");

			MatchTypes = new ReadOnlyObservableCollection<string>(_matchTypes);

			AddPatternCommand = new Command(AddPattern);
			DeletePatternCommand = new Command(DeletePattern);

			SavePatternFileCommand = new Command(SavePatternFile);

			ExitScreenCommand = new Command(ExitScreen);
		}

		private void AddPattern() {
			RuleFile.AddPattern();
		}
		private void DeletePattern(object index) {
			RuleFile.RemovePattern((int)index);
		}

		private void SavePatternFile() {
			if (RuleFile.Patterns.Count == 0)
				return;

			int invalidCount = RuleFile.Patterns.Count(pattern => !pattern.IsComplete());

			if (invalidCount > 0) {
				MessageBoxResult msgResult = MessageBox.Show("Not all rules are complete.  Incomplete rules will be removed before saving.  Press Cancel if you want to adjust the rules.", "Incomplete Rules", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

				if (msgResult == MessageBoxResult.Cancel)
					return;

				foreach(MisspellPattern pattern in RuleFile.Patterns.ToArray()) {
					if (!pattern.IsComplete())
						RuleFile.RemovePattern(pattern.Index);
				}
			}

			PatternFile.SaveFile(RuleFile.Filename, RuleFile);
		}

		private void ExitScreen(object window) {
			((Window)window).Close();
		}

	}
}
