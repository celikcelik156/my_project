using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication17
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //mutlak değer
        {
            textBox2.Text = Math.Abs(Convert.ToInt32(textBox1.Text)).ToString();
        }

        private void button2_Click(object sender, EventArgs e) //çevre alan
        {
            double r = Convert.ToDouble(textBox1.Text);
            textBox4.Text = (2 * Math.PI * r).ToString();  //çevre
            //textBox3.Text = (Math.PI * r * r).ToString();  //alan
            textBox3.Text = (Math.PI * Math.Pow(r,2)).ToString();
           
            
            
            
            //yuvarlamalar:
            textBox5.Text = Math.Ceiling(Convert.ToDecimal(textBox4.Text)).ToString();
            textBox6.Text = Math.Floor(Convert.ToDecimal(textBox4.Text)).ToString();
            textBox7.Text = Math.Round(Convert.ToDecimal(textBox4.Text)).ToString();
            textBox8.Text = Math.Truncate(Convert.ToDecimal(textBox3.Text)).ToString();
        }

        private void button3_Click(object sender, EventArgs e) //karekök
        {
            textBox10.Text = Math.Sqrt(Math.Abs(Convert.ToDouble(textBox9.Text))).ToString();
           // double kup = 1.0 / 3.0;
            textBox23.Text = Math.Pow(Convert.ToDouble(textBox9.Text), 1.0/3.0).ToString(); //küp kök
        }
        private void button4_Click(object sender, EventArgs e) //min
        {
            textBox13.Text = Math.Min(Convert.ToByte(textBox11.Text), Convert.ToByte(textBox12.Text)).ToString();
        }

        private void button5_Click(object sender, EventArgs e) //max
        {
            
            textBox13.Text = Math.Max(Convert.ToByte(textBox11.Text), Convert.ToByte(textBox12.Text)).ToString();
        }

        private void button6_Click(object sender, EventArgs e) //trigonometrik
        {
            textBox15.Text = Math.Sin(Convert.ToDouble(textBox14.Text)*Math.PI/180).ToString();
            textBox16.Text = Math.Cos(Convert.ToDouble(textBox14.Text) * Math.PI / 180).ToString();
            textBox17.Text = Math.Tan(Convert.ToDouble(textBox14.Text) * Math.PI / 180).ToString();
            textBox22.Text = Math.Acos(Convert.ToDouble(textBox14.Text) * Math.PI / 180).ToString();
        }

        private void button7_Click(object sender, EventArgs e) //logaritma
        {
            textBox20.Text = Math.Log(Convert.ToDouble(textBox21.Text)).ToString();
            textBox19.Text = Math.Log10(Convert.ToDouble(textBox21.Text)).ToString();
            textBox18.Text = Math.Log(8,2).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
