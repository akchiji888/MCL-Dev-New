using Microsoft.WindowsAPICodePack.Dialogs;
using MinecaftOAuth;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Panuon.WPF.UI;
using Panuon.WPF.UI.Configurations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static MCL_Dev.LauncherClasses;

namespace MCL_Dev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowX
    {
        private viewStyle ShiTu = viewStyle.Night;
        CurseForgeToolkit ModToolkit = new("$2a$10$CUT15u9CIECXHsa8q2NDqO.5zl3lNjg/5Xw5kYVX.ClujkvAir09K");
        #region 公共变量与void声明
        private async void GetMcVersionList()
        {
            try
            {
                if (IsMCListInitialize == false)
                {
                    ModSpin.IsSpinning = true;
                    ModLoadingText.Visibility = Visibility.Visible;
                    var v = new GameCoreToolkit(gameFolder);
                    var MCList = await GameCoreInstaller.GetGameCoresAsync();
                    verBox.ItemsSource = MCList.Cores;
                    opt_game_verBox.ItemsSource = MCList.Cores;
                    fabric_game_verBox.ItemsSource = MCList.Cores;
                    Forge_game_verBox.ItemsSource = MCList.Cores;
                    Quilt_game_verBox.ItemsSource = MCList.Cores;
                    java_verBox.Items.Add(OpenJdkType.OpenJdk8);
                    java_verBox.Items.Add(OpenJdkType.OpenJdk11);
                    java_verBox.Items.Add(OpenJdkType.OpenJdk17);
                    java_verBox.Items.Add(OpenJdkType.OpenJdk18);
                    var res = await ModToolkit.GetFeaturedModpacksAsync();
                    IsMCListInitialize = true;
                    List<Mod> mods = new();
                    res.ForEach(x =>
                    {
                        Mod modItem = new();
                        modItem.Description = x.Description;
                        modItem.image = new(new System.Uri(x.IconUrl));
                        modItem.Name = x.Name;
                        modItem.Version = $"{x.SupportedVersions.Last()}-{x.SupportedVersions[0]}";
                        modItem.Files = x.Files;
                        mods.Add(modItem);
                    });
                    ModGrid.ItemsSource = mods;
                    ModSpin.IsSpinning = false;
                    ModLoadingText.Visibility = Visibility.Hidden;
                }
            }
            catch
            {
                IsMCListInitialize = false;
                MessageBox.Show("无法获取下载信息！请检查电脑是否连接了互联网！", "MCL启动器", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        bool IsMCListInitialize = false;
        #endregion
        public GameCoreInstaller installer;
        #region 全局变量声明
        public bool waizhi_refresh_exists;
        public string name;
        public string uuid;
        public string accessToken;
        private string modName;
        public static string gameFolder = System.AppDomain.CurrentDomain.BaseDirectory + ".minecraft";
        int mode = 114514;//homo特有的变量（喜）
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            ModGrid.Visibility = Visibility.Visible;
            ModPage.Visibility = Visibility.Hidden;
            #region 控件初始化
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
            #endregion
            var FirstTime = !Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "MCL");            
            
            Task.Run(() =>
            {
                Thread.Sleep(500);
                if (FirstTime == true)
                {
                    var setting = Application.Current.FindResource("YongHuXieYi") as MessageBoxXSetting;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var XieYiMessage = MessageBoxX.Show(null, "使用MCL启动器就代表您同意《用户协议与免责声明》", "MCL启动器", MessageBoxButton.OKCancel, MessageBoxIcon.Info, setting);
                        if (XieYiMessage == MessageBoxResult.OK)
                        {
                            Process.Start(new ProcessStartInfo("https://docs.qq.com/doc/DUklrU3VJTm92SnFG")
                            {
                                UseShellExecute = true,
                                CreateNoWindow = true
                            });
                        }
                        else
                        {

                        }
                    }));                    
                    DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "MCL");
                    directoryInfo.Create();
                }
            });
            Task.Run(new Action(() =>
            {
                try
                {
                    UpdateD.Update updated = new UpdateD.Update();
                    var HaveHigherVersion = updated.GetUpdate("4386F97F6C36488887EBA723C4C99C83", LauncherVersion);
                    if (HaveHigherVersion == true)
                    {
                        NoticeBox.Show("启动器有新版本可供更新，可前往【设置】进行更新！", "MCL启动器", MessageBoxIcon.Info);
                        Thread.Sleep(3000);
                        NoticeBox.DestroyInstance();
                    }
                    else
                    {
                        NoticeBox.Show("启动器已是最新版本！", "MCL启动器", MessageBoxIcon.Success);
                        Thread.Sleep(3000);
                        NoticeBox.DestroyInstance();
                    }
                }
                catch
                {
                    NoticeBox.Show("无法检测是否有更新！", "MCL启动器", MessageBoxIcon.Error);
                    Thread.Sleep(3000);
                    NoticeBox.DestroyInstance();
                }
            }));
        }
        #region 启动游戏(void)
        private async void startGame()
        {
            progressBar.Value = 0;
            if (maxMem.Text != "" && mode != 114514 && versionCombo.Text != "")
            {
                if (mode == 0) // offline
                {
                    if (NameCombo.Text != "")
                    {
                        if (javaCombo.SelectedIndex != 0)
                        {
                            OfflineAuthenticator offline = new(NameCombo.Text);
                            var lc = new LaunchConfig()
                            {
                                Account = offline.Auth(),
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
                                WorkingFolder = new("q"),
                                LauncherName = "ModernCraftLauncher",

                            };
                            JavaClientLauncher clientLauncher = new(lc, new(gameFolder), true);
                            launchLog.Text = "";
                            using var res = await clientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                                progressBar.Value = x.Item1;
                            });
                            if (res.State is LaunchState.Succeess)
                            {
                                //启动成功的情况下会执行的代码块
                                launchLog.AppendText($"[{DateTime.Now}]启动成功");
                                progressBar.Value = 1;
                            }
                            else
                            {
                                //启动失败的情况下会执行的代码块
                                launchLog.AppendText("启动失败\n");
                                launchLog.AppendText("详细异常信息：" + res.Exception);
                                progressBar.Value = 0;
                            }
                        }
                        else
                        {
                            var javaList = JavaToolkit.GetJavas();
                            var gCore = GameCoreToolkit.GetGameCore(gameFolder, versionCombo.Text);
                            var gToolkit = new GameCoreToolkit(gameFolder);
                            var java = JavaToolkit.GetCorrectOfGameJava(javaList, gCore);
                            OfflineAuthenticator offline = new(NameCombo.Text);
                            if (java != null)
                            {
                                var lc = new LaunchConfig()
                                {
                                    Account = offline.Auth(),
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
                                    WorkingFolder = new("q"),
                                    NativesFolder = null,//一般可以无视这个选项
                                    LauncherName = "ModernCraftLauncher"
                                };
                                JavaClientLauncher clientLauncher = new(lc, gToolkit, true);
                                launchLog.Text = "";
                                using var res = await clientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                                {
                                    launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                    launchLog.ScrollToEnd();
                                    progressBar.Value = x.Item1;
                                });
                                if (res.State is LaunchState.Succeess)
                                {
                                    //启动成功的情况下会执行的代码块
                                    launchLog.AppendText($"[{DateTime.Now}]启动成功");
                                    progressBar.Value = 1;
                                }
                                else
                                {
                                    //启动失败的情况下会执行的代码块
                                    launchLog.AppendText("启动失败");
                                    launchLog.AppendText("详细异常信息：" + res.Exception);
                                    progressBar.Value = 0;
                                }
                            }
                            else
                            {
                                MessageBoxX.Show("启动失败！\n错误原因：未能找到合适的Java", "MCL启动器", MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查是否设置了用户名", "MCL启动器", MessageBoxIcon.Error);
                    }
                }
                else if (mode == 1) // microsoft
                {
                    var auth = new MinecaftOAuth.MicrosoftAuthenticator();
                    var core = new GameCoreToolkit(gameFolder);
                    if (javaCombo.SelectedIndex != 0)
                    {
                        var lc = new LaunchConfig()
                        {
                            Account = microsoftaccount,
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
                            WorkingFolder = new("q"),
                            NativesFolder = null,//一般可以无视这个选项
                            LauncherName = "ModernCraftLauncher"
                        };
                        JavaClientLauncher javaClientLauncher = new(lc, core, true);
                        launchLog.Text = "";
                        using var res = await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                        {
                            launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                            launchLog.ScrollToEnd();
                            progressBar.Value = x.Item1;
                        });
                        if (res.State is LaunchState.Succeess)
                        {
                            //启动成功的情况下会执行的代码块
                            launchLog.AppendText($"[{DateTime.Now}]启动成功");
                            progressBar.Value = 1;
                        }
                        else
                        {
                            //启动失败的情况下会执行的代码块
                            launchLog.AppendText("启动失败\n");
                            launchLog.AppendText("详细异常信息：" + res.Exception);
                            progressBar.Value = 0;
                        }
                    }
                    else
                    {
                        var javaList = JavaToolkit.GetJavas();
                        var gameCore = GameCoreToolkit.GetGameCore(gameFolder, versionCombo.Text);
                        var java = JavaToolkit.GetCorrectOfGameJava(javaList, gameCore).JavaPath;
                        if (java == null)
                        {
                            MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查是否设置了用户名", "MCL启动器", MessageBoxIcon.Error);
                        }
                        else
                        {
                            var lc = new LaunchConfig()
                            {
                                Account = microsoftaccount,
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
                                WorkingFolder = new("q"),
                                NativesFolder = null,//一般可以无视这个选项
                                LauncherName = "ModernCraftLauncher"
                            };
                            launchLog.Text = "";
                            JavaClientLauncher javaClientLauncher = new(lc, core, true);
                            using var res = await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                                progressBar.Value = x.Item1;
                            });
                            if (res.State is LaunchState.Succeess)
                            {
                                //启动成功的情况下会执行的代码块
                                launchLog.AppendText($"[{DateTime.Now}]启动成功");
                                progressBar.Value = 1;
                            }
                            else
                            {
                                //启动失败的情况下会执行的代码块
                                launchLog.AppendText("启动失败");
                                launchLog.AppendText("详细异常信息：" + res.Exception);
                                progressBar.Value = 0;
                            }
                        }

                    }
                }

                else //外置登录
                {
                    if (players.SelectedIndex != -1)
                    {
                        if (javaCombo.SelectedIndex != 0)
                        {
                            var launchConfig = new LaunchConfig()
                            {
                                Account = players.SelectedItem as YggdrasilAccount,
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
                                WorkingFolder = new("q"),
                                NativesFolder = null,//一般可以无视这个选项
                                LauncherName = "ModernCraftLauncher"
                            };
                            JavaClientLauncher javaClientLauncher = new(launchConfig, new(gameFolder), true);
                            launchLog.Text = "";
                            using var res = await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                                progressBar.Value = x.Item1;
                            });
                            if (res.State is LaunchState.Succeess)
                            {
                                //启动成功的情况下会执行的代码块
                                launchLog.AppendText($"[{DateTime.Now}]启动成功");
                                progressBar.Value = 1;
                            }
                            else
                            {
                                //启动失败的情况下会执行的代码块
                                launchLog.AppendText("启动失败");
                                launchLog.AppendText("详细异常信息：" + res.Exception);
                                progressBar.Value = 0;
                            }
                        }
                        else
                        {
                            var javaList = JavaToolkit.GetJavas();
                            var gameCore = GameCoreToolkit.GetGameCore(gameFolder, versionCombo.Text);
                            var java = JavaToolkit.GetCorrectOfGameJava(javaList, gameCore).JavaPath;
                            var launchConfig = new LaunchConfig()
                            {
                                Account = players.SelectedItem as YggdrasilAccount,
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
                                WorkingFolder = new("q"),
                                NativesFolder = null,//一般可以无视这个选项
                                LauncherName = "ModernCraftLauncher"
                            };
                            JavaClientLauncher javaClientLauncher = new(launchConfig, new(gameFolder), true);
                            launchLog.Text = "";
                            using var res = await javaClientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                            {
                                launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                                launchLog.ScrollToEnd();
                                progressBar.Value = x.Item1;
                            });
                            if (res.State is LaunchState.Succeess)
                            {
                                //启动成功的情况下会执行的代码块
                                launchLog.AppendText($"[{DateTime.Now}]启动成功");
                                progressBar.Value = 1;
                            }
                            else
                            {
                                //启动失败的情况下会执行的代码块
                                launchLog.AppendText("启动失败");
                                launchLog.AppendText("详细异常信息：" + res.Exception);
                                progressBar.Value = 0;
                            }
                        }
                    }
                    else
                    {
                        MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查外置登录——角色一栏是否选择", "MCL启动器", MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBoxX.Show("无法启动游戏！\n错误原因：参数缺失\n请检查Java、最大内存、登录方式等选项是否为空", "MCL启动器", MessageBoxIcon.Error);
            }
        }
        #endregion
        #region 启动游戏（按钮事件）
        private void start_Click(object sender, RoutedEventArgs e)
        {
            startGame();
        }
        #endregion
        #region 登录UI
        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LoginTab.SelectedIndex)
            {
                case 0://offline
                    mode = 0;
                    break;
                case 1:
                    mode = 2;
                    break;
                case 2:
                    mode = 1;
                    var message = MessageBoxX.Show(null, "确认开始验证微软账户", "MCL启动器", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
                    if (message == MessageBoxResult.OK)
                    {
                        spin.IsSpinning = true;
                        textBlock.Text = "正在登录中，请稍后";
                        mode = 1;
                        #region 登录
                        MicrosoftAccount result = new();
                        var auth = new MinecaftOAuth.MicrosoftAuthenticator();
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
                            MessageBoxX.Show(usrCode + "\n已复制该串字符到剪切板，请粘贴这串字符到弹出的网页中进行登录", "MCL启动器", MessageBoxIcon.Info);
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

                                    StreamWriter w = new StreamWriter(fs);//根据上面创建的文件流创建写数据流
                                    w.BaseStream.Seek(0, SeekOrigin.End);//设置写数据流的起始位置为文件流的末尾
                                    w.Write(result.RefreshToken);//写入内容
                                    w.Flush();//清空缓冲区内容，并把缓冲区内容写入基础流                       
                                    w.Close();//关闭写数据流
                                }
                                #endregion
                                textBlock.Text = "用户名：" + result.Name;
                                spin.IsSpinning = false;
                                microsoftaccount = result;
                                MessageBoxX.Show("已完成登录！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Success);
                            }
                            catch
                            {
                                mode = 114514;
                                MessageBoxX.Show("微软登录失败！\n错误原因可能有:1.你没购买Minecraft\n2.微软的验证服务器炸了\n3.你没联网", "MCL启动器", MessageBoxIcon.Error);
                            }
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
                                    StreamWriter w = new StreamWriter(fs);//根据上面创建的文件流创建写数据流
                                    w.BaseStream.Seek(0, SeekOrigin.End);//设置写数据流的起始位置为文件流的末尾
                                    w.Write(result.RefreshToken);//写入内容
                                    w.Flush();//清空缓冲区内容，并把缓冲区内容写入基础流
                                    w.Close();//关闭写数据流
                                }
                                #endregion
                                textBlock.Text = "用户名：" + result.Name;
                                microsoftaccount = result;
                                spin.IsSpinning = false;
                                MessageBoxX.Show("已完成登录！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Success);
                            }
                            catch
                            {
                                mode = 114514;
                                MessageBoxX.Show("微软登录失败！\n错误原因可能有:1.你没购买Minecraft\n2.微软的验证服务器炸了\n3.你没联网", "MCL启动器", MessageBoxIcon.Error);
                            }
                        }
                        #endregion

                    }
                    break;
            }
        }
        #endregion
        #region 事件
        private async void Waizhi_start_Click(object sender, RoutedEventArgs e)
        {
            if (email.Text != "" && passwd.Password != "")
            {
                try
                {
                    MCL_Dev.LauncherClasses.YggdrasilAuthenticator ya = new(true, email.Text, passwd.Password);
                    var res = await ya.AuthAsync(X => { });
                    players.ItemsSource = res;
                    MessageBoxX.Show("已完成登录", "MCL启动器", MessageBoxIcon.Success);
                }
                catch
                {
                    MessageBoxX.Show("登录失败！\n请检查密码是否输入正确", "MCL启动器", MessageBoxIcon.Error);
                }



            }
            else
            {
                MessageBoxX.Show("信息未填写完整！", "MCL启动器", MessageBoxIcon.Error);
            };
        }
        #endregion
        #region 下载事件
        private async void VanilaStart_Click(object sender, RoutedEventArgs e)
        {
            progress.Value = 0;
            if (verBox.Text != "")
            {
                string version = verBox.Text;
                var v = new GameCoreToolkit(gameFolder);
                await Task.Run(() =>
                {
                    installer = new(v, version);
                });
                await Task.Run(async () =>
                {
                    await installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            downloadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            downloadLog.ScrollToEnd();
                            progress.Value = e.Item1;
                        }));
                    });
                });
                var setting = Application.Current.FindResource("CustomNoticeSetting") as NoticeBoxSetting;
                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true, setting);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show("下载版本未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }

        private async void optInstall_start_Click(object sender, RoutedEventArgs e)
        {
            optInstallProgress.Value = 0;
            if (opt_game_verBox_Copy.Text != "" && opt_game_verBox.Text != "")
            {
                string version = opt_game_verBox.Text;
                string opt_version = opt_game_verBox_Copy.Text;
                var javaList = JavaToolkit.GetJavas();
                var v = new GameCoreToolkit(gameFolder);
                MinecraftLaunch.Modules.Models.Install.OptiFineInstallEntity OPT = opt_game_verBox_Copy.SelectedItem as MinecraftLaunch.Modules.Models.Install.OptiFineInstallEntity;
                List<string> javas = new List<string>();
                foreach (var a in javaList)
                {
                    javas.Add(a.JavaPath);
                }
                await Task.Run(async () =>
                {
                    var opt_installer = new OptiFineInstaller(v, OPT, javas[0]);
                    await opt_installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            optDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            optDownLoadLog.ScrollToEnd();
                            optInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                var setting = Application.Current.FindResource("CustomNoticeSetting") as NoticeBoxSetting;
                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true, setting);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
                //
            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }


        private async void opt_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (opt_game_verBox.Text != "")
            {
                string selectedGameV = (opt_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var OptList = await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(selectedGameV);
                opt_game_verBox_Copy.ItemsSource = OptList;
            }
        }

        private async void javaInstall_start_Click(object sender, RoutedEventArgs e)
        {
            javaInstallProgress.Value = 0;
            if (java_verBox.Text != "")
            {
                var jv = (OpenJdkType)java_verBox.SelectedItem;
                JavaInstaller java = new(JdkDownloadSource.JdkJavaNet, jv, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                await Task.Run(async () =>
                {
                    var res = await java.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            javaDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            javaDownLoadLog.ScrollToEnd();
                            javaInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                var setting = Application.Current.FindResource("CustomNoticeSetting") as NoticeBoxSetting;
                var handler = NoticeBox.Show("安装完成！", "MCL启动器", MessageBoxIcon.Success, true, setting);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }

        private async void fabricInstall_start_Click(object sender, RoutedEventArgs e)
        {
            fabricInstallProgress.Value = 0;
            if (fabric_game_verBox.Text != "" && fabric_game_verBox_Copy.Text != "")
            {
                var FabricBuild = fabric_game_verBox_Copy.SelectedItem as FabricInstallBuild;
                var v = new GameCoreToolkit(gameFolder);
                var fi = new FabricInstaller(v, FabricBuild);
                await Task.Run(async () =>
                {
                    await fi.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            fabricDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            fabricDownLoadLog.ScrollToEnd();
                            fabricInstallProgress.Value = e.Item1;
                        }));
                    });
                    var setting = Application.Current.FindResource("CustomNoticeSetting") as NoticeBoxSetting;
                    var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true, setting);
                    await Task.Run(() =>
                    {
                        Thread.Sleep(1500);
                    });
                    NoticeBox.DestroyInstance();
                });
            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }

        private async void fabric_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (fabric_game_verBox.Text != "")
            {
                string selectedGameV = (fabric_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var FabricList = await FabricInstaller.GetFabricBuildsByVersionAsync(selectedGameV);
                fabric_game_verBox_Copy.ItemsSource = FabricList;
            }
        }
        private async void Forge_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (Forge_game_verBox.Text != "")
            {
                string selectedGameV = (Forge_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var ForgeList = await ForgeInstaller.GetForgeBuildsOfVersionAsync(selectedGameV);
                Forge_game_verBox_Copy.ItemsSource = ForgeList;
            }

        }

        private async void ForgeInstall_start_Click(object sender, RoutedEventArgs e)
        {
            ForgeInstallProgress.Value = 0;
            if (Forge_game_verBox_Copy.Text != "" && Forge_game_verBox.Text != "")
            {
                string version = Forge_game_verBox.Text;
                string Forge_version = Forge_game_verBox_Copy.Text;
                var javaList = JavaToolkit.GetJavas();
                var v = new GameCoreToolkit(gameFolder);
                MinecraftLaunch.Modules.Models.Install.ForgeInstallEntity Forge = Forge_game_verBox_Copy.SelectedItem as MinecraftLaunch.Modules.Models.Install.ForgeInstallEntity;
                List<string> javas = new List<string>();
                foreach (var a in javaList)
                {
                    javas.Add(a.JavaPath);
                }
                await Task.Run(async () =>
                {
                    var Forge_installer = new ForgeInstaller(v, Forge, javas[0]);
                    await Forge_installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ForgeDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            ForgeDownLoadLog.ScrollToEnd();
                            ForgeInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                var setting = Application.Current.FindResource("CustomNoticeSetting") as NoticeBoxSetting;
                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true, setting);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();

            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }

        private async void fabric_game_verBox_Copy_DropDownOpened(object sender, EventArgs e)
        {
            if (fabric_game_verBox.Text != "" && fabric_game_verBox_Copy.Text == "")
            {
                fabric_game_verBox_Copy.ItemsSource = await FabricInstaller.GetFabricBuildsByVersionAsync(fabric_game_verBox.Text);
            }
        }

        private async void Quilt_game_verBox_Copy_DropDownOpened(object sender, EventArgs e)
        {
            if (Quilt_game_verBox.Text != "" && Quilt_game_verBox_Copy.Text == "")
            {
                Quilt_game_verBox_Copy.ItemsSource = await QuiltInstaller.GetQuiltBuildsByVersionAsync(Quilt_game_verBox.Text);
            }
        }

        private async void Quilt_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (Quilt_game_verBox.Text != "")
            {
                string selectedGameV = (Quilt_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var QuiltList = await QuiltInstaller.GetQuiltBuildsByVersionAsync(selectedGameV);
                Quilt_game_verBox_Copy.ItemsSource = QuiltList;
            }
        }

        private async void QuiltInstall_start_Click(object sender, RoutedEventArgs e)
        {
            if (Quilt_game_verBox.Text != "" && Quilt_game_verBox_Copy.Text != "")
            {
                QuiltInstallProgress.Value = 0;
                var QuiltBuild = Quilt_game_verBox_Copy.SelectedItem as QuiltInstallBuild;
                var v = new GameCoreToolkit(gameFolder);
                var fi = new QuiltInstaller(v, QuiltBuild);
                await Task.Run(async () =>
                {
                    await fi.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            QuiltDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            QuiltDownLoadLog.ScrollToEnd();
                            QuiltInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                var setting = Application.Current.FindResource("CustomNoticeSetting") as NoticeBoxSetting;
                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true, setting);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private async void ModSearch_Click(object sender, RoutedEventArgs e)
        {
            ModGrid.ItemsSource = null;
            ModSpin.IsSpinning = true;
            ModLoadingText.Visibility = Visibility.Visible;
            if (ModInput.Text != "")
            {
                var res = await ModToolkit.SearchModpacksAsync(ModInput.Text);
                List<Mod> mods = new();
                res.ForEach(x =>
                {
                    Mod modItem = new();
                    modItem.Description = x.Description;
                    modItem.image = new(new System.Uri(x.IconUrl));
                    modItem.Name = x.Name;
                    modItem.Version = $"{x.SupportedVersions.Last()}-{x.SupportedVersions[0]}";
                    modItem.Files = x.Files;
                    mods.Add(modItem);
                });
                ModGrid.ItemsSource = mods;
            }
            else
            {
                var res = await ModToolkit.GetFeaturedModpacksAsync();
                List<Mod> mods = new();
                res.ForEach(x =>
                {
                    Mod modItem = new();
                    modItem.Description = x.Description;
                    modItem.image = new(new System.Uri(x.IconUrl));
                    modItem.Name = x.Name;
                    modItem.Version = $"{x.SupportedVersions.Last()}-{x.SupportedVersions[0]}";
                    modItem.Files = x.Files;
                    mods.Add(modItem);
                });
                ModGrid.ItemsSource = mods;
            }
            ModSpin.IsSpinning = false;
            ModLoadingText.Visibility = Visibility.Hidden;
        }
        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Page.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    GetMcVersionList();
                    break;
                case 2:
                    GetMcVersions();
                    break;
            }
        }

        private void ModGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ModGrid.SelectedItem as Mod;
            modName = item.Name;
            ModGrid.Visibility = Visibility.Hidden;
            ModPage.Visibility = Visibility.Visible;
            ModDownLoad_Back.Visibility = Visibility.Visible;
            ModInfoImage.Source = item.image;
            ModDescribe.Text = item.Description;
            ModName.Text = item.Name;
            var items = item.Files;
            foreach (var file in items)
            {
                if (file.Value[0].SupportedVersion == null)
                {
                    items.Remove(file.Key);
                }
            }
            ModDownloadFile.Items.Clear();
            foreach (var file in items)
            {
                int item_count = file.Value.Count;
                for (int temp = 0; temp < item_count; temp++)
                {
                    CurseForgeModpackFileInfo modinfo = new();
                    modinfo.FileId = file.Value[temp].FileId;
                    modinfo.DownloadUrl = file.Value[temp].DownloadUrl;
                    modinfo.SupportedVersion = file.Value[temp].SupportedVersion;
                    modinfo.FileName = file.Value[temp].FileName;
                    modinfo.ModLoaderType = file.Value[temp].ModLoaderType;
                    ModDownloadFile.Items.Add(modinfo);
                }
            }
        }

        private void ModDownLoad_Back_Click(object sender, RoutedEventArgs e)
        {
            ModGrid.Visibility = Visibility.Visible;
            ModPage.Visibility = Visibility.Hidden;
            ModDownLoad_Back.Visibility = Visibility.Hidden;
        }

        private void Mod_ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            string sPath = AppDomain.CurrentDomain.BaseDirectory + "MCL\\DownLoads\\Mods";
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            dlg.InitialDirectory = sPath;
            string mod_saveFolder;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                mod_saveFolder = dlg.FileName;
                Mod_SaveFolderText.Text = mod_saveFolder;
            }
        }

        private async void ModDownloadFile_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mod = ModDownloadFile.SelectedItem as CurseForgeModpackFileInfo;
            if (Mod_SaveFolderText.Text != "")
            {
                ModDownloadFile.Visibility = Visibility.Hidden;
                await HttpToolkit.HttpDownloadAsync(mod.DownloadUrl, Mod_SaveFolderText.Text);
                var handler = NoticeBox.Show($"{modName}下载完成！", "MCL启动器", MessageBoxIcon.Success, true);
                ModDownloadFile.Visibility = Visibility.Visible;
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show("未设置保存目录！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }

        }
        private void GetMcVersions()
        {
            var core = new GameCoreToolkit(gameFolder);
            var mcVersions = core.GetGameCores();
            List<MinecraftVersion> YouXiBanBen = new List<MinecraftVersion>();
            foreach (var mcVersion in mcVersions)
            {
                MinecraftVersion minecraftVersion = new MinecraftVersion();
                minecraftVersion.Id = mcVersion.Id;
                var ModLoaderInfomation = mcVersion.ModLoaderInfos.ToList();
                switch (mcVersion.HasModLoader)
                {
                    case true:
                        minecraftVersion.Description = $"继承自{mcVersion.InheritsFrom}，{ModLoaderInfomation[0].ModLoaderType}客户端";
                        switch (ModLoaderInfomation[0].ModLoaderType)
                        {
                            case ModLoaderType.Any:
                                minecraftVersion.bitmapImage = new(new Uri("/Resources/images/normal.png"));
                                break;
                        }
                        break;
                    case false:
                        switch (mcVersion.Type)
                        {
                            case "release":
                                minecraftVersion.Description = $"正式版 {mcVersion.Id}";
                                break;
                            case "snapshot":
                                minecraftVersion.Description = $"快照版 {mcVersion.Id}";
                                break;
                            case "old_alpha":
                                minecraftVersion.Description = $"远古Alpha测试版 {mcVersion.Id}";
                                break;
                            case "old_beta":
                                minecraftVersion.Description = $"远古Beta测试版 {mcVersion.Id}";
                                break;
                        }
                        break;
                }                
                YouXiBanBen.Add(minecraftVersion);
            }
            GameVersionData.ItemsSource = YouXiBanBen;
        }

        private async void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSpin.IsSpinning = true;
            UpdateTextblock.Visibility = Visibility.Visible;
            UpdateD.Update updated = new UpdateD.Update();
            var HaveHigherVersion = updated.GetUpdate("4386F97F6C36488887EBA723C4C99C83", LauncherVersion);
            UpdateSpin.IsSpinning = false;
            UpdateTextblock.Visibility = Visibility.Hidden;
            if(HaveHigherVersion == true)
            {
                var DownLoadLink = updated.GetUpdateFile("4386F97F6C36488887EBA723C4C99C83"); 
                var GengXinNeiRong = updated.GetUpdateRem("4386F97F6C36488887EBA723C4C99C83");
                var setting = Application.Current.FindResource("UpdateMessage") as MessageBoxXSetting;
                var WantToUpdate = MessageBoxX.Show(null,$"有新版本可更新！\n更新内容:\n{GengXinNeiRong}","MCL启动器",MessageBoxButton.OKCancel,MessageBoxIcon.Info,setting);
                if(WantToUpdate == MessageBoxResult.OK)
                {
                    Process.Start(new ProcessStartInfo(DownLoadLink)
                    {
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                }                
            }
            else
            {
                var handler = NoticeBox.Show("启动器已是最新版本", "MCL启动器", MessageBoxIcon.Success, true);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
        }

        private void XieYiButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://docs.qq.com/doc/DUklrU3VJTm92SnFG")
            {
                UseShellExecute = true,
                CreateNoWindow = true
            });
        }
    }
}
