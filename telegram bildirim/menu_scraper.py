import datetime
import logging
from playwright.sync_api import sync_playwright
from config import HEADLESS_MODE

def scrape_daily_menu(target_date=None):
    url = "https://www.gibtu.edu.tr/yemeklistesi"
    
    if target_date is None:
        target_date = datetime.datetime.now()
        
    if target_date.weekday() >= 5: # 5: Cumartesi, 6: Pazar
        return "⚠️ Hafta sonu yemek hizmeti verilmemektedir."
        
    date_str = target_date.strftime("%d.%m.%Y")
    
    logging.info(f"Yemek listesi çekiliyor ({date_str})...")
    
    playwright = None
    browser = None
    
    try:
        from playwright.sync_api import sync_playwright
        playwright = sync_playwright().start()
        browser = playwright.chromium.launch(headless=HEADLESS_MODE)
        context = browser.new_context()
        page = context.new_page()
        
        try:
            page.goto(url, timeout=60000, wait_until="networkidle")
            
            try:
                page.wait_for_selector(".card", timeout=20000)
            except:
                logging.warning("Yemek listesi kartları bulunamadı.")
                return None
                
            cards = page.locator(".card").all()
            
            for card in cards:
                card_text = card.inner_text()
                if date_str in card_text:
                    content_locator = card.locator(".card-content")
                    if content_locator.count() > 0:
                        menu_text = content_locator.inner_text().strip()
                    else:
                        menu_text = card_text
                        
                    lines = [line.strip() for line in menu_text.split('\n') if line.strip()]
                    clean_lines = [line for line in lines if date_str not in line]
                    
                    if clean_lines:
                        return "\n".join(clean_lines)
            
            logging.warning(f"{date_str} için yemek listesi bulunamadı.")
            return None
            
        except Exception as e:
            logging.error(f"Yemek listesi çekilirken hata oluştu: {e}")
            return None
    except Exception as e:
        logging.error(f"Playwright başlatma hatası (Menü): {e}")
        return None
    finally:
        if browser:
            try:
                browser.close()
            except:
                pass
        if playwright:
            try:
                playwright.stop()
            except:
                pass
