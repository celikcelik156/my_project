import sys
import logging
from datetime import datetime
from data_manager import get_all_active_users, set_system_setting
from menu_scraper import scrape_daily_menu
from notifier import send_telegram_message

def run_menu_worker():
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s [MENU WORKER] %(levelname)s: %(message)s',
        handlers=[logging.StreamHandler(sys.stdout)]
    )
    
    now = datetime.now()
    today_str = now.strftime("%Y-%m-%d")
    
    logging.info("Bugünün yemek listesi hazırlanıyor...")
    menu = scrape_daily_menu()
    
    if menu:
        msg = (
            f"<b>🍴 Günün Yemek Listesi</b>\n"
            f"📅 Tarih: {now.strftime('%d.%m.%Y')}\n\n"
            f"{menu}"
        )
        
        users = get_all_active_users()
        sent_count = 0
        for user in users:
            if send_telegram_message(msg, user['chat_id']):
                sent_count += 1
        
        if sent_count > 0:
            set_system_setting("last_menu_sent_date", today_str)
            logging.info(f"Yemek listesi {sent_count} kullanıcıya gönderildi.")
    else:
        logging.warning("Yemek listesi çekilemedi veya henüz yayınlanmamış.")
        
    logging.info("--- Yemek listesi işlemi tamamlandı ---")

if __name__ == "__main__":
    run_menu_worker()
    import time
    time.sleep(3)
