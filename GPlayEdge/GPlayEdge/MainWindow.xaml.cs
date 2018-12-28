using DiscordRPC;
using DiscordRPC.Logging;
using MahApps.Metro.Controls;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using System;
using System.Windows;
using System.Threading.Tasks;

namespace GPlayEdge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		private string CustomStyle;
		private static int DiscordPipe = -1;
		private static string ClientID = "528170989016711169";
		private static DiscordRpcClient client = new DiscordRpcClient(ClientID, true, DiscordPipe);

		public MainWindow()
		{
			InitializeComponent();
			GPlayWebView.Navigate(new Uri("https://play.google.com/music/"));

			#region Initialize RPC
			client.Logger = new FileLogger("discord-rpc.log") { Level = LogLevel.Warning };

			client.OnReady += (sender, msg) =>
			{
				using (System.IO.StreamWriter sw = System.IO.File.AppendText("discord-rpc.log"))
				{
					sw.WriteLine($"Connected to Discord with user {msg.User.Username}");
				}
			};

			client.OnPresenceUpdate += (sender, msg) =>
			{
				using (System.IO.StreamWriter sw = System.IO.File.AppendText("discord-rpc.log"))
				{
					sw.WriteLine("Presence has been updated");
				}
			};

			var timer = new System.Timers.Timer(3000);
			timer.Elapsed += (sender, evt) =>
			{
				client.Invoke();
			};
			timer.Start();

			client.Initialize();
			client.SetPresence(new RichPresence()
			{
				Details = $"Listening to nothing",
				Timestamps = Timestamps.Now,
				Assets = new Assets()
				{
					LargeImageKey = "google-play-music",
					LargeImageText = "Google Play Music"
				}
			});
			#endregion
		}

		private void GPlayWebView_ScriptNotify(object sender, WebViewControlScriptNotifyEventArgs e)
		{
			// Shows messagebox if script notifications are needed
			MessageBox.Show(e.Value, e.Uri?.ToString() ?? string.Empty);
		}

		private void GPlayWebView_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
		{
			// Display error dialog if navigation fails
			if (!e.IsSuccess)
				MessageBox.Show($"Navigation to {e.Uri?.ToString() ?? "NULL"}", $"Error: {e.WebErrorStatus}", MessageBoxButton.OK, MessageBoxImage.Error);

			// Synchronously call Update
			Update();
		}

		private void GPlayWebView_PermissionRequested(object sender, WebViewControlPermissionRequestedEventArgs e)
		{
			// Permissions
			if (e.PermissionRequest.State == WebViewControlPermissionState.Allow)
				return;
			if (e.PermissionRequest.State == WebViewControlPermissionState.Defer)
				GPlayWebView.GetDeferredPermissionRequestById(e.PermissionRequest.Id)?.Allow();
			else
				e.PermissionRequest.Allow();
		}

		private void GPlayWebView_DOMContentLoaded(object sender, WebViewControlDOMContentLoadedEventArgs e)
		{
			// Custom JS function for custom CSS
			string CustomCSSScript = "(function(){" +
				"var style=document.getElementById('gmusic_custom_css');" +
				"if(!style){ style = document.createElement('STYLE');" +
				"style.type='text/css';" +
				"style.id='gmusic_custom_css'; " +
				"style.innerText = \"" + CustomStyle + "\";" +
				"document.getElementsByTagName('HEAD')[0].appendChild(style);" +
				"} } )()";
			// Execute the script asychronously
			GPlayWebView.InvokeScriptAsync("eval", new string[] { CustomCSSScript });
		}

		private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Dispose the RPC client
			client.Dispose();
		}

		/// <summary>
		/// Updates RPC content (DO NOT AWAIT)
		/// </summary>
		/// <returns></returns>
		async Task Update()
		{
			while (true)
			{
				try
				{
					string songName = await GPlayWebView.InvokeScriptAsync("eval", new string[] { "document.getElementById('currently-playing-title').innerText" });
					string artist = await GPlayWebView.InvokeScriptAsync("eval", new string[] { "document.getElementById('player-artist').innerText" });
					Title = $"{songName} - Google Play Music";
					client.SetPresence(new RichPresence()
					{
						Details = $"Listening to {songName}",
						State = $"by {artist}",
						Assets = new Assets()
						{
							LargeImageKey = "google-play-music",
							LargeImageText = "Google Play Music"
						}
					});
				}
				catch
				{
					Title = $"Google Play Music";
					client.SetPresence(new RichPresence()
					{
						Details = $"Listening to nothing",
						Assets = new Assets()
						{
							LargeImageKey = "google-play-music",
							LargeImageText = "Google Play Music"
						}
					});
				}
				await Task.Delay(200);
			}
		}
	}
}
