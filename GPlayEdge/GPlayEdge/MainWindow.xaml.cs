using DiscordRPC;
using DiscordRPC.Logging;
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
		private static int DiscordPipe = -1;
		private static string ClientID = "528170989016711169";
		private static DiscordRpcClient client = new DiscordRpcClient(ClientID, true, DiscordPipe);

		public MainWindow()
		{
			InitializeComponent();
			GPlayWebView.Navigate(new Uri("https://play.google.com/music/"));
			Initialize(client);
		}

		private void GPlayWebView_ScriptNotify(object sender, WebViewControlScriptNotifyEventArgs e)
		{
			MessageBox.Show(e.Value, e.Uri?.ToString() ?? string.Empty);
		}

		private void GPlayWebView_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
		{
			if (!e.IsSuccess)
				MessageBox.Show($"Navigation to {e.Uri?.ToString() ?? "NULL"}", $"Error: {e.WebErrorStatus}", MessageBoxButton.OK, MessageBoxImage.Error);
			Update();
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

		private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Dispose();
		}

		#region DiscordRPC
		async void Update()
		{
			// Get song name (if possible)
			try
			{
				string songName = await GPlayWebView.InvokeScriptAsync("eval", new string[] { "document.getElementById('currently-playing-title').innerText" });
				string artist = await GPlayWebView.InvokeScriptAsync("eval", new string[] { "document.getElementById('player-artist').innerText" });
				Title = $"{songName} - Google Play Music";
				UpdatePresence(client, songName, artist, true);
			}
			catch
			{
				Title = $"Nothing - Google Play Music";
			}
		}

		static void Initialize(DiscordRpcClient client)
		{
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
		}

		void UpdatePresence(DiscordRpcClient client, string songName, string artist, bool playing)
		{
			if (!playing)
			{
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
			}
			else
			{
				client.SetPresence(new RichPresence()
				{
					Details = $"Listening to {songName}",
					State = $"by {artist}",
					Timestamps = Timestamps.Now,
					Assets = new Assets()
					{
						LargeImageKey = "google-play-music",
						LargeImageText = "Google Play Music"
					}
				});
			}
		}

		void Dispose()
		{
			var timer = new System.Timers.Timer(150);
			var client = new DiscordRpcClient(ClientID);
			timer.Dispose();
			client.Dispose();
		}
		#endregion
	}
}
