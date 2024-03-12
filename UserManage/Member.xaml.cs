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

namespace UserManage
{
    /// <summary>
    /// Member.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Member : Window
    {

        public Member()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string strCon = "Server=test;Database=usermanage_db;Uid=root";
            // MySql연결을 위한 connection 객체 생성
            // MySqlConnection 클래스 괄호 안의 내용은, 본인의 MySQL 서버 상황에 맞는 값을 삽입 요망
            MySqlConnection connection = new MySqlConnection(strCon);

            // 앞서 생성한 connection 객체를 통해 MySQL 서버 연결 유지
            connection.Open();

            // MySQL로 보내 ㄹ쿼리문인 문자열 Query 변수 선언
            // City 테이블에 txtbox_name 이름으로 검색했을 때, 나오는 결과값 개수를 cnt 변수로 세도록 한다.
            string Query = "SELECT *, COUNT(*) as cnt FROM usermanage_db WHERE id='" + txtbox_id.Text + "';";

            // MySQL로 쿼리문을 보내기 위해 command 객체 생성 후 쿼리문 전달
            MySqlCommand cmd = new MySqlCommand(Query, connection);

            // 받아온 결과값을 reader 객체에 저장
            MySqlDataReader reader = cmd.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    // 받아온 reader 객체의 값 중, cnt 칼럼의 값이 0이 아니라면
                    // -> cnt 칼럼의 값이 0이 아니란 의미는?
                    // ---> txtbox_id에 입력된 텍스트값이 이미 테이블에 중복되어 있다는 뜻
                    if (reader["cnt"].ToString()!= "0")
                    {
                        // 중복된 정보입니다. 메시지 박스 출력
                        MessageBox.Show("중복된 아이디입니다.");
                    }
                    // 받아온 reader 객체의 값 중, cnt 칼럼의 값이 0이라면
                    // -> cnt 칼럼의 값이 0이란 의미는?
                    // ---> txtbox_id에 입력된 텍스트값이 이미 테이블에 미등록되어 있다는 뜻
                    else
                    {
                        // 등록되지 않은 정보입니다. 메시지 박스 출력
                        MessageBox.Show("사용가능한 아이디입니다.");
                    }
                }
            }
            catch(Exception ex) 
            {
                // 오류 발생 시 오류로그 출력
                MessageBox.Show("오류로그 : " + ex.Message.ToString());
            }
            // MySQL 서버 연결 종료
            connection.Close();
        }
    }
}
