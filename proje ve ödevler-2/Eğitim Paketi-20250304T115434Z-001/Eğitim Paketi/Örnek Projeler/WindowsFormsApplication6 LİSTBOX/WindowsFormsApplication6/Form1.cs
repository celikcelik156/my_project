using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode==Keys.Down)
           // if (e.KeyValue==65)  
            textBox2.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

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
            if (e.KeyCode == Keys.Up)
                textBox2.Focus();
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                textBox3.Focus();

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.LemonChiffon;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.BackColor = Color.Yellow;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.BackColor = Color.White;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.BackColor = Color.Yellow;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            textBox3.BackColor = Color.White;
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.BackColor = Color.Yellow;
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            textBox4.BackColor = System.Drawing.Color.FromArgb(0, 255, 255);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Yellow");
            comboBox1.Items.Add("Blue");
            comboBox1.Items.Add("Red");
            comboBox1.Items.Add("White");
            comboBox1.Items.Add("Black");



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.FromName(comboBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
            this.Hide();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox5.Text);
            comboBox2.Items.Add(textBox5.Text);
            textBox5.Clear();
            listBox1.Sorted = true;
            comboBox2.Sorted = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox7.Text = comboBox2.SelectedIndex.ToString();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox6.Text = listBox1.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            listBox1.Items.Clear();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
          //  comboBox3.Items.Add(listBox1.SelectedItem);
           // listBox2.Items.Add(listBox1.SelectedItem);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button2_Click(sender, e);
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            comboBox3.Items.Add(listBox1.SelectedItem);
            listBox2.Items.Add(listBox1.SelectedItem);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != 0)
            {
                int indeks = listBox2.SelectedIndex;
                int yukaridaki = indeks - 1;
                string yukari = listBox2.Items[yukaridaki].ToString();
                string mevcut = listBox2.Items[indeks].ToString();

                listBox2.Items[yukaridaki] = mevcut;
                listBox2.Items[indeks] = yukari;
            }


        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < listBox2.Items.Count-1)
            {
                int mevcutIndeks = listBox2.SelectedIndex;
                int altIndeks = mevcutIndeks + 1;
                string alt = listBox2.Items[altIndeks].ToString();
                string mevcut = listBox2.Items[mevcutIndeks].ToString();

                listBox2.Items[altIndeks] = mevcut;
                listBox2.Items[mevcutIndeks] = alt;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox2.Items.Remove(listBox2.Text);
        }
    }
}
