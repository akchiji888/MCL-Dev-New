using Panuon.UI.Silver;
using System.Windows;
using System.Windows.Controls;
using static MCL_Dev.LauncherClasses;
using MinecaftOAuth;
using System.Linq;

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
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "MCL";
            string refreshPath = filePath + $"\\RefreshToken_{players.Text}";
            bool file = System.IO.File.Exists(refreshPath);
            
        }
    }
}
