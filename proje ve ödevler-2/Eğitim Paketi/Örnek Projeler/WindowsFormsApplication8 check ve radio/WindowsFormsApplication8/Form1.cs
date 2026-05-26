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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "admin" && textBox2.Text == "1234")
            {
                Form2 frm2 = new Form2(textBox1.Text, textBox2.Text);
                frm2.Show();
                this.Hide();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                checkBox2.Checked = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                checkBox1.Checked = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox3.Text);
            listBox2.Items.Add(textBox3.Text);
            listBox3.Items.Add(textBox3.Text);
            listBox4.Items.Add(textBox3.Text);


        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
            listBox3.SelectedIndex = listBox1.SelectedIndex;
            listBox4.SelectedIndex = listBox1.SelectedIndex;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex;
            listBox3.SelectedIndex = listBox2.SelectedIndex;
            listBox4.SelectedIndex = listBox2.SelectedIndex;
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox3.SelectedIndex;
            listBox1.SelectedIndex = listBox3.SelectedIndex;
            listBox4.SelectedIndex = listBox3.SelectedIndex;
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox4.SelectedIndex;
            listBox3.SelectedIndex = listBox4.SelectedIndex;
            listBox1.SelectedIndex = listBox4.SelectedIndex;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
