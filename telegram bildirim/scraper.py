import time
import os
from playwright.sync_api import sync_playwright, TimeoutError as PlaywrightTimeoutError
import logging
from config import UBYS_LOGIN_URL, UBYS_STUDENT_HOME_URL, HEADLESS_MODE, SLEEP_SHORT, SLEEP_MEDIUM, SLEEP_LONG

EXCLUDE_KEYWORDS = ["Geçmiş Dönem", "Derslerini Göster", "Kayıt bulunamadı", "Yükleniyor", "Seçiniz"]

def _try_navigate(page, url, max_retry=3):
    for attempt in range(1, max_retry + 1):
        try:
            page.goto(url, timeout=60000, wait_until="networkidle")
            return True
        except Exception as e:
            logging.warning(f"Sayfa yüklenemedi ({attempt}/{max_retry}): {e}")
            if attempt < max_retry:
                time.sleep(3)
    return False


def _bypass_survey(page):
    try:
        time.sleep(2)
        url = page.url.lower()
        html = page.content().lower()
        if "survey" in url or "anket" in url or "anket" in html:
            logging.info("Anket/Modal tespit edildi, atlanmaya/doldurulmaya çalışılıyor...")
            
            skip_selectors = [
                "button:has-text('Atla')", "button:has-text('Kapat')", "button:has-text('Daha Sonra')", "button:has-text('Geç')",
                "button:has-text('Tamam')", "button[data-dismiss='modal']", ".close",
                "a:has-text('Şimdi Değil')", "a:has-text('Atla')", "a:has-text('Daha Sonra')", "a:has-text('Geç')", "a:has-text('Kapat')"
            ]
            for sel in skip_selectors:
                try:
                    elements = page.locator(sel).all()
                    for el in elements:
                        if el.is_visible():
                            el.click()
                            logging.info(f"Kapatma butonuna tıklandı: {sel}")
                            time.sleep(2)
                            return
                except: pass
            
            logging.info("Atla butonu bulunamadı, anket dolduruluyor...")
            
            radios = page.locator("input[type='radio']").all()
            handled_names = set()
            for r in radios:
                try:
                    name = r.get_attribute("name")
                    if name and name not in handled_names:
                        r.evaluate("el => el.click()")
                        handled_names.add(name)
                except: pass
                
            textareas = page.locator("textarea").all()
            for ta in textareas:
                try:
                    if ta.is_visible():
                        ta.fill("Teşekkürler")
                except: pass
            
            submit_selectors = [
                "button:has-text('Kaydet')", "button:has-text('Gönder')", "button:has-text('Tamamla')",
                "button.btn-success", "input[type='submit']"
            ]
            for sel in submit_selectors:
                try:
                    elements = page.locator(sel).all()
                    for el in elements:
                        if el.is_visible():
                            el.click()
                            logging.info(f"Anket kaydet/gönder butonuna tıklandı: {sel}")
                            time.sleep(3)
                            return
                except: pass
                        
    except Exception as e:
        logging.debug(f"Anket atlama hatası: {e}")

