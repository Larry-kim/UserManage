using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace UserManage
{
    /// <summary>
    /// DeleteMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeleteMain : Window
    {
        private string connectionString = " Data Source=.; Initial Catalog=UserManage; Integrated Security=True; "; // 데이터베이스 연결 문자열

        public DeleteMain()
        {
            InitializeComponent();
            LoadData(); // 데이터 로드
        }

        // 연락처 하이픈 자동 추가 함수
        private void txtPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text.Replace("-", ""); // 현재 텍스트 박스의 내용에서 하이픈 제거

            // 전화번호 형식에 맞게 하이픈 추가
            if (text.Length >= 4 && text.Length <= 7)
            {
                textBox.Text = $"{text.Substring(0, 3)}-{text.Substring(3)}";
            }
            else if (text.Length >= 8 && text.Length <= 13)
            {
                textBox.Text = $"{text.Substring(0, 3)}-{text.Substring(3, 4)}-{text.Substring(7)}";
            }

            // 커서 위치를 마지막으로 이동
            textBox.CaretIndex = textBox.Text.Length;
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Member_Table"; // 데이터베이스에서 가져올 쿼리
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    
                    dataGridView.ItemsSource = dataTable.DefaultView; // dataGridView에 데이터 바인딩
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터를 로드하는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string phoneNumber = PhoneNumberTextBox.Text.Trim();

            try
            {
                string query = "SELECT * FROM Member_Table WHERE 1=1";

                if (!string.IsNullOrEmpty(name))
                {
                    query += " AND 이름 LIKE @Name";
                }

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    query += " AND 전화번호 LIKE @PhoneNumber";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    if (!string.IsNullOrEmpty(name))
                    {
                        command.Parameters.AddWithValue("@Name", "%" + name + "%");
                    }

                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", "%" + phoneNumber + "%");
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching members: {ex.Message}");
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView selectedMember = (DataRowView)dataGridView.SelectedItem;
                if (selectedMember == null)
                {
                    MessageBox.Show("삭제할 회원을 선택하세요.");
                    return;
                }

                string memberName = selectedMember["이름"].ToString();

                string query = "DELETE FROM Member_Table WHERE 이름 = @MemberName";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MemberName", memberName);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("회원 삭제가 완료되었습니다.");
                        LoadMembers(); // 회원 목록 새로고침
                    }
                    else
                    {
                        MessageBox.Show("선택한 회원을 삭제하는 중에 오류가 발생했습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMembers();
        }

        private void LoadMembers()
        {
            try
            {
                string query = "SELECT * FROM member_Table";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading members: {ex.Message}");
            }
        }

    }
}
