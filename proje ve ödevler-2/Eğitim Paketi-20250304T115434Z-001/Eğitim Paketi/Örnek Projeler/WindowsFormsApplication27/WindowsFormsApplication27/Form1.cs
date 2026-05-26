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
            MessageBox.Show("İlk Form Uygulamamıza Hoşgeldiniz.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult cevap;
            cevap = MessageBox.Show("İşlemi onaylıyor musunuz?", "Onay Kutusu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (cevap == DialogResult.Yes)
            {
                MessageBox.Show("EVET butonuna tıkladınız");
            }
            if (cevap == DialogResult.No)
            {
                MessageBox.Show("HAYIR butonuna tıkladınız");
            }
            if (cevap == DialogResult.Cancel)
            {
                MessageBox.Show("İPTAL butonuna tıkladınız");
            }
            else
            {
                MessageBox.Show("MEsajı kapatmış olabilirsiniz");
            }
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button1.Visible == true)
            {
                button1.Visible = false;
            }
            else
            {
                button1.Visible = true;
            }

            if (button2.Visible == true)
            {
                button2.Visible = false;
            }
            else
            {
                button2.Visible = true;
            }

            if (button4.Visible == true)
            {
                button4.Visible = false;
            }
            else
            {
                button4.Visible = true;
            }

    
            
   
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            button2.Visible = true;
            button4.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button4.Width = button4.Width + 20;
            button4.Height = button4.Height + 10;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button4.Width = button4.Width - 20;
            button4.Height = button4.Height - 10;
        }
    }
}
