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

namespace nesne2
{
    public partial class PersonelEkle : Form
    {
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        public PersonelEkle()
        {
            InitializeComponent();
        }

        private void PersonelEkle_Load(object sender, EventArgs e)
        {
            label1.Text = "Ad:";
            label2.Text = "Soyad:";
            label3.Text = "Doğum Tarihi:";
            label4.Text = "Doğum Yeri:";
            label5.Text = "Telefon:";
            label6.Text = "Email:";
            label7.Text = "Şifre:";

            label8.Text = "Yetkiler:";
            checkBox1.Text = "Is Admin";
            checkBox2.Text = "Departman Yöneticisi";

            label10.Text = "Adres:";

            groupBox1.Text = "Medeni Durum";
            checkBox3.Text = "Evli";
            checkBox4.Text = "Bekar";

            label11.Text = "Çocuk Sayısı:";
            label12.Text = "Unvan:";
            label13.Text = "Maaş:";
            label14.Text = "Zimmetli Araç:";

            button1.Text = "İPTAL";
            button2.Text = "KAYDET";

            VerileriDoldur();
        }

        private void VerileriDoldur()
        {
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                SqlDataAdapter daUnvan = new SqlDataAdapter("SELECT UnvanID, UnvanAdi FROM Unvan", connection);
                DataTable dtUnvan = new DataTable();
                daUnvan.Fill(dtUnvan);

                comboBox1.DataSource = dtUnvan;
                comboBox1.DisplayMember = "UnvanAdi";
                comboBox1.ValueMember = "UnvanID";

                SqlDataAdapter daArac = new SqlDataAdapter("SELECT AracSasiID, Plaka FROM Araclar", connection);
                DataTable dtArac = new DataTable();
                daArac.Fill(dtArac);

                DataRow dr = dtArac.NewRow();
                dr["AracSasiID"] = 0;
                dr["Plaka"] = "Araç Yok / Seçilmedi";
                dtArac.Rows.InsertAt(dr, 0);

                comboBox2.DataSource = dtArac;
                comboBox2.DisplayMember = "Plaka";
                comboBox2.ValueMember = "AracSasiID";
                comboBox2.SelectedIndex = 0;
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

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) checkBox4.Checked = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) checkBox3.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Admin admin = new Admin(); 
            admin.Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Lütfen Ad ve Soyad alanlarını doldurunuz.");
                return;
            }

            try
            {
                Personel yeniPersonel = new Personel();

                yeniPersonel.Ad = textBox1.Text;
                yeniPersonel.Soyad = textBox2.Text;
                yeniPersonel.DogumTarihi = dateTimePicker1.Value;
                yeniPersonel.DogumYeri = textBox3.Text;
                yeniPersonel.Telefon = textBox4.Text;
                yeniPersonel.Email = textBox5.Text;
                yeniPersonel.Sifre = textBox6.Text;
                yeniPersonel.IsAdmin = checkBox1.Checked;
                yeniPersonel.DepartmanYoneticisi = checkBox2.Checked;
                yeniPersonel.Adres = textBox7.Text;

                
                int cocukSayisi = 0;
                int.TryParse(textBox8.Text, out cocukSayisi);
                yeniPersonel.CocukSayisi = cocukSayisi;

                decimal maas = 0;
                decimal.TryParse(textBox9.Text, out maas);
                yeniPersonel.Maas = maas;

                yeniPersonel.Yas = DateTime.Now.Year - dateTimePicker1.Value.Year;
                yeniPersonel.MaasYattiMi = false; 

                yeniPersonel.UnvanID = Convert.ToInt32(comboBox1.SelectedValue);

                if (Convert.ToInt32(comboBox2.SelectedValue) != 0)
                    yeniPersonel.AracSasiID = Convert.ToInt32(comboBox2.SelectedValue);
                else
                    yeniPersonel.AracSasiID = null;


                string medeniDurum = "Belirtilmedi";
                if (checkBox3.Checked) medeniDurum = "Evli";
                if (checkBox4.Checked) medeniDurum = "Bekar";


                if (connection.State == ConnectionState.Closed) connection.Open();

                string insertQuery = @"
                    INSERT INTO Personel (
                        Ad, Soyad, DogumTarihi, DogumYeri, Telefon, Email, Sifre, 
                        IsAdmin, DepartmanYoneticisi, Adres, MedeniDurum, CocukSayisi, 
                        UnvanID, Maas, AracSasiID, MaasYattimi, Yas
                    ) VALUES (
                        @Ad, @Soyad, @DogumTarihi, @DogumYeri, @Telefon, @Email, @Sifre,
                        @IsAdmin, @DepartmanYonetici, @Adres, @Medeni, @Cocuk,
                        @UnvanID, @Maas, @AracID, @MaasYatti, @Yas
                    )";

                SqlCommand cmd = new SqlCommand(insertQuery, connection);

                cmd.Parameters.AddWithValue("@Ad", yeniPersonel.Ad);
                cmd.Parameters.AddWithValue("@Soyad", yeniPersonel.Soyad);
                cmd.Parameters.AddWithValue("@DogumTarihi", yeniPersonel.DogumTarihi);
                cmd.Parameters.AddWithValue("@DogumYeri", yeniPersonel.DogumYeri);
                cmd.Parameters.AddWithValue("@Telefon", yeniPersonel.Telefon);
                cmd.Parameters.AddWithValue("@Email", yeniPersonel.Email);
                cmd.Parameters.AddWithValue("@Sifre", yeniPersonel.Sifre);
                cmd.Parameters.AddWithValue("@IsAdmin", yeniPersonel.IsAdmin);
                cmd.Parameters.AddWithValue("@DepartmanYonetici", yeniPersonel.DepartmanYoneticisi);
                cmd.Parameters.AddWithValue("@Adres", yeniPersonel.Adres);
                cmd.Parameters.AddWithValue("@Cocuk", yeniPersonel.CocukSayisi);
                cmd.Parameters.AddWithValue("@UnvanID", yeniPersonel.UnvanID);
                cmd.Parameters.AddWithValue("@Maas", yeniPersonel.Maas);
                cmd.Parameters.AddWithValue("@MaasYatti", yeniPersonel.MaasYattiMi);
                cmd.Parameters.AddWithValue("@Yas", yeniPersonel.Yas);

                if (yeniPersonel.AracSasiID.HasValue)
                    cmd.Parameters.AddWithValue("@AracID", yeniPersonel.AracSasiID.Value);
                else
                    cmd.Parameters.AddWithValue("@AracID", DBNull.Value);

                cmd.Parameters.AddWithValue("@Medeni", medeniDurum);

                int sonuc = cmd.ExecuteNonQuery();

                if (sonuc > 0)
                {
                    MessageBox.Show("Personel nesnesi oluşturuldu ve veritabanına eklendi.\nGeçici TC: " + yeniPersonel.TC);
                    AlanlariTemizle();
                }
                else
                {
                    MessageBox.Show("Kayıt başarısız.");
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

        private void label5_Click(object sender, EventArgs e) { }

        private void AlanlariTemizle()
        {
            textBox1.Clear(); textBox2.Clear(); textBox4.Clear();
            textBox5.Clear(); textBox6.Clear(); textBox7.Clear();
            textBox8.Clear(); textBox9.Clear(); textBox3.Clear();
            checkBox1.Checked = false; checkBox2.Checked = false;
            checkBox3.Checked = false; checkBox4.Checked = false;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
        }
    }
}