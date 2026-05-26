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
    public partial class Form2 : Form
    {
        //string kullanici, sifree="";
        public Form2(string kullaniciAdi, string sifre)
        {
            InitializeComponent();
         
            label2.Text = "KULLANICI ADINIZ: " + kullaniciAdi;
            label3.Text ="ŞİFRENİZ: " + sifre;
        }

        private void button1_Click(object sender, EventArgs e) //FORM1 E DÖNÜŞ
        {
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Hide();

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
