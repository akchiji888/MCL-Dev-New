using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MinecraftLaunch.Modules.Models.Auth;
using MinecaftOAuth;
using Panuon.WPF.UI;
using static MCL_Dev.LauncherClasses;
using static MCL_Dev.MainWindow;
using System;

namespace MCL_Dev.Resources
{
    /// <summary>
    /// AddAccount.xaml 的交互逻辑
    /// </summary>
    public partial class AddAccount : Window
    {
        MainWindow mainWindow = new();
        public AddAccount()
        {
            InitializeComponent();
        }
        #region 登录UI
        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LoginTab.SelectedIndex)
            {
                case 0://offline
                    break;
                case 1:
                    break;
                case 2:
                    var message = MessageBoxX.Show(mainWindow, "确认开始验证微软账户", "MCL启动器", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
                    if (message == MessageBoxResult.OK)
                    {
                        spin.IsSpinning = true;
                        textBlock.Text = "正在登录中，请稍后";
                        mode = 1;
                        #region 登录                        
                        var auth = new MicrosoftAuthenticator();
                        auth.ClientId = "dd09ec86-031b-4429-adb8-7fec6bc1fd79";
                        auth.AuthType = MinecaftOAuth.Module.Enum.AuthType.Access;
                        var code_1 = await auth.GetDeviceInfo();
                        string usrCode = code_1.UserCode;
                        Clipboard.SetDataObject(usrCode);
                        MessageBoxX.Show(mainWindow, usrCode + "\n已复制该串字符到剪切板，请粘贴这串字符到弹出的网页中进行登录", "MCL启动器", MessageBoxIcon.Info);
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
                            var result = await auth.AuthAsync(x => { });
                            textBlock.Text = "用户名：" + result.Name;
                            spin.IsSpinning = false;
                            var tempAccount = result as Account;
                            if (gameAccountsList.Exists(t => t == result) == false)
                            {
                                gameAccountsList.Add(result);
                            }
                            MessageBoxX.Show(this, "已完成登录！你可以关闭此窗口了", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Success);
                        }
                        catch
                        {
                            mode = 114514;
                            MessageBoxX.Show(mainWindow, "微软登录失败！\n错误原因可能有:1.你没购买Minecraft\n2.微软的验证服务器炸了\n3.你没联网", "MCL启动器", MessageBoxIcon.Error);
                        }
                        #endregion
                    }
                    break;
            }
        }
        private async void Waizhi_start_Click(object sender, RoutedEventArgs e)
        {
            if (email.Text != "" && passwd.Password != "" && serverUri.Text != "")
            {
                try
                {
                    LauncherClasses.YggdrasilAuthenticator ya = new(serverUri.Text, email.Text, passwd.Password);
                    var res = await ya.AuthAsync(X => { });
                    foreach (var yaccount in res)
                    {
                        if (gameAccountsList.Exists(t => t == yaccount) == false)
                        {
                            gameAccountsList.Add(yaccount);
                        }
                    }
                    MessageBoxX.Show(mainWindow, "已完成登录，你可以关闭此窗口了！", "MCL启动器", MessageBoxIcon.Success);
                }
                catch
                {
                    MessageBoxX.Show(mainWindow, "登录失败！\n请检查密码是否输入正确", "MCL启动器", MessageBoxIcon.Error);
                }



            }
            else
            {
                MessageBoxX.Show(mainWindow, "信息未填写完整！", "MCL启动器", MessageBoxIcon.Error);
            };
        }
        #endregion
        private void OfflineButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text.Length <= 16 && NameBox.Text.Length >= 3)
            {
                OfflineAuthenticator oa = new(NameBox.Text);
                var offlinea = oa.Auth();
                if(gameAccountsList.Exists(t => t == offlinea) == false)
                { 
                    gameAccountsList.Add(offlinea);
                }
                MessageBoxX.Show(this, "创建完成！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Success);
            }
            else
            {
                MessageBoxX.Show(this, "用户名字数应为3~16个字符！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = MessageBoxX.Show(mainWindow, "确认退出吗？", "MCL启动器", MessageBoxButton.YesNo, MessageBoxIcon.Warning);
            if (res == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
