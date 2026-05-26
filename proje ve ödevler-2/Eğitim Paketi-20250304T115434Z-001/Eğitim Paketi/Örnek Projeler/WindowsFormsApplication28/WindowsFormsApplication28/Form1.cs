using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication28
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "DÜĞME 1 TIKLANDI";
            textBox1.Text = label1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = "DÜĞME 2 TIKLANDI";
            textBox2.Text = label2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label3.Text = "DÜĞME 3 TIKLANDI";
            textBox3.Text = label3.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 20; i++)
            {
                label4.Text = "Döngü Değeri:" + i;
                DialogResult mesaj;
                mesaj = MessageBox.Show("Devam etmek istiyor musunuz?", "Bilgi", MessageBoxButtons.YesNo);
                if (mesaj == DialogResult.No)
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label8.Text = "Adı Soyadı: " + textBox1.Text + " , Telefonu: " + textBox2.Text + " , Adresi: " + textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox5.Text = textBox4.Text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode==Keys.Down)
                textBox2.Focus();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                textBox3.Focus();

            if (e.KeyCode == Keys.Up)
                textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
