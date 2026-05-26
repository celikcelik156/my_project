using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication27
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("ANKARA");
            comboBox1.Items.Add("İSTANBUL");
            comboBox1.Items.Add("İZMİR");
            comboBox1.Items.Add("GAZİANTEP");
            comboBox1.Items.Add("ADANA");
            comboBox1.Items.Add("MERSİN");
            comboBox1.Items.Add("BURSA");
            comboBox1.Items.Add("ANTALYA");
            comboBox1.Items.Add("KAYSERİ");

        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                comboBox1.Items.Add(textBox3.Text);
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
         //   textBox1.Text = textBox4.Text;
            textBox4.BackColor = Color.White;
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
           // textBox1.Text = textBox1.Text + " " + textBox5.Text;
            textBox5.BackColor = Color.White;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
           textBox1.Text = textBox4.Text + " " + textBox5.Text;
           textBox1.BackColor = Color.Yellow;

        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.BackColor = Color.Yellow;
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            textBox5.BackColor = Color.Yellow;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.FromName(comboBox2.Text);
            textBox2.BackColor = Color.FromName(comboBox2.Text);
            textBox3.BackColor = Color.FromName(comboBox2.Text);
            textBox4.BackColor = Color.FromName(comboBox2.Text);
            textBox5.BackColor = Color.FromName(comboBox2.Text);
            maskedTextBox1.BackColor = Color.FromName(comboBox2.Text);
            maskedTextBox2.BackColor = Color.FromName(comboBox2.Text);
            comboBox1.BackColor = Color.FromName(comboBox2.Text);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox6.Text);
            comboBox3.Items.Add(textBox6.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Sorted = true;
            comboBox3.Sorted = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox7.Text = listBox1.Text;
            textBox8.Text = listBox1.SelectedIndex.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox2.Items.Add(listBox1.Text);
            label12.Text = "TOPLAM TERCİH SAYISI : " + listBox2.Items.Count; 
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
                textBox6.Text = "";

            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            label12.Text = "TOPLAM TERCİH SAYISI : " + listBox2.Items.Count; 

        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox3.Items.Add(textBox1.Text);
            listBox4.Items.Add(textBox2.Text);
            listBox5.Items.Add(maskedTextBox3.Text);

            if (radioButton1.Checked == true)
                listBox6.Items.Add(radioButton1.Text);
            else if (radioButton2.Checked == true)
                listBox6.Items.Add(radioButton2.Text);


        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox4.SelectedIndex = listBox3.SelectedIndex;
            listBox5.SelectedIndex = listBox3.SelectedIndex;
            listBox6.SelectedIndex = listBox3.SelectedIndex;
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.SelectedIndex = listBox4.SelectedIndex;
            listBox5.SelectedIndex = listBox4.SelectedIndex;
            listBox6.SelectedIndex = listBox4.SelectedIndex;
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.SelectedIndex = listBox5.SelectedIndex;
            listBox4.SelectedIndex = listBox5.SelectedIndex;
            listBox6.SelectedIndex = listBox5.SelectedIndex;
        }

        private void listBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.SelectedIndex = listBox6.SelectedIndex;
            listBox4.SelectedIndex = listBox6.SelectedIndex;
            listBox5.SelectedIndex = listBox6.SelectedIndex;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                MessageBox.Show("Kayıt Onaylandı");
            else
                MessageBox.Show("Onaylanmadı");

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                checkBox3.Checked = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
                checkBox2.Checked = false;
        }
    }
}
