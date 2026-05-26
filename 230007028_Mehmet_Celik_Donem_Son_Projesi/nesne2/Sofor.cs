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
    public partial class Sofor : Form
    {
        public int gelenKullaniciID;

        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        public Sofor(int id)
        {
            InitializeComponent();
            gelenKullaniciID = id;

            button2.Enabled = false; 
            radioButton1.Checked = false;
            radioButton2.Checked = false;
        }

        private void Sofor_Load(object sender, EventArgs e)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query = @"
                        SELECT 
                            A.AracSasiID, 
                            A.Marka, 
                            A.Modeli, 
                            A.Kapasite, 
                            A.Plaka, 
                            R.RotaAdi, 
                            D.DurumAdi,
                            P.TC,
                            P.Ad,
                            P.Soyad,
                            P.Yas,
                            P.Telefon,
                            P.DepartmanYoneticisi,
                            P.Adres,
                            P.MedeniDurum,
                            U.UnvanAdi
                        FROM Araclar A
                        INNER JOIN Rota R ON A.RotaID = R.RotaID
                        INNER JOIN Durum D ON A.AracDurumID = D.DurumID
                        INNER JOIN Personel P ON A.AracSasiID = P.AracSasiID
                        LEFT JOIN Unvan U ON P.UnvanID = U.UnvanID 
                        WHERE P.TC = @KullaniciID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@KullaniciID", gelenKullaniciID);

                SqlDataReader reader = command.ExecuteReader();

                Arac arac = new Arac();
                Personel personel = new Personel();

                if (reader.Read())
                {
                    arac.AracSasiID = Convert.ToInt32(reader["AracSasiID"]);
                    arac.Marka = reader["Marka"].ToString();
                    arac.Model = reader["Modeli"].ToString();
                    arac.Plaka = reader["Plaka"].ToString();

                    if (reader["Kapasite"] != DBNull.Value)
                        arac.Kapasite = Convert.ToDecimal(reader["Kapasite"]);

                    arac.Rota = new Rota();
                    arac.Rota.RotaAdi = reader["RotaAdi"].ToString();

                    arac.Durum = new Durum();
                    arac.Durum.DurumAdi = reader["DurumAdi"].ToString();

                    label1.Text = arac.Kapasite.ToString();
                    label2.Text = arac.Rota.RotaAdi;
                    label4.Text = arac.Durum.DurumAdi;
                    label5.Text = arac.AracSasiID.ToString();
                    label6.Text = arac.Marka;
                    label7.Text = arac.Model;
                    label8.Text = arac.Plaka;

                    label9.Text = reader["TC"].ToString();
                    label10.Text = reader["Ad"].ToString() + " " + reader["Soyad"].ToString();
                    label11.Text = reader["Yas"].ToString();
                    label12.Text = reader["Telefon"].ToString();
                    label13.Text = reader["UnvanAdi"].ToString();

                    string yoneticiDurum = reader["DepartmanYoneticisi"].ToString();
                    if (yoneticiDurum == "True" || yoneticiDurum == "1")
                    {
                        label14.Text = "Departman Yöneticisi";
                    }
                    else
                    {
                        label14.Text = "Personel";
                    }

                    label15.Text = reader["Adres"].ToString();
                    label16.Text = reader["MedeniDurum"].ToString();
                }
                else
                {
                    MessageBox.Show("Bu personele zimmetlenmiş bir araç bulunamadı.");
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yükleme Hatası: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int yeniDurumID = 0;
                string durumMetni = "";

                if (radioButton1.Checked == true)
                {
                    yeniDurumID = 2;
                    durumMetni = "Pasif";
                }
                else if (radioButton2.Checked == true)
                {
                    yeniDurumID = 3; 
                    durumMetni = "Hasarlı";
                }
                else
                {
                    MessageBox.Show("Lütfen bir durum seçiniz.");
                    return;
                }

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query = @"
                    UPDATE Araclar 
                    SET AracDurumID = @YeniID 
                    FROM Araclar A
                    INNER JOIN Personel P ON A.AracSasiID = P.AracSasiID
                    WHERE P.TC = @TC";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@YeniID", yeniDurumID);
                command.Parameters.AddWithValue("@TC", gelenKullaniciID);

                int etkilenenKayit = command.ExecuteNonQuery();

                if (etkilenenKayit > 0)
                {
                    MessageBox.Show("Araç durumu '" + durumMetni + "' olarak güncellendi.");

                    label4.Text = durumMetni;
                }
                else
                {
                    MessageBox.Show("Güncelleme başarısız. Araç bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme Hatası: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) button2.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) button2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Giris giris = new Giris();
            this.Close();
            giris.Show();
        }

        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void label13_Click(object sender, EventArgs e) { }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}