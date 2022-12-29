using Panuon.UI.Silver;
using System.Windows;
using System.Windows.Controls;
using static MCL_Dev.LauncherClasses;
using MinecaftOAuth;
using System.Linq;
using System.IO;
using MinecraftLaunch.Modules.Models.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MCL_Dev
{
    /// <summary>
    /// WaiZhi.xaml 的交互逻辑
    /// </summary>
    public partial class WaiZhi : Page
    {
        public WaiZhi()
        {
            InitializeComponent();
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if (email.Text != "" && passwd.Password != "")
            {
                try
                {
                    waizhi_email = email.Text;
                    waizhi_password = passwd.Password;
                    MinecaftOAuth.YggdrasilAuthenticator auth = new(true, waizhi_email, waizhi_password);
                    IList<YggdrasilAccount>? result = new List<YggdrasilAccount>();
                    await Task.Run(() =>
                    {
                        result = auth.AuthAsync(x => { }).ToList();                        
                        int a = result.Count();
                        for (int i = 0; i < a; i++)
                        {
                            players.Items.Add(result[i]);
                        }
                        MessageBoxX.Show("已完成登录", "MCL启动器");
                    });
                }
                catch
                {
                    MessageBoxX.Show("登录失败！\n请检查密码是否输入正确", "MCL启动器");
                }

                /*
                #region 保存RefreshToken
                DirectoryInfo dirInfo = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "MCL\\waizhi");//查看Debug文件夹的信息
                bool file = dirInfo.Exists;
                if (file == true)//有文件，直接写入
                {
                    for(int i = 0; i < a; i++)//依次写入各个角色的AccessToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt").Close();
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt", result[i].AccessToken);
                    }
                    for (int i = 0; i < a; i++)//依次写入各个角色的ClientToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt").Close();
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt", result[i].ClientToken);
                    }
                }
                else//无文件，创建文件夹后写入
                {
                    dirInfo.Create();
                    for (int i = 0; i < a; i++)//依次写入各个角色的AccessToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt").Close();
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Access_{result[i].Name}.txt", result[i].AccessToken);                    
                    }
                    for (int i = 0; i < a; i++)//依次写入各个角色的ClientToken
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt").Close() ;
                        File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"MCL\\waizhi\\WaiZhi_Client_{result[i].Name}.txt", result[i].ClientToken);                       
                    }
                }
                #endregion
                */

            }
            else
            {
                MessageBoxX.Show("信息未填写完整！", "MCL启动器");
            };
        }

        private void players_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            waizhi_selectedplayer = players.SelectedIndex;
            waizhi_selectedUsr = players.Text;
        }
    }
}
