using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (idBox.Text == "user" && pswBox.Password.Equals("user")) 
            {
                // 새로운 창(MainWindow) 열기
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                // 현재 창 닫기
                this.Close();
            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호를 잘못 입력했습니다.");
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Member member = new Member();
            member.Show();

            this.Close();
        }
    }
}
