using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserManage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            groupBox1.Visibility = Visibility.Visible;
            groupBox2.Visibility = Visibility.Hidden;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            groupBox1.Visibility = Visibility.Hidden;
            groupBox2.Visibility = Visibility.Visible;
        }

    }
}