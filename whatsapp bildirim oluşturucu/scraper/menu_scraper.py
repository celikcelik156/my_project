import time
import logging
from datetime import datetime
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

logger = logging.getLogger(__name__)

def scrape_daily_menu(driver=None, target_date=None):
    url = "https://www.gibtu.edu.tr/yemeklistesi"
    from datetime import timedelta
    
    if target_date is None:
        target_date = datetime.now()
    
    is_weekend = False
    weekend_msg = ""
    if target_date.weekday() >= 5:
        is_weekend = True
        days_to_monday = 7 - target_date.weekday()
        orig_date_str = target_date.strftime("%d.%m.%Y")
        target_date = target_date + timedelta(days=days_to_monday)
        day_name = "Cumartesi" if target_date.weekday() == 5 else "Pazar"
        weekend_msg = f"⚠️ *Hafta sonu ({orig_date_str}) okul kapalıdır.*\n🍴 Pazartesi gününün yemek listesi aşağıdadır:\n\n"
        
    date_str = target_date.strftime("%d.%m.%Y")
    logger.info("Yemek listesi çekiliyor (%s)...", date_str)
    
    should_close = False
    if driver is None:
        from ubys_scraper import create_driver
        driver = create_driver()
        should_close = True
    
    try:
        driver.get(url)
        time.sleep(3)
        
        try:
            WebDriverWait(driver, 15).until(
                EC.presence_of_element_located((By.CLASS_NAME, "card"))
            )
        except:
            logger.warning("Yemek listesi kartları bulunamadı.")
            return f"{weekend_msg}Yemek listesi şu an yayında değil." if is_weekend else None
            
        cards = driver.find_elements(By.CLASS_NAME, "card")
        
        for card in cards:
            card_text = card.text
            if date_str in card_text:
                try:
                    content_el = card.find_element(By.CLASS_NAME, "card-content")
                    menu_text = content_el.text.strip()
                except:
                    menu_text = card_text.strip()
                
                lines = [line.strip() for line in menu_text.split('\n') if line.strip()]
                clean_lines = [line for line in lines if date_str not in line]
                
                if clean_lines:
                    final_menu = "\n".join(clean_lines)
                    return f"{weekend_msg}{final_menu}"
        
        logger.warning("%s için yemek listesi bulunamadı.", date_str)
        return f"{weekend_msg}Pazartesi günü için de henüz liste girilmemiş." if is_weekend else None
        
    except Exception as e:
        logger.error("Yemek listesi çekilirken hata: %s", e)
        return None
    finally:
        if should_close and driver:
            driver.quit()

if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO)
    menu = scrape_daily_menu()
    if menu:
        print("--- GÜNÜN MENÜSÜ ---")
        print(menu)
    else:
        print("Menü bulunamadı.")
