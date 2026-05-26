using nesne2.Classs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nesne2
{
    public partial class BakimTeknisyeni : Form
    {
        public int gelenKullaniciID;
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        public BakimTeknisyeni(int id)
        {
            InitializeComponent();
            gelenKullaniciID = id;
        }

        private void BakimTeknisyeni_Load(object sender, EventArgs e)
        {
            KullaniciBilgileriGetir();
            HasarliVeBakimdakiAraclariListele();
        }

        private void KullaniciBilgileriGetir()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string query = @"
                    SELECT 
                        P.TC, 
                        P.Ad, 
                        P.Soyad, 
                        P.Yas, 
                        P.Telefon, 
                        P.Adres, 
                        P.MedeniDurum, 
                        P.DepartmanYoneticisi, 
                        U.UnvanAdi
                    FROM Personel P
                    LEFT JOIN Unvan U ON P.UnvanID = U.UnvanID
                    WHERE P.TC = @TC";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TC", gelenKullaniciID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    label9.Text = dr["TC"].ToString();
                    label10.Text = dr["Ad"].ToString() + " " + dr["Soyad"].ToString();
                    label11.Text = dr["Yas"].ToString();
                    label12.Text = dr["Telefon"].ToString();
                    label13.Text = dr["UnvanAdi"].ToString();

                    string yoneticiDurum = dr["DepartmanYoneticisi"].ToString();
                    if (yoneticiDurum == "True" || yoneticiDurum == "1")
                        label14.Text = "Departman Yöneticisi";
                    else
                        label14.Text = "Personel";

                    label15.Text = dr["Adres"].ToString();
                    label16.Text = dr["MedeniDurum"].ToString();
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı bilgileri yüklenirken hata: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }

        private void HasarliVeBakimdakiAraclariListele()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string query = @"
                    SELECT A.AracSasiID, A.Plaka, A.Marka, A.Modeli, D.DurumAdi 
                    FROM Araclar A
                    INNER JOIN Durum D ON A.AracDurumID = D.DurumID
                    WHERE A.AracDurumID IN (3, 4)";

                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme Hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Lütfen Araç ID giriniz.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string updateQuery = "UPDATE Araclar SET AracDurumID = 4 WHERE AracSasiID = @AracID";
                SqlCommand cmd = new SqlCommand(updateQuery, connection);
                cmd.Parameters.AddWithValue("@AracID", Convert.ToInt32(textBox3.Text));

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Araç Bakıma Alındı.");
                    textBox3.Clear();
                    HasarliVeBakimdakiAraclariListele();
                }
                else
                {
                    MessageBox.Show("Araç bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Lütfen Araç ID giriniz.");
                return;
            }

            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Lütfen bir bakım türü (Ağır/Standart) seçiniz.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                int bakimTurID = 0;
                string bakimAciklama = "";

                if (radioButton1.Checked)
                {
                    bakimTurID = 2;
                    bakimAciklama = "Ağır Bakım";
                }
                else
                {
                    bakimTurID = 1; 
                    bakimAciklama = "Standart Bakım";
                }

                string insertSorgusu = @"
            INSERT INTO Bakimlar (AracSasiID, BakimTurID, BakimTarihi) 
            VALUES (@AracID, @TurID, @Tarih)";

                SqlCommand cmdInsert = new SqlCommand(insertSorgusu, connection);
                cmdInsert.Parameters.AddWithValue("@AracID", Convert.ToInt32(textBox1.Text));
                cmdInsert.Parameters.AddWithValue("@TurID", bakimTurID);
                cmdInsert.Parameters.AddWithValue("@Tarih", DateTime.Now);

                cmdInsert.ExecuteNonQuery();

                string updateArac = "UPDATE Araclar SET AracDurumID = 2 WHERE AracSasiID = @AracID";
                SqlCommand cmdUpdate = new SqlCommand(updateArac, connection);
                cmdUpdate.Parameters.AddWithValue("@AracID", Convert.ToInt32(textBox1.Text));

                int result = cmdUpdate.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show($"Bakım başarıyla kaydedildi.\nTür: {bakimAciklama}\nAraç tekrar kullanıma açıldı.");

                    textBox1.Clear();
                    radioButton1.Checked = false;
                    radioButton2.Checked = false;

                    HasarliVeBakimdakiAraclariListele();
                }
                else
                {
                    MessageBox.Show("Araç durumu güncellenemedi. ID hatalı olabilir.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Giris giris = new Giris();
            this.Close();
            giris.Show();
        }


        public BakimTeknisyeni()
        {
            InitializeComponent();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Giris giris = new Giris();
            this.Close();
            giris.Show();
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }
    }
}