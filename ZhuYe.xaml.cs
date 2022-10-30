using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using static MCL_Dev.LauncherClasses;
using static MCL_Dev.WeiRuan;

namespace MCL_Dev
{

    /// <summary>
    /// ZhuYe.xaml 的交互逻辑
    /// </summary>
    public partial class ZhuYe : Page
    {
        int mode;
        
        public ZhuYe()
        {
            ServicePointManager.DefaultConnectionLimit = 512;
            InitializeComponent();
            javaCombo.ItemsSource = GetJavaPath();
        }
        

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            var minecraftResolver = new MinecraftResolver(@"C:\Users\wodes\source\repos\MCL-Dev\bin\Debug\net6.0-windows7.0\.minecraft");
            var minecraft = minecraftResolver.GetMinecraft("1.19.2");
            if (mode == 0)
            {
                var process = await minecraft
                    .WithAuthentication("akchiji888")
                    .WithJava(javaCombo.Text)
                    .LaunchAsync();
            }
            else
            {
                var authenticator = new MicrosoftAuthenticator();
                if(IsRegeditItemExist() == true)
                {
                    RegistryKey MCL = Registry.LocalMachine;
                    RegistryKey software = MCL.OpenSubKey("SOFTWARE\\ModernCraftLauncher",true);
                    string refreshToken = software.GetValue("RefreshToken").ToString();
                    var result = await authenticator.RefreshAuthenticateAsync(refreshToken);
                    software.SetValue("RefreshToken", result.RefreshToken);
                    var process = await minecraft
                        .WithAuthentication(result)
                        .WithJava(javaCombo.Text)
                        .LaunchAsync();
                }
                else
                {
                    MessageBox.Show("您尚未进行过微软登录，请进行微软登录后再启动游戏", "MCL启动器");
                }                 
            }
        }

        private void start_Copy_Click(object sender, RoutedEventArgs e)
        {
            mode = 0;
            Lixian lixian = new Lixian();
            login.Content = new Frame()
            {
                Content = lixian
            };
        }

        private void start_Copy1_Click(object sender, RoutedEventArgs e)
        {
            mode = 1;
            if (IsRegeditItemExist() == true)
            {
                MessageBox.Show("您已进行过微软登录，请直接启动Minecraft","MCL启动器");
            }
            else
            {
                var authenticator = new MicrosoftAuthenticator();
                WeiRuan weiruan = new WeiRuan();
                login.Content = new Frame()
                {
                    Content = weiruan
                };
                OuterVisit(authenticator.LoginUrl);
            } 
        }
    }
}
