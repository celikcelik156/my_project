using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication21
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void kisilerBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.kisilerBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.database3DataSet);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database3DataSet.Kisiler' table. You can move, or remove it, as needed.
            this.kisilerTableAdapter.Fill(this.database3DataSet.Kisiler);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string metin1 = textBox1.Text;
          //  textBox2.Text = string.Concat(metin1, " metin 2", "metin 3");
            //string[] dizi = metin1.Split(' ');
         //   MessageBox.Show(dizi[0]);
           // textBox2.Text = metin1.IndexOf('i',2).ToString();
            //textBox2.Text = metin1.Substring(3, 5);
       //    MessageBox.Show(string.Compare(metin1, textBox2.Text).ToString());
        
            //  if (metin1.Equals(textBox2.Text))
               // MessageBox.Show("Girilen metinler eşittir");

        //    textBox2.Text = metin1.Length.ToString();
           // textBox2.Text = metin1.Insert(5, " ÜNİVERSİTESİ");
           // textBox2.Text = metin1.Replace('İ', 'I');
           // textBox2.Text = metin1.Remove(4, 4);
      //      textBox2.Text = metin1.ToUpper();
           // textBox2.Text = metin1.ToLower();
          //  textBox2.Text = metin1.Trim();


        }
    }
}
