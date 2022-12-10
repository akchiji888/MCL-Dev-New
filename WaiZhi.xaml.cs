using Panuon.UI.Silver;
using System.Windows;
using System.Windows.Controls;
using static MCL_Dev.LauncherClasses;
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

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (email.Text!=null && apiUri.Text!=null && passwd.Password != null)
            {
                waizhi_email = email.Text;
                waizhi_apiUri = apiUri.Text;
                waizhi_password = passwd.Password;
            }
            else
            {
                MessageBoxX.Show("信息未填写完整！","MCL启动器");
            };
        }
    }
}
