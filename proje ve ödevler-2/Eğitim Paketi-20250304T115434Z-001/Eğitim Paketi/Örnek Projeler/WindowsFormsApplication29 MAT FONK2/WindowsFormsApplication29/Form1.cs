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
        public int sayac = 1;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1.Location = new Point(button1.Location.X + 20, button1.Location.Y+10);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //timer1.Enabled = true;
            timer2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.Hour.ToString();
            label2.Text = DateTime.Now.Minute.ToString();
            label3.Text = DateTime.Now.Second.ToString();

            label4.Text = label1.Text + ":" + label2.Text + ":" + label3.Text;
            
            sayac++;
            if (sayac % 2 == 0)
                label4.BackColor = Color.Yellow;
            else
                label4.BackColor = Color.White;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = Math.Floor(Convert.ToDecimal(textBox1.Text)).ToString();
            textBox3.Text = Math.Ceiling(Convert.ToDecimal(textBox1.Text)).ToString();
            textBox4.Text = Math.Round(Convert.ToDecimal(textBox1.Text)).ToString();
            textBox5.Text = Math.Truncate(Convert.ToDecimal(textBox1.Text)).ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox9.Text = Math.Abs(Convert.ToInt32(textBox10.Text)).ToString();
            textBox8.Text = Math.Sqrt(Convert.ToDouble(textBox10.Text)).ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox11.Text = Math.Pow(Convert.ToDouble(textBox7.Text), Convert.ToDouble(textBox6.Text)).ToString();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox15.Text = Math.Sin(Convert.ToDouble(textBox16.Text) * Math.PI / 180).ToString();
            textBox14.Text = Math.Cos(Convert.ToDouble(textBox16.Text) * Math.PI / 180).ToString();
            textBox13.Text = Math.Tan(Convert.ToDouble(textBox16.Text) * Math.PI / 180).ToString();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox18.Text = Math.Log(Convert.ToDouble(textBox19.Text)).ToString();
            textBox17.Text = Math.Log(Convert.ToDouble(textBox19.Text), Convert.ToDouble(textBox20.Text)).ToString();
            textBox12.Text = Math.Log10(Convert.ToDouble(textBox19.Text)).ToString();
        }
    }
}
