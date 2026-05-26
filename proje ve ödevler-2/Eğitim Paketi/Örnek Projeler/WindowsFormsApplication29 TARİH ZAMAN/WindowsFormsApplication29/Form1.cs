using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication29
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = dateTimePicker1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string uzun_tarih = dateTimePicker1.Value.ToLongDateString();
            string kisa_tarih = dateTimePicker1.Value.ToShortDateString();
            string uzun_zaman = dateTimePicker1.Value.ToLongTimeString();
            string kisa_zaman = dateTimePicker1.Value.ToShortTimeString();

            listBox1.Items.Add(dateTimePicker1.Value.ToString());
            listBox1.Items.Add(uzun_tarih);
            listBox1.Items.Add(kisa_tarih);
            listBox1.Items.Add(uzun_zaman);
            listBox1.Items.Add(kisa_zaman);

            listBox1.Items.Add(DateTime.Today);
            listBox1.Items.Add(DateTime.Now);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime mevcut_tarih = Convert.ToDateTime(textBox1.Text);
            DateTime yeni_tarih = Convert.ToDateTime(textBox1.Text);
            int eklenecek_zaman = Convert.ToInt32(textBox2.Text);
            if (comboBox1.Text == "SANİYE")
            {
             yeni_tarih =mevcut_tarih.AddSeconds(eklenecek_zaman);
            }
            if (comboBox1.Text == "DAKİKA")
            {
                yeni_tarih = mevcut_tarih.AddMinutes(eklenecek_zaman);
            }
            if (comboBox1.Text == "SAAT")
            {
                yeni_tarih = mevcut_tarih.AddHours(eklenecek_zaman);
            }
            if (comboBox1.Text == "GÜN")
            {
                yeni_tarih = mevcut_tarih.AddDays(eklenecek_zaman);
            }
            if (comboBox1.Text == "AY")
            {
                yeni_tarih = mevcut_tarih.AddMonths(eklenecek_zaman);
            }
            if (comboBox1.Text == "YIL")
            {
                yeni_tarih = mevcut_tarih.AddYears(eklenecek_zaman);
            }

            textBox3.Text = yeni_tarih.ToString();

            TimeSpan fark = yeni_tarih - mevcut_tarih;
            listBox2.Items.Add(fark.TotalDays.ToString() + " Toplam Gün");
            listBox2.Items.Add(fark.TotalHours.ToString() + " Toplam Saat");
            listBox2.Items.Add(fark.TotalMinutes.ToString() + " Toplam Dakika");
            listBox2.Items.Add(fark.TotalSeconds.ToString() + " Toplam Saniye");
           

        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox3.Items.Add(DateTime.MinValue);
            listBox3.Items.Add(DateTime.MaxValue);

            listBox3.Items.Add(DateTime.Today);
            listBox3.Items.Add(DateTime.Today.Date);
            listBox3.Items.Add(DateTime.Today.Day);
            listBox3.Items.Add(DateTime.Today.DayOfWeek);
            listBox3.Items.Add(DateTime.Today.DayOfYear);
            listBox3.Items.Add(DateTime.Today.Year);
            listBox3.Items.Add(DateTime.Today.Month);

            listBox3.Items.Add(DateTime.Now);






        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "SANİYE")
                textBox3.Text = (Convert.ToDateTime(textBox1.Text).AddSeconds(Convert.ToInt32(textBox2.Text))).ToString();
            if (comboBox1.Text == "DAKİKA")
                textBox3.Text = (Convert.ToDateTime(textBox1.Text).AddMinutes(Convert.ToInt32(textBox2.Text))).ToString();
            if (comboBox1.Text == "SAAT")
                textBox3.Text = (Convert.ToDateTime(textBox1.Text).AddHours(Convert.ToInt32(textBox2.Text))).ToString();
            if (comboBox1.Text == "GÜN")
                textBox3.Text = (Convert.ToDateTime(textBox1.Text).AddDays(Convert.ToInt32(textBox2.Text))).ToString();
            if (comboBox1.Text == "AY")
                textBox3.Text = (Convert.ToDateTime(textBox1.Text).AddMonths(Convert.ToInt32(textBox2.Text))).ToString();
            if (comboBox1.Text == "YIL")
                textBox3.Text = (Convert.ToDateTime(textBox1.Text).AddYears(Convert.ToInt32(textBox2.Text))).ToString();
        }
    }
}
