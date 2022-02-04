using NSoup;
using NSoup.Nodes;
using NSoup.Select;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EKE_karaoke
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            string year = DateTime.Now.ToString("yyyy");
            int i = Convert.ToInt32(year);

            for (; i >= 2012; i--)
            {
                comboBox1.Items.Add(i.ToString());
            }

            for (i = 12; i >= 1; i--)
            {
                comboBox2.Items.Add(i.ToString("00"));
            }
            comboBox1.Text = year;
            comboBox2.Text = DateTime.Now.ToString("MM");
        }

        List<string> list = new List<string>();

        private void button1_Click(object sender, EventArgs e)
        {
            list.Clear();

            Document doc = NSoupClient.Parse(new Uri("https://docs.google.com/spreadsheets/d/e/2PACX-1vQlA-OAzCK7SEBwW_BdCGUH8R60VaAjfgkSGzzlwRypRHhVsftawVn3_yicQg4YDgg4pHFC4RguOa2S/pubhtml?gid=994251507&single=true"), 5000);
            Elements datas = doc.Select("tbody tr");

            int i = 0;
            string num, name, singer, url, vrcurl = String.Empty, songname = String.Empty, request1 = String.Empty, request2 = String.Empty;

            foreach (Element data in datas)
            {
                i++;
                if (i < 3) continue;
                num = data.Select("td").Eq(0).Text;
                list.Add(num);
                url = data.Select("td").Eq(1).Text;
                name = data.Select("td").Eq(2).Text;
                singer = data.Select("td").Eq(3).Text;

                if (num != "")
                {
                    vrcurl += $"VRCUrl n{num} = new VRCUrl(\"{url}\");\n" +
                        $"VRCUrl q{num} = new VRCUrl(\"https://t-ne.x0.to/?url={url}\");\n";
                    request1 += $"case \"{num}\":\n" +
                    $"if (_quest) addURL(q{num});\n" +
                    $"else addURL(n{num});\n" +
                    $"Debug.Log($\"에케 디버그: Request: {num} - {name}\");\n" +
                    $"break;\n";

                    url = url.Replace("https://t-ne.x0.to/?url=", "").Replace("https://www.youtube.com/watch?v=", "")
                        .Replace("https://youtu.be/", "").Replace("https://youtube.com/watch?v=", "");
                    url = url.Split('?')[0].Split('&')[0];
                    songname += $"case \"{url}\":\n" +
                        $"name = \"[{num}] {name.Replace("_", "")} - {singer}\";\n" +
                        $"break;\n";
                }
            }
            /*
                doc = NSoupClient.Parse(new Uri("https://docs.google.com/spreadsheets/d/e/2PACX-1vQlA-OAzCK7SEBwW_BdCGUH8R60VaAjfgkSGzzlwRypRHhVsftawVn3_yicQg4YDgg4pHFC4RguOa2S/pubhtml?gid=515940993&single=true"), 5000);
                datas = doc.Select("tbody tr");
                i = 0;
                foreach (Element data in datas)
                {
                    i++;
                    if (i < 3) continue;
                    num = data.Select("td").Eq(2).Text;
                    name = data.Select("td").Eq(0).Text;
                    if (num != "")
                    {
                        songname += $"\t\t\t\tcase {num}:" +
                            $"\n\t\t\t\t\treturn \"{name}\";\n";
                    }
                }

                {TextBox_Songname.Text}\n
            */
            TextBox_VRCUrl.Text = TextBox_VRCUrl.Text.Replace("문자열삽입위치", vrcurl);
            TextBox_Songname.Text = TextBox_Songname.Text.Replace("문자열삽입위치", songname);
            TextBox_Request.Text = TextBox_Request.Text.Replace("문자열삽입위치", request1);
            //TextBox_Request.Text = TextBox_Request.Text.Replace("문자열삽입위치2", request2);
            Result.Text = $"#region 노래등록\n{TextBox_VRCUrl.Text}\n{TextBox_Request.Text}\n{TextBox_Songname.Text}\n#endregion";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string days = String.Empty;

            //선택한 년월이 현재면 오늘 날짜를 마지막 날짜로 지정
            if (comboBox1.Text == DateTime.Now.ToString("yyyy") && comboBox2.Text == DateTime.Now.ToString("MM"))
            {
                days = DateTime.Now.ToString("dd");
            }
            else
            {
                days = DateTime.DaysInMonth(Convert.ToInt32(comboBox1.Text), Convert.ToInt32(comboBox2.Text)).ToString();
            }

            //한국어, 영어, 일본어 순으로 3번 반복
            for (int i = 1; i < 4; i++)
            {
                Document doc = NSoupClient.Parse(new Uri($"http://www.tjmedia.com/tjsong/song_monthPopular.asp?strType={i}&SYY={comboBox1.Text}&SMM={comboBox2.Text}&SDD=01&EYY={comboBox1.Text}&EMM={comboBox2.Text}&EDD={days}"), 5000);
                Elements datas = doc.Select("#BoardType1 > table > tbody > tr");

                int ii = -1;

                foreach (Element data in datas)
                {
                    ii++;
                    if (ii == 0) continue;

                    string num = data.Select("td").Eq(1).Text;
                    string name = data.Select("td").Eq(2).Text;
                    string singer = data.Select("td").Eq(3).Text;

                    //이미 등록된 노래면 넘기기, 아니면 등록
                    if (list.Contains(num)) continue;
                    list.Add(num);

                    string lang = String.Empty;
                    switch (i)
                    {
                        case 1:
                            lang = "한국어";
                            break;
                        case 2:
                            lang = "영어";
                            break;
                        case 3:
                            lang = "일본어";
                            break;
                        default:
                            break;
                    }
                    TJTextBox.AppendText($"{num}\t\t{name}\t{singer}\t{lang}\n" +
                                        $"0{num}\t\t_{name}\t{singer}\t{lang}\n");
                }
            }
        }
    }
}
