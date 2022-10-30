using Microsoft.Win32;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Models.Authentication;
using System;
using System.Windows;
using System.Windows.Controls;
namespace MCL_Dev
{
    
    /// <summary>
    /// WeiRuan.xaml 的交互逻辑
    /// </summary>
    public partial class WeiRuan : Page
    {
        public string AccessToken;
        public string RefreshToken;
        public string name;
        public WeiRuan()
        {
            InitializeComponent();
            nameBox.Text = "玩家名字：等待验证中……";
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            var authenticator = new MicrosoftAuthenticator();
            string Code_URI = codeInput.Text;
            if (Code_URI.Contains("https://login.live.com/oauth20_desktop.srf?code=") != true)
            {
                MessageBox.Show("非法的URI，请重新复制","MCL启动器");
            }
            else
            {
                Code_URI = Code_URI.Replace("https://login.live.com/oauth20_desktop.srf?code=","");
                string[] strArray = Code_URI.Split('&');
                authenticator.Code = strArray[0];
                nameBox.Text = "玩家名字：获取数据中……";
                start.Visibility = System.Windows.Visibility.Hidden;
                var result = await authenticator.AuthenticateAsync();
                name = result.Name;
                AccessToken = result.AccessToken;
                RefreshToken = result.RefreshToken;
                nameBox.Text = "玩家名字：" + name;
                RegistryKey key = Registry.LocalMachine;
                key.CreateSubKey("software\\ModernCraftLauncher");
                RegistryKey MCL = key.OpenSubKey("software\\ModernCraftLauncher", true);
                MCL.SetValue("RefreshToken", RefreshToken);
                MCL.SetValue("AccessToken", AccessToken);
                MCL.SetValue("firstTime", "No");
                MCL.Close();
            }
        }
    }
}
