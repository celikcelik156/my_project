@echo off
chcp 65001 > nul
echo ============================================
echo   UBYS WhatsApp Bildirim Sistemi - Kurulum
echo ============================================
echo.

python --version > nul 2>&1
if errorlevel 1 (
    echo [HATA] Python bulunamadi!
    echo Python 3.8+ indirip kurun: https://www.python.org/downloads/
    pause
    exit /b 1
)

echo [OK] Python bulundu.
echo.

echo [1/2] Python paketleri yukleniyor...
python -m pip install --upgrade pip --quiet
python -m pip install -r requirements.txt
if errorlevel 1 (
    echo [HATA] Paketler yuklenemedi!
    pause
    exit /b 1
)

echo [OK] Python paketleri yuklendi.
echo.

if not exist "data" mkdir data
echo [OK] Data klasoru hazir.
echo.

if not exist "config\.env" (
    echo [UYARI] config\.env dosyasi bulunamadi!
    echo Lutfen config\.env dosyasini olusturun ve bilgilerinizi girin.
) else (
    echo [OK] config\.env dosyasi mevcut.
)

echo.
echo ============================================
echo   Kurulum tamamlandi!
echo   Baslatmak icin: start.bat
echo ============================================
echo.
pause