def scrape_ubys_data(username, password):
    scraped_data = {}
    playwright = None
    browser = None

    from config import LOG_FILE_PATH
    log_dir = os.path.dirname(LOG_FILE_PATH)
    if not os.path.exists(log_dir):
        os.makedirs(log_dir, exist_ok=True)

    try:
        playwright = sync_playwright().start()
        browser = playwright.chromium.launch(headless=HEADLESS_MODE)
        context = browser.new_context(viewport={'width': 1280, 'height': 800})
        page = context.new_page()

        try:
            logging.info(f"UBYS giriş sayfasına gidiliyor... ({UBYS_LOGIN_URL})")
            if not _try_navigate(page, UBYS_LOGIN_URL):
                logging.error("Giriş sayfasına ulaşılamadı. İnternet bağlantısını kontrol edin.")
                return {}

            try:
                login_selector = 'input[name="username"], #KullaniciAdi, #username, input#username'
                page.wait_for_selector(login_selector, timeout=45000, state="visible")
            except Exception as e:
                screenshot_path = os.path.join(log_dir, f"login_timeout_{username}.png")
                page.screenshot(path=screenshot_path)
                logging.error(f"Giriş alanları bulunamadı. Ekran görüntüsü kaydedildi: {screenshot_path}")
                return {}

            user_input = page.locator('input[name="username"], #KullaniciAdi, #username').first
            user_input.fill(username)

            pass_input = page.locator('input[name="password"], #Sifre, #password').first
            pass_input.fill(password)

            login_selectors = [
                'button:has-text("Giriş")', 
                '#btnGiris', 
                '.login-btn', 
                'input[type="submit"]',
                'button.btn-primary'
            ]
            
            clicked = False
            for sel in login_selectors:
                if page.locator(sel).count() > 0:
                    page.locator(sel).first.click()
                    clicked = True
                    break
            
            if not clicked:
                page.keyboard.press("Enter")

            logging.info("Giriş yapıldı, yönlendirme bekleniyor...")
            page.wait_for_load_state("networkidle", timeout=60000)

            _bypass_survey(page)

            logging.info("Öğrenci Bilgi Ekranı'na gidiliyor...")
            if not _try_navigate(page, UBYS_STUDENT_HOME_URL):
                logging.error("Öğrenci Bilgi Ekranı'na ulaşılamadı.")
                return {}
            
            time.sleep(SLEEP_SHORT)
            _bypass_survey(page)

            logging.info("Derslerim butonu aranıyor...")
            derslerim_selectors = [
                "li:has(h2:text('Derslerim'))",
                "a:has-text('Derslerim')",
                ".card:has-text('Derslerim')",
                "h2:text('Derslerim')",
                "div.widget:has-text('Derslerim')"
            ]

            final_page = None
            for selector in derslerim_selectors:
                try:
                    if page.locator(selector).count() > 0:
                        logging.info(f"'Derslerim' bulundu: {selector}")
                        with context.expect_page(timeout=45000) as p_info:
                            page.locator(selector).first.click()
                        final_page = p_info.value
                        break
                except Exception as e:
                    continue

            if not final_page:
                logging.warning("'Derslerim' butonu bulunamadı, doğrudan URL'ye gidiliyor...")
                final_page = context.new_page()
                if not _try_navigate(final_page, "https://ubys.gibtu.edu.tr/AIS/Student/Course/Index"):
                    logging.error("Ders listesi sayfasına doğrudan erişilemedi.")
                    return {}

            final_page.wait_for_load_state("networkidle")
            time.sleep(SLEEP_SHORT)

            logging.info("Ders listesi taranıyor...")
            course_selector = "a[href*='ClassDetail'], a[href*='sapid=']"
            try:
                final_page.wait_for_selector(course_selector, timeout=30000)
            except:
                final_page.screenshot(path=os.path.join(log_dir, f"error_courselist_{username}.png"))
                logging.error("Ders listesi yüklenemedi.")
                return {}

            all_links = final_page.locator(course_selector).all()
            ordered_course_names = []
            course_link_map = {}

            for link in all_links:
                name = " ".join(link.inner_text().split()).strip()
                if (name and len(name) > 3 and name not in course_link_map
                        and not any(kw in name for kw in EXCLUDE_KEYWORDS)):
                    ordered_course_names.append(name)
                    course_link_map[name] = link

            logging.info(f"Toplam {len(ordered_course_names)} ders bulundu.")

            for course_title in ordered_course_names:
                detail_page = None
                try:
                    link_element = course_link_map[course_title]

                    try:
                        row = link_element.locator("xpath=ancestor::tr").first
                        course_name = " ".join(row.locator("td").nth(1).inner_text().split())
                        if not course_name:
                            course_name = course_title
                    except:
                        course_name = course_title

                    logging.info(f"-> [{course_title}] {course_name}")

                    scraped_data[course_title] = {
                        "name": course_name,
                        "grades": {},
                        "averages": {},
                        "announcements": [],
                        "assignments": []
                    }

                    try:
                        with context.expect_page(timeout=45000) as detail_info:
                            link_element.evaluate("el => el.click()")
                        detail_page = detail_info.value
                        detail_page.wait_for_load_state("domcontentloaded", timeout=30000)
                        time.sleep(1.5)
                    except Exception as e:
                        logging.warning(f"[{course_title}] Detay sayfası açılamadı, atlanıyor: {e}")
                        continue

                    try:
                        not_sel = ".tab-pane.active table tr, #GenelBilgiler table tr"
                        detail_page.wait_for_selector(not_sel, timeout=8000)
                        for r in detail_page.locator(not_sel).all():
                            cols = r.locator("td").all()
                            if len(cols) >= 7:
                                ex = (cols[1].text_content() or "").strip()
                                gr = (cols[3].text_content() or "").strip()
                                av = (cols[6].text_content() or "").strip()
                                if ex and gr and gr != "-":
                                    scraped_data[course_title]["grades"][ex] = gr
                                    scraped_data[course_title]["averages"][ex] = av
                    except: pass

                    for tab_name, key, mode in [
                        ("Duyurular", "announcements", "ann"),
                        ("Ödevler", "assignments", "ass")
                    ]:
                        try:
                            tab = detail_page.locator(f"a:has-text('{tab_name}')").first
                            if tab.count() > 0:
                                tab.evaluate("el => el.click()")
                                time.sleep(1.5)
                                for item in detail_page.locator(".tab-pane.active table tbody tr").all():
                                    cols = item.locator("td").all()
                                    if mode == "ann" and len(cols) >= 4:
                                        scraped_data[course_title][key].append({
                                            "date": cols[1].inner_text().strip(),
                                            "title": cols[2].inner_text().strip(),
                                            "content": cols[3].inner_text().strip()
                                        })
                                    elif mode == "ass" and len(cols) >= 5:
                                        scraped_data[course_title][key].append({
                                            "title": cols[3].inner_text().strip(),
                                            "due_date": cols[4].inner_text().strip()
                                        })
                        except: pass

                except Exception as e:
                    logging.error(f"[{course_title}] işlenirken hata: {e}")
                finally:
                    if detail_page:
                        try: detail_page.close()
                        except: pass

        except Exception as e:
            logging.error(f"Veri çekme sırasında beklenmeyen hata: {e}")

    except Exception as e:
        logging.error(f"Tarayıcı başlatma hatası: {e}")
    finally:
        if browser:
            try: browser.close()
            except: pass
        if playwright:
            try: playwright.stop()
            except: pass

    return scraped_data
