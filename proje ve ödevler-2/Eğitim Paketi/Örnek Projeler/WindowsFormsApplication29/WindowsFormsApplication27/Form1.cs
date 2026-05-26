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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) + Convert.ToDouble(textBox2.Text));
            label4.Text = button1.Text + " İŞLEMİNİZİN SONUCU";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) - Convert.ToDouble(textBox2.Text));
            label4.Text = button3.Text + " İŞLEMİNİZİN SONUCU";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) * Convert.ToDouble(textBox2.Text));
            label4.Text = button4.Text + " İŞLEMİNİZİN SONUCU";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(textBox2.Text) > 0)
            {
                textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) / Convert.ToDouble(textBox2.Text));
                label4.Text = button5.Text + " İŞLEMİNİZİN SONUCU";
            }
            else
                MessageBox.Show("Bölen Sıfır olamaz");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) /100 * Convert.ToDouble(textBox2.Text));
            label4.Text = button6.Text + " İŞLEMİNİZİN SONUCU";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) % Convert.ToDouble(textBox2.Text));
            label4.Text = button7.Text + " İŞLEMİNİZİN SONUCU";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "TOPLAMA")
            {
                textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) + Convert.ToDouble(textBox2.Text));
                label4.Text = comboBox1.Text + " İŞLEMİNİZİN SONUCU";
            }
            if (comboBox1.Text == "ÇIKARMA")
            {
                textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) - Convert.ToDouble(textBox2.Text));
                label4.Text = comboBox1.Text + " İŞLEMİNİZİN SONUCU";
            }
            if (comboBox1.Text == "ÇARPMA")
            {
                textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) * Convert.ToDouble(textBox2.Text));
                label4.Text = comboBox1.Text + " İŞLEMİNİZİN SONUCU";
            }
            if (comboBox1.Text == "BÖLME")
            {
                if (Convert.ToDouble(textBox2.Text) > 0)
                {
                    textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) / Convert.ToDouble(textBox2.Text));
                    label4.Text = comboBox1.Text + " İŞLEMİNİZİN SONUCU";
                }
                else
                    MessageBox.Show("Bölen Sıfır olamaz");
                
            }
            if (comboBox1.Text == "YÜZDE")
            {
                textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) /100 * Convert.ToDouble(textBox2.Text));
                label4.Text = comboBox1.Text + " İŞLEMİNİZİN SONUCU";
            }
            if (comboBox1.Text == "MOD")
            {
                textBox3.Text = Convert.ToString(Convert.ToDouble(textBox1.Text) % Convert.ToDouble(textBox2.Text));
                label4.Text = comboBox1.Text + " İŞLEMİNİZİN SONUCU";
            }


        }
    }
}
