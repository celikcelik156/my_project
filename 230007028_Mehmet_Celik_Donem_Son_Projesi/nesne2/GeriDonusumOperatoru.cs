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
    public partial class GeriDonusumOperatoru : Form
    {
        public int gelenKullaniciID;
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");
        int tesisID = 1;
        int AtikIslemID = 0;

        decimal tesisKapasitesi = 0;
        decimal mevcutAtikMiktari = 0;

        public GeriDonusumOperatoru(int id)
        {
            InitializeComponent();
            gelenKullaniciID = id;
        }

        private void GeriDonusumOperatoru_Load(object sender, EventArgs e)
        {
            VerileriGuncelle();
            VerimHesapla();
        }

        private void VerileriGuncelle()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string queryPersonel = @"
                    SELECT P.TC, P.Ad, P.Soyad, P.Yas, P.Telefon, P.Adres, P.MedeniDurum, 
                           P.DepartmanYoneticisi, U.UnvanAdi
                    FROM Personel P
                    LEFT JOIN Unvan U ON P.UnvanID = U.UnvanID
                    WHERE P.TC = @TC";

                SqlCommand cmdPersonel = new SqlCommand(queryPersonel, connection);
                cmdPersonel.Parameters.AddWithValue("@TC", gelenKullaniciID);
                SqlDataReader readerP = cmdPersonel.ExecuteReader();

                if (readerP.Read())
                {
                    label9.Text = readerP["TC"].ToString();
                    label10.Text = readerP["Ad"].ToString() + " " + readerP["Soyad"].ToString();
                    label11.Text = readerP["Yas"].ToString();
                    label12.Text = readerP["Telefon"].ToString();
                    label13.Text = readerP["UnvanAdi"].ToString();

                    string yoneticiDurum = readerP["DepartmanYoneticisi"].ToString();
                    if (yoneticiDurum == "True" || yoneticiDurum == "1")
                    {
                        label14.Text = "Departman Yöneticisi";
                        label14.ForeColor = Color.Green;
                    }
                    else
                    {
                        label14.Text = "Personel";
                    }

                    label15.Text = readerP["Adres"].ToString();
                    label16.Text = readerP["MedeniDurum"].ToString();
                }
                readerP.Close();

                SqlCommand cmdTesis = new SqlCommand("SELECT Kapasite, TesisAdi FROM GeriDonusumTesisi WHERE TesisID = @TesisID", connection);
                cmdTesis.Parameters.AddWithValue("@TesisID", tesisID);
                SqlDataReader readerT = cmdTesis.ExecuteReader();
                if (readerT.Read())
                {
                    tesisKapasitesi = Convert.ToDecimal(readerT["Kapasite"]);
                    label1.Text = tesisKapasitesi.ToString("N0");
                    label2.Text = readerT["TesisAdi"].ToString();
                }
                readerT.Close();

                SqlCommand cmdToplam = new SqlCommand("SELECT SUM(Miktar) FROM AtikIslem WHERE TesisID = @TesisID AND IslemTipiID = 1", connection);
                cmdToplam.Parameters.AddWithValue("@TesisID", tesisID);
                object resultToplam = cmdToplam.ExecuteScalar();

                if (resultToplam != DBNull.Value && resultToplam != null)
                    mevcutAtikMiktari = Convert.ToDecimal(resultToplam);
                else
                    mevcutAtikMiktari = 0;

                decimal dolulukOrani = 0;
                if (tesisKapasitesi > 0)
                    dolulukOrani = (mevcutAtikMiktari / tesisKapasitesi) * 100;

                label4.Text = $"{mevcutAtikMiktari:N0} / {tesisKapasitesi:N0} TON\n(Doluluk: %{dolulukOrani:N1})";

                SqlCommand cmdFiyat = new SqlCommand("SELECT AtikAdi, BirimFiyat FROM AtikTurleri", connection);
                SqlDataReader readerF = cmdFiyat.ExecuteReader();
                StringBuilder sbFiyatlar = new StringBuilder();
                while (readerF.Read())
                {
                    sbFiyatlar.AppendLine($"{readerF["AtikAdi"]}: {readerF["BirimFiyat"]} TL/kg");
                }
                label21.Text = sbFiyatlar.ToString();
                readerF.Close();

                SqlCommand cmdStok = new SqlCommand(@"
                    SELECT T.AtikAdi, SUM(I.Miktar) as ToplamMiktar
                    FROM AtikIslem I
                    INNER JOIN AtikTurleri T ON I.AtikID = T.AtikID
                    WHERE I.TesisID = @TesisID AND I.IslemTipiID = 1
                    GROUP BY T.AtikAdi", connection);
                cmdStok.Parameters.AddWithValue("@TesisID", tesisID);
                SqlDataReader readerS = cmdStok.ExecuteReader();

                label5.Text = "0 Kg"; label7.Text = "0 Kg"; label6.Text = "0 Kg";
                label8.Text = "0 Kg"; label27.Text = "0 Kg";

                while (readerS.Read())
                {
                    string atikAdi = readerS["AtikAdi"].ToString();
                    string miktar = Convert.ToDecimal(readerS["ToplamMiktar"]).ToString("N0") + " Kg";

                    switch (atikAdi)
                    {
                        case "Organik": label5.Text = miktar; break;
                        case "Plastik": label7.Text = miktar; break;
                        case "Metal": label6.Text = miktar; break;
                        case "Kağıt": label8.Text = miktar; break;
                        case "Cam": label27.Text = miktar; break;
                    }
                }
                readerS.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private decimal ParseMiktar(string text)
        {
            if (decimal.TryParse(text, out decimal sonuc))
            {
                return sonuc;
            }
            return 0;
        }

        private void AtikVeritabaninaEkle(string atikAdi, decimal miktar, int AtikIslemID)
        {
            int atikID = 0;
            decimal birimFiyat = 0;

            SqlCommand cmdBul = new SqlCommand("SELECT AtikID, BirimFiyat FROM AtikTurleri WHERE AtikAdi = @Ad", connection);
            cmdBul.Parameters.AddWithValue("@Ad", atikAdi);
            SqlDataReader dr = cmdBul.ExecuteReader();
            if (dr.Read())
            {
                atikID = Convert.ToInt32(dr["AtikID"]);
                birimFiyat = Convert.ToDecimal(dr["BirimFiyat"]);
            }
            dr.Close();

            if (atikID == 0) return;

            decimal toplamTutar = miktar * birimFiyat;

            string sqlInsert = "INSERT INTO AtikIslem (TesisID, AtikID, Miktar, IslemTarihi, ToplamTutar, IslemTipiID) VALUES (@Tesis, @Atik, @Miktar, @Tarih, @Tutar, @IslemTipiID)";
            SqlCommand cmdInsert = new SqlCommand(sqlInsert, connection);
            cmdInsert.Parameters.AddWithValue("@Tesis", tesisID);
            cmdInsert.Parameters.AddWithValue("@Atik", atikID);
            cmdInsert.Parameters.AddWithValue("@Miktar", miktar);
            cmdInsert.Parameters.AddWithValue("@Tarih", DateTime.Now);
            cmdInsert.Parameters.AddWithValue("@Tutar", toplamTutar);
            cmdInsert.Parameters.AddWithValue("@IslemTipiID", AtikIslemID);
            cmdInsert.ExecuteNonQuery();
        }

        private void GeriDonusenEkle(string atikAdi, decimal miktar, int AtikIslemID)
        {
            int atikID = 0;
            SqlCommand cmdBul = new SqlCommand("SELECT AtikID FROM AtikTurleri WHERE AtikAdi = @Ad", connection);
            cmdBul.Parameters.AddWithValue("@Ad", atikAdi);
            object sonuc = cmdBul.ExecuteScalar();
            if (sonuc != null) atikID = Convert.ToInt32(sonuc);

            string sql = "INSERT INTO AtikIslem (TesisID, AtikID, Miktar, IslemTarihi, ToplamTutar, IslemTipiID) VALUES (@Tesis, @Atik, @Miktar, @Tarih, 0, @IslemTipiID)";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Tesis", tesisID);
            cmd.Parameters.AddWithValue("@Atik", atikID);
            cmd.Parameters.AddWithValue("@Miktar", miktar);
            cmd.Parameters.AddWithValue("@Tarih", DateTime.Now);
            cmd.Parameters.AddWithValue("@IslemTipiID", AtikIslemID);
            cmd.ExecuteNonQuery();
        }

        private void VerimHesapla()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                decimal toplamGiris = 0;
                SqlCommand cmdGiris = new SqlCommand("SELECT SUM(Miktar) FROM AtikIslem WHERE TesisID = @Tesis AND IslemTipiID = 1", connection);
                cmdGiris.Parameters.AddWithValue("@Tesis", tesisID);
                object objGiris = cmdGiris.ExecuteScalar();
                if (objGiris != DBNull.Value && objGiris != null) 
                    toplamGiris = Convert.ToDecimal(objGiris);

                decimal toplamCikis = 0;
                SqlCommand cmdCikis = new SqlCommand("SELECT SUM(Miktar) FROM AtikIslem WHERE TesisID = @Tesis AND IslemTipiID = 2", connection);
                cmdCikis.Parameters.AddWithValue("@Tesis", tesisID);
                object objCikis = cmdCikis.ExecuteScalar();
                if (objCikis != DBNull.Value && objCikis != null) 
                    toplamCikis = Convert.ToDecimal(objCikis);

                decimal verim = 0;
                if (toplamGiris > 0)
                {
                    verim = (toplamCikis / toplamGiris) * 100;
                }

                label42.Text = "%" + verim.ToString();

                if (verim > 80) label42.ForeColor = Color.Green;
                else if (verim > 50) label42.ForeColor = Color.Orange;
                else label42.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verim hesaplanırken hata: " + ex.Message);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            AtikIslemID = 1;

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Lütfen Gelen Araç ID giriniz!");
                return;
            }

            try
            {
                decimal mOrganik = ParseMiktar(textBox3.Text);
                decimal mPlastik = ParseMiktar(textBox4.Text);
                decimal mMetal = ParseMiktar(textBox5.Text);
                decimal mKagit = ParseMiktar(textBox6.Text);
                decimal mCam = ParseMiktar(textBox7.Text);

                decimal toplamEklenecek = mOrganik + mPlastik + mMetal + mKagit + mCam;

                if (toplamEklenecek <= 0)
                {
                    MessageBox.Show("Lütfen en az bir atık türüne miktar giriniz.");
                    return;
                }

                if ((mevcutAtikMiktari + toplamEklenecek) > tesisKapasitesi)
                {
                    MessageBox.Show("HATA: Tesis kapasitesi yetersiz! İşlem iptal edildi.\nKalan Yer: " + (tesisKapasitesi - mevcutAtikMiktari) + " TON", "Kapasite Dolu", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (connection.State == ConnectionState.Closed) connection.Open();

                if (mOrganik > 0) AtikVeritabaninaEkle("Organik", mOrganik, AtikIslemID);
                if (mPlastik > 0) AtikVeritabaninaEkle("Plastik", mPlastik, AtikIslemID);
                if (mMetal > 0) AtikVeritabaninaEkle("Metal", mMetal, AtikIslemID);
                if (mKagit > 0) AtikVeritabaninaEkle("Kağıt", mKagit, AtikIslemID);
                if (mCam > 0) AtikVeritabaninaEkle("Cam", mCam, AtikIslemID);

                string sqlAracUpdate = "UPDATE Araclar SET AracDurumID = 1 WHERE AracSasiID = @AracID";
                SqlCommand cmdArac = new SqlCommand(sqlAracUpdate, connection);
                cmdArac.Parameters.AddWithValue("@AracID", Convert.ToInt32(textBox2.Text));
                cmdArac.ExecuteNonQuery();

                label35.Text = "Eklendi ";
                MessageBox.Show("Atıklar başarıyla eklendi ve araç Aktif duruma getirildi.", "İşlem Tamam");

                textBox1.Clear(); textBox5.Clear(); textBox6.Clear(); textBox7.Clear(); textBox4.Clear();
                textBox2.Clear(); textBox3.Clear();

                VerileriGuncelle();
                VerimHesapla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AtikIslemID = 2;
            try
            {
                decimal recOrganik = ParseMiktar(textBox12.Text);
                decimal recPlastik = ParseMiktar(textBox11.Text);
                decimal recMetal = ParseMiktar(textBox10.Text);
                decimal recKagit = ParseMiktar(textBox9.Text);
                decimal recCam = ParseMiktar(textBox8.Text);

                decimal toplamDonusen = recOrganik + recPlastik + recMetal + recKagit + recCam;

                if (toplamDonusen <= 0)
                {
                    MessageBox.Show("Lütfen dönüştürülen atık miktarlarını giriniz.");
                    return;
                }

                if (connection.State == ConnectionState.Closed) connection.Open();

                if (recOrganik > 0) GeriDonusenEkle("Organik", recOrganik, AtikIslemID);
                if (recPlastik > 0) GeriDonusenEkle("Plastik", recPlastik, AtikIslemID);
                if (recMetal > 0) GeriDonusenEkle("Metal", recMetal, AtikIslemID);
                if (recKagit > 0) GeriDonusenEkle("Kağıt", recKagit, AtikIslemID);
                if (recCam > 0) GeriDonusenEkle("Cam", recCam, AtikIslemID);

                MessageBox.Show("Geri dönüşüm verileri kaydedildi.", "Başarılı");

                textBox8.Clear(); textBox9.Clear(); textBox10.Clear(); textBox11.Clear(); textBox12.Clear();

                VerimHesapla();
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

        private void button1_Click(object sender, EventArgs e)
        {
            Giris giris = new Giris();
            this.Close();
            giris.Show();
        }

        private void groupBox6_Enter(object sender, EventArgs e) { }
        private void groupBox4_Enter(object sender, EventArgs e) { }
    }
}