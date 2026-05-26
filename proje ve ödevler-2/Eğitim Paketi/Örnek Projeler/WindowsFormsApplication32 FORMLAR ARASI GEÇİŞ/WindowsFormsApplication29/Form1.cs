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

        private void button1_Click(object sender, EventArgs e)  // VERİ AKTARARAK GİRİŞ
        {

            if (textBox1.Text == "admin" && textBox2.Text == "1234")
            {
                Form2 yeniForm = new Form2(textBox1.Text,textBox2.Text);
                yeniForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı ve/veya şifre hatalıdır. Yeniden giriş yapınız. ");
                textBox1.Focus();
            }

        }

        private void button2_Click(object sender, EventArgs e) //ŞİFRE GÖSTER GİZLE
        {
            if (textBox2.UseSystemPasswordChar == true)

                textBox2.UseSystemPasswordChar = false;
            else
                textBox2.UseSystemPasswordChar = true;
        }

        private void button3_Click(object sender, EventArgs e) // GİRİŞ BUTONU
        {
            if (textBox1.Text == "admin" && textBox2.Text == "1234")
            {
                Form3 yeniForm3 = new Form3();
                yeniForm3.Show();
              //  this.Hide();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı ve/veya şifre hatalıdır. Yeniden giriş yapınız. ");
                textBox1.Focus();
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
