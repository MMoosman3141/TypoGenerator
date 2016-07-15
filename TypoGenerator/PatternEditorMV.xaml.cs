using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TypoGenerator {
	/// <summary>
	/// Interaction logic for PatternEditorMV.xaml
	/// </summary>
	public partial class PatternEditorMV : Window {
		private PatternEditorVM _vm;

		public PatternEditorMV(PatternFile ruleFile) {
			InitializeComponent();

			_vm = (PatternEditorVM)Resources["vm"];
			_vm.RuleFile = ruleFile;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (_vm.RuleFile.Changed) {
				MessageBoxResult msgResult = MessageBox.Show("Do you want to save your changes before exiting?", "Save Changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

				if (msgResult == MessageBoxResult.Yes) {
					_vm.SavePatternFileCommand.Execute(null);
					this.DialogResult = true;
				} else if (msgResult == MessageBoxResult.No) {
					this.DialogResult = false;
				} else if (msgResult == MessageBoxResult.Cancel) {
					e.Cancel = true;
				}
			}
		}
	}
}
