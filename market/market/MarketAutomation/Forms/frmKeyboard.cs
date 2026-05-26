using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmKeyboard : Form
    {
        public string InputText { get; private set; }
        private TextBox txtInput;
        private Panel pnlKeyboard;
        private bool isNumeric = false;
        private bool isShift = false;

        public frmKeyboard(string initialText = "", bool isNumeric = false, bool isPassword = false)
        {
            this.InputText = initialText;
            this.isNumeric = isNumeric;
            
            if (isNumeric) this.Size = new Size(400, 600);
            else this.Size = new Size(1060, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = isNumeric ? "Sayısal Tuş Takımı" : "Modern Dokunmatik Türkçe Q Klavye";
            this.BackColor = Color.FromArgb(236, 240, 241); // Modern gri arka plan

            txtInput = new TextBox
            {
                Text = initialText,
                Font = new Font("Segoe UI", isNumeric ? 36 : 32, FontStyle.Bold),
                Location = new Point(20, 20),
                Width = this.Width - 60,
                UseSystemPasswordChar = isPassword,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                TextAlign = isNumeric ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };
            this.Controls.Add(txtInput);

            pnlKeyboard = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(this.Width - 60, isNumeric ? 440 : 400),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pnlKeyboard);

            LoadLayout();
            txtInput.SelectionStart = txtInput.Text.Length;
        }

        private void LoadLayout()
        {
            pnlKeyboard.Controls.Clear();
            
            if (isNumeric)
            {
                LoadNumericLayout();
                return;
            }

            int keyW = 68, keyH = 65, space = 6;

            // Türkçe Q Klavye Dizilimi
            string[] row0_lower = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "*", "-" };
            string[] row0_upper = { "!", "'", "^", "+", "%", "&", "/", "(", ")", "=", "?", "_" };
            string[] row1_lower = { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "ğ", "ü" };
            string[] row1_upper = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "Ğ", "Ü" };
            string[] row2_lower = { "a", "s", "d", "f", "g", "h", "j", "k", "l", "ş", "i" };
            string[] row2_upper = { "A", "S", "D", "F", "G", "H", "J", "K", "L", "Ş", "İ" };
            string[] row3_lower = { "z", "x", "c", "v", "b", "n", "m", "ö", "ç", ".", "," };
            string[] row3_upper = { "Z", "X", "C", "V", "B", "N", "M", "Ö", "Ç", ":", ";" };

            string[] r0 = isShift ? row0_upper : row0_lower;
            string[] r1 = isShift ? row1_upper : row1_lower;
            string[] r2 = isShift ? row2_upper : row2_lower;
            string[] r3 = isShift ? row3_upper : row3_lower;

            int currentY = 0;

            // Row 0 (Sayılar)
            int startX = 0;
            for (int i = 0; i < r0.Length; i++) {
                AddKey(r0[i], startX + i * (keyW + space), currentY, keyW, keyH);
            }
            int bsX = startX + r0.Length * (keyW + space);
            AddKey("Sil ⌫", bsX, currentY, 1000 - bsX, keyH, ThemeManager.DangerColor, Color.White);

            currentY += keyH + space;

            // Row 1 (QWERTY...)
            startX = 40; // Hassas boşlukla Q satırını kaydırma
            for (int i = 0; i < r1.Length; i++) {
                AddKey(r1[i], startX + i * (keyW + space), currentY, keyW, keyH);
            }

            currentY += keyH + space;

            // Row 2 (ASDFG...)
            startX = 60; // A satırı biraz daha sağa
            for (int i = 0; i < r2.Length; i++) {
                AddKey(r2[i], startX + i * (keyW + space), currentY, keyW, keyH);
            }

            currentY += keyH + space;

            // Row 3 (ZXCVB...)
            startX = 0;
            AddKey("BÜYÜT ⇧", startX, currentY, 80, keyH, isShift ? ThemeManager.SuccessColor : ThemeManager.SecondaryColor, Color.White);
            startX = 80 + space;
            for (int i = 0; i < r3.Length; i++) {
                AddKey(r3[i], startX + i * (keyW + space), currentY, keyW, keyH);
            }
            int rsX = startX + r3.Length * (keyW + space);
            if (rsX < 1000) {
                AddKey("BÜYÜT ⇧", rsX, currentY, 1000 - rsX, keyH, isShift ? ThemeManager.SuccessColor : ThemeManager.SecondaryColor, Color.White);
            }

            currentY += keyH + space;

            // Row 4 (Boşluk ve Enter)
            AddKey("TEMİZLE", 0, currentY, 120, keyH, ThemeManager.SecondaryColor, Color.White);
            AddKey("BOŞLUK", 120 + space, currentY, 1000 - 120 - 160 - space * 2, keyH);
            AddKey("GİRİŞ (ENTER)", 1000 - 160, currentY, 160, keyH, ThemeManager.PrimaryColor, Color.White);
        }

        private void LoadNumericLayout()
        {
            int btnW = (pnlKeyboard.Width - 20) / 3;
            int btnH = (pnlKeyboard.Height - 80) / 5;
            int space = 10;
            
            string[,] keys = {
                { "7", "8", "9" },
                { "4", "5", "6" },
                { "1", "2", "3" },
                { "0", ",", "Sil ⌫" }
            };

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    string txt = keys[r, c];
                    Color? bg = (txt == "Sil ⌫") ? ThemeManager.DangerColor : Color.White;
                    Color? fg = (txt == "Sil ⌫") ? Color.White : ThemeManager.TextColor;
                    AddKey(txt, c * (btnW + space), r * (btnH + space), btnW, btnH, bg, fg);
                }
            }

            int bottomY = 4 * (btnH + space);
            AddKey("TEMİZLE", 0, bottomY, btnW, btnH - 10, ThemeManager.SecondaryColor, Color.White);
            AddKey("GİRİŞ (ENTER)", btnW + space, bottomY, btnW * 2 + space, btnH - 10, ThemeManager.PrimaryColor, Color.White);
        }

        private void AddKey(string text, int x, int y, int w, int h, Color? bg = null, Color? fg = null)
        {
            Button b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = new Font("Segoe UI", text.Length > 2 ? 14 : 18, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = bg ?? Color.White,
                ForeColor = fg ?? ThemeManager.TextColor,
                TabStop = false
            };
            
            // Modern dokunmatik hissi için sıfır çerçeve ve etkileşim renkleri
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = bg != null ? ControlPaint.Light(bg.Value) : Color.LightGray;
            b.FlatAppearance.MouseDownBackColor = bg != null ? ControlPaint.Dark(bg.Value) : Color.DarkGray;

            b.Click += (s, e) => HandleKeyPress(text);
            pnlKeyboard.Controls.Add(b);
        }

        private void HandleKeyPress(string key)
        {
            if (key == "Sil ⌫")
            {
                if (txtInput.Text.Length > 0)
                    txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
            }
            else if (key == "TEMİZLE")
            {
                txtInput.Clear();
            }
            else if (key == "BÜYÜT ⇧")
            {
                isShift = !isShift;
                LoadLayout();
            }
            else if (key == "BOŞLUK")
            {
                txtInput.Text += " ";
            }
            else if (key == "GİRİŞ (ENTER)")
            {
                InputText = txtInput.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                txtInput.Text += key;
            }
            txtInput.SelectionStart = txtInput.Text.Length;
            txtInput.Focus();
        }
    }
}
