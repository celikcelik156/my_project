using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace nesne2
{
    public partial class Giris : Form
    {

        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");
        public Giris()
        {
            InitializeComponent();

            textBox2.UseSystemPasswordChar = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        bool move;
        int mouse_x;
        int mouse_y;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            mouse_x = e.X;
            mouse_y = e.Y;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                this.SetDesktopLocation(MousePosition.X - mouse_x, MousePosition.Y - mouse_y);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string kullanici_adi = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();

            bool girisBasarili = false;
            int bulunanKullaniciID = 0;
            int bulunanUnvanID = 0;

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Personel", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string dbEmail = reader["Email"].ToString().TrimEnd();
                    string dbSifre = reader["Sifre"].ToString().TrimEnd();

                    if (kullanici_adi == dbEmail && sifre == dbSifre)
                    {
                        girisBasarili = true;

                        bulunanKullaniciID = Convert.ToInt32(reader["TC"]);

                        if (reader["UnvanID"] != DBNull.Value)
                        {
                            bulunanUnvanID = Convert.ToInt32(reader["UnvanID"]);
                        }
                        break;
                    }
                }

                reader.Close();
                connection.Close();

                if (girisBasarili == true)
                {
                    this.Hide();

                    switch (bulunanUnvanID)
                    {
                        case 6: 
                            Admin adminPanel = new Admin();
                            adminPanel.Show();
                            break;

                        case 1:
                            Sofor soforForm = new Sofor(bulunanKullaniciID);
                            soforForm.Show();
                            break;

                        case 2: 
                            ToplamaGorevlisi toplamaForm = new ToplamaGorevlisi(bulunanKullaniciID);
                            toplamaForm.Show();
                            break;

                        case 3:
                            GeriDonusumOperatoru geriDonusumOperatoru = new GeriDonusumOperatoru(bulunanKullaniciID);
                            geriDonusumOperatoru.Show();
                            break;


                        case 4:
                            BakimTeknisyeni bakimTeknisyeni = new BakimTeknisyeni(bulunanKullaniciID);
                            bakimTeknisyeni.Show();
                            break;

                        case 5: 
                            Muhasebe muhasebe = new Muhasebe(bulunanKullaniciID);
                            muhasebe.Show();
                            break;

                        default:
                            MessageBox.Show("Bu unvan için atanmış bir ekran yok. (Unvan ID: " + bulunanUnvanID + ")");
                            Application.Exit();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre yanlış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        int goster_say = 0;
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string path;
                if (goster_say % 2 == 0)
                {
                    path = @"C:\Users\semsi\OneDrive\Belgeler\nesne2\Resources\close_eye.png";
                    textBox2.UseSystemPasswordChar = false;
                }
                else
                {
                    path = @"C:\Users\semsi\OneDrive\Belgeler\nesne2\Resources\open_eye.png";
                    textBox2.UseSystemPasswordChar = true;
                }

                var newImg = Image.FromFile(path);
                button3.BackgroundImage = newImg;

                goster_say++;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Resim yüklenemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                textBox2.Focus();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button2_Click(sender, e);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Select();
        }
    }
}
