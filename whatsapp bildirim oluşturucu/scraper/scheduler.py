import os
import sys
import time
import logging
import schedule
from pathlib import Path
from datetime import datetime

sys.path.insert(0, str(Path(__file__).parent))

from dotenv import load_dotenv
env_path = Path(__file__).parent.parent / "config" / ".env"
load_dotenv(dotenv_path=env_path)

from ubys_scraper import run_scrape
from whatsapp_sender import send_all_notifications, send_whatsapp_messages_selenium
from menu_scraper import scrape_daily_menu
from db_manager import get_system_setting, set_system_setting

LOG_LEVEL = os.getenv("LOG_LEVEL", "INFO").upper()
log_dir = Path(__file__).parent.parent / "data"
log_dir.mkdir(exist_ok=True)

logging.basicConfig(
    level=getattr(logging, LOG_LEVEL, logging.INFO),
    format="%(asctime)s [%(levelname)s] %(name)s - %(message)s",
    handlers=[
        logging.FileHandler(log_dir / "ubys_notifier.log", encoding="utf-8"),
        logging.StreamHandler(sys.stdout)
    ]
)
logger = logging.getLogger("scheduler")

CHECK_INTERVAL = int(os.getenv("CHECK_INTERVAL_MINUTES", "15"))


def job():
    logger.info("=" * 60)
    logger.info("🔍 UBYS kontrol başlıyor... (%s)", datetime.now().strftime("%d.%m.%Y %H:%M:%S"))
    logger.info("=" * 60)

    try:
        new_items = run_scrape()

        if new_items:
            logger.info("🆕 %d yeni içerik bulundu!", len(new_items))
            sent = send_all_notifications(new_items)
            logger.info("✅ %d/%d bildirim WhatsApp'a gönderildi.", sent, len(new_items))
        else:
            logger.info("✅ Yeni içerik yok.")

    except Exception as e:
        logger.error("❌ Kontrol sırasında hata: %s", e, exc_info=True)

    check_and_send_menu()

    logger.info("⏰ Sonraki kontrol %d dakika sonra.", CHECK_INTERVAL)


def check_and_send_menu():
    now = datetime.now()
    
    if now.hour >= 10:
        today_str = now.strftime("%Y-%m-%d")
        last_sent = get_system_setting("last_menu_sent_date")
        
        if last_sent != today_str:
            logger.info("🍴 Bugünün yemek listesi kontrol ediliyor...")
            menu = scrape_daily_menu()
            
            if menu:
                if "⚠️" in menu:
                    msg = menu
                else:
                    msg = (
                        f"🍴 *Günün Yemek Listesi*\n"
                        f"📅 Tarih: {now.strftime('%d.%m.%Y')}\n\n"
                        f"{menu}"
                    )
                
                sent = send_whatsapp_messages_selenium([msg])
                if sent > 0:
                    set_system_setting("last_menu_sent_date", today_str)
                    logger.info("✅ Yemek listesi WhatsApp'a gönderildi.")
            else:
                logger.warning("⚠️ Yemek listesi bugün için bulunamadı.")


def main():
    logger.info("🚀 UBYS WhatsApp Bildirim Sistemi başlatıldı!")
    logger.info("⚙️  Kontrol aralığı: %d dakika", CHECK_INTERVAL)
    logger.info("👤  Kullanıcı: %s", os.getenv("UBYS_USERNAME", "?"))
    from whatsapp_sender import TARGET_CHAT
    logger.info("📱  WhatsApp Hedef: %s", TARGET_CHAT)
    logger.info("-" * 60)

    logger.info("İlk kontrol başlıyor...")
    job()

    schedule.every(CHECK_INTERVAL).minutes.do(job)

    logger.info("Zamanlayıcı çalışıyor. Durdurmak için Ctrl+C basın.")
    try:
        while True:
            schedule.run_pending()
            time.sleep(30)
    except KeyboardInterrupt:
        logger.info("🛑 Program durduruldu.")


if __name__ == "__main__":
    main()
