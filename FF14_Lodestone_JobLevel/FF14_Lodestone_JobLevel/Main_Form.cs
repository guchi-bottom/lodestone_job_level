using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FF14_Lodestone_JobLevel
{
    public partial class Main_Form : Form
    {
        public Main_Form()
        {
            // コンポーネント初期化するよ(触んな)
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 保存値復元(テスト中はしなくていいよ)
            //Lodestone_Chara_ID = Properties.Settings.Default.Chara_ID;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Properties.Settings.Default.Chara_ID = Lodestone_Chara_ID;
            Properties.Settings.Default.Save();
        }

        private string[] HTML_Catch(string URL)
        {
            string[] return_string = new string[2];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            HttpWebResponse res = null;
            HttpStatusCode statusCode;
            try
            {
                res = (HttpWebResponse)req.GetResponse();
                statusCode = res.StatusCode;
                int code = (int)statusCode;
                MessageBox.Show(code.ToString(), "HTTP STATUS");
                var resSt = res.GetResponseStream();
                var sr = new StreamReader(resSt, Encoding.UTF8);
                return_string[0] = sr.ReadToEnd();
                return_string[1] = code.ToString();
                return return_string;

            }
            catch (WebException ex)
            {

                res = (HttpWebResponse)ex.Response;

                if (res != null)
                {
                    statusCode = res.StatusCode;
                    int code = (int)statusCode;
                    return_string[1] = code.ToString();
                    return return_string;
                }
                else
                {
                    throw; // サーバ接続不可などの場合は再スロー
                }
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            label1.Visible = true;         // 可視化
            label1.Text = "取得中";       // 「取得中」の文字列を表示することで処理中であることを明記します。
            label1.BringToFront();         // Objectを最善面に移動します。
            label1.Update();               // 表示を更新します。


            // 画面上からHTMLを取得するサイトのURLを取得します。
            string url = "https://jp.finalfantasyxiv.com/lodestone/character/35075529/class_job/";

            // htmlを取得するメソッドを実行し、画面描画します。
            string[] html = HTML_Catch(url);
            textBox1.Text = html[0];
            textBox1.Update();

            label1.Text = "処理中";

            //下処理を開始します。
            html[0] = html[0].Replace("	", "");
            textBox1.Text = html[0]; textBox1.Update();
            html[0] = html[0].Replace("\n", "\r\n");
            textBox1.Text = html[0]; textBox1.Update();
            for (; ; )
            {
                if (html[0].Contains("\r\n\r\n"))
                    html[0] = html[0].Replace("\r\n\r\n", "\r\n");
                else
                    break;
            }
            textBox1.Text = html[0]; textBox1.Update();
            for (; ; )
            {
                if (html[0].Contains("\"\r\n"))
                    html[0] = html[0].Replace("\"\r\n", "\"");
                else
                    break;
            }
            textBox1.Text = html[0]; textBox1.Update();

            //検索を開始します。
            List<string> Job_level = new List<string>();
            List<string> Job_name = new List<string>();
            List<string> Job_exp = new List<string>();
            int start_index = 0;
            int end_index = 0;
            string extraction = "";

            for (; ; )
            {
                if (0 < html[0].IndexOf("<div class=\"character__job__level\">", start_index))
                {
                    start_index = html[0].IndexOf("<div class=\"character__job__level\">", start_index);
                    end_index = html[0].IndexOf("</div>", start_index);
                    extraction = html[0][start_index..end_index];
                    Job_level.Add(extraction);
                    start_index = end_index;
                }
                else break;
            }
            
            List<int> indexs = new List<int>();
            start_index = 0;
            for (; ; )
            {
                if (0 < html[0].IndexOf("<div class=\"character__job__name js__tooltip\"", start_index))
                {

                    start_index = html[0].IndexOf("<div class=\"character__job__name js__tooltip\"", start_index);
                    end_index = html[0].IndexOf("</div>", start_index);
                    extraction = html[0][start_index..end_index];
                    Job_name.Add(extraction);
                    indexs.Add(start_index);
                    start_index = end_index;
                }
                else break;
            }

            start_index = 0;
            for (; ; )
            {
                if (0 < html[0].IndexOf("<div class=\"character__job__name character__job__name--meister js__tooltip\"", start_index))
                {

                    start_index = html[0].IndexOf("<div class=\"character__job__name character__job__name--meister js__tooltip\"", start_index);
                    end_index = html[0].IndexOf("</div>", start_index);
                    extraction = html[0][start_index..end_index];
                    Job_name.Add(extraction);
                    indexs.Add(start_index);
                    start_index = end_index;
                }
                else break;
            }

            start_index = 0;
            for (; ; )
            {
                if (0 < html[0].IndexOf("<div class=\"character__job__name\"", start_index))
                {

                    start_index = html[0].IndexOf("<div class=\"character__job__name\"", start_index);
                    end_index = html[0].IndexOf("</div>", start_index);
                    extraction = html[0][start_index..end_index];
                    Job_name.Add(extraction);
                    indexs.Add(start_index);
                    start_index = end_index;
                }
                else break;
            }

            for(int i = 0; i < indexs.Count - 1; i++)
            {
                if(indexs[i] > indexs[i + 1])
                {
                    int temp = indexs[i];
                    indexs[i] = indexs[i + 1];
                    indexs[i + 1] = temp;

                    string Temp = Job_name[i];
                    Job_name[i] = Job_name[i + 1];
                    Job_name[i + 1] = Temp;

                    i = -1;
                }
            }

            start_index = 0;
            for (; ; )
            {
                if (0 < html[0].IndexOf("<div class=\"character__job__exp\">", start_index))
                {
                    start_index = html[0].IndexOf("<div class=\"character__job__exp\">", start_index);
                    end_index = html[0].IndexOf("</div>", start_index);
                    extraction = html[0][start_index..end_index];
                    Job_exp.Add(extraction);
                    start_index = end_index;
                }
                else break;
            }

            for (int i = 0; i < Job_level.Count; i++)
                Job_level[i] = Job_level[i][(Job_level[i].LastIndexOf(">") + 1)..];

            for (int i = 0; i < Job_name.Count; i++)
                Job_name[i] = Job_name[i][(Job_name[i].LastIndexOf(">") + 1)..];

            for (int i = 0; i < Job_exp.Count; i++)
                Job_exp[i] = Job_exp[i][(Job_exp[i].LastIndexOf(">") + 1)..];

            int counting = Job_level.Count;
            if (counting > Job_name.Count) counting = Job_name.Count;
            if (counting > Job_exp.Count) counting = Job_exp.Count;

            label1.Text = "表示中";

            textBox1.Text += Environment.NewLine;


            textBox1.Text = "";

            for (int i = 0; i < counting; i++)
            {
                textBox1.Text = textBox1.Text + Job_name[i] + " : Lv." + Job_level[i] + "(" + Job_exp[i] + ")" + Environment.NewLine;
            }

            label1.Visible = false;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}