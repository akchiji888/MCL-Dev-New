using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Panuon.UI.Silver;

namespace MCL_Dev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowX
    {
        public MainWindow()
        {
            InitializeComponent();
            test test = new test();

            ZhuYe zhuye = new ZhuYe();
            
            
            page.Content = new Frame()
            {
                Content = zhuye
            };
            Color color = (Color)ColorConverter.ConvertFromString("#FF0067FF");
            banner.Background = new SolidColorBrush(color);
            #region 主页版图颜色
            zhuye.start.BorderBrush = new SolidColorBrush(color);
            zhuye.start.Foreground = new SolidColorBrush(color);
            zhuye.versionCombo.Foreground = new SolidColorBrush(color);
            zhuye.gameVersion.Foreground = new SolidColorBrush(color);
            #endregion
            #region 设置主题色
            zhuye.javaBanBen.Foreground = new SolidColorBrush(color);
            zhuye.javaCombo.Foreground = new SolidColorBrush(color);
            zhuye.ZuiDaRAM.Foreground = new SolidColorBrush(color);
            zhuye.maxMem.Foreground = new SolidColorBrush(color);
            #endregion

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ZhuYe zhuye = new ZhuYe();
            page.Content = new Frame()
            {
                Content = zhuye
            };
        }
        private void Button_Click_setting(object sender, RoutedEventArgs e)
        {
            

        }
    }
}
