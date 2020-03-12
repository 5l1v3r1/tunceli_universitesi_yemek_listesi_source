using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Eklenenler
using HtmlAgilityPack;
using System.Threading;

namespace Tunceli_Uni___Yemek_Listesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        //Değişkenler
        Thread tVeriCek;
        //Thread tYemekResimleri;

        //Bugün Tarihi
        string tarih = null;
        DateTime dt = DateTime.Now;

        #region Form_LOAD
       private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //Form boyutu
                this.Size = new Size(831, 513);

                //Geri - İleri Buton Resimleri
                pictureBox1.Image = new Bitmap(Tunceli_Uni___Yemek_Listesi.Properties.Resources.geriPasif);
                ToolTip geriAciklama = new ToolTip();
                geriAciklama.SetToolTip(pictureBox1, "Önceki günün yemekleri.");
                pictureBox2.Image = new Bitmap(Tunceli_Uni___Yemek_Listesi.Properties.Resources.ileriPasif);
                ToolTip ileriAciklama = new ToolTip();
                ileriAciklama.SetToolTip(pictureBox2, "Sonraki günün yemekleri.");

                //Thread Aktifleştir
                CheckForIllegalCrossThreadCalls = false;

                //Thread Yemek Listesini Çek
                tVeriCek = new Thread(new ThreadStart(vVeriCek));
                tVeriCek.Start();

                //Bugün Tarihi
                label1.Text = String.Format("{0:dd MMMM yyyy}", dt) + " tarihli günün yemekleri.";
                ToolTip tarihAciklama = new ToolTip();
                tarihAciklama.SetToolTip(label1, "Tarihli günün yemekleri.");

                //Siteye Göre Tarih Formatı Düzenle
                tarih = String.Format("{0:dd.MM.yyyy}", dt);
                dateTimePicker1.Value = DateTime.Parse(tarih);
            }
            catch { }
        }
       #endregion

        #region Yemek Listesi Çek
       void vVeriCek()
        {
            label6.Text = "";
            listBox1.Items.Clear();
            try
            {
                Uri url = new Uri("http://www.tunceli.edu.tr/idari/sks/beslenme.html");
                WebClient client = new WebClient() { Encoding = Encoding.Default };
                string html = client.DownloadString(url);
                HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
                dokuman.LoadHtml(html);
                HtmlNodeCollection XPath = dokuman.DocumentNode.SelectNodes(textBox1.Text);
                foreach (var veri in XPath)
                {
                    richTextBox1.Text = veri.InnerHtml;
                    textBox2.Text = veri.InnerText.Replace("                          ", "+").Remove(0, 1);
                    textBox2.Text = textBox2.Text.Replace("&ccedil;", "ç");
                    textBox2.Text = textBox2.Text.Replace("&Ccedil;", "Ç");
                    textBox2.Text = textBox2.Text.Replace("&Uuml;", "Ü");
                    textBox2.Text = textBox2.Text.Replace("&uuml;", "ü");
                    textBox2.Text = textBox2.Text.Replace("Ouml;", "ö");
                    textBox2.Text = textBox2.Text.Replace("ouml;", "ö");
                    textBox2.Text = textBox2.Text.Replace("&Ouml;", "ö");
                    textBox2.Text = textBox2.Text.Replace("&ouml;", "ö");
                    textBox2.Text = textBox2.Text.Replace("&nbsp;", "");
                    textBox2.Text = textBox2.Text.Replace("  ", " ");

                    try
                    {
                        string[] kelimeler1 = null;
                        string s1 = textBox2.Text;
                        char[] ayirici = { '+' };
                        kelimeler1 = s1.Split(ayirici);

                        string[] elemanlar ={
                                    kelimeler1[0].ToString(),
                                    kelimeler1[1].ToString(),
                                    kelimeler1[2].ToString(),
                                    kelimeler1[3].ToString(),
                                    kelimeler1[4].ToString(),
                                    kelimeler1[5].ToString()
                                    };

                        ListViewItem veriler = new ListViewItem(elemanlar);
                        listView1.Items.Add(veriler);

                        // Tarih ara yemekleri aktar
                        if (textBox2.Text.IndexOf(tarih) != -1)
                        {
                            //Sulu Yemek
                            label2.Text = kelimeler1[2].ToString();
                            //Ana Yemek 1
                            label3.Text = kelimeler1[3].ToString();
                            //Ana Yemek 2
                            label4.Text = kelimeler1[4].ToString();
                            //İçecek
                            label5.Text = kelimeler1[5].ToString();
                        }
                    }
                    catch 
                    {
                        listBox1.Items.Add(textBox2.Text.Substring(2, 10));
                    }
                }
            }
            catch
            {
                MessageBox.Show("Yemek listesi çekilemiyor, sitede veya internetinizde problem olabilir. 'Yemek Listesi Yenile' butonuna tıklayarak yeniden deneyiniz.","Hata;",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
       #endregion

        #region Yemek Listesini Çek Buton
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //ListView Yemek Listesini Temizle
                listView1.Items.Clear();
                //Thread Yemek Listesini Çek
                tVeriCek = new Thread(new ThreadStart(vVeriCek));
                tVeriCek.Start();
            }
            catch { }
        }
        #endregion

        #region Yemek Listesini Çekmeyi Durdur
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //Thread Yemek Listesi Çekmeyi Durdur
                tVeriCek.Abort();
            }
            catch { }
        }
        #endregion

        #region Geri - İleri Buton Hover
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(Tunceli_Uni___Yemek_Listesi.Properties.Resources.geriPasif);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(Tunceli_Uni___Yemek_Listesi.Properties.Resources.geriAktif);
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = new Bitmap(Tunceli_Uni___Yemek_Listesi.Properties.Resources.ileriPasif);
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Image = new Bitmap(Tunceli_Uni___Yemek_Listesi.Properties.Resources.ileriAktif);
        }
        #endregion

        #region Bir Önceki Yemek
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                string buAy = String.Format("{0:MM}", dt);
                string seciliAy = dateTimePicker1.Value.ToString("MM");

                if (buAy == seciliAy)
                {
                    int geri = int.Parse(tarih.Substring(0, 2)) - 1;

                    tarih = String.Format("{0:" + geri.ToString() + ".MM.yyyy}", dt);

                    //Bugün Tarihi
                    label1.Text = String.Format("{0:" + geri.ToString() + " MMMM yyyy}", dt) + " tarihli günün yemekleri.";
                    label6.Text = String.Format("{0:" + geri.ToString() + " MMMM yyyy}", dt) + " tarihli günün yemekleri.";
                    try
                    {
                        //Thread Yemek Listesini Çek
                        tVeriCek = new Thread(new ThreadStart(vVeriCek));
                        tVeriCek.Start();
                    }
                    catch { }
                    dateTimePicker1.Value = DateTime.Parse(tarih);

                }
                else
                {
                    MessageBox.Show("Sadece bu ayın yemek listesini görebilirsiniz.", "Bildirim", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }
        #endregion

        #region Sonraki Yemek
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                string buAy = String.Format("{0:MM}", dt);
                string seciliAy = dateTimePicker1.Value.ToString("MM");

                if (buAy == seciliAy)
                {
                    int geri = int.Parse(tarih.Substring(0, 2)) + 1;

                    tarih = String.Format("{0:" + geri.ToString() + ".MM.yyyy}", dt);

                    //Bugün Tarihi
                    label1.Text = String.Format("{0:" + geri.ToString() + " MMMM yyyy}", dt) + " tarihli günün yemekleri.";
                    label6.Text = String.Format("{0:" + geri.ToString() + " MMMM yyyy}", dt) + " tarihli günün yemekleri.";

                    try
                    {
                        //Thread Yemek Listesini Çek
                        tVeriCek = new Thread(new ThreadStart(vVeriCek));
                        tVeriCek.Start();
                    }
                    catch { }

                    dateTimePicker1.Value = DateTime.Parse(tarih);

                }
                else
                {
                    MessageBox.Show("Sadece bu ayın yemek listesini görebilirsiniz.", "Bildirim", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }
        #endregion

        #region İstenilen Tarih
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                string buAy = String.Format("{0:MM}", dt);
                string seciliAy = dateTimePicker1.Value.ToString("MM");

                if (buAy == seciliAy)
                {
                    //Tarih düzenleme
                    tarih = dateTimePicker1.Value.ToString("dd.MM.yyyy");

                    //Bugün Tarihi
                    label1.Text = dateTimePicker1.Value.ToString("dd MMMM yyyy") + " tarihli günün yemekleri.";
                    label6.Text = dateTimePicker1.Value.ToString("dd MMMM yyyy") + " tarihli günün yemekleri.";

                    try
                    {
                        //Thread Yemek Listesini Çek
                        tVeriCek = new Thread(new ThreadStart(vVeriCek));
                        tVeriCek.Start();
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("Sadece bu ayın yemek listesini görebilirsiniz.", "Bildirim", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }
        #endregion

        #region Yemek Olmayan Gün Kontrol
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                int index = listBox1.FindString(tarih);
                if (index != -1)
                {
                    label6.Text = "Bugün yemek çıkmayacak..";
                    //Sulu Yemek
                    label2.Text = "Yok";
                    //Ana Yemek 1
                    label3.Text = "Yok";
                    //Ana Yemek 2
                    label4.Text = "Yok";
                    //İçecek
                    label5.Text = "Yok";
                }
                else
                {
                    label6.Text = label1.Text;
                }
            }
            catch { }
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try {
                Application.Exit();
            }
            catch { }
        }

        private void programHakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { 
            Form2 frm2 = new Form2();
            frm2.Show();
            }
            catch { }
        }


    }
}
