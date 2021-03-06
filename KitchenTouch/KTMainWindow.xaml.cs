﻿/* 
 *  Project     : KitchenTouch
 *  Source      : http://kitchentouch.codeplex.com/
 *  Author      : Ruslan Ulanov
 *  Description : KitchenTouch - home automation GUI for 
 *                touch-capable devices.
 * 
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace KitchenTouch
{

    /// <summary>
    /// Interaction logic for KTMainWindow.xaml
    /// </summary>
    public partial class KTMainWindow : Window
    {

        #region * GLOBAL CONFIG *
		private const double dblVersion = 1.0;
		private bool bEnableDebug = false;
		private string strSysPicturesDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		private string strSysMusicDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
		//private string strSysDocumentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		private string strConfigFile = "KitchenTouch.xml";
        private string strPlayBtnUrl = "pack://application:,,,/Resources/playerplay.png";
        //private string strStopBtnUrl = "pack://application:,,,/Resources/playerstop.png";
		private string strPauseBtnUrl = "pack://application:,,,/Resources/playerpause.png";
        private string strRadioAlbumArt = "pack://application:,,,/Resources/kajol.jpg";
		// internet browser
		string iBrowser_CurrentURL;
		System.Windows.Controls.TextBox tbCurrent;
		// background color for Album Art
		private const double COLOR_VARIATION = 10;
		private double R = 100;
		private double G = 80;
		private double B = 100;
		private ArrayList arrFilesFound = new ArrayList();
		private ArrayList arrPlaylists = new ArrayList();
		#endregion

		#region * USER CONFIG *
		// Config values to save off to XML
		[XmlRoot("KitchenTouchConfig")]
		public class KitchenTouchConfig
		{
			[XmlAttribute("Saved")]	public DateTime dCfgDateSaved;
			[XmlAttribute("Version")] public double dblCfgVersion;
			#region CFG Generic
				[XmlElement("EnableTouchKeyboard")]	public bool bCfgEnableTouchKeyboard;
			#endregion

			#region CFG Home/Pictures
				[XmlElement("PicturesDir")]	public string sCfgPicturesDir;

				[XmlElement("PicturesInterval")] public int iCfgSlideShowInterval;
			#endregion
			
			#region CFG Music
				[XmlElement("MusicEnabled")] public bool bCfgMusicEnabled;
				[XmlElement("MusicDir")] public string sCfgMusicDir;

				[XmlElement("Radio1Caption")] public string sCfgRadio1Caption;
				[XmlElement("Radio1URL")] public string sCfgRadio1URL;

				[XmlElement("Radio2Caption")] public string sCfgRadio2Caption;
				[XmlElement("Radio2URL")] public string sCfgRadio2URL;

				[XmlElement("Radio3Caption")] public string sCfgRadio3Caption;
				[XmlElement("Radio3URL")] public string sCfgRadio3URL;

				[XmlElement("Radio4Caption")] public string sCfgRadio4Caption;
				[XmlElement("Radio4URL")] public string sCfgRadio4URL;
			#endregion
			
			#region CFG Webcams
				[XmlElement("WebCamEnabled")] public bool bCfgWebCamEnabled;
				[XmlElement("WebCamInterval")] public int iCfgWebCamInterval;

				[XmlElement("WebCam1Caption")] public string sCfgWebCam1Caption;
				[XmlElement("WebCam1URL")] public string sCfgWebCam1URL;

				[XmlElement("WebCam2Caption")] public string sCfgWebCam2Caption;
				[XmlElement("WebCam2URL")] public string sCfgWebCam2URL;

				[XmlElement("WebCam3Caption")] public string sCfgWebCam3Caption;
				[XmlElement("WebCam3URL")] public string sCfgWebCam3URL;

				[XmlElement("WebCam4Caption")] public string sCfgWebCam4Caption;
				[XmlElement("WebCam4URL")] public string sCfgWebCam4URL;
			#endregion
			
			#region CFG Lights
				[XmlElement("AutomationEnabled")] public bool bCfgAutomationEnabled;
				[XmlElement("AutomationURL")] public string sCfgAutomationURL;
			#endregion

			#region CFG Internet
				[XmlElement("BrowserEnabled")]	public bool bCfgBrowserEnabled;
				[XmlElement("WebSite1Caption")]  public string sCfgWebSite1Caption;
				[XmlElement("WebSite1URL")] public string sCfgWebSite1URL;

				[XmlElement("WebSite2Caption")] public string sCfgWebSite2Caption;
				[XmlElement("WebSite2URL")] public string sCfgWebSite2URL;

				[XmlElement("WebSite3Caption")] public string sCfgWebSite3Caption;
				[XmlElement("WebSite3URL")] public string sCfgWebSite3URL;
			#endregion

			#region CFG Weather
				[XmlElement("WeatherEnabled")]	public bool bCfgWeatherEnabled;
				[XmlElement("ZipCode")] public string sCfgZipCode;
			#endregion

			#region CFG Calendar
				[XmlElement("CalendarEnabled")] public bool bCfgCalendarEnabled;
			#endregion
		}

		#region USR Generic
			private bool bEnableTouchKeyboard;
		#endregion

		#region USR Home/Pictures
			private string strMyPicturesDir;
			private int iSlideShowInterval;
		#endregion

		#region USR Music
			private string strMusicDir;

			private string strRadio1Caption, strRadio2Caption, strRadio3Caption, strRadio4Caption;
			private string strRadio1URL, strRadio2URL, strRadio3URL, strRadio4URL;
		#endregion

		#region USR Webcams
			private int iWebCamInterval;
			private string strWebCam1Caption;
			private string strWebCam1URL;

			private string strWebCam2Caption;
			private string strWebCam2URL;

			private string strWebCam3Caption;
			private string strWebCam3URL;

			private string strWebCam4Caption;
			private string strWebCam4URL;
		#endregion

		#region USR Lights
			private string strAutomationURL;
		#endregion

		#region USR Internet
			private string strWebSite1Caption, strWebSite2Caption, strWebSite3Caption;
			private string strWebSite1URL, strWebSite2URL, strWebSite3URL;
		#endregion

		#region USR Weather
			private string strZipCode;
		#endregion

		#endregion

        #region * Init *
        protected DispatcherTimer ClockTimer;
        protected DispatcherTimer SlideShowTimer;
        protected DispatcherTimer PlayerTimer;
		protected DispatcherTimer WebCamTimer;
        //private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;

        private int iCurrentSlide = 0;
        private string[] jpgFiles = new string[0];
        private bool bStartSlideShow = false;

        private int iCurrentMP3 = 0;
        private string[] mp3Files = new string[0];
		private bool bMusicPlaying = false;
		private bool bRadioPlaying = false;

        public KTMainWindow()
        {
            //Initialize application
            InitializeComponent();

			fnGetAppSettings();

            this.SetClock();
            this.ClockTimer = new DispatcherTimer();
            this.ClockTimer.Interval = new TimeSpan(0, 1, 0);
            this.ClockTimer.Tick += new EventHandler(Timer_Tick);
            this.ClockTimer.Start();

            this.SlideShowTimer = new DispatcherTimer();
            this.SlideShowTimer.Interval = new TimeSpan(0, 0, iSlideShowInterval);
            this.SlideShowTimer.Tick += new EventHandler(SlideShow_Tick);

			this.WebCamTimer = new DispatcherTimer();
			this.WebCamTimer.Interval = new TimeSpan(0, 0, iWebCamInterval);
			this.WebCamTimer.Tick += new EventHandler(WebCam_Tick);

            this.mediaElement.MediaEnded += new RoutedEventHandler(btnMNext_Click);
			if (strAutomationURL == null)
			{
				tabLights.Visibility = Visibility.Collapsed;
			}
			
			//populate Playlists in the Music Library
			fnGetFilesRecursive(strSysMusicDir, @"*.m3u");
			arrPlaylists = arrFilesFound;
			//arrFilesFound = new string[0]; //clean array for next use
			//arrPlaylists = System.IO.Directory.GetFiles(strSysMusicDir, @"*.wpl|*.m3u");
        }

        private void lblWelcome_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Exit the application
            this.Close();
        }
        #endregion

        #region * GENERIC FUNCTIONS *
        private void fnDebugWrite(string msg)
        {
			if (!bEnableDebug) return;

			try
			{
				this.statusBarDebug.Content = msg;

				//TODO: write to file
			}
			catch
			{
			}
        }

        private void fnStatusWrite(string msg)
        {
			try
			{
				this.statusBarMain.Content = msg;
			}
			catch
			{
			}
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
			fnShowWindowSize();

			if (tabMusic.IsSelected)
			{
				try
				{
					btnMVolDn.Visibility = this.Width >= 1024 ? Visibility.Visible : Visibility.Collapsed;
					btnMVolUp.Visibility = this.Width >= 1024 ? Visibility.Visible : Visibility.Collapsed;
				}
				catch
				{
				}
			}
        }

		private void fnShowWindowSize()
		{
			try
			{
				string msg = "Window size: " + this.Width + "x" + this.Height;
				lblStatusResolution.Content = msg;
				fnDebugWrite(msg);
				//this.Title = msg;
			}
			catch
			{
			}
		}

        private BitmapImage GetImage(string myUrl)
        {
            BitmapImage bi = new BitmapImage();
            try
            {
                bi.BeginInit();
                bi.UriSource = new Uri(myUrl, UriKind.RelativeOrAbsolute);
                //bi.DecodePixelWidth = 320;
                bi.EndInit();
            }
            catch (Exception ex)
            {
                fnDebugWrite("Error in GetImage: " + ex.Message);
                bi = new BitmapImage();
            }
            return bi;
        }

		#region * GET RECURSIVE FILE LIST *
		/// <summary>
		/// Retrieves a list of files using a wildcard and stores it 
		/// in arrFilesFound
		/// </summary>
		/// <param name="sDir">Root folder</param>
		/// <param name="sExt">Files wildcard</param>
		private void fnGetFilesRecursive(string sDir, string sExt)
		{
			try
			{
				this.Cursor = System.Windows.Input.Cursors.Wait;
				foreach (string d in Directory.GetDirectories(sDir))
				{
					foreach (string f in Directory.GetFiles(d, sExt))
					{
						this.arrFilesFound.Add(f);
					}
					fnGetFilesRecursive(d, sExt);
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				this.Cursor = System.Windows.Input.Cursors.Arrow;
			}
		}
		#endregion;

        #endregion

        #region * CLOCK WORKS *
        /// <summary>
        /// Timer.Tick event handler
        /// </summary>
        /// <param name="sender"></param>
        void Timer_Tick(object sender, EventArgs e)
        {
            this.SetClock();
        }

        /// <summary>
        /// Sets the time on the clock label
        /// </summary>
        public void SetClock()
        {
			try
			{
				this.lblCurrentDate.Content = "Today is " + DateTime.Now.ToString("D");
				this.lblCurrentDate.Content += " " + DateTime.Now.ToString("t");
			}
			catch
			{
			}
        }
        #endregion

        #region * HOME PANE *

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				System.Windows.MessageBox.Show("I know you are home. Stop pushing my buttons! :)", "Doh!", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			catch
			{
			}
        }

        private void btnAway_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tabPHome_Loaded(object sender, RoutedEventArgs e)
        {
            //fnDebugWrite("tabPHome_Loaded");
			try
			{
				string[] images = fnGetSlides();
				if (jpgFiles.Length > 0)
				{
					fnShowSlide();
				}
			}
			catch (Exception ex)
			{
				fnDebugWrite("Error in tabPHome_Loaded: " + ex.Message);
			}
        }

        private void tabPHome_Unloaded(object sender, RoutedEventArgs e)
        {
            //fnDebugWrite("tabPHome_Unloaded");
            fnSlideShowStop();
        }

        private void fnShowSlide()
        {
			try
			{
				string img = jpgFiles[iCurrentSlide];
				if (img != null)
				{
					try
					{
						imgSlideShow.Source = GetImage(img);
						imgSlideShow.ToolTip = img;
						fnStatusWrite(img);
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
        }

        private string[] fnGetSlides()
        {
			try
			{
				if (jpgFiles.Length == 0)
				{
					jpgFiles = System.IO.Directory.GetFiles(strMyPicturesDir, @"*.jpg");
					fnDebugWrite("Found " + jpgFiles.Length + " image(s)");
				}
			}
			catch
			{
			}
			return jpgFiles;
		}

        private void SlideShow_Tick(object sender, EventArgs e)
        {
            this.fnSlideShowPlay();
        }

        private void fnSlideShowPrev()
        {
			int i = iCurrentSlide;
			--i;

			if (i < 0 && jpgFiles.Length > 0)
			{
				i = jpgFiles.Length - 1;
			}

			iCurrentSlide = i;
            fnDebugWrite("Current image index: " + iCurrentSlide);
        }

        private void fnSlideShowNext()
        {
			int i = iCurrentSlide;
			++i;

            if (i >= jpgFiles.Length)
            {
                i = 0;
            }

			iCurrentSlide = i;
            fnDebugWrite("Current image index: " + iCurrentSlide);
        }

        private void fnSlideShowStop()
        {
			try
			{
				bStartSlideShow = false;
				btnToggleSlideShow.Content = "Start slide show";
				this.SlideShowTimer.Stop();
			}
			catch
			{
			}
        }

        private void fnSlideShowStart()
        {
			try
			{
				bStartSlideShow = true;
				btnToggleSlideShow.Content = "Stop slide show";
				this.SlideShowTimer.Start();
			}
			catch
			{
			}
        }

        private void fnSlideShowToggle()
        {
            switch (bStartSlideShow)
            {
                case true:
                    fnSlideShowStop();
                    break;
                case false:
                    fnSlideShowStart();
                    break;
            }
        }

        public void fnSlideShowPlay()
        {
            try
            {
                fnSlideShowNext();
                fnShowSlide();
            }
            catch (Exception ex)
            {
                fnDebugWrite("Error in fnSlideShowPlay: " + ex.Message);
            }
        }

        private void btnToggleSlideShow_Click(object sender, RoutedEventArgs e)
        {
            fnSlideShowToggle();
        }

        private void imgSlideShow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //string img = jpgFiles[iCurrentSlide];
            fnSlideShowToggle();
            
            //TODO: open current image in external viewer or in Explorer or in fullscreen
            /*
            if (imgSlideShow.Width < 1024)
            {
                imgSlideShow.Width = 1024;
                imgSlideShow.Height = 768;
            }
            else {
                imgSlideShow.Width = 320;
                imgSlideShow.Height = 240;
            }*/
        }

        private void btnSlideShowPrev_Click(object sender, RoutedEventArgs e)
        {
            fnSlideShowStop();
            fnSlideShowPrev();
            fnShowSlide();
        }

        private void btnSlideShowNext_Click(object sender, RoutedEventArgs e)
        {
            fnSlideShowStop();
            fnSlideShowNext();
            fnShowSlide();
        }
        #endregion

        #region * WEBCAMS PANE *
		private void WebCam_Tick(object sender, EventArgs e)
		{
			this.fnWebCamRefresh();
		}
		
		private void fnWebCamRefresh()
		{
			try
			{
				if (wbCam.Source != null)
				{
					wbCam.Refresh(true);
				}
			}
			catch
			{
			}
		}

        private void tabPCameras_Loaded(object sender, RoutedEventArgs e)
        {
            lblCamName.Content = "Select camera";
            //fnDebugWrite("tabPCameras_Loaded");
        }

        private void tabPCameras_Unloaded(object sender, RoutedEventArgs e)
        {
			this.WebCamTimer.Stop();
			cbWebCamRefresh.IsChecked = false;

			fnDebugWrite("tabPCameras_Unloaded");
        }

        private void fnUpdateCamImage(string title, string url, int refresh)
        {
            try
            {
                this.lblCamName.Content = title + " camera";
                wbCam.Navigate(new Uri(url, UriKind.Absolute));
				//WebCamTimer.Start();
            }
            catch (Exception ex)
            {
                fnDebugWrite("Error in fnUpdateCamImage: " + ex.Message);
            }
        }

		private void btnWebCam_Click(object sender, RoutedEventArgs e)
        {
			System.Windows.Controls.Button btn = (System.Windows.Controls.Button)e.OriginalSource;
			string caption = "", url = "";
			switch (btn.Name)
			{
				case "btnWebCam1":
					caption = strWebCam1Caption;
					url = strWebCam1URL;
					break;
				case "btnWebCam2":
					caption = strWebCam2Caption;
					url = strWebCam2URL;
					break;
				case "btnWebCam3":
					caption = strWebCam3Caption;
					url = strWebCam3URL;
					break;
				case "btnWebCam4":
					caption = strWebCam4Caption;
					url = strWebCam4URL;
					break;
				default:
					break;
			}
			if (caption != "" && url != "")
			{
				fnStatusWrite("Loading " + caption + ": " + url);
				fnUpdateCamImage(caption, url, 10);
			}
        }

		private void cbWebCamRefresh_Clicked(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.CheckBox cb = (System.Windows.Controls.CheckBox)e.OriginalSource;
			if (cb.IsChecked == true)
			{
				this.WebCamTimer.Start();
			}
			else
			{
				this.WebCamTimer.Stop();
			}
		}
        #endregion

        #region * WEATHER PANE *
        private void tabPWeather_Loaded(object sender, RoutedEventArgs e)
        {
			frameWeather.Source = new Uri("http://www.wunderground.com/cgi-bin/findweather/getForecast?brand=wxmap&query=" + strZipCode);
        }

        private void tabPWeather_Unloaded(object sender, RoutedEventArgs e)
        {
			frameWeather.Navigate(new Uri("about:blank"));
        }
		/*
				private void goWeatherDefault_Click(object sender, RoutedEventArgs e)
				{
					frameWeather.Navigate(new Uri("http://www.wunderground.com/cgi-bin/findweather/getForecast?brand=wxmap&query=" + strZipCode));
				}

				private void goWund_Click(object sender, RoutedEventArgs e)
				{
					frameWeather.Navigate(new Uri("http://www.wund.com/cgi-bin/findweather/getForecast?query=94568"));
				}

				private void goWeather_Click(object sender, RoutedEventArgs e)
				{
					frameWeather.Navigate(new Uri("http://xhtml.weather.com/xhtml/cc/94568"));
				}
		*/
		#endregion

		#region * LIGHTS / HOME AUTOMATION *
		private void tabPLights_Loaded(object sender, RoutedEventArgs e)
		{
			lBrowser.Navigate(new Uri(strAutomationURL));
		}
		#endregion

		#region * INTERNET PANE *
		private void fnBrowserNavigate(string url)
        {
			try
			{
				iBrowser.Navigate(new Uri(url));
			}
			catch (UriFormatException ex)
			{
				System.Windows.MessageBox.Show("Please check the address and try again.\n\n" + ex.Message, "Incorrect address!", MessageBoxButton.OK);
			}
			catch (Exception ex)
			{
				fnDebugWrite("Error in fnBrowserNavigate: " + ex.Message);
			}
        }

        private void tabPBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            if (iBrowser.Source == null)
            {
                iBrowser_CurrentURL = "http://google.com/";
            }

            if (iBrowser_CurrentURL != "about:blank")
            {
                fnBrowserNavigate(iBrowser_CurrentURL);
            }
            else
            {
                fnBrowserNavigate("http://google.com/");
            }
        }

        private void tabPBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            iBrowser_CurrentURL = iBrowser.Source.ToString();
            fnBrowserNavigate("about:blank");
			fnStatusWrite("");
        }

		private void iBrowser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			try
			{
				//display URL of the last navigated page's in the status bar
				string strCurrentURL = iBrowser.Source.ToString();
				fnStatusWrite(strCurrentURL);
			}
			catch
			{
			}
		}

		private void btnBStop_Click(object sender, RoutedEventArgs e)
		{
			//TODO: Implement real stop function
			fnBrowserNavigate("about:blank");
			fnStatusWrite("");
		}

		private void btnBRefresh_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (iBrowser.Source.ToString() != "")
				{
					iBrowser.Refresh(true);
				}
			}
			catch
			{
			}
		}

        private void btnGoTo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string url = txtUrl.Text.Trim();
            fnBrowserNavigate(url);
        }

        private void goHistory_Click(object sender, RoutedEventArgs e)
        {
            //go to previous page
			try
			{
				System.Windows.Controls.Button btn = (System.Windows.Controls.Button)e.OriginalSource;
				switch (btn.Name)
				{
					case "goBack":
						if (iBrowser.CanGoBack) iBrowser.GoBack();
						break;
					case "goForward":
						if (iBrowser.CanGoForward) iBrowser.GoForward();
						break;
				}
			}
			catch
			{
			}
        }

        private void goWeb_Click(object sender, RoutedEventArgs e)
        {
			System.Windows.Controls.Button btn = (System.Windows.Controls.Button)e.OriginalSource;
			string caption = "", url = "";
			switch (btn.Name)
			{
				case "btnGoWeb0":
					caption = "Home";
					url = "http://www.google.com/";
					break;
				case "btnGoWeb1":
					caption = strWebSite1Caption;
					url = strWebSite1URL;
					break;
				case "btnGoWeb2":
					caption = strWebSite2Caption;
					url = strWebSite2URL;
					break;
				case "btnGoWeb3":
					caption = strWebSite3Caption;
					url = strWebSite3URL;
					break;
				default:
					break;
			}
			if (caption != "" && url != "")
			{
				fnStatusWrite("Loading " + caption + ": " + url);
				fnBrowserNavigate(url);
			}
			
        }
        #endregion

        #region * MUSIC PANE *
        private void tabPMusic_Loaded(object sender, RoutedEventArgs e)
        {
			try
			{
				btnMVolDn.Visibility = this.Width >= 1023 ? Visibility.Visible : Visibility.Collapsed;
				btnMVolUp.Visibility = this.Width >= 1023 ? Visibility.Visible : Visibility.Collapsed;
				fnGetMusic(false);
				//CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

				this.lbMLib.Items.Clear();
				if (this.arrPlaylists.Count > 0)
				{
					foreach (string p in this.arrPlaylists)
					{
						this.lbMLib.Items.Add(System.IO.Path.GetFileNameWithoutExtension(p));
					}
				}
				else
				{
					this.lbMLib.Items.Add("No playlists were found in " + strSysMusicDir);
				}
			}
			catch
			{
			}
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            fnPlayerUpdateInfo();
        }

		private void lbPlayLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				fnMusicStop();
				iCurrentMP3 = lbPlayLists.SelectedIndex;
				fnMusicLoad();
				fnMusicPlay();
				fnDebugWrite("Selected index: " + lbPlayLists.SelectedIndex);
			}
			catch
			{
			}
		}

		private void lbMLib_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				fnMusicStop();
				fnPlaylistPlay(lbMLib.SelectedIndex);
				fnDebugWrite("Selected playlist index: " + lbMLib.SelectedIndex);
			}
			catch
			{
			}
		}

		private void tabpMCurrent_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				lbPlayLists.Width = tabpMCurrent.ActualWidth;
				lbPlayLists.Height = tabpMCurrent.ActualHeight;
			}
			catch
			{
			}
		}

		private void tabpMLib_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				lbMLib.Width = tabpMLib.ActualWidth;
				lbMLib.Height = tabpMLib.ActualHeight;
			}
			catch
			{
			}
		}

        public string[] fnGetMusic(bool reload)
        {
            try
            {
                if (mp3Files.Length == 0 || reload)
                {
					fnMusicStop();
					iCurrentMP3 = 0;
                    mp3Files = System.IO.Directory.GetFiles(strMusicDir, @"*.mp3");
                    this.lbPlayLists.Items.Clear();
                    foreach (string p in mp3Files)
                    {
						this.lbPlayLists.Items.Add(System.IO.Path.GetFileNameWithoutExtension(p));
                    }
                    fnUpdateAlbumCover();
                }

				if (mp3Files.Length == 0)
				{
					fnMusicAllowPlay(false);
				}
				else
				{
					fnMusicAllowPlay(true);
				}

                fnDebugWrite("Found " + mp3Files.Length + " mp3(s) in " + strMusicDir);
            }
            catch (Exception ex)
            {
                fnDebugWrite("Error in fnGetMusic: " + ex.Message);
            }
            return mp3Files;
        }

		private void fnMusicAllowPlay(bool bAllowPlay)
		{
			if (bAllowPlay)
			{
				btnPlayerCtrl.IsEnabled = btnMPlay.IsEnabled = true;
			}
			else
			{
				btnPlayerCtrl.IsEnabled = btnMPlay.IsEnabled = false;
			}
		}

		private void fnMusicLoad()
		{
			try
			{
				if (mp3Files.Length <= 0)
				{
					fnGetMusic(true);
				}

				if (mediaElement.Source == null || !bMusicPlaying)
				{
					string mp3file = mp3Files[iCurrentMP3];
					if (mp3file != null)
					{
						mediaElement.Source = new Uri(mp3file, UriKind.RelativeOrAbsolute);
						mediaElement.ToolTip = mp3file;
						fnStatusWrite(mp3file);
						/* 
						 * Crude hack to get artist/album info from the path. 
						 * This assumes that your collection is organised within the following structure
						 * [My Music folder]\<Artist>\<Album>\<SongName.mp3>
						 * 
						 * TODO: Read info from ID3 tags
						 */
						string[] w = Regex.Split(mp3file, @"\\");
						int i = w.Length;
						if (i > 4)
						{
							lblMArtist.Content = "Artist: " + w[i - 3];
							lblMAlbum.Content = "Album:  " + w[i - 2];
						}
						lblMFile.Content = "Song: " + System.IO.Path.GetFileNameWithoutExtension(mp3file);
						mediaElement.Position = new TimeSpan();
					}
					else
					{
						fnDebugWrite("Error: There are no MP3 files to play!");
					}
				}
			}
			catch (Exception e)
			{
				fnDebugWrite("Error: " + e.Message);
			}
		}

		private void fnPlaylistPlay(int iCurrent)
		{
			try
			{
				if (true)
				{
					string plsFile = arrPlaylists[iCurrent].ToString();
					if (plsFile != null)
					{
						mediaElement2.Source = new Uri(plsFile, UriKind.RelativeOrAbsolute);
						mediaElement2.ToolTip = lblMFile.Content = "Playlist: " + plsFile;
						fnStatusWrite("Playlist: " + plsFile);
						/* 
						 * Crude hack to get artist/album info from the path. 
						 * This assumes that your collection is organised within the following structure
						 * [My Music folder]\<Artist>\<Album>\<SongName.mp3>
						 * 
						 * TODO: Read info from ID3 tags
						 
						string[] w = Regex.Split(mp3file, @"\\");
						int i = w.Length;
						if (i > 4)
						{
							lblMArtist.Content = "Artist: " + w[i - 3];
							lblMAlbum.Content = "Album:  " + w[i - 2];
						}
						lblMFile.Content = "Song: " + System.IO.Path.GetFileNameWithoutExtension(mp3file);
						*/
						mediaElement2.Position = new TimeSpan();
						mediaElement2.Play();
						bRadioPlaying = true;
						//bMusicPlaying = true;
						fnPlayerCtrlPlay();
					}
					else
					{
						fnDebugWrite("Error: There are no MP3 files to play!");
					}
				}
			}
			catch (Exception e)
			{
				fnDebugWrite("Error: " + e.Message);
			}
		}

        public void fnMusicPlay()
        {
			if (bRadioPlaying)
			{
				mediaElement2.Stop();
			}

			if (mediaElement.Source == null)
			{
				fnMusicLoad();
			}
			mediaElement.Play();
			bMusicPlaying = true;
			fnPlayerCtrlPlay();
        }

		public void fnMusicPrev()
        {
            int i = iCurrentMP3;
			--i;

			if (i < 0 && mp3Files.Length > 0)
			{
				i = mp3Files.Length - 1;
			}

			iCurrentMP3 = i;
            fnDebugWrite("Current mp3 index: " + iCurrentMP3);
        }
		
		public void fnMusicNext()
        {
			int i = iCurrentMP3;
			++i;
            
			if (i >= mp3Files.Length)
            {
                i = 0;
            }
			iCurrentMP3 = i;
            fnDebugWrite("Current mp3 index: " + iCurrentMP3);
        }
        
        public void fnMusicPause()
        {
			try
			{
				if (bMusicPlaying)
				{
					mediaElement.Pause();
					bMusicPlaying = false;
				}
				if (bRadioPlaying)
				{
					mediaElement2.Pause();
					bRadioPlaying = false;
				}
				fnPlayerCtrlPause();
			}
			catch
			{
			}
        }
        
        public void fnMusicStop()
        {
			try
			{
				//if (bMusicPlaying)
				//{
					mediaElement.Stop();
					bMusicPlaying = false;
				//}
				//if (bRadioPlaying)
				//{
					mediaElement2.Stop();
					bRadioPlaying = false;
				//}
				fnPlayerCtrlPause();
			}
			catch
			{
			}
        }

        private void fnUpdateAlbumCover()
        {
			try
			{
				string[] strAlbumCoverImages = System.IO.Directory.GetFiles(strMusicDir, @"Folder.jpg");
				if (strAlbumCoverImages.Length > 0)
				{
					string strAlbumCoverImage = strAlbumCoverImages[0];
					this.imgAlbumCover.Source = GetImage(strAlbumCoverImage);
				}
			}
			catch
			{
			}
        }

        private void fnPlayerCtrlPlay()
        {
			try
			{
				imgMPlay.Source = imgPlayerCtrl.Source = GetImage(strPauseBtnUrl);
				CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
			}
			catch
			{
			}
        }

        private void fnPlayerCtrlPause()
        {
			try
			{
				imgMPlay.Source = imgPlayerCtrl.Source = GetImage(strPlayBtnUrl);
				CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
			}
			catch
			{
			}
        }

        private void fnPlayerUpdateInfo()
        {
			try
			{
				if (bMusicPlaying && mediaElement.NaturalDuration.HasTimeSpan)
				{
					// update label
					lblMPosition.Content = string.Format("{0:D2}", mediaElement.Position.Minutes) + ":" +
						string.Format("{0:D2}", mediaElement.Position.Seconds) + " / " +
						string.Format("{0:D2}", mediaElement.NaturalDuration.TimeSpan.Minutes) + ":" +
						string.Format("{0:D2}", mediaElement.NaturalDuration.TimeSpan.Seconds);

					// update progress bar
					double current = mediaElement.Position.TotalSeconds;
					double total = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
					pbarMPosition.Value = current / total * 100;
				}

				if (bMusicPlaying)
				{
					int seed = (int)DateTime.Now.Ticks;
					Random r = new Random(seed);

					R += (COLOR_VARIATION / 2 - r.NextDouble() * COLOR_VARIATION);
					G += (COLOR_VARIATION / 2 - r.NextDouble() * COLOR_VARIATION);
					B += (COLOR_VARIATION / 2 - r.NextDouble() * COLOR_VARIATION);

					R = Math.Max(0, Math.Min(R, 255));
					G = Math.Max(0, Math.Min(G, 255));
					B = Math.Max(0, Math.Min(B, 255));

					this.borderAlbumCover.Background = new SolidColorBrush(Color.FromArgb(255, (byte)R, (byte)G, (byte)B));
				}
			}
			catch
			{
			}
        }

        private void fnPlayRadio(string title, string url)
        {
            try
            {
                fnMusicStop();
                //mp3Files = new string[0];
                mediaElement2.Source = new Uri(url, UriKind.Absolute);
                mediaElement2.ToolTip = lblMFile.Content = "Internet Radio: " + title;
                fnStatusWrite("Internet Radio: " + title);
                fnDebugWrite("Playing Internet stream ["+url+"]");
                mediaElement2.Position = new TimeSpan();
                //this.lblMPosition.Content = "--:-- / --:--";
                this.imgAlbumCover.Source = GetImage(strRadioAlbumArt);
                //this.lbPlayLists.Items.Clear();
                //this.lbPlayLists.Items.Add(title);
                mediaElement2.Play();
				bRadioPlaying = true;
                //bMusicPlaying = true;
                fnPlayerCtrlPlay();
            }
            catch (Exception ex)
            {
                fnDebugWrite("Error in fnPlayRadio: " + ex.Message);
            }
        }

        private void btnMRadio_Click(object sender, RoutedEventArgs e)
        {
			System.Windows.Controls.Button btn = (System.Windows.Controls.Button)e.OriginalSource;
			string caption = "", url = "";
			switch (btn.Name)
			{
				case "btnMRadio1":
					caption = strRadio1Caption;
					url = strRadio1URL;
					break;
				case "btnMRadio2":
					caption = strRadio2Caption;
					url = strRadio2URL;
					break;
				case "btnMRadio3":
					caption = strRadio3Caption;
					url = strRadio3URL;
					break;
				case "btnMRadio4":
					caption = strRadio4Caption;
					url = strRadio4URL;
					break;
				default:
					break;
			}
			if (caption != "" && url != "")
			{
				fnPlayRadio(caption, url);
			}
        }

        private void btnMPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
				if (bMusicPlaying || bRadioPlaying)
				{
					fnMusicPause();
				}
				else
				{
					fnMusicPlay();
					//mediaElement.MediaEnded += new RoutedEventHandler(btnMPlay_Click);
				}
            }
            catch (Exception ex)
            {
                fnDebugWrite("Error in btnMPlay_Click: " + ex.Message);
            }
        }

        private void btnMStop_Click(object sender, RoutedEventArgs e)
        {
			fnMusicStop();
        }

        private void btnMNext_Click(object sender, RoutedEventArgs e)
        {
            fnMusicStop();
            fnMusicNext();
			fnMusicLoad();
            fnMusicPlay();
        }

        private void btnMPrev_Click(object sender, RoutedEventArgs e)
        {
            fnMusicStop();
            fnMusicPrev();
			fnMusicLoad();
            fnMusicPlay();
        }

        private void btnMVolUp_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				if (bMusicPlaying)
				{
					double vol = mediaElement.Volume + 0.1;
					mediaElement.Volume = (double)Math.Min(1.0, vol);
					fnDebugWrite("Volume: " + mediaElement.Volume * 100);
				}
				if (bRadioPlaying)
				{
					double vol = mediaElement2.Volume + 0.1;
					mediaElement2.Volume = (double)Math.Min(1.0, vol);
					fnDebugWrite("Volume: " + mediaElement2.Volume * 100);
				}
			}
			catch
			{
			}
        }

        private void btnMVolDn_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				if (bMusicPlaying)
				{
					double vol = mediaElement.Volume - 0.1;
					mediaElement.Volume = (double)Math.Max(0.0, vol);
					fnDebugWrite("Volume: " + mediaElement.Volume * 100);
				}
				if (bRadioPlaying)
				{
					double vol = mediaElement2.Volume - 0.1;
					mediaElement2.Volume = (double)Math.Max(0.0, vol);
					fnDebugWrite("Volume: " + mediaElement2.Volume * 100);
				}
			}
			catch
			{
			}
        }

        private void btnMOpen_Click(object sender, RoutedEventArgs e)
        {
			//System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
			try
			{
				System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
				folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyMusic;
				folderBrowserDialog.ShowNewFolderButton = false;
				folderBrowserDialog.Description = "Please select folder.";
				
				DialogResult dr = folderBrowserDialog.ShowDialog();
				if (dr == System.Windows.Forms.DialogResult.OK)
				{
					string foldername = folderBrowserDialog.SelectedPath;
					strMusicDir = foldername;
					fnMusicStop();
					fnGetMusic(true);
					fnMusicPlay();
				}
			}
			catch (Exception ex)
			{
				fnDebugWrite("Error in btnMOpen_Click: " + ex.Message);
			}
        }

        #endregion

        #region * SETTINGS PANE *
		/**
		 * Apply values set in the variables to GUI controls.
		 **/
		private void fnApplyAppSettings()
		{
			try
			{
				chkTouchKbd.IsChecked = bEnableTouchKeyboard;

				//home
				fnSetTextBoxValue(tbDefaultPictureDir, strMyPicturesDir);
				fnSetTextBoxValue(tbSlideShowDelay,iSlideShowInterval.ToString());

				//calendar
				tabCalendar.Visibility = (cbCalendarEnabled.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
				boxSettingsCalendar.IsEnabled = (bool)cbCalendarEnabled.IsChecked;

				//cameras
				tabCameras.Visibility = (cbCamerasEnabled.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
				boxSettingsCameras.IsEnabled = (bool)cbCamerasEnabled.IsChecked;
				fnSetButtonCaption(btnWebCam1, strWebCam1Caption);
				fnSetButtonCaption(btnWebCam2, strWebCam2Caption);
				fnSetButtonCaption(btnWebCam3, strWebCam3Caption);
				fnSetButtonCaption(btnWebCam4, strWebCam4Caption);

				fnSetTextBoxValue(tbWebCamBtn1Caption, strWebCam1Caption);
				fnSetTextBoxValue(tbWebCamBtn1URL, strWebCam1URL);
				fnSetTextBoxValue(tbWebCamBtn2Caption, strWebCam2Caption);
				fnSetTextBoxValue(tbWebCamBtn2URL, strWebCam2URL);
				fnSetTextBoxValue(tbWebCamBtn3Caption, strWebCam3Caption);
				fnSetTextBoxValue(tbWebCamBtn3URL, strWebCam3URL);
				fnSetTextBoxValue(tbWebCamBtn4Caption, strWebCam4Caption);
				fnSetTextBoxValue(tbWebCamBtn4URL, strWebCam4URL);
				
				//music
				tabMusic.Visibility = (cbMusicEnabled.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
				boxSettingsMusic.IsEnabled = (bool)cbMusicEnabled.IsChecked;
				fnSetTextBoxValue(tbDefaultMusicDir, strMusicDir);

				fnSetButtonCaption(btnMRadio1, strRadio1Caption);
				fnSetButtonCaption(btnMRadio2, strRadio2Caption);
				fnSetButtonCaption(btnMRadio3, strRadio3Caption);
				fnSetButtonCaption(btnMRadio4, strRadio4Caption);

				fnSetTextBoxValue(tbRadioBtn1Caption, strRadio1Caption);
				fnSetTextBoxValue(tbRadioBtn1URL, strRadio1URL);
				fnSetTextBoxValue(tbRadioBtn2Caption, strRadio2Caption);
				fnSetTextBoxValue(tbRadioBtn2URL, strRadio2URL);
				fnSetTextBoxValue(tbRadioBtn3Caption, strRadio3Caption);
				fnSetTextBoxValue(tbRadioBtn3URL, strRadio3URL);
				fnSetTextBoxValue(tbRadioBtn4Caption, strRadio4Caption);
				fnSetTextBoxValue(tbRadioBtn4URL, strRadio4URL);
				
				//lights
				tabLights.Visibility = (cbLightsEnabled.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
				boxSettingsLights.IsEnabled = (bool)cbLightsEnabled.IsChecked;
				fnSetTextBoxValue(tbAutomationURL, strAutomationURL);

				//web
				tabBrowser.Visibility = (cbBrowserEnabled.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
				boxSettingsBrowser.IsEnabled = (bool)cbBrowserEnabled.IsChecked;
				fnSetButtonCaption(btnGoWeb1, strWebSite1Caption);
				fnSetButtonCaption(btnGoWeb2, strWebSite2Caption);
				fnSetButtonCaption(btnGoWeb3, strWebSite3Caption);

				fnSetTextBoxValue(tbInternetBtn1Caption, strWebSite1Caption);
				fnSetTextBoxValue(tbInternetBtn1URL, strWebSite1URL);
				fnSetTextBoxValue(tbInternetBtn2Caption, strWebSite2Caption);
				fnSetTextBoxValue(tbInternetBtn2URL, strWebSite2URL);
				fnSetTextBoxValue(tbInternetBtn3Caption, strWebSite3Caption);
				fnSetTextBoxValue(tbInternetBtn3URL, strWebSite3URL);

				//weather
				tabWeather.Visibility = (cbWeatherEnabled.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
				boxSettingsWeather.IsEnabled = (bool)cbWeatherEnabled.IsChecked;

			}
			catch
			{
			}
		}

		private bool fnSetTextBoxValue(System.Windows.Controls.TextBox tb, string value)
		{
			if (value != "")
			{
				tb.Text = value.Trim();
				return true;
			}
			else
			{
				tb.Text = "";
				return false;
			}
		}

		private bool fnSetButtonCaption(System.Windows.Controls.Button btn, string caption) {
			try
			{
				if (caption != "")
				{
					btn.Content = caption.Trim();
					return true;
				}
				else
				{
					btn.Visibility = Visibility.Collapsed;
					return false;
				}
			}
			catch
			{
			}
			return false;
		}

		private void fnGetAppSettings()
		{
			//Read user settings from XML file
			try
			{
				XmlSerializer xs = new XmlSerializer(typeof(KitchenTouchConfig));
				KitchenTouchConfig cfg;
				using (Stream s = File.OpenRead(strConfigFile))
					cfg = (KitchenTouchConfig)xs.Deserialize(s);
				
				//TODO: Validate ALL cfg values before assigning them!
				KTValidator v = new KTValidator();
				
				//gerenic
				bEnableTouchKeyboard = v.IsBool(cfg.bCfgEnableTouchKeyboard) ? cfg.bCfgEnableTouchKeyboard : false;
				//pictures
				strMyPicturesDir = v.IsLocalPath(cfg.sCfgPicturesDir) ? cfg.sCfgPicturesDir : "";
				iSlideShowInterval = v.IsIntRange(cfg.iCfgSlideShowInterval, 3, 60) ? cfg.iCfgSlideShowInterval : 3;
				//calendar
				cbCalendarEnabled.IsChecked = v.IsBool(cfg.bCfgCalendarEnabled) ? cfg.bCfgCalendarEnabled : false;
				//music
				cbMusicEnabled.IsChecked = v.IsBool(cfg.bCfgMusicEnabled) ? cfg.bCfgMusicEnabled : false;
				strMusicDir = v.IsLocalPath(cfg.sCfgMusicDir) ? cfg.sCfgMusicDir : "";
				strRadio1Caption = cfg.sCfgRadio1Caption;
				strRadio1URL = v.IsHttpUrl(cfg.sCfgRadio1URL) ? cfg.sCfgRadio1URL : "";
				strRadio2Caption = cfg.sCfgRadio2Caption;
				strRadio2URL = v.IsHttpUrl(cfg.sCfgRadio2URL) ? cfg.sCfgRadio2URL : "";
				strRadio3Caption = cfg.sCfgRadio3Caption;
				strRadio3URL = v.IsHttpUrl(cfg.sCfgRadio3URL) ? cfg.sCfgRadio3URL : "";
				strRadio4Caption = cfg.sCfgRadio4Caption;
				strRadio4URL = v.IsHttpUrl(cfg.sCfgRadio4URL) ? cfg.sCfgRadio4URL : "";
				//weather
				cbWeatherEnabled.IsChecked = v.IsBool(cfg.bCfgWeatherEnabled) ? cfg.bCfgWeatherEnabled : false;
				strZipCode = v.IsZIP(cfg.sCfgZipCode) ? cfg.sCfgZipCode : "";
				//cameras
				cbCamerasEnabled.IsChecked = v.IsBool(cfg.bCfgWebCamEnabled) ? cfg.bCfgWebCamEnabled : false;
				iWebCamInterval = v.IsIntRange(cfg.iCfgWebCamInterval, 3, 60) ? cfg.iCfgWebCamInterval : 10;
				strWebCam1Caption = cfg.sCfgWebCam1Caption;
				strWebCam1URL = v.IsHttpUrl(cfg.sCfgWebCam1URL) ? cfg.sCfgWebCam1URL : "";
				strWebCam2Caption = cfg.sCfgWebCam2Caption;
				strWebCam2URL = v.IsHttpUrl(cfg.sCfgWebCam2URL) ? cfg.sCfgWebCam2URL : "";
				strWebCam3Caption = cfg.sCfgWebCam3Caption;
				strWebCam3URL = v.IsHttpUrl(cfg.sCfgWebCam3URL) ? cfg.sCfgWebCam3URL : "";
				strWebCam4Caption = cfg.sCfgWebCam4Caption;
				strWebCam4URL = v.IsHttpUrl(cfg.sCfgWebCam4URL) ? cfg.sCfgWebCam4URL : "";
				//lights
				cbLightsEnabled.IsChecked = v.IsBool(cfg.bCfgAutomationEnabled) ? cfg.bCfgAutomationEnabled : false;
				strAutomationURL = v.IsHttpUrl(cfg.sCfgAutomationURL) ? cfg.sCfgAutomationURL : "about:blank";
				//web
				cbBrowserEnabled.IsChecked = v.IsBool(cfg.bCfgBrowserEnabled) ? cfg.bCfgBrowserEnabled : false;
				strWebSite1Caption = cfg.sCfgWebSite1Caption;
				strWebSite1URL = v.IsHttpUrl(cfg.sCfgWebSite1URL) ? cfg.sCfgWebSite1URL : "";
				strWebSite2Caption = cfg.sCfgWebSite2Caption;
				strWebSite2URL = v.IsHttpUrl(cfg.sCfgWebSite2URL) ? cfg.sCfgWebSite2URL : "";
				strWebSite3Caption = cfg.sCfgWebSite3Caption;
				strWebSite3URL = v.IsHttpUrl(cfg.sCfgWebSite3URL) ? cfg.sCfgWebSite3URL : "";
				//apply values to GUI
				fnApplyAppSettings();

				fnDebugWrite("fnGetAppSettings: Settings were retreived.");
			}
			catch (FileNotFoundException)
			{
				//set defaults if no config file found

				//gerenic
				bEnableTouchKeyboard = false;
				//pictures
				strMyPicturesDir = strSysPicturesDir;
				iSlideShowInterval = 3;
				//music
				strMusicDir = strSysMusicDir;
				strRadio1Caption = "Bombay Beats FM";
				strRadio1URL = "http://bb.1.fm/bbfm32k";
				strRadio2Caption = "DeSi RaDiO";
				strRadio2URL = "http://www.desi-radio.com/servers/desiradio.m3u";
				strRadio3Caption = "Bollywood";
				strRadio3URL = "http://stream.bollywoodmusicradio.com/";
				strRadio4Caption = "NovoeRadio";
				strRadio4URL = "http://s5.viastreaming.net/cgi-bin/scproxy.cgi?port=7020";
				//weather
				strZipCode = "94568";
				//cameras
				iWebCamInterval = 10;
				strWebCam1Caption = "Bay Bridge";
				strWebCam1URL = "http://static.cbslocal.com/cbs/kpix/webcams/BayBridgeCam.jpg";
				strWebCam2Caption = "";
				strWebCam2URL = "";
				strWebCam3Caption = "";
				strWebCam3URL = "";
				strWebCam4Caption = "";
				strWebCam4URL = "";
				//lights
				strAutomationURL = "about:blank";
				//web
				strWebSite1Caption = "Odnoklassniki";
				strWebSite1URL = "http://m.odnoklassniki.ru";
				strWebSite2Caption = "CNN";
				strWebSite2URL = "http://cnn.com";
				strWebSite3Caption = "Yahoo! mobile";
				strWebSite3URL = "http://m.yahoo.com";

				//apply values to GUI
				fnApplyAppSettings();
				//navigate to Setting pane to let user customize the app
				tabiSettings.IsSelected = true;
				gbWelcomeMessage.Visibility = Visibility.Visible;

				fnDebugWrite("Config file not found! Loaded default settings.");
			}
			catch (Exception ex)
			{
				fnDebugWrite("Error in fnGetAppSettings: " + ex.Message);
			}
		}

		/**
		 * Apply values set in the Settings pane to local variables and save them to XML file
		 */
		private void fnSaveAppSettings()
		{
			//TODO: Write current settings to file (or registry)

			XmlSerializer xs = new XmlSerializer(typeof(KitchenTouchConfig));
			try
			{
				KitchenTouchConfig cfg = new KitchenTouchConfig();
				
				//generic
				cfg.dblCfgVersion = dblVersion;
				cfg.bCfgEnableTouchKeyboard = bEnableTouchKeyboard = (bool)chkTouchKbd.IsChecked ? true : false;
				cfg.dCfgDateSaved = DateTime.Now;

				//home/pictures
				cfg.sCfgPicturesDir = strMyPicturesDir = tbDefaultPictureDir.Text;
				cfg.iCfgSlideShowInterval = iSlideShowInterval = Convert.ToInt32(tbSlideShowDelay.Text);

				//calendar
				cfg.bCfgCalendarEnabled = (bool)cbCalendarEnabled.IsChecked;

				//music
				cfg.bCfgMusicEnabled = (bool)cbMusicEnabled.IsChecked;
				cfg.sCfgMusicDir = strMusicDir = tbDefaultMusicDir.Text;
				
				cfg.sCfgRadio1Caption = strRadio1Caption = tbRadioBtn1Caption.Text;
				cfg.sCfgRadio1URL = strRadio1URL = tbRadioBtn1URL.Text;

				cfg.sCfgRadio2Caption = strRadio2Caption = tbRadioBtn2Caption.Text;
				cfg.sCfgRadio2URL = strRadio2URL = tbRadioBtn2URL.Text;

				cfg.sCfgRadio3Caption = strRadio3Caption = tbRadioBtn3Caption.Text;
				cfg.sCfgRadio3URL = strRadio3URL = tbRadioBtn3URL.Text;

				cfg.sCfgRadio4Caption = strRadio4Caption = tbRadioBtn4Caption.Text;
				cfg.sCfgRadio4URL = strRadio4URL = tbRadioBtn4URL.Text;

				//weather
				cfg.bCfgWeatherEnabled = (bool)cbWeatherEnabled.IsChecked;
				cfg.sCfgZipCode = strZipCode = tbZip.Text;
				
				//webcams
				cfg.bCfgWebCamEnabled = (bool)cbCamerasEnabled.IsChecked;
				cfg.iCfgWebCamInterval = iWebCamInterval = 10;

				cfg.sCfgWebCam1Caption = strWebCam1Caption = tbWebCamBtn1Caption.Text;
				cfg.sCfgWebCam1URL = strWebCam1URL = tbWebCamBtn1URL.Text;

				cfg.sCfgWebCam2Caption = strWebCam2Caption = tbWebCamBtn2Caption.Text;
				cfg.sCfgWebCam2URL = strWebCam2URL = tbWebCamBtn2URL.Text;

				cfg.sCfgWebCam3Caption = strWebCam3Caption = tbWebCamBtn3Caption.Text;
				cfg.sCfgWebCam3URL = strWebCam3URL = tbWebCamBtn3URL.Text;

				cfg.sCfgWebCam4Caption = strWebCam4Caption = tbWebCamBtn4Caption.Text;
				cfg.sCfgWebCam4URL = strWebCam4URL = tbWebCamBtn4URL.Text;
				
				//lights
				cfg.bCfgAutomationEnabled = (bool)cbLightsEnabled.IsChecked;
				cfg.sCfgAutomationURL = strAutomationURL = tbAutomationURL.Text;
				
				//internet
				cfg.bCfgBrowserEnabled = (bool)cbBrowserEnabled.IsChecked;
				cfg.sCfgWebSite1Caption = strWebSite1Caption = tbInternetBtn1Caption.Text;
				cfg.sCfgWebSite1URL = strWebSite1URL = tbInternetBtn1URL.Text;

				cfg.sCfgWebSite2Caption = strWebSite2Caption = tbInternetBtn2Caption.Text;
				cfg.sCfgWebSite2URL = strWebSite2URL = tbInternetBtn2URL.Text;

				cfg.sCfgWebSite3Caption = strWebSite3Caption = tbInternetBtn3Caption.Text;
				cfg.sCfgWebSite3URL = strWebSite3URL = tbInternetBtn3URL.Text;

				fnApplyAppSettings();

				using (Stream s = File.Create(strConfigFile))
					xs.Serialize(s, cfg);

				fnDebugWrite("fnSaveAppSettings: Settings were saved.");
				gbWelcomeMessage.Visibility = Visibility.Collapsed;
				System.Windows.MessageBox.Show("Your Settings were successfully saved.", "KitchenTouch");
			}
			catch (Exception ex)
			{
				fnDebugWrite("Error in fnSaveAppSettings: " + ex.Message);
				System.Windows.MessageBox.Show("Your Settings could not be saved.\n\nError:" + ex.Message, "KitchenTouch", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
			}
			
		}

		private void SaveConfig_Click(object sender, RoutedEventArgs e)
		{
			fnSaveAppSettings();
		}

		private void tabpSettings_Loaded(object sender, RoutedEventArgs e)
        {
			try
			{
				svSettings.Width = tabpSettings.ActualWidth;
			}
			catch
			{
			}
        }

        private void tabpSettings_Unloaded(object sender, RoutedEventArgs e)
        {
			//TODO: read bool whether any settings were changed and prompt to save.

        }
		
		private void tabpSettings_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			try
			{
				svSettings.Width = tabpSettings.ActualWidth;
			}
			catch
			{
			}
		}
        
		private void btnSlideShowDelayLess_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				int val = Convert.ToInt16(tbSlideShowDelay.Text);
				tbSlideShowDelay.Text = Math.Max(0, --val).ToString();
			}
			catch
			{
			}
        }

        private void btnSlideShowDelayMore_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				int val = Convert.ToInt16(tbSlideShowDelay.Text);
				tbSlideShowDelay.Text = Math.Min(60, ++val).ToString();
			}
			catch
			{
			}
        }
		
		private void chkTouchKbd_Set(object sender, RoutedEventArgs e)
		{
			bEnableTouchKeyboard = (bool)chkTouchKbd.IsChecked ? true : false;
		}
		
		private void fnSetWindowSize(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button btn = (System.Windows.Controls.Button)e.OriginalSource;
			double W, H;

			try
			{
				switch (btn.Name)
				{
					case "btnSetFullscreen":
						this.WindowState = WindowState.Maximized;
						return;
					case "btnSet1280x1024":
						W = 1280; H = 1024;
						break;
					case "btnSet1024x768":
						W = 1024; H = 768;
						break;
					case "btnSet800x600":
						W = 800; H = 600;
						break;
					default:
						W = 1024; H = 768;
						break;
				}
				
				this.WindowState = WindowState.Normal;
				this.Left = 0;
				this.Top = 0;
				this.Width = W;
				this.Height = H;
			}
			catch
			{
			}
		}

		private void cbEnableDebug_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.CheckBox cb = (System.Windows.Controls.CheckBox)e.OriginalSource;
			string msg = (bool)cb.IsChecked ? "Debug messages enabled" : "Debug messages disabled";
			fnDebugWrite(msg);
			bEnableDebug = (bool)cb.IsChecked;
		}
		
		private void cbCalendarEnabled_Click(object sender, RoutedEventArgs e)
		{
			boxSettingsCalendar.IsEnabled = (bool)cbCalendarEnabled.IsChecked;
		}

		private void cbWeatherEnabled_Click(object sender, RoutedEventArgs e)
		{
			boxSettingsWeather.IsEnabled = (bool)cbWeatherEnabled.IsChecked;
		}

		private void cbCamerasEnabled_Click(object sender, RoutedEventArgs e)
		{
			boxSettingsCameras.IsEnabled = (bool)cbCamerasEnabled.IsChecked;
		}

		private void cbLightsEnabled_Click(object sender, RoutedEventArgs e)
		{
			boxSettingsLights.IsEnabled = (bool)cbLightsEnabled.IsChecked;
		}

		private void cbBrowserEnabled_Click(object sender, RoutedEventArgs e)
		{
			boxSettingsBrowser.IsEnabled = (bool)cbBrowserEnabled.IsChecked;
		}

		private void cbMusicEnabled_Click(object sender, RoutedEventArgs e)
		{
			boxSettingsMusic.IsEnabled = (bool)cbMusicEnabled.IsChecked;
		}
		
		#endregion

		#region * KEYBOARD HANDLER *
		private void fnTextEntryHandler(object sender, RoutedEventArgs e)
		{
			try
			{
				if (bEnableTouchKeyboard)
				{
					System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)e.OriginalSource;
					
					if (tb == tbCurrent) return; //temp fix for "Enter key press on the keyboard shows keyboard again if text has changed"
					
					//Ignore TextBox in the virtual keyboard
					string tbN = tb.Name.ToString();
					if (tbN == "lblTextEntryField" ||
						tbN == "txtUrl") return;

					tbCurrent = tb;
					if (tbCurrent.IsFocused)
					{
						fnDebugWrite("Showing keyboard for [" + tb.Name + "] text box.");
						TouchKeyboard cTouchKeyboard = new TouchKeyboard(tbCurrent.Text);
						bool? dialogResult = cTouchKeyboard.ShowDialog();
						switch (dialogResult)
						{
							case true:
								// User closed keyboard by pressing Enter button
								tbCurrent.Text = cTouchKeyboard.ResultText;
								break;
							case false:
								// User closed keyboard by pressing Cancel button
								// do nothing
								break;
							default:
								// Indeterminate
								break;
						}
					}
				}
			}
			catch
			{
			}
		}
		#endregion

		#region * FOLDER SELECTION DIALOG *
		private void fnSelectFolder(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button btn = (System.Windows.Controls.Button)e.OriginalSource;
			System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

			switch (btn.Name)
			{
				case "btnPictureDir":
					folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyPictures;
					break;
				case "btnMusicDir":
					folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyMusic;
					break;
				default:
					folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
					break;
			}
			
			folderBrowserDialog.ShowNewFolderButton = false;
			folderBrowserDialog.Description = "Please select the folder.";
				
			DialogResult dr = folderBrowserDialog.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				string foldername = folderBrowserDialog.SelectedPath;
				switch (btn.Name)
				{
					case "btnPictureDir":
						tbDefaultPictureDir.Text = strMyPicturesDir = foldername;
						break;
					case "btnMusicDir":
						tbDefaultMusicDir.Text = strMusicDir = foldername;
						break;
					default:
						break;
				}
			}
		}
		#endregion;

	}
}
