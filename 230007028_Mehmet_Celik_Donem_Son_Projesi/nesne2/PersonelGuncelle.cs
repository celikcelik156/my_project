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
    public partial class PersonelGüncelle : Form
    {
        SqlConnection connection = new SqlConnection("Data Source=--MEHMETCELIK--;Initial Catalog=AtikToplama;Integrated Security=True");

        long bulunanPersonelTC = 0;

        public PersonelGüncelle()
        {
            InitializeComponent();
        }

        private void PersonelGuncelle_Load(object sender, EventArgs e)
        {
            label15.Text = "TC Kimlik No:"; 
            button3.Text = "BUL";

            label1.Text = "Ad:";
            label2.Text = "Soyad:";
            label3.Text = "Doğum Tarihi:";
            label4.Text = "Doğum Yeri:";
            label5.Text = "Telefon:";
            label6.Text = "Email:";
            label7.Text = "Şifre:";

            label8.Text = "Yetkiler:";
            checkBox1.Text = "Yönetici (Admin)";
            checkBox2.Text = "Departman Yöneticisi";

            label10.Text = "Adres:";

            groupBox1.Text = "Medeni Durum";
            checkBox3.Text = "Evli";
            checkBox4.Text = "Bekar";

            label11.Text = "Çocuk Sayısı:";
            label12.Text = "Unvan:";
            label13.Text = "Maaş:";
            label14.Text = "Zimmetli Araç:";

            button1.Text = "GÜNCELLE";
            button2.Text = "İPTAL";

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
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TextBox txtArama = (TextBox)Controls.Find("textBox10", true)[0]; 
            string arananTC = txtArama.Text;

            if (string.IsNullOrEmpty(arananTC))
            {
                MessageBox.Show("Lütfen aranacak personelin TC kimlik numarasını giriniz.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Personel WHERE TC = @TC", connection);
                cmd.Parameters.AddWithValue("@TC", arananTC);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    bulunanPersonelTC = Convert.ToInt64(dr["TC"]);

                    textBox1.Text = dr["Ad"].ToString();
                    textBox2.Text = dr["Soyad"].ToString();
                    
                    if (dr["DogumTarihi"] != DBNull.Value)
                        dateTimePicker1.Value = Convert.ToDateTime(dr["DogumTarihi"]);

                    textBox3.Text = dr["DogumYeri"].ToString();
                    textBox4.Text = dr["Telefon"].ToString();
                    textBox5.Text = dr["Email"].ToString();
                    textBox6.Text = dr["Sifre"].ToString();

                    checkBox1.Checked = dr["IsAdmin"] != DBNull.Value && Convert.ToBoolean(dr["IsAdmin"]);
                    checkBox2.Checked = dr["DepartmanYoneticisi"] != DBNull.Value && Convert.ToBoolean(dr["DepartmanYoneticisi"]);

                    textBox7.Text = dr["Adres"].ToString();

                    string medeni = dr["MedeniDurum"].ToString();
                    if (medeni == "Evli") { checkBox3.Checked = true; checkBox4.Checked = false; }
                    else if (medeni == "Bekar") { checkBox4.Checked = true; checkBox3.Checked = false; }
                    else { checkBox3.Checked = false; checkBox4.Checked = false; }

                    textBox8.Text = dr["CocukSayisi"].ToString();
                    textBox9.Text = dr["Maas"].ToString();

                    if (dr["UnvanID"] != DBNull.Value) 
                        comboBox1.SelectedValue = Convert.ToInt32(dr["UnvanID"]);
                    
                    if (dr["AracSasiID"] != DBNull.Value) 
                        comboBox2.SelectedValue = Convert.ToInt32(dr["AracSasiID"]);
                    else 
                        comboBox2.SelectedIndex = 0;

                    MessageBox.Show("Personel bilgileri getirildi. Düzenleyip GÜNCELLE butonuna basabilirsiniz.");
                }
                else
                {
                    MessageBox.Show("Bu TC numarasına ait personel bulunamadı.");
                    bulunanPersonelTC = 0;
                    Temizle();
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama sırasında hata: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bulunanPersonelTC == 0)
            {
                MessageBox.Show("Lütfen önce sol taraftan TC girip BUL butonuna basınız.");
                return;
            }

            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                string medeniDurum = "Belirtilmedi";
                if (checkBox3.Checked) medeniDurum = "Evli";
                if (checkBox4.Checked) medeniDurum = "Bekar";

                string updateQuery = @"
                    UPDATE Personel SET 
                        Ad=@Ad, Soyad=@Soyad, DogumTarihi=@DogumTarihi, DogumYeri=@DogumYeri,
                        Telefon=@Telefon, Email=@Email, Sifre=@Sifre, 
                        IsAdmin=@IsAdmin, DepartmanYoneticisi=@DY, Adres=@Adres,
                        MedeniDurum=@Medeni, CocukSayisi=@Cocuk, UnvanID=@Unvan, 
                        Maas=@Maas, AracSasiID=@Arac, Yas=@Yas
                    WHERE TC=@TC";

                SqlCommand cmd = new SqlCommand(updateQuery, connection);

                cmd.Parameters.AddWithValue("@Ad", textBox1.Text);
                cmd.Parameters.AddWithValue("@Soyad", textBox2.Text);
                cmd.Parameters.AddWithValue("@DogumTarihi", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@DogumYeri", textBox4.Text);
                cmd.Parameters.AddWithValue("@Telefon", textBox5.Text);
                cmd.Parameters.AddWithValue("@Email", textBox6.Text);
                cmd.Parameters.AddWithValue("@Sifre", textBox7.Text);
                
                cmd.Parameters.AddWithValue("@IsAdmin", checkBox1.Checked);
                cmd.Parameters.AddWithValue("@DY", checkBox2.Checked);
                
                cmd.Parameters.AddWithValue("@Adres", textBox10.Text);
                cmd.Parameters.AddWithValue("@Medeni", medeniDurum);

                int cocuk = 0; int.TryParse(textBox8.Text, out cocuk);
                cmd.Parameters.AddWithValue("@Cocuk", cocuk);

                decimal maas = 0; decimal.TryParse(textBox9.Text, out maas);
                cmd.Parameters.AddWithValue("@Maas", maas);

                int yas = DateTime.Now.Year - dateTimePicker1.Value.Year;
                cmd.Parameters.AddWithValue("@Yas", yas);

                cmd.Parameters.AddWithValue("@Unvan", Convert.ToInt32(comboBox1.SelectedValue));

                object aracID = DBNull.Value;
                if (comboBox2.SelectedValue != null && Convert.ToInt32(comboBox2.SelectedValue) != 0)
                    aracID = Convert.ToInt32(comboBox2.SelectedValue);
                
                cmd.Parameters.AddWithValue("@Arac", aracID);

                cmd.Parameters.AddWithValue("@TC", bulunanPersonelTC);

                int sonuc = cmd.ExecuteNonQuery();
                if (sonuc > 0)
                {
                    MessageBox.Show("Personel bilgileri başarıyla güncellendi.");
                    Temizle();
                    bulunanPersonelTC = 0;
                }
                else
                {
                    MessageBox.Show("Güncelleme başarısız oldu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
            }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Admin admin = new Admin();
            admin.Show();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) checkBox4.Checked = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) checkBox3.Checked = false;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) checkBox1.Checked = false;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) checkBox2.Checked = false;
        }

        private void Temizle()
        {
            textBox1.Clear(); textBox2.Clear(); textBox4.Clear();
            textBox5.Clear(); textBox6.Clear(); textBox7.Clear();
            textBox10.Clear(); textBox3.Clear(); textBox8.Clear();
            checkBox1.Checked = false; checkBox2.Checked = false;
            checkBox3.Checked = false; checkBox4.Checked = false;
            if(comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
            
            if (Controls.Find("textBox15", true).Length > 0)
               Controls.Find("textBox15", true)[0].Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}