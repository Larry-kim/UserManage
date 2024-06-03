using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
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
        // DB를 연결하기 위한 연결 문자열
        private string connectionString = " Data Source=.; Initial Catalog=UserManage; Integrated Security=True; "; // 데이터베이스 연결 문자열 설정
        private SecureString securePassword;

        public Login()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            securePassword = passwordBox.SecurePassword;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string userID = idBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(userID))
            {
                MessageBox.Show("아이디를 입력해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (securePassword == null || securePassword.Length == 0)
            {
                MessageBox.Show("비밀번호를 입력해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string password = ConvertToUnsecureString(securePassword);
            // MessageBox.Show($"사용자 입력 비밀번호: {password}");  // 디버깅을 위해 비밀번호 출력

            if (AuthenticateUser(userID, password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호가 올바르지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AuthenticateUser(string userID, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Password FROM User_Table WHERE User_ID = @ID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", userID);

                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("해당 사용자 ID가 데이터베이스에 존재하지 않습니다.");
                        return false;
                    }

                    string storedHashedPassword = result as string;
                    // MessageBox.Show($"저장된 해싱 비밀번호: {storedHashedPassword}");  // 디버깅을 위해 저장된 해싱 비밀번호 출력

                    string hashedPassword = HashPassword(password);
                    // MessageBox.Show($"입력한 비밀번호의 해싱 값: {hashedPassword}");  // 디버깅을 위해 입력한 비밀번호의 해싱 값 출력

                    return storedHashedPassword != null && storedHashedPassword == hashedPassword;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류가 발생했습니다: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null)
                return string.Empty;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Member member = new Member();
            member.Show();

            // this.Close();
        }
    }
}
