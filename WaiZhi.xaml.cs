using Panuon.UI.Silver;
using System.Windows;
using System.Windows.Controls;
using static MCL_Dev.LauncherClasses;
using MinecaftOAuth;
using System.Linq;
using System.IO;

namespace MCL_Dev
{
    /// <summary>
    /// WaiZhi.xaml 的交互逻辑
    /// </summary>
    public partial class WaiZhi : Page
    {
        public WaiZhi()
        {
            InitializeComponent();
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if (email.Text!=null&& passwd.Password != null)
            {
                waizhi_email = email.Text;
                waizhi_password = passwd.Password;
                MinecaftOAuth.YggdrasilAuthenticator auth = new(true,waizhi_email,waizhi_password);
                var result = await auth.AuthAsync(x => { });
                int a = result.Count();
                for(int i = 0; i < a; i++)
                {
                    players.Items.Add(result[i]);
                }
                #region 保存RefreshToken
                DirectoryInfo dirInfo = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "MCL\\waizhi");//查看Debug文件夹的信息
                bool file = dirInfo.Exists;
                if (file == true)//有文件，直接写入
                {
                    for(int i = 0; i < a; i++)//依次写入各个角色的AccessToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt").Close();
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt", result[i].AccessToken);
                    }
                    for (int i = 0; i < a; i++)//依次写入各个角色的ClientToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt").Close();
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt", result[i].ClientToken);
                    }
                }
                else//无文件，创建文件夹后写入
                {
                    dirInfo.Create();
                    for (int i = 0; i < a; i++)//依次写入各个角色的AccessToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt").Close();
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt", result[i].AccessToken);                    
                    }
                    for (int i = 0; i < a; i++)//依次写入各个角色的ClientToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt").Close() ;
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt", result[i].ClientToken);                       
                    }
                }
                #endregion
                MessageBoxX.Show("已完成登录", "MCL启动器");
            }
            else
            {
                MessageBoxX.Show("信息未填写完整！","MCL启动器");
            };
        }

        private void players_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            waizhi_selectedplayer = players.SelectedIndex;
            waizhi_selectedUsr = players.Text;
        }
    }
}
