using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication15
{
    public partial class Form1 : Form
    {
        int sayac=1;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          //  listBox1.Items.Add("GAZİANTEP");
           // button3.Width = button3.Width + 20;
           // button3.Location = new Point(button3.Location.X + 20, button3.Location.Y);
            sayac++;
            label1.Text = DateTime.Now.Hour.ToString();
            label2.Text = DateTime.Now.Minute.ToString();
            label3.Text = DateTime.Now.Second.ToString();

            label4.Text = label1.Text + ":" + label2.Text + " " + label3.Text;
         //   if (sayac % 2 == 0)
           //     label4.BackColor = Color.White;
           // else
             //   label4.BackColor = Color.Yellow;

            if (Convert.ToDecimal(DateTime.Now.Second) % 2 == 0)
                label4.BackColor = Color.White;
            else
                label4.BackColor = Color.Yellow;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
