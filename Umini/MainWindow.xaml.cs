﻿using System;
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
using Umini.Test.junpil;
using Umini.ViewModel;
using Umini.Test;
using Player;
using Umini.Test.mgh3326;
using System.Web;
using Umini.Test.hhhh24;
using System.Threading;
using System.Windows.Threading;
using VideoLibrary;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Umini
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public Test_play play;
        public Account mAccount;
        DispatcherTimer mtimer = new DispatcherTimer();

        public MainWindow()
        {


            InitializeComponent();
            DataContext = new WindowViewModel(this);

            mtimer.Interval = TimeSpan.FromMilliseconds(1000);
            mtimer.Tick += new EventHandler(Update_timeslider);
            mtimer.Start();

            play = new Test_play();

            play.video.MediaEnded += new RoutedEventHandler(MediaEnded);
            mAccount = new Account();

            mAccount.mPlaylistList.Add(new Playlist() { mName = "TestPlaylist1" });
            mAccount.mPlaylistList.Add(new Playlist() { mName = "TestPlaylist2" });
            mAccount.mPlaylistList.Add(new Playlist() { mName = "TestPlaylist3" });
            mAccount.mPlaylistList.Add(new Playlist() { mName = "TestPlaylist4" });

            foreach (Playlist playlist in mAccount.mPlaylistList)
            {
                tviPlaylist.Items.Add(new TreeViewItem() { Header = playlist.mName });
            }

            LoadAccount();
        }

        void Update_timeslider(object sender, EventArgs e)
        {
            NowPlayingList npl = mAccount.mNowPlayingList;
            Slider_time.Value = play.video.Position.TotalSeconds;
            Slider_time.Maximum = play.total == 0 ? 1 : play.total;
            //time.Text = new TimeSpan(0, 0, (Convert.ToInt32(Slider_time.Value))).ToString() + "/" + new TimeSpan(0, 0, Convert.ToInt32(t.TotalSeconds)); ;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = new TestWindow();
            window.Show();
        }



        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            NowPlayingList npl = mAccount.mNowPlayingList;
            if (npl.mMediaList[npl.mNowPlayingOrder].mPath != null)
            {
                UpdatePlayer(npl);
                //txtLyric.Text = media.mLyric;
                Play();

            }
        }

        public void UpdatePlayer(NowPlayingList npl)
        {
            if (npl.mMediaList.Count == 0)
            {
                album.Source = null;
                lbTitleBar.Content = "";
                return;
            }
            BitmapImage bi = new BitmapImage(new Uri(npl.mMediaList[npl.mNowPlayingOrder].mImagePath, UriKind.RelativeOrAbsolute));
            album.Source = bi;
            MediaFile media = npl.mMediaList[npl.mNowPlayingOrder];
            lbTitleBar.Content = media.mTitle + " - " + media.mArtist;
        }



        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }


        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            NextPlay();
        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            PrevPlay();
        }
        private void AppWindow_Loaded(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new NowPlayingPage());
        }


        public void Play()
        {
            NowPlayingList npl = mAccount.mNowPlayingList;
            if (mAccount.mNowPlayingList.mMediaList.Count == 0)
                return;
            play.Music_Open(npl.mMediaList[npl.mNowPlayingOrder].mPath);
            play.video.Position = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(npl.mNowMediaPosition));
            Slider_time.Minimum = 0;
            Slider_time.IsMoveToPointEnabled = true;
            npl.mIsPlay = true;
        }

        public void Pause()
        {
            NowPlayingList npl = mAccount.mNowPlayingList;

            npl.mNowMediaPosition = play.CurPosition();
            npl.mIsPlay = false;
            play.VideoPause();
        }
        public void Stop()
        {
            NowPlayingList npl = mAccount.mNowPlayingList;
            play.video.Stop();
            npl.mIsPlay = false;
        }

        public void NextPlay()
        {
            NowPlayingList npl = mAccount.mNowPlayingList;

            if (++npl.mNowPlayingOrder == npl.mMediaList.Count)
                npl.mNowPlayingOrder = 0;
            if (npl.mMediaList[npl.mNowPlayingOrder].mPath != null)
            {
                npl.mNowMediaPosition = 0;
                play.Music_Open(npl.mMediaList[npl.mNowPlayingOrder].mPath);

            }
            UpdatePlayer(mAccount.mNowPlayingList);

        }

        public void PrevPlay()
        {
            NowPlayingList npl = mAccount.mNowPlayingList;

            if (--npl.mNowPlayingOrder < 0)
                npl.mNowPlayingOrder = npl.mMediaList.Count - 1;
            if (npl.mMediaList[npl.mNowPlayingOrder].mPath != null)
            {
                npl.mNowMediaPosition = 0;
                play.Music_Open(npl.mMediaList[npl.mNowPlayingOrder].mPath);
            }
            UpdatePlayer(mAccount.mNowPlayingList);

        }
        public void MediaEnded(object sender, RoutedEventArgs e)
        {
            NextPlay();

        }

        /// <summary>
        /// load account profile in ./profile/[account-name].json
        /// return no;
        /// </summary>
        public void LoadAccount()//이거 파라미터가 들어온다면 다르게 폴더 이름이 달리질듯 하오
        {
            String path;
            ImportExport importExport = new ImportExport();
            path = importExport.makeFolder("profile");
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            if (di.GetFiles().Length > 0)
            {
                foreach (System.IO.FileInfo File in di.GetFiles())
                {

                    if (File.Extension.ToLower().CompareTo(".json") == 0)
                    {
                        String FileNameOnly = File.Name.Substring(0, File.Name.Length - 5);
                        String FullFileName = File.FullName;
                        if (FileNameOnly.Equals(mAccount.mID))
                        {
                            mAccount = importExport.importAccount(mAccount.mID);
                            return;
                        }
                    }
                }
                //ㅠㅠ 디포트가 없으면 여길로 오겠구먼
                if (mAccount.mID == null)
                {
                    mAccount.mID = "default";
                    mAccount.mNowPlayingList = new NowPlayingList();
                }
                importExport.exportAccount(mAccount);
            }
            else//아무것도 없을때
            {
                //MessageBox.Show("default.json 파일이 없습니다. 그래서 json 파일을 만들겠습니다.");
                mAccount.mID = "default";
                mAccount.mNowPlayingList = new NowPlayingList();
                importExport.exportAccount(mAccount);
            }


        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            new AccountWindow().Show();

        }

        private void trvPlaylistItem_Selected(object sender, RoutedEventArgs e)
        {
            tviNowPlaying.IsSelected = false;
            tviSearch.IsSelected = false;

            TreeViewItem tvi = e.OriginalSource as TreeViewItem;

            string menu = tvi.Header.ToString();
            if (menu == "PlayList")
            {
                frame.NavigationService.Navigate(new ManagePlaylistPage());
                return;
            }

            Playlist playlist = FindPlaylist(menu);
            frame.NavigationService.Navigate(new PlaylistPage(playlist));

        }

        private void trvSideMenu_Selected(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem item in tviPlaylist.Items)
            {
                item.IsSelected = false;
            }
            tviNowPlaying.IsSelected = false;
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;

            string menu = tvi.Header.ToString();

            switch (menu)
            {
                case "Search":
                    frame.NavigationService.Navigate(new YoutubeSearchPage());
                    break;
            }
        }

        private void trvNowPlaying_Selected(object sender, RoutedEventArgs e)
        {
            tviSearch.IsSelected = false;
            foreach (TreeViewItem item in tviPlaylist.Items)
            {
                item.IsSelected = false;
            }

            frame.NavigationService.Navigate(new NowPlayingPage());
        }


        public Playlist FindPlaylist(string name)
        {
            Playlist playlist = mAccount.mPlaylistList.FirstOrDefault(x => x.mName.Equals(name));
            return playlist;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (mAccount.mID == null)
            {
                mAccount.mID = "default";
                mAccount.mNowPlayingList = new NowPlayingList();
            }
            ImportExport importExport = new ImportExport();

            importExport.exportAccount(mAccount);
        }

        public void UpdateProfile()
        {
            btnProfile.Content = mAccount.mID;
        }

        private void AppWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Process.Start("cmd", "taskkill /f /im umini.exe");
        }




        private void Slider_time_LostMouseCapture(object sender, MouseEventArgs e)
        {
            int pos = Convert.ToInt32(Slider_time.Value);
            play.video.Position = new TimeSpan(0, 0, 0, pos);
        }

        private void Slider_time_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int pos = Convert.ToInt32(Slider_time.Value);
                play.video.Position = new TimeSpan(0, 0, 0, pos);
            }
        }

        private void Slider_volume_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                play.video.Volume = Slider_volume.Value;
            }
        }

        private void Slider_volume_LostMouseCapture(object sender, MouseEventArgs e)
        {
            play.video.Volume = Slider_volume.Value;
        }

        private void btnDetailMedia_Click(object sender, RoutedEventArgs e)
        {
            int num = mAccount.mNowPlayingList.mNowPlayingOrder;
            MediaFile media = mAccount.mNowPlayingList.mMediaList[num];
            DetailMediaWindow dmw = new DetailMediaWindow(media);
            dmw.Show();
        }
    }
}


