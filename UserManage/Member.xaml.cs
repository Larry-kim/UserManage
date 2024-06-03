using MySql.Data.MySqlClient;
using System.Diagnostics;
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
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Reflection;
using System.Security.Policy;
using System.Xml.Linq;
using System.Globalization;

namespace UserManage
{
    /// <summary>
    /// Member.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public static class SecureStringHelper
    {
        public static string ConvertToUnsecureString(this System.Security.SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return System.Runtime.InteropServices.Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }

    public partial class Member : Window
    {
        private string password;
        private string confirmPassword;

        public Member()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            password = passwordBox.SecurePassword.ConvertToUnsecureString();
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox confirmPasswordBox = sender as PasswordBox;
            confirmPassword = confirmPasswordBox.SecurePassword.ConvertToUnsecureString();
        }

        // DB를 연결하기 위한 연결 문자열
        private string connectionString = " Data Source=.; Initial Catalog=UserManage; Integrated Security=True; "; // 데이터베이스 연결 문자열 설정

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtID.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM User_Table WHERE 이름 = @Username";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);

                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("이미 사용중인 아이디입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("사용 가능한 아이디입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("오류가 발생했습니다: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UserMake_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string id = txtID.Text.Trim();
                string hashedPassword = HashPassword(password);
                string name = txtName.Text;
                string birth_date = txtBirth.Text;
                string phone_number = txtPhoneNum.Text;
                string email = txtEmail.Text;

                // 비밀번호 확인
                if (password != confirmPassword)
                {
                    MessageBox.Show("비밀번호와 확인 비밀번호가 일치하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 생년월일 형식 검증
                if (!DateTime.TryParseExact(birth_date, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    MessageBox.Show("올바른 생년월일 형식을 입력하세요 (yyMMdd).", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 이메일 형식 검증
                if (!IsValidEmail(email))
                {
                    MessageBox.Show("올바른 이메일 주소를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL 쿼리 생성
                    string query = "INSERT INTO User_Table VALUES (@User_ID, @Password, @Name, @Birth_Date, @Phone_Number, @Email)";
                    SqlCommand command = new SqlCommand(query, connection);
                          
                    // 파라미터 추가
                    command.Parameters.AddWithValue("@User_ID", id);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Birth_Date", birth_date);
                    command.Parameters.AddWithValue("@Phone_Number", phone_number);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("회원가입이 완료되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류가 발생했습니다: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // 이메일 형식 검증 메서드
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
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

    }
}
