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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Document doc = NSoupClient.Parse(new Uri("https://docs.google.com/spreadsheets/d/e/2PACX-1vQlA-OAzCK7SEBwW_BdCGUH8R60VaAjfgkSGzzlwRypRHhVsftawVn3_yicQg4YDgg4pHFC4RguOa2S/pubhtml?gid=994251507&single=true"), 5000);
            Elements datas = doc.Select("tbody tr");

            int i = 0;
            string num, name, url, vrcurl = "", songname = "", request1 = "", request2 = "";

            foreach (Element data in datas)
            {
                i++;
                if (i < 3) continue;
                num = data.Select("td").Eq(0).Text;
                url = data.Select("td").Eq(1).Text;
                name = data.Select("td").Eq(2).Text;
                if (num != "")
                {
                    vrcurl+= $"VRCUrl n{num} = new VRCUrl(\"{url}\");\n" +
                        $"VRCUrl q{num} = new VRCUrl(\"https://t-ne.x0.to/?url={url}\");\n";
                    request1 += $"case \"{num}\":\n" +
                    $"if (_quest) addURL(q{num});\n" +
                    $"else addURL(n{num});\n" +
                    $"Debug.Log($\"에케 디버그: Request: {num}\");\n" +
                    $"break;\n";
                    /*
                    request2 += $"\t\t\t\t\tcase \"{num}\":" +
                        $"\n\t\t\t\t\t\twolfePlayerController.AppendToQueue(n{num});" +
                        $"\n\t\t\t\t\t\tbreak;\n";*/
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
            Result.Text = $"#region 노래등록\n{TextBox_VRCUrl.Text}\n{TextBox_Request.Text}\n#endregion";
        }
    }
}
