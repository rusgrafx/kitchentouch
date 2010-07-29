using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private bool bEnableDebug = false;
		private string strSysPicturesDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		private string strSysMusicDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
		//private string strSysDocumentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		private string strConfigFile = "KitchenTouch.xml";
        private string strPlayBtnUrl = "pack://application:,,,/Resources/playerplay.png";
        private string strStopBtnUrl = "pack://application:,,,/Resources/playerstop.png";
        private string strRadioAlbumArt = "pack://application:,,,/Resources/kajol.jpg";

		#region * USER CONFIG *
		// Config values to save off to XML
		[XmlRoot("KitchenTouchConfig")]
		public class KitchenTouchConfig
		{
			#region CFG Generic
				[XmlElement("EnableTouchKeyboard")]	public bool bCfgEnableTouchKeyboard;
			#endregion

			#region CFG Home/Pictures
				[XmlElement("PicturesDir")]	public string sCfgPicturesDir;

				[XmlElement("PicturesInterval")] public int iCfgSlideShowInterval;
			#endregion
			
			#region CFG Music
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

			#region CFG Internet
				[XmlElement("WebSite1Caption")]  public string sCfgWebSite1Caption;
				[XmlElement("WebSite1URL")] public string sCfgWebSite1URL;

				[XmlElement("WebSite2Caption")] public string sCfgWebSite2Caption;
				[XmlElement("WebSite2URL")] public string sCfgWebSite2URL;

				[XmlElement("WebSite3Caption")] public string sCfgWebSite3Caption;
				[XmlElement("WebSite3URL")] public string sCfgWebSite3URL;
			#endregion

			#region CFG Weather
				[XmlElement("ZipCode")] public string sCfgZipCode;
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

		#region USR Internet
			private string strWebSite1Caption, strWebSite2Caption, strWebSite3Caption;
			private string strWebSite1URL, strWebSite2URL, strWebSite3URL;
		#endregion

		#region USR Weather
			private string strZipCode;
		#endregion

		#endregion

        // background color for Album Art
        private const double COLOR_VAR = 3;
        private double R = 180;
        private double G = 100;
        private double B = 0;

        // internet browser
        string iBrowser_CurrentURL;
		System.Windows.Controls.TextBox tbCurrent;
		
        #endregion

        #region * Init *
        protected DispatcherTimer ClockTimer;
        protected DispatcherTimer SlideShowTimer;
        protected DispatcherTimer PlayerTimer;
		protected DispatcherTimer WebCamTimer;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;

        private int iCurrentSlide = 0;
        private string[] jpgFiles = new string[0];
        private bool bStartSlideShow = false;

        private int iCurrentMP3 = 0;
        private string[] mp3Files = new string[0];
        private bool bMusicPlaying = false;

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

            //this.btnPlayerCtrl.FontSize = 38;
            //this.btnPlayerCtrl.Content = PLAY_LABEL;
            this.mediaElement.MediaEnded += new RoutedEventHandler(btnMNext_Click);

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
				this.Title = msg;
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
            
            //TODO: open current image in external viewer or in Explorer of in fullscreen
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
        }

		private void btnBStop_Click(object sender, RoutedEventArgs e)
		{
			//TODO: Implement real stop function
			fnBrowserNavigate("about:blank");
		}

		private void btnBRefresh_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				iBrowser.Refresh(true);
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

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            //go to previous page
			try
			{
				iBrowser.GoBack();
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
				btnMVolDn.Visibility = this.Width >= 1024 ? Visibility.Visible : Visibility.Collapsed;
				btnMVolUp.Visibility = this.Width >= 1024 ? Visibility.Visible : Visibility.Collapsed;
				fnGetMusic(false);
				CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
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
				int si = lbPlayLists.SelectedIndex;
				fnMusicStop();
				iCurrentMP3 = si;
				fnMusicPlay();
				fnDebugWrite("Selected index: " + lbPlayLists.SelectedIndex);
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
                    iCurrentMP3 = 0;
                    mp3Files = System.IO.Directory.GetFiles(strMusicDir, @"*.mp3");
                    this.lbPlayLists.Items.Clear();
                    foreach (string f in mp3Files)
                    {
                        this.lbPlayLists.Items.Add(f);
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
						lblMFile.Content = "File: " + mp3file;
						mediaElement.Position = new TimeSpan();
						mediaElement.Play();
						bMusicPlaying = true;
					}
					else
					{
						fnDebugWrite("Error: There are no MP3 files to play!");
					}
				}
				else
				{
					mediaElement.Play();
				}
			}
			catch (Exception e)
			{
				fnDebugWrite("Error: " + e.Message);
			}
		}

        public void fnMusicPlay()
        {
			mediaElement.Play();
			bMusicPlaying = true;
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
				mediaElement.Pause();
				bMusicPlaying = false;
			}
			catch
			{
			}
        }
        
        public void fnMusicStop()
        {
			try
			{
				mediaElement.Stop();
				bMusicPlaying = false;
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
				imgPlayerCtrl.Source = GetImage(strStopBtnUrl);
			}
			catch
			{
			}
        }

        private void fnPlayerCtrlPause()
        {
			try
			{
				imgPlayerCtrl.Source = GetImage(strPlayBtnUrl);
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

					R += (COLOR_VAR / 2 - r.NextDouble() * COLOR_VAR);
					G += (COLOR_VAR / 2 - r.NextDouble() * COLOR_VAR);
					B += (COLOR_VAR / 2 - r.NextDouble() * COLOR_VAR);

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
                mediaElement.Source = new Uri(url, UriKind.Absolute);
                mediaElement.ToolTip = lblMFile.Content = "Internet Radio: " + title;
                fnStatusWrite("Internet Radio: " + title);
                fnDebugWrite("Playing Internet stream");
                mediaElement.Position = new TimeSpan();
                this.lblMPosition.Content = "--:-- / --:--";
                this.imgAlbumCover.Source = GetImage(strRadioAlbumArt);
                //this.lbPlayLists.Items.Clear();
                //this.lbPlayLists.Items.Add(title);
                //mediaElement.NaturalDuration = 
                mediaElement.Play();
                bMusicPlaying = true;
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
                fnMusicPlay();
                fnPlayerCtrlPlay();
                //fnMusicNext();
                //mediaElement.MediaEnded += new RoutedEventHandler(btnMPlay_Click);
            }
            catch (Exception ex)
            {
                fnDebugWrite("Error in btnMPlay_Click: " + ex.Message);
            }
        }

        private void btnMStop_Click(object sender, RoutedEventArgs e)
        {
            fnMusicPause();
            fnPlayerCtrlPause();
        }

        private void btnMNext_Click(object sender, RoutedEventArgs e)
        {
            fnMusicStop();
            fnMusicNext();
            fnMusicPlay();
            fnPlayerCtrlPlay();
        }

        private void btnMPrev_Click(object sender, RoutedEventArgs e)
        {
            fnMusicStop();
            fnMusicPrev();
            fnMusicPlay();
            fnPlayerCtrlPlay();
        }

        private void btnPlayerCtrl_Click(object sender, RoutedEventArgs e)
        {
            //if ((string)btnPlayerCtrl.Content == PAUSE_LABEL)
            if (bMusicPlaying)
            {
                fnPlayerCtrlPause();
                fnMusicPause();
            }
            else
            {
                fnPlayerCtrlPlay();
                fnMusicPlay();
            }
        }

        private void btnMVolUp_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				double vol = mediaElement.Volume + 0.1;
				mediaElement.Volume = (double)Math.Min(1.0, vol);
				fnDebugWrite("Volume: " + mediaElement.Volume * 100);
			}
			catch
			{
			}
        }

        private void btnMVolDn_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				double vol = mediaElement.Volume - 0.1;
				mediaElement.Volume = (double)Math.Max(0.0, vol);
				fnDebugWrite("Volume: " + mediaElement.Volume * 100);
			}
			catch
			{
			}
        }

        private void btnMOpen_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				if (folderBrowserDialog == null)
				{
					this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
					//this.SuspendLayout();
					this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyMusic;
					this.folderBrowserDialog.ShowNewFolderButton = false;
					this.folderBrowserDialog.Description = "Please select folder.";
					//this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
					//this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
					//this.ClientSize = new System.Drawing.Size(292, 266);
					//this.ResumeLayout(false);
				}
				DialogResult dr = folderBrowserDialog.ShowDialog();
				if (dr == System.Windows.Forms.DialogResult.OK)
				{
					string foldername = this.folderBrowserDialog.SelectedPath;
					strMusicDir = foldername;
					fnGetMusic(true);
					fnMusicStop();
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

				//cameras
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

				//web
				fnSetButtonCaption(btnGoWeb1, strWebSite1Caption);
				fnSetButtonCaption(btnGoWeb2, strWebSite2Caption);
				fnSetButtonCaption(btnGoWeb3, strWebSite3Caption);

				fnSetTextBoxValue(tbInternetBtn1Caption, strWebSite1Caption);
				fnSetTextBoxValue(tbInternetBtn1URL, strWebSite1URL);
				fnSetTextBoxValue(tbInternetBtn2Caption, strWebSite2Caption);
				fnSetTextBoxValue(tbInternetBtn2URL, strWebSite2URL);
				fnSetTextBoxValue(tbInternetBtn3Caption, strWebSite3Caption);
				fnSetTextBoxValue(tbInternetBtn3URL, strWebSite3URL);
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
				
				//gerenic
				bEnableTouchKeyboard = cfg.bCfgEnableTouchKeyboard;
				//pictures
				strMyPicturesDir = cfg.sCfgPicturesDir;
				iSlideShowInterval = cfg.iCfgSlideShowInterval;
				//music
				strMusicDir = cfg.sCfgMusicDir;
				strRadio1Caption = cfg.sCfgRadio1Caption;
				strRadio1URL = cfg.sCfgRadio1URL;
				strRadio2Caption = cfg.sCfgRadio2Caption;
				strRadio2URL = cfg.sCfgRadio2URL;
				strRadio3Caption = cfg.sCfgRadio3Caption;
				strRadio3URL = cfg.sCfgRadio3URL;
				strRadio4Caption = cfg.sCfgRadio4Caption;
				strRadio4URL = cfg.sCfgRadio4URL;
				//weather
				strZipCode = cfg.sCfgZipCode;
				//cameras
				iWebCamInterval = cfg.iCfgWebCamInterval;
				strWebCam1Caption = cfg.sCfgWebCam1Caption;
				strWebCam1URL = cfg.sCfgWebCam1URL;
				strWebCam2Caption = cfg.sCfgWebCam2Caption;
				strWebCam2URL = cfg.sCfgWebCam2URL;
				strWebCam3Caption = cfg.sCfgWebCam3Caption;
				strWebCam3URL = cfg.sCfgWebCam3URL;
				strWebCam4Caption = cfg.sCfgWebCam4Caption;
				strWebCam4URL = cfg.sCfgWebCam4URL;
				//web
				strWebSite1Caption = cfg.sCfgWebSite1Caption;
				strWebSite1URL = cfg.sCfgWebSite1URL;
				strWebSite2Caption = cfg.sCfgWebSite2Caption;
				strWebSite2URL = cfg.sCfgWebSite2URL;
				strWebSite3Caption = cfg.sCfgWebSite3Caption;
				strWebSite3URL = cfg.sCfgWebSite3URL;
				//apply values to GUI
				fnApplyAppSettings();

				fnDebugWrite("fnGetAppSettings: Settings were retreived.");
			}
			catch (FileNotFoundException)
			{
				//set global defines if no config file found

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
				strRadio3Caption = "Bollywood Music Radio";
				strRadio3URL = "http://stream.bollywoodmusicradio.com/";
				strRadio4Caption = "Radio Nimbooda";
				strRadio4URL = "http://87.98.216.140";
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
				cfg.bCfgEnableTouchKeyboard = bEnableTouchKeyboard = (bool)chkTouchKbd.IsChecked ? true : false; ;

				//home/pictures
				cfg.sCfgPicturesDir = strMyPicturesDir = tbDefaultPictureDir.Text;
				cfg.iCfgSlideShowInterval = iSlideShowInterval = Convert.ToInt32(tbSlideShowDelay.Text);

				//music
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
				cfg.sCfgZipCode = strZipCode = tbZip.Text;
				
				//webcams
				cfg.iCfgWebCamInterval = iWebCamInterval = 10;

				cfg.sCfgWebCam1Caption = strWebCam1Caption = tbWebCamBtn1Caption.Text;
				cfg.sCfgWebCam1URL = strWebCam1URL = tbWebCamBtn1URL.Text;

				cfg.sCfgWebCam2Caption = strWebCam2Caption = tbWebCamBtn2Caption.Text;
				cfg.sCfgWebCam2URL = strWebCam2URL = tbWebCamBtn2URL.Text;

				cfg.sCfgWebCam3Caption = strWebCam3Caption = tbWebCamBtn3Caption.Text;
				cfg.sCfgWebCam3URL = strWebCam3URL = tbWebCamBtn3URL.Text;

				cfg.sCfgWebCam4Caption = strWebCam4Caption = tbWebCamBtn4Caption.Text;
				cfg.sCfgWebCam4URL = strWebCam4URL = tbWebCamBtn4URL.Text;
				
				//internet
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
			}
			catch (Exception ex)
			{
				fnDebugWrite("Error in fnSaveAppSettings: " + ex.Message);
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
		#endregion

		#region * KEYBOARD HANDLER *
		private void fnTextEntryHandler(object sender, RoutedEventArgs e)
		{
			try
			{
				if (bEnableTouchKeyboard)
				{
					System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)e.OriginalSource;

					//Ignore TextBox in the virtual keyboard
					string tbN = tb.Name.ToString();
					if (tbN == "lblTextEntryField" ||
						tbN == "txtUrl") return;

					tbCurrent = tb;
					if (tbCurrent.IsFocused)
					{
						fnDebugWrite("Showing keyboard for [" + tb.Name + "] text box.");
						TouchKeyboard cTouchKeyboard = new TouchKeyboard(tbCurrent.Text);
						cTouchKeyboard.TouchKeyboard_Show();
					}
				}
			}
			catch
			{
			}
		}
		#endregion

	}
}
