using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Natsurainko.FluentCore.Class.Model.Launch;
using Natsurainko.FluentCore.Module.Authenticator;
using Natsurainko.FluentCore.Module.Launcher;
using Natsurainko.FluentCore.Wrapper;
using Natsurainko.FluentCore.Extension.Windows.Module.Authenticator;
using static MCL_Dev.LauncherClasses;
using Natsurainko.FluentCore.Class.Model.Auth;
using Natsurainko.FluentCore.Service;

namespace MCL_Dev
{
    /// <summary>
    /// ZhuYe.xaml 的交互逻辑
    /// </summary>
    public partial class ZhuYe : Page
    {
        public static string gameFolder;
        int mode = 114514;//homo特有的变量（喜）

        public ZhuYe()
        {
            InitializeComponent();
            gameFolder = System.AppDomain.CurrentDomain.BaseDirectory + ".minecraft";
            var gameLocator = new GameCoreLocator(gameFolder);
            var gameList = gameLocator.GetGameCores();
            foreach (var game in gameList)
            {
                versionCombo.Items.Add(game.Id);
            }
            javaCombo.ItemsSource = GetJavaPath();

        }

        /// <summary>
        /// 
        /// </summary>
        private async void startGame()
        {
            if (javaCombo.Text != null && maxMem.Text != null && mode != 114514 && versionCombo.Text != null)
            {
                var gameLocator = new GameCoreLocator(gameFolder);
                string javaPath = javaCombo.Text;
                string core = versionCombo.Text;
                var launchSetting = new LaunchSetting()
                {
                    Account = Account.Default,
                    GameWindowSetting = new GameWindowSetting
                    {
                        Width = 854,
                        Height = 480,
                        IsFullscreen = false
                    },
                    IsDemoUser = false,
                    JvmSetting = new JvmSetting(javaPath)
                    {
                        MaxMemory = Convert.ToInt32(maxMem.Text),
                        MinMemory = 256,
                        AdvancedArguments = DefaultSettings.DefaultAdvancedArguments,
                        GCArguments = DefaultSettings.DefaultGCArguments
                    },
                    NativesFolder = null,
                    XmlOutputSetting = new XmlOutputSetting
                    {
                        Enable = false
                    }
                };
                var locator = new GameCoreLocator(gameFolder); // 初始化核心定位器
                if (mode == 0)
                {
                    if (offlineName != null)
                    {
                        var authenticator = new OfflineAuthenticator(offlineName);
                        var launcher = new MinecraftLauncher(launchSetting, gameLocator);
                        using var launchResponse = launcher.LaunchMinecraft(core);
                        
                    }
                    else
                    {
                        MessageBox.Show("无法启动游戏！\n错误原因：参数缺失\n请检查您是否输入了用户名以及“确定”键是否按下", "MCL启动器");
                    }
                }
                else
                {
                    var microsoftAuthenticator = new MicrosoftAuthenticator();
                    await microsoftAuthenticator.GetAccessCode(); // 调用系统默认浏览器取回验证令牌 需要 Natsurainko.FluentCore 的 Windows 扩展
                    var account = await microsoftAuthenticator.AuthenticateAsync(); // 验证账户
                    // 将验证得到的账户 添加到启动 方法 1
                    var setting = new LaunchSetting(new JvmSetting(javaPath));
                    setting.Account = account;
                    var launcher = new MinecraftLauncher(setting, locator);
                }
            }
            else
            {
                MessageBox.Show("无法启动游戏！\n错误原因：参数缺失\n请检查Java、最大内存、登录方式等选项是否为空", "MCL启动器");
            }
        }
        // 启动游戏（按钮事件）
        private async void start_Click(object sender, RoutedEventArgs e)
        {
            startGame();
        }
        //离线登录
        private void start_Copy_Click(object sender, RoutedEventArgs e)
        {
            mode = 0;
            Lixian lixian = new Lixian();
            login.Content = new Frame()
            {
                Content = lixian
            };
        }
        //微软登录
        private async void start_Copy1_Click(object sender, RoutedEventArgs e)
        {
            mode = 1;
        }
    }
}
