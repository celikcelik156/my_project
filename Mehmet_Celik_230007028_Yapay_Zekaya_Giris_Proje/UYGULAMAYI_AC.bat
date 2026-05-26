@echo off
chcp 65001 >nul
title Emlak Fiyat Tahmin Sistemi - Mehmet Çelik

echo.
echo  ============================================
echo    EMLAK FIYAT TAHMIN SISTEMI BASLATILIYOR
echo    Ogrenci: Mehmet Celik
echo  ============================================
echo.

cd /d "c:\Users\mehme\OneDrive\Desktop\yapay zekaya giriş dönem sonu projesi"

echo  [1/2] Modeller kontrol ediliyor...
if not exist "data\modeller.pkl" (
    echo  Modeller bulunamadi, egitim basliyor... (1-2 dk surebilir)
    python -X utf8 main.py
    echo.
)

echo  [2/2] Web arayuzu baslatiliyor...
echo.
echo  Tarayicinizda otomatik acilacak:
echo  http://localhost:8501
echo.
echo  Kapatmak icin bu pencereyi kapatin.
echo  ============================================

python -X utf8 -m streamlit run ui/app.py --server.port 8501 --server.headless false

pause
