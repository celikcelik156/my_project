using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication18
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        { //TABLODÜZENLE
            dataGridView2.ColumnCount = 5;
            dataGridView2.Columns[0].HeaderText = "Öğrenci No";
            dataGridView2.Columns[1].HeaderText = "Adı Soyadı";
            dataGridView2.Columns[2].HeaderText = "Vize";
            dataGridView2.Columns[3].HeaderText = "Final";
            dataGridView2.Columns[4].HeaderText = "Ortalama";
            dataGridView2.Columns[0].Width = 150;
            dataGridView2.Columns[1].Width = 200;
            dataGridView2.Columns[2].Width = 50;
            dataGridView2.Columns[3].Width = 50;
            dataGridView2.Columns[4].Width = 50;
       }
        private void button1_Click(object sender, EventArgs e)
        {   // TEXTLERDEN TABLOYA EKLEME
            dataGridView2.RowCount += 1;
            int satir = dataGridView2.RowCount - 1;
            dataGridView2.Rows[satir].Cells[0].Value = textBox2.Text;
            dataGridView2.Rows[satir].Cells[1].Value = textBox1.Text;
            dataGridView2.Rows[satir].Cells[2].Value = textBox3.Text;
            dataGridView2.Rows[satir].Cells[3].Value = textBox4.Text;
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {   // TABLODAN TEXTLERE AKTARMA
            textBox2.Text = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            textBox1.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            textBox4.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {  // TABLO DÜZELTME
            dataGridView2.CurrentRow.Cells[0].Value = textBox2.Text;
            dataGridView2.CurrentRow.Cells[1].Value = textBox1.Text;
            dataGridView2.CurrentRow.Cells[2].Value = textBox3.Text;
            dataGridView2.CurrentRow.Cells[3].Value = textBox4.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {  // TABLO ÜZERİNDE ÇOKLU İŞLEM
            double vize,final, ort, toplam=0;
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                vize = Convert.ToDouble(dataGridView2.Rows[i].Cells[2].Value);
                final = Convert.ToDouble(dataGridView2.Rows[i].Cells[3].Value);
                ort = vize * 0.4 + final * 0.6;
                dataGridView2.Rows[i].Cells[4].Value = ort.ToString();
                toplam = toplam + ort;
            }
            textBox5.Text = (toplam / dataGridView2.RowCount).ToString();
        }
    }
}
