using System.Drawing;
using System.Windows.Forms;
using MarketAutomation.Forms;

namespace MarketAutomation.Helpers
{
    public static class ThemeManager
    {
        public static Color PrimaryColor = Color.FromArgb(41, 128, 185);
        public static Color SecondaryColor = Color.FromArgb(52, 73, 94);
        public static Color BackgroundColor = Color.FromArgb(236, 240, 241);
        public static Color SurfaceColor = Color.White;
        public static Color TextColor = Color.FromArgb(44, 62, 80);
        public static Color PrimaryTextColor = Color.White;
        public static Color SuccessColor = Color.FromArgb(39, 174, 96);
        public static Color DangerColor = Color.FromArgb(192, 57, 43);

        public static Font PrimaryFont = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        public static Font HeaderFont = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);

        private static Icon? _appIcon;

        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.ForeColor = TextColor;
            form.Font = PrimaryFont;
            
            // Apply shopping cart icon to all forms
            try
            {
                if (_appIcon == null)
                {
                    string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                    if (File.Exists(iconPath))
                        _appIcon = new Icon(iconPath);
                }
                if (_appIcon != null)
                    form.Icon = _appIcon;
            }
            catch { /* icon yüklenemezse devam et */ }
            
            string fName = form.GetType().Name;
            if (fName == "frmLogin" || fName == "frmDashboard" || fName == "frmPOS")
            {
                form.FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                form.FormBorderStyle = FormBorderStyle.Sizable;
            }
            
            form.StartPosition = FormStartPosition.CenterScreen;

            ApplyThemeToControls(form.Controls);
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    
                    if (btn.BackColor == Control.DefaultBackColor || btn.BackColor == Color.Empty || btn.BackColor == SystemColors.Control)
                        btn.BackColor = PrimaryColor;
                        
                    if (btn.ForeColor == Control.DefaultForeColor || btn.ForeColor == Color.Empty || btn.ForeColor == SystemColors.ControlText)
                        btn.ForeColor = PrimaryTextColor;
                        
                    if (btn.Font.Name == Control.DefaultFont.Name || btn.Font.Name == "Microsoft Sans Serif")
                        btn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                        
                    btn.Cursor = Cursors.Hand;
                }
                else if (control is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = PrimaryFont;
                    txt.BackColor = SurfaceColor;
                    txt.ForeColor = TextColor;

                    // NO_NAV tag'li TextBox'larda navigasyon ekleme (barkod okuyucu gibi alanlar)
                    if (!txt.Multiline && txt.Tag?.ToString() != "NO_NAV")
                    {
                        txt.KeyDown -= TextBox_Navigation_KeyDown;
                        txt.KeyDown += TextBox_Navigation_KeyDown;
                    }

                    // Global Sanal Klavye Tetikleyicisi (Çift Tıklama)
                    txt.DoubleClick -= TextBox_VirtualKeyboard_DoubleClick;
                    txt.DoubleClick += TextBox_VirtualKeyboard_DoubleClick;
                }
                else if (control is ComboBox cb)
                {
                    cb.KeyDown -= ComboBox_Navigation_KeyDown;
                    cb.KeyDown += ComboBox_Navigation_KeyDown;
                }
                else if (control is Label lbl)
                {
                    lbl.ForeColor = TextColor;
                }
                else if (control is Panel panel)
                {
                    panel.BackColor = SurfaceColor;
                }

                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        private static void TextBox_Navigation_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                SendKeys.Send("+{TAB}");
            }
        }

        private static void ComboBox_Navigation_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private static void TextBox_VirtualKeyboard_DoubleClick(object? sender, System.EventArgs e)
        {
            if (sender is TextBox txt)
            {
                using (var kbd = new frmKeyboard(txt.Text, txt.UseSystemPasswordChar))
                {
                    if (kbd.ShowDialog() == DialogResult.OK)
                    {
                        txt.Text = kbd.InputText;
                        txt.SelectionStart = txt.Text.Length;
                    }
                }
            }
        }
    }
}
