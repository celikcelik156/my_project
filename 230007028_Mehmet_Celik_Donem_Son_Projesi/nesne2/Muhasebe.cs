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
    public partial class Muhasebe : Form
    {
        public int gelenKullaniciID;
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        public Muhasebe(int id)
        {
            InitializeComponent();
            gelenKullaniciID = id;
        }

        private void Muhasebe_Load(object sender, EventArgs e)
        {
            KullaniciBilgileriGetir();
            PersonelListesiniGetir();
        }

        private void KullaniciBilgileriGetir()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string query = @"
                    SELECT 
                        P.TC, P.Ad, P.Soyad, P.Yas, P.Telefon, P.Adres, P.MedeniDurum, 
                        P.DepartmanYoneticisi, U.UnvanAdi
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
                    {
                        label14.Text = "Departman Yöneticisi";
                        label14.ForeColor = Color.Green;
                    }
                    else
                    {
                        label14.Text = "Personel";
                        label14.ForeColor = Color.Black;
                    }

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

        private void PersonelListesiniGetir()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string query = "SELECT TC, Ad, Soyad, Maas, CocukSayisi, MaasYattimi FROM Personel";

                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Lütfen Personel TC ve Yeni Maaş giriniz.");
                return;
            }

            decimal yeniMaas = 0;
            if (!decimal.TryParse(textBox2.Text, out yeniMaas))
            {
                MessageBox.Show("Lütfen maaş kısmına geçerli bir sayı giriniz.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string kontrolQuery = @"
                    SELECT U.MaasMin, U.MaasMax, U.UnvanAdi 
                    FROM Personel P
                    INNER JOIN Unvan U ON P.UnvanID = U.UnvanID
                    WHERE P.TC = @TC";

                SqlCommand cmdKontrol = new SqlCommand(kontrolQuery, connection);
                cmdKontrol.Parameters.AddWithValue("@TC", textBox1.Text);

                SqlDataReader dr = cmdKontrol.ExecuteReader();

                if (dr.Read())
                {
                    decimal minMaas = Convert.ToDecimal(dr["MaasMin"]);
                    decimal maxMaas = Convert.ToDecimal(dr["MaasMax"]);
                    string unvanAdi = dr["UnvanAdi"].ToString();

                    dr.Close();

                    if (yeniMaas < minMaas || yeniMaas > maxMaas)
                    {
                        MessageBox.Show($"HATA: '{unvanAdi}' unvanı için maaş aralığı:\nMin: {minMaas:C2}\nMax: {maxMaas:C2}\nLütfen bu aralıkta bir değer giriniz.", "Maaş Sınırı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string updateQuery = "UPDATE Personel SET Maas = @Maas WHERE TC = @TC";
                    SqlCommand cmdUpdate = new SqlCommand(updateQuery, connection);
                    cmdUpdate.Parameters.AddWithValue("@Maas", yeniMaas);
                    cmdUpdate.Parameters.AddWithValue("@TC", textBox1.Text);

                    int sonuc = cmdUpdate.ExecuteNonQuery();
                    if (sonuc > 0)
                    {
                        MessageBox.Show("Personel maaşı başarıyla güncellendi.");
                        textBox1.Clear();
                        textBox2.Clear();
                        PersonelListesiniGetir();
                    }
                }
                else
                {
                    dr.Close();
                    MessageBox.Show("Bu TC kimlik numarasına sahip personel bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (DateTime.Now.Day != 15)
            {
                MessageBox.Show("Maaşlar sadece ayın 15'inde yatırılabilir!\nBugün ayın: " + DateTime.Now.Day, "Tarih Hatası", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult cevap = MessageBox.Show("Tüm personelin maaşlarına 'Çocuk Sayısı x 400 TL' eklenerek yatırılacak.\nOnaylıyor musunuz?", "Maaş Ödemesi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                try
                {
                    if (connection.State == ConnectionState.Closed) connection.Open();

                    string updateQuery = @"
                        UPDATE Personel 
                        SET Maas = Maas + (ISNULL(CocukSayisi, 0) * 400), 
                            MaasYattimi = 1";

                    SqlCommand cmd = new SqlCommand(updateQuery, connection);

                    int etkilenen = cmd.ExecuteNonQuery();

                    if (etkilenen > 0)
                    {
                        MessageBox.Show($"{etkilenen} personelin maaşına çocuk yardımı eklendi ve başarıyla yatırıldı.");
                        PersonelListesiniGetir(); 
                    }
                    else
                    {
                        MessageBox.Show("İşlem yapılacak personel bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Maaş yatırma hatası: " + ex.Message);
                }
                finally { if (connection.State == ConnectionState.Open) connection.Close(); }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Giris giris = new Giris();
            this.Close();
            giris.Show();
        }

        private void groupBox1_Enter(object sender, EventArgs e) { }
    }
}