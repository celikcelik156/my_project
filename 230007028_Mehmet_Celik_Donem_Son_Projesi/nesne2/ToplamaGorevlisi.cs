using nesne2.Classs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace nesne2
{
    public partial class ToplamaGorevlisi : Form
    {
        public int gelenKullaniciID;
        bool isManager = false;

        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        public ToplamaGorevlisi(int id)
        {
            InitializeComponent();
            gelenKullaniciID = id;
        }

        private void ToplamaGorevlisi_Load_1(object sender, EventArgs e)
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

                if (reader.Read())
                {
                    label1.Text = reader["Kapasite"] != DBNull.Value ? reader["Kapasite"].ToString() : "0";
                    label22.Text = reader["RotaAdi"].ToString();
                    label4.Text = reader["DurumAdi"].ToString();

                    label5.Text = reader["AracSasiID"].ToString();
                    label6.Text = reader["Marka"].ToString();
                    label7.Text = reader["Modeli"].ToString();
                    label8.Text = reader["Plaka"].ToString();

                    label9.Text = reader["TC"].ToString();
                    label10.Text = reader["Ad"].ToString() + " " + reader["Soyad"].ToString();
                    label11.Text = reader["Yas"].ToString();
                    label12.Text = reader["Telefon"].ToString();
                    label13.Text = reader["UnvanAdi"].ToString();

                    string yoneticiDurum = reader["DepartmanYoneticisi"].ToString();

                    isManager = (yoneticiDurum == "True" || yoneticiDurum == "1");

                    if (isManager)
                    {
                        label14.Text = "Departman Yöneticisi";
                        label14.ForeColor = Color.Green;

                        EnableRadioButtons(true);
                        button2.Visible = true; 
                    }
                    else
                    {
                        label14.Text = "Personel (Yetkisiz)";
                        label14.ForeColor = Color.Red; 

                        EnableRadioButtons(false);
                        button2.Visible = false; 
                    }

                    label15.Text = reader["Adres"].ToString();
                    label16.Text = reader["MedeniDurum"].ToString();
                }
                else
                {
                    MessageBox.Show("Bu kullanıcıya ait araç veya personel kaydı bulunamadı.");
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

        private void EnableRadioButtons(bool state)
        {
            radioButton1.Enabled = state;
            radioButton2.Enabled = state;
            radioButton3.Enabled = state;
            radioButton4.Enabled = state;
            radioButton6.Enabled = state;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!isManager)
            {
                MessageBox.Show("Bu işlemi yapmaya yetkiniz yok!");
                return;
            }

            try
            {
                int yeniRotaID = 0;
                string rotaAdi = "";

                if (radioButton1.Checked) { yeniRotaID = 1; rotaAdi = "Merkez"; }
                else if (radioButton2.Checked) { yeniRotaID = 2; rotaAdi = "Doğu"; }
                else if (radioButton4.Checked) { yeniRotaID = 3; rotaAdi = "Batı"; }
                else if (radioButton3.Checked) { yeniRotaID = 4; rotaAdi = "Kuzey"; }
                else if (radioButton6.Checked) { yeniRotaID = 5; rotaAdi = "Güney"; }
                else
                {
                    MessageBox.Show("Lütfen bir rota seçiniz.");
                    return;
                }

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query = @"
                    UPDATE Araclar 
                    SET RotaID = @YeniRotaID 
                    FROM Araclar A
                    INNER JOIN Personel P ON A.AracSasiID = P.AracSasiID
                    WHERE P.TC = @TC";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@YeniRotaID", yeniRotaID);
                command.Parameters.AddWithValue("@TC", gelenKullaniciID);

                int etkilenenKayit = command.ExecuteNonQuery();

                if (etkilenenKayit > 0)
                {
                    MessageBox.Show("Rota başarıyla '" + rotaAdi + "' olarak güncellendi.");
                    label22.Text = rotaAdi;
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

        private void RadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if (isManager)
            {
                button2.Enabled = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Giris giris = new Giris();
            this.Close();
            giris.Show();
        }
    }
}