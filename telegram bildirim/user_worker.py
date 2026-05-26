import sys
import logging
import argparse
from data_manager import (
    load_previous_state, save_current_state, check_for_changes, 
    get_user_by_id, update_user_status
)
from scraper import scrape_ubys_data
from notifier import process_and_notify

def run_worker(chat_id):
    logging.basicConfig(
        level=logging.INFO,
        format=f'%(asctime)s [USER:{chat_id}] %(levelname)s: %(message)s',
        handlers=[
            logging.StreamHandler(sys.stdout)
        ]
    )

    user = get_user_by_id(chat_id)
    if not user:
        logging.error(f"Kullanıcı bulunamadı!")
        return

    username = user['username']
    password = user['password']
    
    logging.info(f"--- {username} için tarama başlatıldı ---")
    update_user_status(chat_id, "Tarama başlatıldı...")
    
    try:
        old_state = load_previous_state(chat_id)
        
        new_state = scrape_ubys_data(username, password)
        
        if not new_state:
            msg = "Hata: Veri çekilemedi (Giriş sorunu?)"
            logging.warning(msg)
            update_user_status(chat_id, msg)
            return
            
        logging.info(f"{len(new_state)} ders verisi başarıyla çekildi.")
        
        changes = check_for_changes(chat_id, old_state, new_state)
        
        if changes:
            msg = f"TAMAMLANDI: {len(changes)} yeni değişiklik tespit edildi!"
            logging.info(msg)
            process_and_notify(changes)
            update_user_status(chat_id, msg)
        else:
            msg = "TAMAMLANDI: Değişiklik yok."
            logging.info(msg)
            update_user_status(chat_id, msg)
            
        save_current_state(chat_id, new_state)
        
    except Exception as e:
        msg = f"Kritik Hata: {str(e)}"
        logging.error(msg)
        update_user_status(chat_id, msg)
    
    logging.info(f"--- {username} için işlem tamamlandı. ---")

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--chat_id", required=True, help="Telegram Chat ID of the user")
    args = parser.parse_args()
    
    run_worker(args.chat_id)
    import time
    time.sleep(3)
