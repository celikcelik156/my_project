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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace nesne2
{
    public partial class VeriKordinasyon : Form
    {
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");
        int tesisID = 1;
        decimal tesisKapasitesi = 0;
        decimal mevcutAtikMiktari = 0;

        public VeriKordinasyon()
        {
            InitializeComponent();
        }

        private void VeriKordinasyon_Load(object sender, EventArgs e)
        {
            TesisBilgileriniGetir();
            AraclariListele();
            PersonelleriListele();
        }  

        private void TesisBilgileriniGetir()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                SqlCommand cmdTesis = new SqlCommand("SELECT Kapasite, TesisAdi FROM GeriDonusumTesisi WHERE TesisID = @TesisID", connection);
                cmdTesis.Parameters.AddWithValue("@TesisID", tesisID);
                SqlDataReader dr = cmdTesis.ExecuteReader();
                if (dr.Read())
                {
                    tesisKapasitesi = Convert.ToDecimal(dr["Kapasite"]);
                    label1.Text = tesisKapasitesi.ToString("N0");
                    label2.Text = dr["TesisAdi"].ToString();
                }
                dr.Close();

                SqlCommand cmdToplam = new SqlCommand("SELECT SUM(Miktar) FROM AtikIslem WHERE TesisID = @TesisID", connection);
                cmdToplam.Parameters.AddWithValue("@TesisID", tesisID);
                object result = cmdToplam.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                    mevcutAtikMiktari = Convert.ToDecimal(result);
                else
                    mevcutAtikMiktari = 0;

                decimal dolulukOrani = (tesisKapasitesi > 0) ? (mevcutAtikMiktari / tesisKapasitesi) * 100 : 0;
                label4.Text = $"{mevcutAtikMiktari:N0} / {tesisKapasitesi:N0} TON\n(Doluluk: %{dolulukOrani:N1})";

                SqlCommand cmdFiyat = new SqlCommand("SELECT AtikAdi, BirimFiyat FROM AtikTurleri", connection);
                SqlDataReader drFiyat = cmdFiyat.ExecuteReader();
                StringBuilder sb = new StringBuilder();
                while (drFiyat.Read())
                {
                    sb.AppendLine($"{drFiyat["AtikAdi"]}: {drFiyat["BirimFiyat"]} TL/kg");
                }
                label21.Text = sb.ToString();
                drFiyat.Close();

                SqlCommand cmdStok = new SqlCommand(@"
                    SELECT T.AtikAdi, SUM(I.Miktar) as ToplamMiktar
                    FROM AtikIslem I
                    INNER JOIN AtikTurleri T ON I.AtikID = T.AtikID
                    WHERE I.TesisID = @TesisID
                    GROUP BY T.AtikAdi", connection);
                cmdStok.Parameters.AddWithValue("@TesisID", tesisID);
                SqlDataReader drStok = cmdStok.ExecuteReader();

                label5.Text = "0 Kg"; label7.Text = "0 Kg"; label6.Text = "0 Kg";
                label8.Text = "0 Kg"; label27.Text = "0 Kg";

                while (drStok.Read())
                {
                    string atik = drStok["AtikAdi"].ToString();
                    string miktar = Convert.ToDecimal(drStok["ToplamMiktar"]).ToString("N0") + " Kg";
                    switch (atik)
                    {
                        case "Organik": label5.Text = miktar; break;
                        case "Plastik": label7.Text = miktar; break;
                        case "Metal": label6.Text = miktar; break;
                        case "Kağıt": label8.Text = miktar; break;
                        case "Cam": label27.Text = miktar; break;
                    }
                }
                drStok.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tesis verileri hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void AraclariListele()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string query = @"
                    SELECT 
                        A.AracSasiID, A.Plaka, A.Marka, A.Modeli, A.Kapasite, 
                        R.RotaAdi, D.DurumAdi
                    FROM Araclar A
                    INNER JOIN Rota R ON A.RotaID = R.RotaID
                    INNER JOIN Durum D ON A.AracDurumID = D.DurumID";

                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("Araç listeleme hatası: " + ex.Message); }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void PersonelleriListele()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string query = @"
                    SELECT P.TC, P.Ad, P.Soyad, P.Telefon, U.UnvanAdi 
                    FROM Personel P
                    LEFT JOIN Unvan U ON P.UnvanID = U.UnvanID";

                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView2.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("Personel listeleme hatası: " + ex.Message); }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Lütfen görevlendirilecek Araç ID giriniz.");
                return;
            }

            int secilenRotaID = 0;
            string rotaAdi = "";

            if (radioButton1.Checked) { secilenRotaID = 1; rotaAdi = "Merkez"; }
            else if (radioButton2.Checked) { secilenRotaID = 2; rotaAdi = "Doğu"; }
            else if (radioButton4.Checked) { secilenRotaID = 3; rotaAdi = "Batı"; } 
            else if (radioButton3.Checked) { secilenRotaID = 4; rotaAdi = "Kuzey"; }
            else if (radioButton6.Checked) { secilenRotaID = 5; rotaAdi = "Güney"; }

            if (secilenRotaID == 0)
            {
                MessageBox.Show("Lütfen bir rota seçiniz.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string updateQuery = @"
                    UPDATE Araclar 
                    SET RotaID = @RotaID, AracDurumID = 5 
                    WHERE AracSasiID = @AracID";

                SqlCommand cmd = new SqlCommand(updateQuery, connection);
                cmd.Parameters.AddWithValue("@RotaID", secilenRotaID);
                cmd.Parameters.AddWithValue("@AracID", Convert.ToInt32(textBox3.Text));

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    label12.Text = rotaAdi;
                    MessageBox.Show($"Araç {rotaAdi} rotasına yönlendirildi ve durumu 'Görevde' olarak güncellendi.");

                    AraclariListele();
                    textBox3.Clear();
                }
                else
                {
                    MessageBox.Show("Araç bulunamadı! ID'yi kontrol edin.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Görev atama hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Admin adminPanel = new Admin();
            adminPanel.Show();
        }
    }
}