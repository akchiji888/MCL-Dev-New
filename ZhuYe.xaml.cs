using System.Windows;
using System.Windows.Controls;
using static MCL_Dev.LauncherClasses;
using MinecraftLaunch.Modules.Toolkits;
using MinecraftLaunch.Launch;
using MinecaftOAuth;
using MinecraftLaunch.Modules.Models.Auth;
using System;
using MinecraftLaunch.Modules.Models.Launch;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;

namespace MCL_Dev
{
    /// <summary>
    /// ZhuYe.xaml 的交互逻辑
    /// </summary>
    public partial class ZhuYe : Page
    {
        public static string name;
        public static string uuid;
        public static string accessToken;
        public static string gameFolder;
        int mode = 114514;//homo特有的变量（喜）

        public ZhuYe()
        {
            InitializeComponent();
            gameFolder = System.AppDomain.CurrentDomain.BaseDirectory + ".minecraft";
            var core = new GameCoreToolkit(gameFolder);
            versionCombo.ItemsSource = core.GetGameCores();
            javaCombo.ItemsSource = GetJavaPath();

        }

        /// <summary>
        /// 
        /// </summary>
        private async void startGame()
        {
            if (javaCombo.Text != null && maxMem.Text != null && mode != 114514 && versionCombo.Text != null)
            {
                if(mode == 0) // offline
                {
                    if (offlineName != null)
                    {
                        Lixian lixian = new Lixian();
                        var core = new GameCoreToolkit(gameFolder);
                        OfflineAccount offline = new OfflineAccount()
                        {                            
                            Name = lixian.NameCombo.Text,
                            Uuid = Guid.NewGuid(),
                            AccessToken = Guid.NewGuid().ToString("N"),
                            ClientToken = Guid.NewGuid().ToString("N")
                        };
                        JavaClientLauncher javaClientLauncher = new(new(offline,new(javaCombo.Text)), core);
                        await javaClientLauncher.LaunchTaskAsync(versionCombo.Text);
                    }
                    else
                    {
                        MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查是否设置了用户名以及确定键是否按下", "MCL启动器");
                    }
                }
                else if(mode == 1) // microsoft
                {
                    progressBar.IsIndeterminate = true;
                    var auth = new MinecaftOAuth.MicrosoftAuthenticator();
                    auth.ClientId = "dd09ec86-031b-4429-adb8-7fec6bc1fd79";
                    var code_1 = await auth.GetDeviceInfo();
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                    p.Start();//启动程序

                    //向cmd窗口发送输入信息
                    p.StandardInput.WriteLine("start https://www.microsoft.com/link" + "&exit");
                    p.StandardInput.AutoFlush = true;
                    p.WaitForExit();//等待程序执行完退出进程
                    p.Close();
                    string usrCode = code_1.UserCode;
                    MessageBoxX.MessageBoxXConfigurations.Add("standard", new Panuon.UI.Silver.Core.MessageBoxXConfigurations()
                    {
                        YesButton = "我已完成登录",
                        OKButton = "我已完成登录"
                    });
                    var a = MessageBoxX.Show(usrCode + "\n请输入这串字符到弹出的网页中进行登录，完成后再关闭此弹窗", "MCL启动器", configKey: "standard");
                    var Code_2 = await auth.GetTokenResponse(code_1);
                    accessToken = Code_2.AccessToken;
                    var result = await auth.AuthAsync(x => { });
                    progressBar.IsIndeterminate = false;
                    var core = new GameCoreToolkit(gameFolder);
                    JavaClientLauncher javaClientLauncher = new(new(result, new(javaCombo.Text)), core);
                    await javaClientLauncher.LaunchTaskAsync(versionCombo.Text);
                }
                else //外置登录
                {

                }

            }
            else
            {
                MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查Java、最大内存、登录方式等选项是否为空", "MCL启动器");
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
