using System.Collections.ObjectModel;
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
    // ListView에 표시할 데이터 클래스 정의
    public class MyData
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone_Number { get; set; }
        public string Birth_Date { get; set; }
        
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public ObservableCollection<MyData> DataList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataList = new ObservableCollection<MyData>();
            listView1.ItemsSource = DataList; // ListView와 데이터 바인딩
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

            try
            {
                string connectionString = " Data Source=.; Initial Catalog=UserManage; Integrated Security=True "; // 데이터베이스 연결 문자열 설정
                string query = "SELECT ID, Password, Name, Email, Birth_Date, Phone_Number FROM TableDB"; // 가져올 데이터의 SQL 쿼리

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    DataList.Clear(); // 기존의 데이터를 모두 지웁니다.

                    while (reader.Read())
                    {
                        DataList.Add(new MyData { 
                            ID = reader["ID"].ToString(), 
                            Password = reader["Password"].ToString(),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Birth_Date = reader["Birth_Date"].ToString(),
                            Phone_Number = reader["Phone_Number"].ToString()
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터를 가져오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}