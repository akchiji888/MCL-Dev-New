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
        int mode = 114514;//homo特有的变量（喜）
        
        public ZhuYe()
        {
            ServicePointManager.DefaultConnectionLimit = 512;
            InitializeComponent();
            javaCombo.ItemsSource = GetJavaPath();
        }
        

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if(javaCombo.Text != null && maxMem.Text != null && mode != 114514)
            {
                string MinecraftPath_thisPath = System.AppDomain.CurrentDomain.BaseDirectory + ".minecraft";
                var minecraftResolver = new MinecraftResolver(MinecraftPath_thisPath);
                var minecraft = minecraftResolver.GetMinecraft("1.19.2");
                if (mode == 0)
                {
                    if(offlineName != null)
                    {
                        var process = await minecraft
                            .WithAuthentication(offlineName)
                            .WithJava(javaCombo.Text)
                            .WithMaxMemorySize(Convert.ToInt32(maxMem.Text))
                            .LaunchAsync();
                    }
                    else
                    {
                        MessageBox.Show("无法启动游戏！\n错误原因：参数缺失\n请检查您是否输入了用户名以及“确定”键是否按下","MCL启动器");
                    }
                }
                else
                {
                    var authenticator = new MicrosoftAuthenticator();
                    if (IsRegeditItemExist() == true)
                    {
                        RegistryKey MCL = Registry.LocalMachine;
                        RegistryKey software = MCL.OpenSubKey("SOFTWARE\\ModernCraftLauncher", true);
                        string refreshToken = software.GetValue("RefreshToken").ToString();
                        var result = await authenticator.RefreshAuthenticateAsync(refreshToken);
                        software.SetValue("RefreshToken", result.RefreshToken);
                        var process = await minecraft
                            .WithAuthentication(result)
                            .WithMaxMemorySize(Convert.ToInt32(maxMem.Text))
                            .WithJava(javaCombo.Text)
                            .LaunchAsync();
                    }
                    else
                    {
                        MessageBox.Show("您尚未进行过微软登录，请进行微软登录后再启动游戏", "MCL启动器");
                    }
                }
            }
            else
            {
                MessageBox.Show("无法启动游戏！\n错误原因：参数缺失\n请检查Java、最大内存、登录方式等选项是否为空","MCL启动器");
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
