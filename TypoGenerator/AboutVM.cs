using NemMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Deployment;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Deployment.Application;

namespace TypoGenerator {
	public class AboutVM : NotifyPropertyChanged {
		private ObservableCollection<TabItem> _licenses;

		public string Title {
			get {
				return Assembly.GetExecutingAssembly().GetName().ToString();
			}
		}

		public string Version {
			get {
				if (!Debugger.IsAttached && ApplicationDeployment.IsNetworkDeployed) {
					return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
				} else {
					return Assembly.GetExecutingAssembly().GetName().Version.ToString();
				}
			}
		}
		public ObservableCollection<TabItem> Licenses {
			get {
				return _licenses;
			}
			private set {
				SetProperty(ref _licenses, value);
			}
		}

		public AboutVM() {
			Licenses = GetLicenses();
		}

		private ObservableCollection<TabItem> GetLicenses() {
			ObservableCollection<TabItem> tabs = new ObservableCollection<TabItem>();

			foreach (string file in Directory.GetFiles(@"Licenses\")) {
				string filename = Path.GetFileNameWithoutExtension(file);
				string header = Regex.Replace(filename, "_License$", "", RegexOptions.IgnoreCase);

				TabItem tab = new TabItem();
				tab.Header = header;
				tab.Content = new TextBox() { Text = File.ReadAllText(file), IsReadOnly = true, TextWrapping = System.Windows.TextWrapping.Wrap, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };

				tabs.Add(tab);
			}

			return tabs;
		}
	}
}
