using System.Windows.Controls;
using static MCL_Dev.LauncherClasses;

namespace MCL_Dev
{
    /// <summary>
    /// Lixian.xaml 的交互逻辑
    /// </summary>
    public partial class Lixian : Page
    {
        public Lixian()
        {
            InitializeComponent();
        }

        private void start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            offlineName = NameCombo.Text;
            start.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
