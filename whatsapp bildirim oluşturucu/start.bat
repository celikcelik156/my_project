@echo off
chcp 65001 > nul
echo ============================================
echo   UBYS WhatsApp Bildirim Sistemi
echo ============================================
echo.
echo Baslatiliyor...
echo Durdurmak icin bu pencereyi kapatin veya Ctrl+C basin.
echo.
cd /d "%~dp0"
python scraper\scheduler.py
echo.
echo Program sonlandi. Cikis icin bir tusa basin.
pause
