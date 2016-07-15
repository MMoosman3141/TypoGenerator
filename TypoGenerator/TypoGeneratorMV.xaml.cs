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
  /// Interaction logic for TypoGeneratorMV.xaml
  /// </summary>
  public partial class TypoGeneratorMV : Window {
		public TypoGeneratorMV() {
			InitializeComponent();
		}

		private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			TypoGeneratorVM vm =(TypoGeneratorVM)Resources["vm"];

			if (vm.WordFile.Changed == true) {
				MessageBoxResult msgResult = MessageBox.Show("Do you want to save your changes before exiting?", "Save Changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

				if (msgResult == MessageBoxResult.Yes)
					vm.SaveWordFileCommand.Execute(null);
				else if (msgResult == MessageBoxResult.Cancel)
					e.Cancel = true;
			}
		}
	}
}
