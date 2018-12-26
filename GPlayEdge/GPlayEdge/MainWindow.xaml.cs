using MahApps.Metro.Controls;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using System;
using System.Windows;

namespace GPlayEdge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		private string CustomStyle;
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
			if (e.PermissionRequest.State == WebViewControlPermissionState.Defer)
				GPlayWebView.GetDeferredPermissionRequestById(e.PermissionRequest.Id)?.Allow();
			else
				e.PermissionRequest.Allow();
		}

		private void GPlayWebView_DOMContentLoaded(object sender, WebViewControlDOMContentLoadedEventArgs e)
		{
			string CustomCSSScript = "(function(){" +
				"var style=document.getElementById('gmusic_custom_css');" +
				"if(!style){ style = document.createElement('STYLE');" +
				"style.type='text/css';" +
				"style.id='gmusic_custom_css'; " +
				"style.innerText = \"" + CustomStyle + "\";" +
				"document.getElementsByTagName('HEAD')[0].appendChild(style);" +
				"} } )()";
			GPlayWebView.InvokeScriptAsync("eval", new string[] { CustomCSSScript });
		}
	}
}
