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
        public string UUID;
        public string AccessToken;
        public string RefreshToken;
        public double ExpireIn_ms;
        public string name;
        public string code;
        public WeiRuan()
        {
            InitializeComponent();
            nameBox.Text = "玩家名字：等待验证中……";
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            var authenticator = new MicrosoftAuthenticator();
            authenticator.Code = codeInput.Text;
            code = codeInput.Text;
            nameBox.Text = "玩家名字：获取数据中……";
            start.Visibility = System.Windows.Visibility.Hidden;
            var result = await authenticator.AuthenticateAsync();
            name = result.Name;
            RefreshToken = result.RefreshToken;
            nameBox.Text = "玩家名字：" + name;
            RegistryKey key = Registry.LocalMachine;
            key.CreateSubKey("software\\ModernCraftLauncher");
            RegistryKey MCL = key.OpenSubKey("software\\ModernCraftLauncher", true);
            MCL.SetValue("RefreshToken", RefreshToken);
            MCL.SetValue("firstTime","No");
            MCL.Close();
        }
    }
}
