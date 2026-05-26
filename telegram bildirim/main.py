import time
import logging
import os
import sys
import msvcrt
import threading
from config import LOG_FILE_PATH, SLEEP_LONG
from scraper import scrape_ubys_data
from data_manager import (
    load_previous_state, save_current_state, check_for_changes, 
    get_system_setting, set_system_setting, get_all_active_users,
    get_user_status
)
from notifier import send_telegram_message
from menu_scraper import scrape_daily_menu
from datetime import datetime
from bot_handler import bot_poll_updates

os.environ["PYTHONIOENCODING"] = "utf-8"
os.environ["PROMPT"] = "$P$G"

lock_file_handle = None

def ensure_single_instance():
    global lock_file_handle
    try:
        lock_file_handle = open("app.lock", "w")
        msvcrt.locking(lock_file_handle.fileno(), msvcrt.LK_NBLCK, 1)
    except (IOError, PermissionError):
        print("\n[HATA] Sistem zaten başka bir pencerede çalışıyor!")
        print("Lütfen açık olan diğer siyah pencereleri kapatın.")
        time.sleep(5)
        sys.exit(1)

import subprocess

def run_scraper_for_all_users():
    logging.info("--- Paralel Tarama İşlemi Başlatıldı ---")
    users = get_all_active_users()
    
    if not users:
        logging.info("Sistemde henüz kayıtlı ve aktif kullanıcı yok.")
        return

    active_processes = []

    for user in users:
        chat_id = user['chat_id']
        username = user['username']
        
        logging.info(f"USER[{chat_id}] ({username}) için yeni terminal başlatılıyor...")
        
        try:
            cmd = [sys.executable, "user_worker.py", "--chat_id", str(chat_id)]
            p = subprocess.Popen(
                cmd, 
                creationflags=subprocess.CREATE_NEW_CONSOLE
            )
            active_processes.append((user, p))
            
            time.sleep(0.5)
            
        except Exception as e:
            logging.error(f"USER[{chat_id}] terminal başlatma hatası: {e}")

    logging.info(f"Tüm pencereler açıldı. İşlemlerin tamamlanması bekleniyor...")

    while active_processes:
        for item in active_processes[:]:
            user, p = item
            if p.poll() is not None:
                status_data = get_user_status(user['chat_id'])
                msg = status_data['last_status'] if status_data and status_data['last_status'] else "İşlem bitti (Durum kaydedilmedi)"
                
                logging.info(f" [SONUÇ] {user['username']}: {msg}")
                active_processes.remove(item)
        
        time.sleep(1)
            
    logging.info(f"--- Tüm paralel işlemler tamamlandı ---")

last_menu_attempt_date = None

def check_and_send_menu():
    global last_menu_attempt_date
    now = datetime.now()
    
    if now.weekday() >= 5:
        return
    
    if now.hour >= 1:
        today_str = now.strftime("%Y-%m-%d")
        last_sent = get_system_setting("last_menu_sent_date")
        
        if last_sent != today_str:
            if last_menu_attempt_date != today_str:
                last_menu_attempt_date = today_str
                logging.info("Yemek listesi için yeni terminal başlatılıyor...")
                
                try:
                    cmd = [sys.executable, "menu_worker.py"]
                    subprocess.Popen(
                        cmd, 
                        creationflags=subprocess.CREATE_NEW_CONSOLE
                    )
                except Exception as e:
                    logging.error(f"Yemek listesi terminali başlatma hatası: {e}")

if __name__ == "__main__":
    ensure_single_instance()
    
    bot_thread = threading.Thread(target=bot_poll_updates, daemon=True)
    bot_thread.start()
    logging.info("Bot dinleyici (etkileşimli kayıt) arka planda başlatıldı.")

    logging.info("Sistem başlatıldı. Her 20 dakikada bir tüm kullanıcılar taranacak.")
    
    while True:
        try:
            run_scraper_for_all_users()
            
            check_and_send_menu()
            
        except Exception as e:
            logging.error(f"Ana döngüde beklenmeyen hata: {e}")
            
        logging.info("Bir sonraki toplu tarama için 20 dakika bekleniyor...")
        time.sleep(1200)
