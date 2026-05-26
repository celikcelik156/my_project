using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication8
{
    public partial class Form2 : Form
    {
        public Form2(string kullanici, string sifre)
        {
          
          //  MessageBox.Show("Hoşgeldiniz " + kullanici);
            InitializeComponent();
            label1.Text = kullanici;
            label2.Text = sifre;
        }

        private void button1_Click(object sender, EventArgs e)
        {
          //  textBox1.Text = maskedTextBox1.Mask;
         //   textBox1.Text = maskedTextBox1.Text;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
