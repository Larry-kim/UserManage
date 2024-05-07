using System.Collections.ObjectModel;
using System.Configuration;
using System.Data; // DB를 사용하기 위해 추가
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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

        // TextBox에 정수만 입력 받고 싶을때 사용
        private void VerPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 정수만 입력 받고 싶을때
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            TextBox textBox = (TextBox)sender;
        }

        // 텍스트박스에 통화단위(3자리 마다 콤마) 표현하기
        private void txtCash_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // 이벤트 핸들러를 일시적으로 해제하여 무한루프를 방지합니다.
                txtCash.TextChanged -= txtCash_TextChanged;

                // 사용자가 입력한 숫자를 가져옴
                string input = txtCash.Text;

                // 입력된 문자열에서 숫자만 남기고 모두 제거
                string numbersOnly = Regex.Replace(input, @"[^\d]", "");

                // 콤마를 포함하여 숫자를 표시
                if (!string.IsNullOrEmpty(numbersOnly))
                {
                    long number = long.Parse(numbersOnly);
                    string formattedNumber = string.Format("{0:#,##0}", number);
                    txtCash.Text = formattedNumber;
                }

                // 커서 위치를 마지막으로 이동
                txtCash.SelectionStart = txtCash.Text.Length;
            }
            catch (FormatException)
            {
                // 숫자로 변환할 수 없는 경우, 사용자에게 메시지를 표시하거나 다른 작업을 수행할 수 있습니다.
                MessageBox.Show("숫자 형식으로 입력하세요.");
            }
            finally
            {
                // 이벤트 핸들러를 다시 연결합니다.
                txtCash.TextChanged += txtCash_TextChanged;
            }
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

        // DB를 연결하기 위한 연결 문자열
        private string connectionString = " Data Source=.; Initial Catalog=UserManage; Integrated Security=True "; // 데이터베이스 연결 문자열 설정


        // 회원 정보 추가란으로 변경하기 버튼
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            groupBox1.Visibility = Visibility.Visible;
            groupBox2.Visibility = Visibility.Hidden;
        }

        // 회원 정보 추가 버튼
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 선택된 날짜 가져오기
                DateTime? selectedDate = datePicker.SelectedDate; // 회원 등록일자

                // 선택된 콤보 박스 항목 가져오기
                ComboBoxItem selectItem = (ComboBoxItem)txtLineUp.SelectedItem;
                string lineup = selectItem.Content.ToString(); // 회원권

                // 선택된 라디오 버튼에 따라 성별을 결정
                string gender = radMale.IsChecked == true ? "남성" : "여성"; // 회원 성별

                // 입력된 회원 정보 가져오기
                string name = txtName.Text; // 회원 이름
                string phone_number = txtPhoneNumber.Text; // 회원 전화번호
                string birth_date = txtBirthDate.Text; // 회원 생년월일
                string reason = txtReason.Text; // 회원 등록사유
                string cash = txtCash.Text; // 회원권 금액

                // SQL 쿼리 생성
                string query = "INSERT INTO member_Table VALUES (@Name, @BirthDate, @PhoneNumber, @Gender, @Reason, @SelectedDate, @Lineup, @Cash)";

                // SqlConnection 및 SqlCommand를 사용하여 데이터베이스에 연결 및 쿼리 실행
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@BirthDate", birth_date);
                    command.Parameters.AddWithValue("@PhoneNumber", phone_number);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@Reason", reason);
                    command.Parameters.AddWithValue("@SelectedDate", selectedDate);
                    command.Parameters.AddWithValue("@Lineup", lineup);
                    command.Parameters.AddWithValue("@Cash", cash);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("회원 추가가 완료되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"회원 추가 중 오류가 발생했습니다: {ex.Message}");
            }

        }

        // 회원 데이터 조회 버튼
        private void btnInquiry_Click(object sender, RoutedEventArgs e)
        {
            groupBox1.Visibility = Visibility.Hidden;
            groupBox2.Visibility = Visibility.Visible;
            
            try
            {
                string query = "SELECT * FROM member_Table"; // 가져올 데이터의 SQL 쿼리

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command); // DataGridView에 데이터를 표시하기 위해 SqlDataAdpter와 DataSer 사용
                    DataTable dataTable = new DataTable();
                    
                    adapter.Fill(dataTable);
                    dataGridView.ItemsSource = dataTable.DefaultView; // dataGridView에 데이터 바인딩
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"회원 정보를 불러오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnTrainer_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}