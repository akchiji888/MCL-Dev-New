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
using System.IO;

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
                if (mode == 0) // offline
                {
                    progressBar.IsIndeterminate = true;
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
                        JavaClientLauncher javaClientLauncher = new(new(offline, new(javaCombo.Text)), core);
                        await javaClientLauncher.LaunchTaskAsync(versionCombo.Text);
                        progressBar.IsIndeterminate = false;
                    }
                    else
                    {
                        MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查是否设置了用户名以及确定键是否按下", "MCL启动器");
                    }
                }
                else if (mode == 1) // microsoft
                {
                    var auth = new MinecaftOAuth.MicrosoftAuthenticator();
                    progressBar.IsIndeterminate = true;
                    string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "MCL";
                    string refreshPath = filePath + "\\RefreshToken.txt";
                    auth.ClientId = "dd09ec86-031b-4429-adb8-7fec6bc1fd79";
                    bool file = System.IO.File.Exists(refreshPath);
                    if (file != true)//无RefreshToken存在
                    {
                        auth.AuthType = MinecaftOAuth.Module.Enum.AuthType.Access;                        
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
                        p.StandardInput.WriteLine("start " + code_1.VerificationUrl + "&exit");
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
                        string configPath = System.AppDomain.CurrentDomain.BaseDirectory + "MCL";
                        if (!Directory.Exists(configPath))
                        {
                            Directory.CreateDirectory(configPath);
                        }
                        string fname = refreshPath;
                        System.IO.File.WriteAllText(fname, string.Empty);
                        FileInfo finfo = new FileInfo(fname);

                        if (!finfo.Exists)
                        {
                            FileStream fs;
                            fs = File.Create(fname);
                            fs.Close();
                            finfo = new FileInfo(fname);
                        }
                        using (FileStream fs = finfo.OpenWrite())
                        {
                            //根据上面创建的文件流创建写数据流
                            StreamWriter w = new StreamWriter(fs);

                            //设置写数据流的起始位置为文件流的末尾
                            w.BaseStream.Seek(0, SeekOrigin.End);

                            //写入内容
                            w.Write(result.RefreshToken);

                            //清空缓冲区内容，并把缓冲区内容写入基础流
                            w.Flush();

                            //关闭写数据流
                            w.Close();
                        }
                        var core = new GameCoreToolkit(gameFolder);
                        JavaClientLauncher javaClientLauncher = new(new(result, new(javaCombo.Text)), core);
                        await javaClientLauncher.LaunchTaskAsync(versionCombo.Text);
                        progressBar.IsIndeterminate = false;
                    }
                    else//存在RefreshToken
                    {
                        string strCon = string.Empty;
                        // 创建一个 StreamReader 的实例来读取文件
                        // using 语句也能关闭 StreamReader
                        using (StreamReader sr = new StreamReader(refreshPath))
                        {
                            string line;
                            // 从文件读取并显示行，直到文件的末尾 
                            while ((line = sr.ReadLine()) != null)
                            {
                                strCon += line + " ";
                            }
                        }
                        auth.AuthType = MinecaftOAuth.Module.Enum.AuthType.Refresh;
                        auth.RefreshToken = strCon;
                        var result = await auth.AuthAsync(x => { });
                        string fname = refreshPath;
                        System.IO.File.WriteAllText(fname, string.Empty);
                        FileInfo finfo = new FileInfo(fname);

                        if (!finfo.Exists)
                        {
                            FileStream fs;
                            fs = File.Create(fname);
                            fs.Close();
                            finfo = new FileInfo(fname);
                        }
                        using (FileStream fs = finfo.OpenWrite())
                        {
                            //根据上面创建的文件流创建写数据流
                            StreamWriter w = new StreamWriter(fs);

                            //设置写数据流的起始位置为文件流的末尾
                            w.BaseStream.Seek(0, SeekOrigin.End);

                            //写入内容
                            w.Write(result.RefreshToken);

                            //清空缓冲区内容，并把缓冲区内容写入基础流
                            w.Flush();

                            //关闭写数据流
                            w.Close();
                        }
                        var core = new GameCoreToolkit(gameFolder);
                        JavaClientLauncher javaClientLauncher = new(new(result, new(javaCombo.Text)), core);
                        await javaClientLauncher.LaunchTaskAsync(versionCombo.Text);
                        progressBar.IsIndeterminate = false;
                    }
                }
                /*else //外置登录
                {
                    progressBar.IsIndeterminate = true;
                    string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "MCL";
                    string accessPath = filePath + "\\YggdrasilAccessToken.txt";
                    string ClientPath = filePath + "\\YggdrasilClientToken.txt";
                    bool file = System.IO.File.Exists(accessPath)&&System.IO.File.Exists(ClientPath);
                    var auth = new MinecaftOAuth.YggdrasilAuthenticator(true,"你想看我邮箱地址？","想看密码？想得美！");
                    if(file != true)//无相关文件
                    {
                        var result = await auth.AuthAsync(x => { });
                        string configPath = System.AppDomain.CurrentDomain.BaseDirectory + "MCL";
                        if (!Directory.Exists(configPath))
                        {
                            Directory.CreateDirectory(configPath);
                        }
                        string fname = accessPath;
                        string fname_cli = ClientPath;
                        System.IO.File.WriteAllText(fname, string.Empty);
                        FileInfo finfo = new FileInfo(fname);
                        FileInfo cliInfo = new FileInfo(fname_cli);
                        if (!finfo.Exists)
                        {
                            FileStream fs;
                            fs = File.Create(fname);
                            fs.Close();
                            finfo = new FileInfo(fname);
                        }                        
                        using (FileStream fs = finfo.OpenWrite())
                        {
                            //根据上面创建的文件流创建写数据流
                            StreamWriter w = new StreamWriter(fs);

                            //设置写数据流的起始位置为文件流的末尾
                            w.BaseStream.Seek(0, SeekOrigin.End);

                            //写入内容
                            w.Write(result);

                            //清空缓冲区内容，并把缓冲区内容写入基础流
                            w.Flush();

                            //关闭写数据流
                            w.Close();
                        }
                    }
                    else//相关文件存在
                    {

                    };
                    
                };*/

            }
            else
            {
                MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查Java、最大内存、登录方式等选项是否为空", "MCL启动器");
            }
        }
        // 启动游戏（按钮事件）
        private void start_Click(object sender, RoutedEventArgs e)
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
        private void start_Copy1_Click(object sender, RoutedEventArgs e)
        {
            mode = 1;
        }

        private void start_Copy2_Click(object sender, RoutedEventArgs e) //暂时注释
        {
            MessageBoxX.Show("外置登录功能将于下一个版本对其进行支持", "MCL启动器");
            /*
            mode = 2;
            WaiZhi waizhi = new();
            login.Content = new Frame()
            {
                Content = waizhi
            };
            */ 
        }
    }
}
