using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication19
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'kasaDataSet.unvan' table. You can move, or remove it, as needed.
            this.unvanTableAdapter.Fill(this.kasaDataSet.unvan);
            dataGridView2.ColumnCount = 5;
            dataGridView2.Columns[0].HeaderText = "ÖĞRENCİ NO";
            dataGridView2.Columns[1].HeaderText = "ADI SOYADI";
            dataGridView2.Columns[2].HeaderText = "VİZE";
            dataGridView2.Columns[3].HeaderText = "FİNAL";
            dataGridView2.Columns[4].HeaderText = "BAŞARI ORTALAMASI";

            dataGridView2.Columns[0].Width = 150;
            dataGridView2.Columns[1].Width = 200;
            dataGridView2.Columns[2].Width = 50;
            dataGridView2.Columns[3].Width = 50;
            dataGridView2.Columns[4].Width = 100;

           // dataGridView2.RowCount = 10;

        }

        private void button1_Click(object sender, EventArgs e)
        { //TABLOYA EKLEME
            dataGridView2.RowCount += 1;
            int satir_indisi = dataGridView2.RowCount - 1;
            dataGridView2.Rows[satir_indisi].Cells[0].Value = textBox1.Text;
            dataGridView2.Rows[satir_indisi].Cells[1].Value = textBox2.Text;
            dataGridView2.Rows[satir_indisi].Cells[2].Value = textBox3.Text;
            dataGridView2.Rows[satir_indisi].Cells[3].Value = textBox4.Text;
 
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        { // TEXTLERE AKTARMA
            textBox1.Text = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            textBox2.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            textBox4.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        { //TABLODA DÜZELTME
            dataGridView2.CurrentRow.Cells[0].Value = textBox1.Text;
            dataGridView2.CurrentRow.Cells[1].Value = textBox2.Text;
            dataGridView2.CurrentRow.Cells[2].Value = textBox3.Text;
            dataGridView2.CurrentRow.Cells[3].Value = textBox4.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Remove(dataGridView2.CurrentRow);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.RemoveAt(Convert.ToInt32(textBox5.Text)-1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double vize, final, ort, sinifOrt = 0;
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                vize = Convert.ToDouble(dataGridView2.Rows[i].Cells[2].Value);
                final = Convert.ToDouble(dataGridView2.Rows[i].Cells[3].Value);
                ort = vize * 0.4 + final * 0.6;
                dataGridView2.Rows[i].Cells[4].Value = ort.ToString();
                sinifOrt = sinifOrt + ort;

            }
            textBox6.Text = (sinifOrt / dataGridView2.RowCount).ToString();

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

      
    }
}
