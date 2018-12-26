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
			GPlayWebView.Navigate(new Uri("https://play.google.com/music"));
		}

		private void GPlayWebView_ScriptNotify(object sender, WebViewControlScriptNotifyEventArgs e)
		{
			MessageBox.Show(e.Value, e.Uri?.ToString() ?? string.Empty);
		}

		private void GPlayWebView_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
		{
			if (!e.IsSuccess)
				MessageBox.Show($"Navigation to {e.Uri?.ToString() ?? "NULL"}", $"Error: {e.WebErrorStatus}", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void GPlayWebView_PermissionRequested(object sender, WebViewControlPermissionRequestedEventArgs e)
		{
			if (e.PermissionRequest.State == WebViewControlPermissionState.Allow) return;

			string message = $"{e.PermissionRequest.Uri.Host} is requesting access to {e.PermissionRequest.PermissionType}";
			if (e.PermissionRequest.State == WebViewControlPermissionState.Defer)
				GPlayWebView.GetDeferredPermissionRequestById(e.PermissionRequest.Id)?.Allow();
			else
				e.PermissionRequest.Allow();
		}

		private void GPlayWebView_DOMContentLoaded(object sender, WebViewControlDOMContentLoadedEventArgs e)
		{

		}
	}
}
