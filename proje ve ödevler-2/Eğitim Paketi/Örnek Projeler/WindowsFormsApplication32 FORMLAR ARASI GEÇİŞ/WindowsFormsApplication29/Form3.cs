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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
                groupBox2.Visible = true;
            else
                groupBox2.Visible = false;
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
               if (radioButton7.Checked == true)
                groupBox5.Visible = true;
            else
                groupBox5.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tc = textBox1.Text;
            string adi = textBox2.Text;
            string dt= maskedTextBox1.Text;
            string medeni="";
            if (radioButton1.Checked==true)
                medeni = radioButton1.Text;
            else if (radioButton2.Checked==true)
            {
                medeni= radioButton2.Text;
                string esAdi = textBox3.Text;
                string calismaDurumu="";
                if (checkBox1.Checked==true)
                    calismaDurumu=checkBox1.Text;
                else
                    calismaDurumu = "ÇALIŞMIYOR";
                listBox2.Items.Add(esAdi + " " + calismaDurumu);

            }
            else if (radioButton3.Checked==true)
                medeni=radioButton3.Text;

            string ogrenimDurumu="";
            string fakulte="";
            if (radioButton4.Checked==true)
                ogrenimDurumu=radioButton4.Text;
            else if (radioButton5.Checked==true)
                ogrenimDurumu=radioButton5.Text;
            else if (radioButton6.Checked==true)
                ogrenimDurumu=radioButton6.Text;
            else if (radioButton7.Checked==true)   //öğrenim durumu lisans ise
            {
                ogrenimDurumu=radioButton7.Text;
                
                if (radioButton8.Checked==true)
                fakulte=radioButton8.Text;
                else  if (radioButton9.Checked==true)
                fakulte=radioButton9.Text;
                else if (radioButton10.Checked==true)
                fakulte=radioButton10.Text;
                else  if (radioButton11.Checked==true)
                fakulte=radioButton11.Text;
                else   if (radioButton12.Checked==true)
                fakulte=radioButton12.Text;
            }
            
            listBox1.Items.Add(tc + " " + adi + " " + medeni + " " + ogrenimDurumu + " " + fakulte);
            if (medeni != "EVLİ")
                listBox2.Items.Add(medeni);

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex;
        }

        }
    }
