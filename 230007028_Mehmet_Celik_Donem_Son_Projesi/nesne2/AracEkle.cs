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
    public partial class AracEkle : Form
    {
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        public AracEkle()
        {
            InitializeComponent();
        }

        private void AracEkle_Load(object sender, EventArgs e)
        {
            label1.Text = "Plaka:";
            label2.Text = "Marka:";
            label3.Text = "Model:";
            label4.Text = "Kapasite (Ton):";
            label5.Text = "Araç Tipi:";
            label6.Text = "Rota:";
            label7.Text = "Durum:";

            button1.Text = "KAYDET";
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
                MessageBox.Show("Veri yükleme hatası: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e) { }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string plaka = textBox1.Text.ToUpper();
                string marka = textBox2.Text;
                string model = textBox3.Text;

                if (!decimal.TryParse(textBox4.Text, out decimal kapasite))
                {
                    MessageBox.Show("Kapasite sayısal olmalıdır (Örn: 15.5)");
                    return;
                }

                int tipID = Convert.ToInt32(comboBox1.SelectedValue);
                int rotaID = Convert.ToInt32(comboBox2.SelectedValue);
                int durumID = Convert.ToInt32(comboBox3.SelectedValue);

                if (connection.State == ConnectionState.Closed) connection.Open();

                string insertQuery = @"
                    INSERT INTO Araclar (Plaka, Marka, Modeli, Kapasite, AracTipiID, RotaID, AracDurumID) 
                    VALUES (@Plaka, @Marka, @Model, @Kapasite, @TipID, @RotaID, @DurumID)";

                SqlCommand cmd = new SqlCommand(insertQuery, connection);

                cmd.Parameters.AddWithValue("@Plaka", plaka);
                cmd.Parameters.AddWithValue("@Marka", marka);
                cmd.Parameters.AddWithValue("@Model", model);
                cmd.Parameters.AddWithValue("@Kapasite", kapasite);
                cmd.Parameters.AddWithValue("@TipID", tipID);
                cmd.Parameters.AddWithValue("@RotaID", rotaID);
                cmd.Parameters.AddWithValue("@DurumID", durumID);

                int sonuc = cmd.ExecuteNonQuery();

                if (sonuc > 0)
                {
                    MessageBox.Show("Araç başarıyla eklendi.", "Başarılı");

                    textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear();
               
                    if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Kayıt eklenemedi.");
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Admin admin = new Admin();
            admin.Show();
        }
    }
}