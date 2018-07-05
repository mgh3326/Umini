using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Umini
{
    /// <summary>
    /// Interaction logic for AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        MainWindow mainWindow;
        ImportExport importExport = new ImportExport();
        public AccountWindow()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;

            textboxId.Text = mainWindow.mAccount.mID;
            LoadAccount();

        }


        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mAccount.mID = textboxId.Text;
            ImportExport importExport = new ImportExport();
            importExport.exportAccount(mainWindow.mAccount);
            MessageBox.Show("저장 완료!");
        }


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

                        listviewIDs.Items.Add(FileNameOnly);

                    }
                }

            }
            else//아무것도 없을때
            {

            }

        }

        private void listviewIDs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ImportExport importExport = new ImportExport();

            if (listviewIDs.SelectedItems.Count == 1)
            {
                var item = (sender as ListView).SelectedItem;

                mainWindow.mAccount = importExport.importAccount(item.ToString());
                mainWindow.UpdatePlayer(mainWindow.mAccount.mNowPlayingList);
                mainWindow.UpdateProfile();
                

            }
            ////((ListView)sender).Items.s
            //ListViewItem item = sender as ListViewItem;
            //item.Selected()
            //if (item.IsSelected == true)
            //{

            //}

            //MessageBox.Show(item.ToString());
        }


    }
}
