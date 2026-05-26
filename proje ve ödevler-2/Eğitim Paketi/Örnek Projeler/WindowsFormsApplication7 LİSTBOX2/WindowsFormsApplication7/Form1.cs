using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                if (textBox1.Text != "")
                    textBox2.Focus();
                else
                    MessageBox.Show("Ad alanı boş geçilemez");
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                textBox3.Focus();

            if (e.KeyCode == Keys.Up)
                textBox1.Focus();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                textBox4.Focus();

            if (e.KeyCode==Keys.Up)
                textBox2.Focus();
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                textBox1.Focus();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.Yellow;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.BackColor = Color.Yellow;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.BackColor = Color.Yellow;
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.BackColor = Color.Yellow;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.BackColor = Color.White;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            textBox3.BackColor = Color.White;
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            textBox4.BackColor = Color.White;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Yellow");
            comboBox1.Items.Add("Red");
            comboBox1.Items.Add("Blue");
            comboBox1.Items.Add("White");
            comboBox1.Items.Add("Green");
            comboBox1.Items.Add("Black");

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.FromName(comboBox1.Text);
            //textBox1.BackColor = Color.FromArgb(0, 255, 255);
        //    textBox2.BackColor = Color.FromArgb(255, 0, 255);
          //  textBox3.BackColor = Color.FromArgb(0, 255, 0);
           // textBox4.BackColor = Color.FromArgb(120, 120, 120);
            
        }

        private void button1_Click(object sender, EventArgs e) // ŞEHİR EKLE BUTONU
        {
            if (listBox1.Items.Count <5) // ELEMAN SAYISI SONRADAN.
            {
                listBox1.Items.Add(textBox5.Text);
                comboBox2.Items.Add(textBox5.Text);
                listBox1.Sorted = true;
                comboBox2.Sorted = true;
                textBox5.Text = "";
               textBox5.Focus();
               label9.Text = "Liste Eleman Sayısı : " + listBox1.Items.Count.ToString();

            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender,e);
            button2_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();  TEMİZLEME
            listBox1.Items.Remove(listBox1.Text); //ELEMAN SİLME
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox6.Text = listBox1.Text;
            textBox7.Text = listBox1.SelectedIndex.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox2.Items.Add(listBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox3.Items.Add(textBox1.Text + " " + textBox2.Text);
            listBox4.Items.Add(textBox3.Text);
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox4.SelectedIndex = listBox3.SelectedIndex;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                MessageBox.Show("Onayladınız");
            else
                MessageBox.Show("reddettiniz");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string cinsiyet="-";
            if (radioButton1.Checked == true)
                cinsiyet = radioButton1.Text;
            else if (radioButton2.Checked == true)
                cinsiyet = radioButton2.Text;

            button5.Text = cinsiyet;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
