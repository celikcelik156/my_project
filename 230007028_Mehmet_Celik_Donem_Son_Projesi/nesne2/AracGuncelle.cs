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
    public partial class AracGuncelle : Form
    {
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        int bulunanAracID = 0;

        public AracGuncelle()
        {
            InitializeComponent();
        }

        private void AracGuncelle_Load(object sender, EventArgs e)
        {
            label8.Text = "Araç ID Giriniz:";
            button3.Text = "BUL"; 

            label1.Text = "Plaka:";
            label2.Text = "Marka:";
            label3.Text = "Model:";
            label4.Text = "Kapasite (Ton):";
            label5.Text = "Araç Tipi:";
            label6.Text = "Rota:";
            label7.Text = "Durum:";

            button1.Text = "GÜNCELLE";
            button2.Text = "İPTAL";

            ComboBoxlariDoldur();
        }

        private void ComboBoxlariDoldur()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                SqlDataAdapter daTip = new SqlDataAdapter("SELECT AracTipiID, TipAdi FROM AracTipi", connection);
                DataTable dtTip = new DataTable();
                daTip.Fill(dtTip);
                comboBox1.DataSource = dtTip;
                comboBox1.DisplayMember = "TipAdi";
                comboBox1.ValueMember = "AracTipiID";

                SqlDataAdapter daRota = new SqlDataAdapter("SELECT RotaID, RotaAdi FROM Rota", connection);
                DataTable dtRota = new DataTable();
                daRota.Fill(dtRota);
                comboBox2.DataSource = dtRota;
                comboBox2.DisplayMember = "RotaAdi";
                comboBox2.ValueMember = "RotaID";

                SqlDataAdapter daDurum = new SqlDataAdapter("SELECT DurumID, DurumAdi FROM Durum", connection);
                DataTable dtDurum = new DataTable();
                daDurum.Fill(dtDurum);
                comboBox3.DataSource = dtDurum;
                comboBox3.DisplayMember = "DurumAdi";
                comboBox3.ValueMember = "DurumID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Liste yükleme hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void AlanlariTemizle()
        {
            textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear();
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
            if (comboBox3.Items.Count > 0) comboBox3.SelectedIndex = 0;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string arananID = textBox5.Text;

            if (string.IsNullOrEmpty(arananID))
            {
                MessageBox.Show("Lütfen bir Araç ID giriniz.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Araclar WHERE AracSasiID = @ID", connection);
                cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(arananID));

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    bulunanAracID = Convert.ToInt32(dr["AracSasiID"]);

                    textBox1.Text = dr["Plaka"].ToString();
                    textBox2.Text = dr["Marka"].ToString();
                    textBox3.Text = dr["Modeli"].ToString();
                    textBox4.Text = dr["Kapasite"].ToString();

                    if (dr["AracTipiID"] != DBNull.Value) comboBox1.SelectedValue = Convert.ToInt32(dr["AracTipiID"]);
                    if (dr["RotaID"] != DBNull.Value) comboBox2.SelectedValue = Convert.ToInt32(dr["RotaID"]);
                    if (dr["AracDurumID"] != DBNull.Value) comboBox3.SelectedValue = Convert.ToInt32(dr["AracDurumID"]);

                    MessageBox.Show("Araç bilgileri getirildi.");
                }
                else
                {
                    MessageBox.Show("Bu ID'ye sahip araç bulunamadı.");
                    AlanlariTemizle();
                    bulunanAracID = 0;
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (bulunanAracID == 0)
            {
                MessageBox.Show("Lütfen önce güncellenecek aracı BUL butonuna basarak seçiniz.");
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Plaka ve Marka alanları boş olamaz.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string updateQuery = @"
                    UPDATE Araclar 
                    SET Plaka = @Plaka, 
                        Marka = @Marka, 
                        Modeli = @Model, 
                        Kapasite = @Kapasite, 
                        AracTipiID = @TipID, 
                        RotaID = @RotaID, 
                        AracDurumID = @DurumID
                    WHERE AracSasiID = @ID";

                SqlCommand cmd = new SqlCommand(updateQuery, connection);

                cmd.Parameters.AddWithValue("@Plaka", textBox1.Text.ToUpper());
                cmd.Parameters.AddWithValue("@Marka", textBox2.Text);
                cmd.Parameters.AddWithValue("@Model", textBox3.Text);

                decimal kapasite = 0;
                decimal.TryParse(textBox4.Text, out kapasite);
                cmd.Parameters.AddWithValue("@Kapasite", kapasite);

                cmd.Parameters.AddWithValue("@TipID", Convert.ToInt32(comboBox1.SelectedValue));
                cmd.Parameters.AddWithValue("@RotaID", Convert.ToInt32(comboBox2.SelectedValue));
                cmd.Parameters.AddWithValue("@DurumID", Convert.ToInt32(comboBox3.SelectedValue));

                cmd.Parameters.AddWithValue("@ID", bulunanAracID);

                int sonuc = cmd.ExecuteNonQuery();

                if (sonuc > 0)
                {
                    MessageBox.Show("Araç başarıyla güncellendi.");
                }
                else
                {
                    MessageBox.Show("Güncelleme yapılamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Admin admin = new Admin();
            admin.Show();
        }
    }
}