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
using MinecaftOAuth.Module.Enum;
using MinecraftLaunch.Modules.Installer;
using System.Diagnostics;

namespace MCL_Dev
{
    /// <summary>
    /// ZhuYe.xaml 的交互逻辑
    /// </summary>
    public partial class ZhuYe : Page
    {
        public static bool waizhi_refresh_exists;
        public static string name;
        public static string uuid;
        public static string accessToken;
        public static string gameFolder;
        public AuthType waizhi_authType = MinecaftOAuth.Module.Enum.AuthType.Access;
        int mode = 114514;//homo特有的变量（喜）
        public ZhuYe()
        {
            InitializeComponent();
            gameFolder = System.AppDomain.CurrentDomain.BaseDirectory + ".minecraft";
            var core = new GameCoreToolkit(gameFolder);
            versionCombo.ItemsSource = core.GetGameCores();
            javaCombo.Items.Add("自动选择Java");
            javaCombo.SelectedItem = 0;
            var java = JavaToolkit.GetJavas();
            foreach (var j in java)
            {
                javaCombo.Items.Add(j.JavaPath);
            }
            javaCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private async void startGame()
        {
            if (maxMem.Text != "" && mode != 114514 && versionCombo.Text != "")
            {
                if (mode == 0) // offline
                {
                    progressBar.IsIndeterminate = true;
                    if (offlineName != null)
                    {
                        Lixian lixian = new Lixian();
                        if (javaCombo.SelectedIndex != 0)
                        {
                            var lc = new LaunchConfig()
                            {
                                Account = new OfflineAccount(offlineName, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
                                GameWindowConfig = new GameWindowConfig()
                                {
                                    Width = 854,
                                    Height = 480,
                                    IsFullscreen = false
                                },
                                JvmConfig = new JvmConfig(javaCombo.Text)
                                {
                                    MaxMemory = Convert.ToInt32(maxMem.Text),
                                },
                                NativesFolder = null,//一般可以无视这个选项
                            };
                            JavaClientLauncher clientLauncher = new(lc, new(gameFolder));
                            launchLog.Text = "";
                            using var res = await clientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                            });
                        }
                        else
                        {
                            var javaList = JavaToolkit.GetJavas();
                            var gCore = GameCoreToolkit.GetGameCore(gameFolder, versionCombo.Text);
                            var gToolkit = new GameCoreToolkit(gameFolder);
                            var java = JavaToolkit.GetCorrectOfGameJava(javaList, gCore);
                            if (java != null)
                            {
                                var lc = new LaunchConfig()
                                {
                                    Account = new OfflineAccount(offlineName, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
                                    GameWindowConfig = new GameWindowConfig()
                                    {
                                        Width = 854,
                                        Height = 480,
                                        IsFullscreen = false
                                    },
                                    JvmConfig = new JvmConfig(java.JavaPath)
                                    {
                                        MaxMemory = Convert.ToInt32(maxMem.Text),
                                    },
                                    NativesFolder = null,//一般可以无视这个选项
                                };
                                JavaClientLauncher clientLauncher = new(lc,gToolkit);
                                launchLog.Text = "";
                                using var res = await clientLauncher.LaunchTaskAsync(versionCombo.Text,x =>
                                {
                                    launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                    launchLog.ScrollToEnd();
                                });
                            }
                            else
                            {
                                MessageBoxX.Show("启动失败！\n错误原因：未能找到合适的Java", "MCL启动器");
                            }
                        }
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

                    var core = new GameCoreToolkit(gameFolder);
                    if (javaCombo.SelectedIndex != 0)
                    {
                        JavaClientLauncher javaClientLauncher = new(new(microsoftaccount, new(javaCombo.Text)), core);
                        using var res = await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                        {
                            launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                            launchLog.ScrollToEnd();
                        });
                    }
                    else
                    {
                        var javaList = JavaToolkit.GetJavas();
                        var gameCore = GameCoreToolkit.GetGameCore(gameFolder, versionCombo.Text);
                        var java = JavaToolkit.GetCorrectOfGameJava(javaList, gameCore).JavaPath;
                        if (java == null)
                        {
                            MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查是否设置了用户名以及确定键是否按下", "MCL启动器");
                        }
                        else
                        {
                            JavaClientLauncher javaClientLauncher = new(new(microsoftaccount, new(java)), core);
                            using var res = await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                            });
                        }

                    }
                    progressBar.IsIndeterminate = false;
                }

                else //外置登录
                {
                    if (waizhi_selectedplayer != 114514)
                    {
                        // string[] Waizhi_accessToken = File.ReadAllLines($"{System.AppDomain.CurrentDomain.BaseDirectory}MCL\\waizhi\\WaiZhi_Access_{waizhi_selectedplayer}.txt");
                        // string[] Waizhi_clientToken = File.ReadAllLines($"{System.AppDomain.CurrentDomain.BaseDirectory}MCL\\waizhi\\WaiZhi_Client_{waizhi_selectedplayer}.txt");
                        progressBar.IsIndeterminate = true;
                        MinecaftOAuth.YggdrasilAuthenticator waizhiAuth = new(true, waizhi_email, waizhi_password);
                        // waizhiAuth.AccessToken = Waizhi_accessToken.ToString();
                        // waizhiAuth.ClientToken = Waizhi_clientToken.ToString();
                        var result = await waizhiAuth.AuthAsync(x => { });
                        var core = new GameCoreToolkit(gameFolder);
                        if (javaCombo.SelectedIndex != 0)
                        {
                            WaiZhi waizhi = new();
                            YggdrasilAccount account = new();
                            waizhi.Dispatcher.Invoke(new Action(() =>
                            {
                                account = waizhi.players.SelectedItem as YggdrasilAccount;
                            }));
                            var launchConfig = new LaunchConfig()
                            {
                                Account = account,
                                GameWindowConfig = new GameWindowConfig()
                                {
                                    Width = 854,
                                    Height = 480,
                                    IsFullscreen = false
                                },
                                JvmConfig = new JvmConfig(javaCombo.Text)
                                {
                                    MaxMemory = Convert.ToInt32(maxMem.Text),
                                },
                                NativesFolder = null,//一般可以无视这个选项
                            };
                            JavaClientLauncher javaClientLauncher = new(launchConfig,new(gameFolder));
                            using var res =await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                            });
                        }
                        else
                        {
                            WaiZhi waizhi = new();
                            YggdrasilAccount account = new();
                            waizhi.Dispatcher.Invoke(new Action(() =>
                            {
                                account = waizhi.players.SelectedItem as YggdrasilAccount;
                            }));
                            var javaList = JavaToolkit.GetJavas();
                            var gameCore = GameCoreToolkit.GetGameCore(gameFolder, versionCombo.Text);
                            var java = JavaToolkit.GetCorrectOfGameJava(javaList, gameCore).JavaPath;
                            var launchConfig = new LaunchConfig()
                            {
                                Account = account,
                                GameWindowConfig = new GameWindowConfig()
                                {
                                    Width = 854,
                                    Height = 480,
                                    IsFullscreen = false
                                },
                                JvmConfig = new JvmConfig(java)
                                {
                                    MaxMemory = Convert.ToInt32(maxMem.Text),
                                },
                                NativesFolder = null,//一般可以无视这个选项
                            };
                            JavaClientLauncher javaClientLauncher = new(launchConfig,new(gameFolder));
                            using var res = await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                            });
                            progressBar.IsIndeterminate = false;
                        }
                    }
                    else
                    {
                        MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查外置登录——角色一栏是否选择", "MCL启动器");
                    }
                }
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
        private async void start_Copy1_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageBoxX.Show("确认开始验证微软账户", "MCL启动器", null, MessageBoxButton.OKCancel);
            if (message == MessageBoxResult.OK)
            {
                mode = 1;
                WeiRuan weiRuan = new();
                login.Content = new Frame()
                {
                    Content = weiRuan
                };
                #region 登录
                MicrosoftAccount result = new();
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
                    string usrCode = code_1.UserCode;
                    Clipboard.SetDataObject(usrCode);
                    MessageBoxX.Show(usrCode + "\n已复制该串字符到剪切板，请粘贴这串字符到弹出的网页中进行登录，完成后再关闭此弹窗", "MCL启动器");
                    #region 打开网页
                    Process.Start(new ProcessStartInfo(code_1.VerificationUrl)
                    {
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                    #endregion         
                    try
                    {
                        var Code_2 = await auth.GetTokenResponse(code_1);
                        result = await auth.AuthAsync(x => { });
                        #region 保存数据
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
                        #endregion
                        weiRuan.loadRing.Visibility = Visibility.Hidden;
                        weiRuan.textBlock.Text = "用户名：" + result.Name;
                        microsoftaccount = result;
                    }
                    catch
                    {
                        mode = 114514;
                        MessageBoxX.Show("微软登录失败！\n错误原因可能有:1.你没购买Minecraft\n2.微软的验证服务器炸了\n3.你没联网", "MCL启动器");
                    }
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
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                        while ((line = sr.ReadLine()) != null)
                        {
                            strCon += line + " ";
                        }
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                    }
                    auth.AuthType = MinecaftOAuth.Module.Enum.AuthType.Refresh;
                    auth.RefreshToken = strCon;
                    try
                    {
                        result = await auth.AuthAsync(x => { });
                        #region 保存数据
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
                        #endregion
                        weiRuan.loadRing.Visibility = Visibility.Hidden;
                        weiRuan.textBlock.Text = "用户名：" + result.Name;                        
                        microsoftaccount = result;
                    }
                    catch
                    {
                        mode = 114514;
                        MessageBoxX.Show("微软登录失败！\n错误原因可能有:1.你没购买Minecraft\n2.微软的验证服务器炸了\n3.你没联网", "MCL启动器");
                    }
                    progressBar.IsIndeterminate = false;
                }
                #endregion
                
            }

        }

        private void start_Copy2_Click(object sender, RoutedEventArgs e) //暂时注释
        {
            bool a = false;
            //DirectoryInfo dirInfo = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "MCL\\waizhi");//查看WaiZhi文件夹是否存在
            if (a == true)//曾经登陆过
            {
                //mode = 2;
                //MessageBoxX.Show("您可以直接启动游戏，无需登录！", "MCL启动器");
            }
            else
            {
                mode = 2;
                WaiZhi waizhi = new();
                login.Content = new Frame()
                {
                    Content = waizhi
                };
            }
        }
    }
}
