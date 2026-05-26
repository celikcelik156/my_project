using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string esCalismaDurumu="";
            if (checkBox1.Checked == true)
                esCalismaDurumu = "EVET";
            else
                esCalismaDurumu = "HAYIR";

            MessageBox.Show(esCalismaDurumu);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string siparis = "";

            if (checkBox2.Checked == true)
                siparis =siparis + " " + checkBox2.Text;
            if (checkBox3.Checked == true)
                siparis = siparis + " " + checkBox3.Text;

            listBox1.Items.Add(siparis);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                MessageBox.Show("Medeni Durumunuz: " + radioButton1.Text);
            if (radioButton2.Checked == true)
                MessageBox.Show("Medeni Durumunuz: " + radioButton2.Text);
            if (radioButton3.Checked == true)
                MessageBox.Show("Medeni Durumunuz: " + radioButton3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = maskedTextBox1.Text;
               
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
          //  this.Hide();
            frm.Show();
        }
    }
}
