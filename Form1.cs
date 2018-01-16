using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace KeddWFA
{
    public partial class Form1 : Form
    {
        List<string> images = new List<string>();
        int index = 0;
        private readonly List<string> _topics = new List<string> { "dog", "pussy", "boobs", "ass", "tits", "cat", "szar"};
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            index--;
            try
            {
                if (index < 0)
                {
                    index = images.Count - 1;
                    pictureBox1.Load(images[index]);
                }
                else
                {
                    pictureBox1.Load(images[index]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nincs több kép");
            }

        }

        private void next_Click(object sender, EventArgs e)
        {
            index++;
            try
            {
                if (index > images.Count - 1)
                {
                    index = 0;
                    pictureBox1.Load(images[index]);
                }
                else
                {
                    pictureBox1.Load(images[index]);
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Nincs több kép");
            }
        }
    
        private void open_Click(object sender, EventArgs e)
        {
            string[] files = null;
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    files = Directory.GetFiles(folderBrowserDialog.SelectedPath);
                    if (files != null)
                    {
                        foreach(string file in files)
                        {
                            if (file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".png") || file.EndsWith(".gif"))
                            {
                                images.Add(file);
                            }
                        }
                    }
                }
                pictureBox1.Load(images[index]);
            }
            catch
            {

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                /*Random random = new Random();
                int num = random.Next(0, images.Count - 1);
                pictureBox1.Load(images[num]);*/
                string html = GetHtmlCode();
                List<string> urls = GetUrls(html);
                var rnd = new Random();

                int randomUrl = rnd.Next(0, urls.Count - 1);

                string luckyUrl = urls[randomUrl];

                byte[] image = GetImage(luckyUrl);
                using (var ms = new MemoryStream(image))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nincs több kép");
            }

        }
        private string GetHtmlCode()
        {
            var rnd = new Random();

            int topic = rnd.Next(0, _topics.Count - 1);

            string url = "https://www.google.com/search?q=" + _topics[topic] + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        private List<string> GetUrls(string html)
        {
            var urls = new List<string>();

            int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

            while (ndx >= 0)
            {
                ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                ndx++;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
            }
            return urls;
        }

        private byte[] GetImage(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return null;
                using (var sr = new BinaryReader(dataStream))
                {
                    byte[] bytes = sr.ReadBytes(100000000);

                    return bytes;
                }
            }

            return null;
        }
    }

}
