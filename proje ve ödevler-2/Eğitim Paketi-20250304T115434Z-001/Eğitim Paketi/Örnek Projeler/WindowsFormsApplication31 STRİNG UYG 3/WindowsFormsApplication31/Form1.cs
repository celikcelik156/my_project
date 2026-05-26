using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication31
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string metin1 = textBox1.Text;
            textBox3.Text = metin1.ToUpper(); // büyük harfe dönüştür
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string metin1 = textBox1.Text;
            textBox3.Text = metin1.ToLower();   //küçük harfe dönüştür
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string metin = textBox1.Text;
            //textBox3.Text = metin.Trim(); // başındaki ve sonundaki boşlukları sil
            //textBox3.Text = metin.TrimStart();   // metnin başındaki boşlukları sil
            textBox3.Text = metin.TrimEnd();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string metin = textBox1.Text;
            textBox3.Text = metin.Length.ToString();   //karakter uzunluk sayı
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string metin = textBox1.Text;
          //  textBox3.Text = metin.Substring(4, 5);   //parça al
            textBox3.Text = metin.Substring(Convert.ToInt32(textBox4.Text), Convert.ToInt32(textBox5.Text));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string metin1 = textBox1.Text;
            string metin2 = textBox2.Text;
            textBox3.Text = metin1.Insert(0,metin2); //METİN YAPIŞTIR
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //string metin = "GAZİANTEP İSLAM BİLİM VE TEKNOLOJİ ÜNİVERSİTESİ";
            //textBox3.Text = metin.Replace("BİLİM", "MÜHENDİSLİK");

            string metin = textBox1.Text;
            string eski_metin = textBox2.Text;
            string degisecek_metin = textBox6.Text;
            textBox3.Text = metin.Replace(eski_metin, degisecek_metin);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string metin = textBox1.Text;
            textBox3.Text = metin.Remove(4, 5);
         //   textBox3.Text = metin.Remove(5); // karakter sil

        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox3.Text = string.Concat(textBox1.Text," ", textBox2.Text," ", textBox6.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox3.Text = string.Compare(textBox1.Text, textBox2.Text).ToString();//KARŞILAŞTIRMA
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string metin = textBox1.Text;
            textBox3.Text = metin.IndexOf("İ",4).ToString();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string adSoyad = textBox1.Text;
            int bosluk = adSoyad.IndexOf(' ');
            string adi = adSoyad.Substring(0, bosluk);
            textBox2.Text = adi;
            string soyadi = adSoyad.Substring(bosluk + 1);
            textBox6.Text = soyadi;

        }

        private void button13_Click(object sender, EventArgs e)
        {
            string uzun_metin = richTextBox1.Text;
            string[] dizi = uzun_metin.Split(' ');

            for (int i = 0; i < dizi.Length; i++)
                listBox1.Items.Add(dizi[i]);
        }
    }
}
