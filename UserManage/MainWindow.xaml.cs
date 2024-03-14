using MySql.Data.MySqlClient; // DB를 사용하기 위해 추가
using System.Configuration;
using System.Data; // DB를 사용하기 위해 추가
using System.Data.SqlClient;
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
        private MySqlConnection connection;

        public MainWindow()
        {
            InitializeComponent();
        }
        

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            groupBox1.Visibility = Visibility.Visible;
            groupBox2.Visibility = Visibility.Hidden;

            // App.config에서 Connection String을 읽어옵니다.
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            // SQL 쿼리를 작성합니다. 여기서는 간단히 예시로 삽입 쿼리를 작성합니다.
            string query = "INSERT INTO YourTableName (ColumnName1, ColumnName2) VALUES (@Value1, @Value2)";

            try
            {
                // SqlConnection을 사용하여 데이터베이스에 연결합니다.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SqlCommand를 사용하여 SQL 쿼리를 실행합니다.
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // 파라미터를 추가하여 SQL Injection 공격을 방지합니다.
                        command.Parameters.AddWithValue("@Value1", "Value1");
                        command.Parameters.AddWithValue("@Value2", "Value2");

                        // 데이터베이스 연결을 열고 SQL 쿼리를 실행합니다.
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        // 작업 결과에 따라 메시지를 표시합니다.
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("데이터가 성공적으로 삽입되었습니다.");
                        }
                        else
                        {
                            MessageBox.Show("데이터 삽입에 실패했습니다.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            groupBox1.Visibility = Visibility.Hidden;
            groupBox2.Visibility = Visibility.Visible;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}