using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmLogin : Form
    {
        private Panel pnlBackground;
        private Label lblTitle;
        private Label lblSubtitle;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnKeyboard;
        private Button btnExit;
        private Label lblError;

        public frmLogin()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            ApplyCustomLoginTheme();
        }

        private void InitializeComponent()
        {
            this.pnlBackground = new Panel();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnKeyboard = new Button();
            this.btnExit = new Button();
            this.lblError = new Label();

            this.SuspendLayout();

            // Form
            this.ClientSize = new Size(400, 560);
            this.Name = "frmLogin";
            this.Text = "Kullanıcı Girişi";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Background Panel
            this.pnlBackground.Dock = DockStyle.Fill;
            this.Controls.Add(this.pnlBackground);

            // Title
            this.lblTitle.Text = "Market Otomasyonu";
            this.lblTitle.AutoSize = false;
            this.lblTitle.Size = new Size(300, 40);
            this.lblTitle.Location = new Point(50, 50);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.pnlBackground.Controls.Add(this.lblTitle);

            // Subtitle
            this.lblSubtitle.Text = "Lütfen Giriş Yapın";
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Size = new Size(300, 30);
            this.lblSubtitle.Location = new Point(50, 100);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.pnlBackground.Controls.Add(this.lblSubtitle);

            // Kullanıcı Adı
            Label lblUserText = new Label() { Text = "Kullanıcı Adı", Location = new Point(50, 160), AutoSize = true };
            this.pnlBackground.Controls.Add(lblUserText);

            this.txtUsername.Location = new Point(50, 190);
            this.txtUsername.Size = new Size(300, 30);
            this.pnlBackground.Controls.Add(this.txtUsername);

            // Şifre
            Label lblPassText = new Label() { Text = "Şifre", Location = new Point(50, 240), AutoSize = true };
            this.pnlBackground.Controls.Add(lblPassText);

            this.txtPassword.Location = new Point(50, 270);
            this.txtPassword.Size = new Size(300, 30);
            this.txtPassword.PasswordChar = '*';
            this.pnlBackground.Controls.Add(this.txtPassword);

            // Hata Mesajı
            this.lblError.Location = new Point(50, 310);
            this.lblError.Size = new Size(300, 30);
            this.lblError.ForeColor = ThemeManager.DangerColor;
            this.lblError.TextAlign = ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            this.pnlBackground.Controls.Add(this.lblError);

            // Giriş Butonu
            this.btnLogin.Text = "GİRİŞ YAP";
            this.btnLogin.Location = new Point(50, 350);
            this.btnLogin.Size = new Size(300, 45);
            this.btnLogin.Click += BtnLogin_Click;
            this.pnlBackground.Controls.Add(this.btnLogin);

            // Klavye Butonu
            this.btnKeyboard.Text = "SANAL KLAVYE AÇ";
            this.btnKeyboard.Location = new Point(50, 410);
            this.btnKeyboard.Size = new Size(300, 45);
            this.btnKeyboard.Click += (s, e) => {
                TextBox target = txtUsername.Focused ? txtUsername : (txtPassword.Focused ? txtPassword : txtUsername);
                using (var kbd = new frmKeyboard(target.Text, target.UseSystemPasswordChar)) {
                    if (kbd.ShowDialog() == DialogResult.OK) {
                        target.Text = kbd.InputText;
                        target.Focus();
                    }
                }
            };
            this.pnlBackground.Controls.Add(this.btnKeyboard);

            // Çıkış Butonu
            this.btnExit.Text = "ÇIKIŞ";
            this.btnExit.Location = new Point(50, 470);
            this.btnExit.Size = new Size(300, 45);
            this.btnExit.Click += (s, e) => Application.Exit();
            this.pnlBackground.Controls.Add(this.btnExit);

            // Varsayılan Kullanıcı Bilgileri
            this.txtUsername.Text = "kasiyer";
            this.txtPassword.Text = "1234";

            this.ResumeLayout(false);
        }

        private void ApplyCustomLoginTheme()
        {
            this.lblTitle.Font = ThemeManager.HeaderFont;
            this.lblSubtitle.Font = new Font("Segoe UI", 12F, FontStyle.Italic);
            
            this.btnLogin.BackColor = ThemeManager.PrimaryColor;
            this.btnLogin.ForeColor = ThemeManager.PrimaryTextColor;
            
            this.btnKeyboard.BackColor = ThemeManager.SecondaryColor;
            this.btnKeyboard.ForeColor = ThemeManager.PrimaryTextColor;
            
            this.btnExit.BackColor = ThemeManager.DangerColor;
            this.btnExit.ForeColor = ThemeManager.PrimaryTextColor;

            this.pnlBackground.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.pnlBackground.ClientRectangle, Color.Gray, ButtonBorderStyle.Solid);
            };
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Kullanıcı adı ve şifre boş bırakılamaz.";
                lblError.Visible = true;
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                using (var db = new MarketDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Username == username && u.IsActive);
                    if (user != null && SecurityHelper.VerifyPassword(password, user.PasswordHash))
                    {
                        ActiveUser.Id = user.Id;
                        ActiveUser.Username = user.Username;
                        ActiveUser.FullName = user.FullName;
                        ActiveUser.Role = user.Role;

                        Logger.LogActivity("LOGIN", $"{user.Username} sisteme giriş yaptı.");

                        Form nextForm;
                        if (ActiveUser.IsAdmin)
                        {
                            nextForm = new frmDashboard();
                        }
                        else
                        {
                            nextForm = new frmPOS();
                        }

                        this.Hide();
                        nextForm.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        lblError.Text = "Kullanıcı adı veya şifre hatalı!";
                        lblError.Visible = true;
                        
                        Logger.LogActivity("LOGIN_FAILED", $"Başarısız giriş denemesi: {username}");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
