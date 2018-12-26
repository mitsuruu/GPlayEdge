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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using MahApps.Metro.Controls;

namespace GPlayEdge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void GPlayWebView_ScriptNotify(object sender, WebViewControlScriptNotifyEventArgs e)
		{

		}

		private void GPlayWebView_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
		{

		}

		private void GPlayWebView_PermissionRequested(object sender, WebViewControlPermissionRequestedEventArgs e)
		{

		}

		private void GPlayWebView_DOMContentLoaded(object sender, WebViewControlDOMContentLoadedEventArgs e)
		{

		}
	}
}
