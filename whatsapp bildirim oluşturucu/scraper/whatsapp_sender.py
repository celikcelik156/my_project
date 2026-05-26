import os
import sys
import time
import glob
import logging
from pathlib import Path
from dotenv import load_dotenv

env_path = Path(__file__).parent.parent / "config" / ".env"
load_dotenv(dotenv_path=env_path)

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.common.exceptions import StaleElementReferenceException

logger = logging.getLogger(__name__)

TARGET_CHAT = os.getenv("WHATSAPP_GROUP_NAME", os.getenv("WHATSAPP_GROUP", "Fusionbit"))
PROFILE_DIR = str(Path(__file__).parent.parent / "data" / "chrome_profile")

def find_chromedriver():
    wdm = Path.home() / ".wdm" / "drivers" / "chromedriver"
    hits = glob.glob(str(wdm / "**" / "chromedriver.exe"), recursive=True)
    if hits:
        hits.sort(key=lambda p: Path(p).stat().st_mtime, reverse=True)
        return hits[0]
    return None

def format_message(item: dict) -> str:
    t      = item.get("type", "")
    course = item.get("course", "")
    
    course_info = f"*{course}*"

    if t == "grade":
        exam_name_orig = item.get("exam_name", "")
        exam_name_lower = exam_name_orig.lower()
        score     = item.get("score", "")
        average   = item.get("average", "")
        letter    = item.get("letter", "")
        
        if "vize" in exam_name_lower:
            exam_label = "Vize"
        elif "final" in exam_name_lower:
            exam_label = "Final"
        elif "bütünleme" in exam_name_lower:
            exam_label = "Bütünleme"
        else:
            exam_label = exam_name_orig

        msg = (
            f"🎓 *Sınav Notu Açıklandı*\n"
            f"{course_info}\n\n"
            f"*Sınav:* {exam_label}\n"
            f"*Notunuz:* {score}\n"
            f"*Sınıf Ortalaması:* {average}"
        )
        if letter and letter != "-":
            msg += f"\n*Harf Notu:* {letter}"

    elif t == "assignment":
        title    = item.get("title", "")
        due_date = item.get("due_date", "")
        
        msg = (
            f"📝 *Yeni Ödev Eklendi*\n"
            f"{course_info}\n\n"
            f"*Ödev Tanımı:* {title}"
        )
        if due_date and due_date != "Belirtilmemiş":
            msg += f"\n*Son Teslim Tarihi:* {due_date}"

    elif t == "announcement":
        title    = item.get("title", "")
        content  = item.get("content", "")
        
        msg = (
            f"📢 *Yeni Duyuru Eklendi*\n"
            f"{course_info}\n\n"
            f"*Başlık:* {title}\n\n"
            f"*İçerik:*\n_{content}_"
        )

    elif t == "content":
        c_name = item.get("name", "")
        c_type = item.get("content_type", "")
        c_date = item.get("date", "")
        c_desc = item.get("description", "")
        
        msg = f"📚 *Yeni Ders İçeriği Eklendi*\n{course_info}\n\n"
        msg += f"*İçerik Adı:* {c_name}\n"
        
        if c_type:
            msg += f"*İçerik Tipi:* {c_type}\n"
        if c_date:
            msg += f"*Yayınlanma Tarihi:* {c_date}\n"
        if c_desc:
            msg += f"\n*Açıklama:*\n_{c_desc}_"

    else:
        msg = f"🔔 *UBYS Bildirimi*\n{course_info}\n\n{item.get('title','')}"

    return msg

def send_whatsapp_messages_selenium(messages: list) -> int:
    if not messages:
        return 0

    cd = find_chromedriver()
    if not cd:
        logger.error("ChromeDriver bulunamadı! Lütfen ubys_scraper.py'nin indirebilmesi için sistemi bir kez çalıştırın.")
        return 0

    opts = Options()
    opts.add_argument(f"--user-data-dir={PROFILE_DIR}")
    opts.add_argument("--profile-directory=Default")
    opts.add_argument("--no-sandbox")
    opts.add_argument("--disable-dev-shm-usage")
    opts.add_argument("--disable-gpu")
    opts.add_argument("--window-size=1280,900")
    opts.add_experimental_option("excludeSwitches", ["enable-logging"])

    logger.info("WhatsApp Web başlatılıyor... (Profil Yolu: %s)", PROFILE_DIR)
    driver = webdriver.Chrome(service=Service(cd), options=opts)
    
    sent_count = 0
    try:
        driver.get("https://web.whatsapp.com")
        
        logger.info("WhatsApp Web'in yüklenmesi bekleniyor...")
        for i in range(24):
            if 'data-tab' in driver.page_source:
                break
            time.sleep(5)
        time.sleep(2)

        logger.info(f"Hedef grup/kullanıcı '{TARGET_CHAT}' aranıyor...")
        found = False
        
        for attempt in range(5):
            try:
                for sp in driver.find_elements(By.CSS_SELECTOR, '#pane-side span[title]'):
                    if sp.get_attribute("title") == TARGET_CHAT:
                        driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", sp)
                        time.sleep(1)
                        sp.click()
                        found = True
                        logger.info("  └─> Bulundu ve tıklandı!")
                        break
                if found:
                    break
            except StaleElementReferenceException:
                logger.warning(f"WhatsApp arayüzü güncellendi, liste tekrar taranıyor... ({attempt+1}/5)")
                time.sleep(2)
                
        if not found:
            logger.error(f"❌ '{TARGET_CHAT}' sohbet listesinde bulunamadı! WA Web'de üst sıralarda olduğundan emin olun.")
            driver.quit()
            return 0

        time.sleep(3)

        msg_box = None
        for xp in [
            '//div[@role="textbox"][@data-tab="10"]',
            '//div[@role="textbox"]',
            '//footer//div[@contenteditable="true"]',
        ]:
            els = driver.find_elements(By.XPATH, xp)
            if els:
                msg_box = els[-1]
                break

        if not msg_box:
            logger.error("❌ Mesaj kutusu bulunamadı!")
            driver.quit()
            return 0

        msg_box.click()
        time.sleep(1)

        for msg in messages:
            logger.info("Mesaj yazılıyor...")
            driver.execute_script(
                "arguments[0].focus();"
                "document.execCommand('insertText', false, arguments[1]);",
                msg_box, msg
            )
            time.sleep(2)
            
            send = driver.find_elements(By.CSS_SELECTOR, 'span[data-icon="send"]')
            if send:
                send[0].click()
            else:
                msg_box.send_keys(Keys.RETURN)
            
            logger.info("  └─> Mesaj gönderildi!")
            sent_count += 1
            time.sleep(3)

        logger.info("Tüm mesajlar iletiliyor, bekleniyor...")
        time.sleep(7)

    except Exception as e:
        logger.error(f"WhatsApp gönderim hatası: {e}", exc_info=True)
    finally:
        driver.quit()
        logger.info("WhatsApp Web kapatıldı.")

    return sent_count


def send_all_notifications(new_items: list) -> int:
    if not new_items:
        return 0

    messages = [format_message(item) for item in new_items]
    sent = send_whatsapp_messages_selenium(messages)
    return sent

if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s")
    test_msg = [{"type": "other", "course": "Sistem", "title": "WhatsApp Selenium Test"}]
    send_all_notifications(test_msg)
