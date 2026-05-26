import os
import sys
import time
import hashlib
import logging
import glob
from pathlib import Path

from dotenv import load_dotenv
env_path = Path(__file__).parent.parent / "config" / ".env"
load_dotenv(dotenv_path=env_path)

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException, NoSuchElementException

from db_manager import (
    init_db, is_new_item,
    save_grade, save_assignment, save_announcement, save_content,
    get_existing_grade, get_existing_assignment, get_existing_announcement, get_existing_content
)

logger = logging.getLogger(__name__)

UBYS_BASE        = "https://ubys.gibtu.edu.tr"
LOGIN_URL        = f"{UBYS_BASE}/Home/Index"
STUDENT_HOME_URL = f"{UBYS_BASE}/AIS/Student/Home/Index"


def _find_cached_chromedriver() -> str | None:
    wdm_root = Path.home() / ".wdm" / "drivers" / "chromedriver"
    patterns = [
        str(wdm_root / "**" / "chromedriver.exe"),
        str(wdm_root / "**" / "chromedriver-win32" / "chromedriver.exe"),
        str(wdm_root / "**" / "chromedriver-win64" / "chromedriver.exe"),
    ]
    found = []
    for pat in patterns:
        found.extend(glob.glob(pat, recursive=True))
    if found:
        # En son değiştirilen
        found.sort(key=lambda p: Path(p).stat().st_mtime, reverse=True)
        return found[0]
    return None


def _get_driver_service() -> Service:
    cached = _find_cached_chromedriver()
    if cached:
        logger.info("Cached ChromeDriver kullanılıyor: %s", cached)
        return Service(cached)

    logger.info("Cache bulunamadı, webdriver-manager ile indiriliyor...")
    from webdriver_manager.chrome import ChromeDriverManager
    path = ChromeDriverManager().install()
    return Service(path)


def create_driver() -> webdriver.Chrome:
    options = Options()
    headless = os.getenv("HEADLESS", "True").lower() == "true"
    use_profile = os.getenv("USE_PROFILE", "True").lower() == "true"

    if headless:
        options.add_argument("--headless=new")

    if use_profile:
        profile_path = Path(__file__).parent.parent / "data" / "ubys_profile"
        profile_path.mkdir(parents=True, exist_ok=True)
        p_str = os.path.normpath(str(profile_path.absolute()))
        options.add_argument(f"--user-data-dir={p_str}")

    options.add_argument("--no-sandbox")
    options.add_argument("--disable-dev-shm-usage")
    options.add_argument("--disable-gpu")
    options.add_argument("--disable-extensions")
    options.add_argument("--disable-software-rasterizer")
    options.add_argument("--disable-blink-features=AutomationControlled")
    options.add_argument("--remote-debugging-port=9222")
    options.add_argument("--remote-allow-origins=*")
    options.add_argument("--window-size=1280,1024")
    
    options.page_load_strategy = "none"
    options.add_experimental_option("excludeSwitches", ["enable-logging", "enable-automation"])

    service = _get_driver_service()
    try:
        driver = webdriver.Chrome(service=service, options=options)
    except Exception as e:
        logger.error("Sürücü başlatılamadı. Hata: %s", e)
        if use_profile:
            logger.error("İpucu: Profil kilitli olabilir. Lütfen 'data/ubys_profile' klasörünü temizlemeyi veya tüm Chrome pencerelerini kapatmayı deneyin.")
        raise

    caps = driver.capabilities
    logger.info("Browser: %s %s | Driver: %s", caps.get('browserName'), caps.get('browserVersion'), caps.get('chrome', {}).get('chromedriverVersion', '?').split(' ')[0])

    driver.set_page_load_timeout(30)
    return driver

def login(driver: webdriver.Chrome) -> bool:
    username = os.getenv("UBYS_USERNAME", "")
    password = os.getenv("UBYS_PASSWORD", "")

    if not username or not password:
        logger.error("UBYS_USERNAME veya UBYS_PASSWORD tanımlanmamış!")
        return False

    try:
        driver.get(STUDENT_HOME_URL)
        time.sleep(4)
        
        is_logged_in = False
        try:
            driver.find_element(By.ID, "username")
            is_logged_in = False
        except:
            curr_url = driver.current_url
            src_lower = driver.page_source.lower()
            if "Student/Home" in curr_url or "Dashboard" in curr_url or "Class/Index" in curr_url:
                is_logged_in = True
            elif "selectuserusersgroup" in src_lower or "sisteme gir" in src_lower:
                is_logged_in = True
            elif "cikis" in src_lower or "logout" in src_lower or "sign out" in src_lower:
                is_logged_in = True
        
        if is_logged_in:
            logger.info("Zaten giris yapilmis gorunuyor (URL: %s), devam ediliyor.", driver.current_url)
            _handle_post_login_checks(driver)
            return True

        logger.info("UBYS'e giriş yapılıyor (Geçerli URL: %s)...", driver.current_url)
        if "Login" not in driver.current_url:
            driver.get(LOGIN_URL)
            time.sleep(3)

        time.sleep(3)
        logger.info("Geçerli URL: %s", driver.current_url)

        wait = WebDriverWait(driver, 40)

        u_el = None
        USERNAME_SELECTORS = [(By.ID, "username"), (By.ID, "UserName"), (By.NAME, "username"), (By.ID, "KullaniciAdi")]
        for by, sel in USERNAME_SELECTORS:
            try:
                u_el = wait.until(EC.presence_of_element_located((by, sel)))
                break
            except: continue

        if u_el:
            u_el.clear()
            u_el.send_keys(username)
            
            # Şifre alanını bul
            p_el = None
            PASSWORD_SELECTORS = [(By.ID, "password"), (By.ID, "Password"), (By.NAME, "password"), (By.ID, "Sifre")]
            for by, sel in PASSWORD_SELECTORS:
                try:
                    p_el = driver.find_element(by, sel)
                    break
                except: continue
            
            if p_el:
                p_el.clear()
                p_el.send_keys(password)
                
                # Giriş butonunu tıkla
                BUTTON_SELECTORS = ["button.btn-primary", "#kc-login", "#btnLogin", "input[type='submit']"]
                clicked = False
                for sel in BUTTON_SELECTORS:
                    els = driver.find_elements(By.CSS_SELECTOR, sel)
                    if els:
                        driver.execute_script("arguments[0].click();", els[0])
                        clicked = True
                        break
                if not clicked:
                    driver.execute_script("try { document.querySelector('form').submit(); } catch(e) {}")
            else:
                logger.error("Şifre alanı bulunamadı!")
                return False
        else:
            logger.error("Kullanıcı adı alanı bulunamadı!")
            return False

        time.sleep(5)

        src_lower = driver.page_source.lower()
        if "code" in src_lower and ("dogrulama" in src_lower or "verification" in src_lower or "sure doldu" in src_lower):
            logger.warning("[!] UBYS dogrulama kodu (2FA) bekliyor!")
            logger.warning("Lutfen tarayicida manuel giris yapip oturumu kaydedin (HEADLESS=False yaparak).")
            _save_debug_page(driver, "debug_2fa_prompt.html")
            return False

        success = False
        for _ in range(15):
            curr_url = driver.current_url
            src_lower = driver.page_source.lower()
            if "logout" in src_lower or "cikis" in src_lower or "sign out" in src_lower:
                success = True
                break
            if "AIS/Student" in curr_url or "Home/Index" in curr_url:
                # Dashboard veya ana sayfa
                success = True
                break
            if "selectuserusersgroup" in src_lower or "sisteme gir" in src_lower:
                success = True
                break
            time.sleep(3)

        if success:
            logger.info("Giris basarili!")
            _handle_post_login_checks(driver)
            return True
        else:
            logger.error("Giris basarisiz gorunuyor. URL: %s", driver.current_url)
            _save_debug_page(driver, "debug_after_login_fail.html")
            return False

    except Exception as e:
        logger.error("Giriş hatası: %s", e)
        _save_debug_page(driver, "debug_login_exception.html")
        return False


def _bypass_survey(driver: webdriver.Chrome):
    try:
        time.sleep(2)
        curr_url = driver.current_url.lower()
        if "survey" in curr_url or "anket" in curr_url or "anket" in driver.page_source.lower():
            logger.info("Anket tespit edildi, atlanmaya/doldurulmaya çalışılıyor...")
            
            skip_btns = driver.find_elements(By.XPATH, "//button[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÖÇŞİĞÜ', 'abcdefghijklmnopqrstuvwxyzöçşiğü'), 'atla') or contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÖÇŞİĞÜ', 'abcdefghijklmnopqrstuvwxyzöçşiğü'), 'kapat') or contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÖÇŞİĞÜ', 'abcdefghijklmnopqrstuvwxyzöçşiğü'), 'daha sonra') or contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÖÇŞİĞÜ', 'abcdefghijklmnopqrstuvwxyzöçşiğü'), 'geç')]")
            skip_btns += driver.find_elements(By.CSS_SELECTOR, "a.btn-danger, button.btn-danger, .close")
            for btn in skip_btns:
                if btn.is_displayed():
                    try:
                        driver.execute_script("arguments[0].click();", btn)
                        logger.info("Anket atlama butonuna tıklandı.")
                        time.sleep(2)
                        return
                    except: pass
            
            logger.info("Atla butonu bulunamadı, anket dolduruluyor...")
            radios = driver.find_elements(By.CSS_SELECTOR, "input[type='radio']")
            
            handled_names = set()
            for r in radios:
                name = r.get_attribute("name")
                if name and name not in handled_names:
                    try:
                        driver.execute_script("arguments[0].click();", r)
                        handled_names.add(name)
                    except: pass
            
            textareas = driver.find_elements(By.CSS_SELECTOR, "textarea")
            for ta in textareas:
                if ta.is_displayed():
                    try:
                        ta.clear()
                        ta.send_keys("Teşekkürler")
                    except: pass
            
            submit_btns = driver.find_elements(By.XPATH, "//button[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÖÇŞİĞÜ', 'abcdefghijklmnopqrstuvwxyzöçşiğü'), 'kaydet') or contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÖÇŞİĞÜ', 'abcdefghijklmnopqrstuvwxyzöçşiğü'), 'gönder') or contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZÖÇŞİĞÜ', 'abcdefghijklmnopqrstuvwxyzöçşiğü'), 'tamamla')]")
            submit_btns += driver.find_elements(By.CSS_SELECTOR, "button.btn-success, input[type='submit']")
            for btn in submit_btns:
                if btn.is_displayed():
                    driver.execute_script("arguments[0].click();", btn)
                    logger.info("Anket kaydet butonuna tıklandı.")
                    time.sleep(3)
                    break
    except Exception as e:
        logger.debug("Anket atlama hatası: %s", e)


def _handle_post_login_checks(driver: webdriver.Chrome):
    _bypass_survey(driver)
    _dismiss_modals(driver)
    
    try:
        group_btns = driver.find_elements(By.XPATH, "//button[contains(@onclick, 'selectUserUsersGroup')]")
        if group_btns:
            for btn in group_btns:
                if btn.is_displayed():
                    logger.info("'Sisteme Gir' (Grup Seçimi) butonu bulundu, tıklanıyor...")
                    driver.execute_script("arguments[0].click();", btn)
                    time.sleep(3)
                    _bypass_survey(driver)
                    _dismiss_modals(driver)
                    break
    except Exception as e:
        logger.debug("Grup seçimi hatası: %s", e)

def _dismiss_modals(driver: webdriver.Chrome):
    try:
        time.sleep(2)
        from selenium.webdriver.common.keys import Keys
        try:
            driver.find_element(By.TAG_NAME, "body").send_keys(Keys.ESCAPE)
            time.sleep(1)
        except: pass

        close_selectors = [
            "button[data-dismiss='modal']",
            ".modal.in .btn-default",
            ".modal.in .close",
            ".modal.show .btn-default",
            ".modal.show .close",
            "button.btn-primary",
            ".btn-success"
        ]
        
        button_texts = ["KAPAT", "TAMAM", "KABUL EDİYORUM", "OKUDUM", "ANLADIM", "ONAYLIYORUM", "GEÇ", "ATLA", "DAHA SONRA"]
        
        for sel in close_selectors:
            els = driver.find_elements(By.CSS_SELECTOR, sel)
            for el in els:
                try:
                    if el.is_displayed():
                        txt = el.text.upper()
                        if any(bt in txt for bt in button_texts) or sel != "button.btn-primary":
                            driver.execute_script("arguments[0].click();", el)
                            logger.info("Modal/Buton kapatıldı: %s (%s)", sel, txt)
                            time.sleep(1)
                except Exception:
                    pass

        try:
            driver.execute_script("""
                $('.modal-backdrop').remove();
                $('.modal').hide();
                $('body').removeClass('modal-open');
            """)
            logger.info("Modal temizleme (JS) tamamlandı.")
        except: pass
    except Exception as e:
        logger.debug("Modal kapatma hatası (önemsiz): %s", e)


def get_sapid(driver: webdriver.Chrome) -> str | None:
    try:
        logger.info("Dashboard'dan sapid aranıyor...")
        driver.get(STUDENT_HOME_URL)
        time.sleep(3)

        for link in driver.find_elements(By.TAG_NAME, "a"):
            href = link.get_attribute("href") or ""
            if "sapid=" in href:
                sapid = href.split("sapid=")[-1].split("&")[0]
                logger.info("sapid dinamik bulundu: %s", sapid)
                return sapid

        sapid_env = os.getenv("UBYS_SAPID", "").strip()
        if sapid_env:
            logger.info("sapid .env'den okundu: %s", sapid_env)
            return sapid_env

        logger.warning("sapid bulunamadi! Manuel olarak .env dosyasina UBYS_SAPID ekleyebilirsiniz.")
        _save_debug_page(driver, "debug_student_home.html")
        return None

    except Exception as e:
        logger.error("sapid alma hatasi: %s", e)
        return None

def get_course_list(driver: webdriver.Chrome) -> list[dict]:
    courses = []
    sapid = get_sapid(driver)

    if sapid:
        course_list_url = f"{UBYS_BASE}/AIS/Student/Class/Index?sapid={sapid}"
    else:
        course_list_url = f"{UBYS_BASE}/AIS/Student/Class/Index"

    try:
        logger.info("Ders sayfasına gidiliyor: %s", course_list_url)
        driver.get(course_list_url)

        logger.info("Ders tablosunun yüklenmesi bekleniyor (max 90sn)...")
        found = False
        for i in range(45):
            try:
                links = driver.find_elements(By.TAG_NAME, "a")
                if any("ClassDetail" in (l.get_attribute("href") or "") for l in links):
                    logger.info("  ClassDetail linkleri bulundu! (%d. denemede)", i + 1)
                    found = True
                    break
            except Exception:
                pass
            time.sleep(2)

        if not found:
            logger.warning("90sn içinde ClassDetail bulunamadı!")
            _save_debug_page(driver, "debug_course_list.html")
            if "ClassDetail" in driver.page_source:
                logger.info("  Sayfa kaynağında ClassDetail var, BeautifulSoup ile parse ediliyor...")
                courses = _extract_courses_from_source(driver.page_source)
            return courses

        time.sleep(1)
        courses = _extract_courses_from_table(driver)

        if not courses:
            logger.warning("DOM'dan ders çıkarılamadı, sayfa kaynağı deneniyor...")
            _save_debug_page(driver, "debug_course_list.html")
            if "ClassDetail" in driver.page_source:
                courses = _extract_courses_from_source(driver.page_source)

    except Exception as e:
        logger.error("Ders listesi hatası: %s", e)
        _save_debug_page(driver, "debug_course_list.html")

    logger.info("Toplam %d ders: %s", len(courses), [c["name"] for c in courses])
    return courses


def _extract_courses_from_source(page_source: str) -> list[dict]:
    """Sayfa kaynağını regex ile tarayarak ClassDetail linklerini çıkarır."""
    import re
    courses = []
    seen = set()

    pattern = re.compile(
        r'href="(/AIS/Student/Class/ClassDetail\?classId=[^"]+)"[^>]*>\s*([A-Z]{2,6}\d{3,4})\s*</a>',
        re.IGNORECASE
    )
    name_pattern = re.compile(
        r'ClassDetail[^"]*"[^>]*>\s*<i[^>]*></i>\s*([A-Z]{2,6}\d{3,4})\s*</a>.*?<td[^>]*>(.*?)</td>',
        re.DOTALL | re.IGNORECASE
    )

    for m in re.finditer(
        r'href="(/AIS/Student/Class/ClassDetail\?classId=[^"]+)"[^>]*>\s*(?:<i[^>]*/>\s*)?([A-Z]{2,6}\d{3,4})',
        page_source, re.IGNORECASE
    ):
        href = "https://ubys.gibtu.edu.tr" + m.group(1) if m.group(1).startswith("/") else m.group(1)
        code = m.group(2).strip()
        if href in seen:
            continue
        seen.add(href)

        pos = m.end()
        chunk = page_source[pos:pos+500]
        td_m = re.search(r'<td[^>]*>([^<]{3,100})</td>', chunk)
        if td_m:
            name = td_m.group(1).strip()
            course_name = f"{code} - {name}" if name else code
        else:
            course_name = code

        courses.append({"name": course_name, "detail_url": href})
        logger.info("  [Kaynak] Ders: %s", course_name)

    return courses


def _extract_courses_from_table(driver: webdriver.Chrome) -> list[dict]:
    courses = []
    seen = set()
    exclude = ["Geçmiş Dönem", "Derslerini Göster", "Kayıt bulunamadı", "Yükleniyor", "Seçiniz"]

    try:
        all_links = driver.find_elements(By.CSS_SELECTOR, "a[href*='ClassDetail'], a[href*='sapid=']")
        for link in all_links:
            try:
                href = link.get_attribute("href") or ""
                if not href or href in seen:
                    continue

                code_text = link.text.strip().replace("\n", " ").strip()
                if not code_text or any(x in code_text for x in exclude):
                    continue
                
                course_name = code_text
                try:
                    parent_tr = link.find_element(By.XPATH, "./ancestor::tr")
                    cells = parent_tr.find_elements(By.TAG_NAME, "td")
                    for cell in cells:
                        t = cell.text.strip().replace("\n", " ")
                        if t and len(t) > 5 and not any(x in t for x in exclude) and t != code_text:
                            course_name = f"{code_text} - {t}"
                            break
                except: pass

                seen.add(href)
                courses.append({"name": course_name, "detail_url": href})
                logger.info("  ✓ Ders yakalandı: %s", course_name)
            except: continue
    except Exception as e:
        logger.error("Tablo parse hatası: %s", e)
    
    return courses



def _get_courses_from_home(driver: webdriver.Chrome, sapid: str | None) -> list[dict]:
    courses = []
    try:
        driver.get(STUDENT_HOME_URL)
        time.sleep(3)
        courses = _extract_courses_from_table(driver)

        if not courses:
            _save_debug_page(driver, "debug_student_home2.html")

    except Exception as e:
        logger.error("Home'dan ders çekme hatası: %s", e)

    return courses

def make_hash(*args) -> str:
    raw = "|".join(str(a) for a in args)
    return hashlib.md5(raw.encode("utf-8")).hexdigest()


def _click_tab_and_parse(driver: webdriver.Chrome, selectors: list[str], parse_fn) -> list[dict]:
    from selenium.webdriver.support.ui import WebDriverWait
    
    for sel in selectors:
        for el in driver.find_elements(By.CSS_SELECTOR, sel):
            if not el.is_displayed(): continue
            try:
                driver.execute_script("arguments[0].scrollIntoView({block: 'center'}); arguments[0].click();", el)
                time.sleep(1.5)
                
                try:
                    WebDriverWait(driver, 2).until(
                        lambda d: d.find_elements(By.TAG_NAME, "table") or "Kayıt bulunamadı" in d.page_source
                    )
                except: pass
                
                res = parse_fn(driver)
                if res is not None:
                    return res 
            except:
                continue
    return []


def scrape_course_grades(driver: webdriver.Chrome, course: dict) -> list[dict]:
    course_name = course["name"]

    def parse_grades(d):
        try:
            rows = d.find_elements(By.CSS_SELECTOR, "table tr")
            if not rows:
                rows = d.find_elements(By.XPATH, "//tr[td]")
            
            if not rows: return []

            items = []
            headers = [h.text.strip().lower() for h in rows[0].find_elements(By.TAG_NAME, "th")]
            if not headers:
                headers = [h.text.strip().lower() for h in rows[0].find_elements(By.TAG_NAME, "td")]
            
            if not any(x in "".join(headers) for x in ["not", "puan", "sınav", "vize", "final", "grade"]):
                return None

            idx_name = 0
            idx_score = 1
            idx_avg = 2
            idx_letter = 3
            
            for i, h in enumerate(headers):
                if any(x in h for x in ["sınav", "adı", "exam", "name"]): idx_name = i
                if any(x in h for x in ["not", "puan", "score", "grade"]): idx_score = i
                if any(x in h for x in ["ort", "avg"]): idx_avg = i
                if any(x in h for x in ["harf", "letter"]): idx_letter = i

            for row in rows[1:]:
                cells = row.find_elements(By.TAG_NAME, "td")
                if len(cells) <= max(idx_name, idx_score): continue

                exam_name = cells[idx_name].text.strip()
                score     = cells[idx_score].text.strip()
                average   = cells[idx_avg].text.strip() if len(cells) > idx_avg else "-"
                letter    = cells[idx_letter].text.strip() if len(cells) > idx_letter else "-"
                
                if not exam_name or not score: continue
                if score in ("-", "—", "?", "", "Girilmedi"): continue

                old_data = get_existing_grade(course_name, exam_name)
                old_score = old_data[0] if old_data else "Yok"
                old_letter = old_data[1] if old_data and old_data[1] else ""

                logger.info("  [KONTROL - NOT] %s | %s | Eski Değer: %s %s -> Yeni Değer: %s %s | Ort: %s", 
                            course_name, exam_name, old_score, old_letter, score, letter, average)

                h = make_hash(course_name, exam_name, score)
                if is_new_item("grades", h):
                    save_grade(course_name, exam_name, score, letter, h)
                    items.append({
                        "type":      "grade",
                        "course":    course_name,
                        "exam_name": exam_name,
                        "score":     score,
                        "letter":    letter,
                        "average":   average,
                        "old_score": old_score,
                        "old_letter": old_letter
                    })
                    logger.info("    └─> [YENİ SINAV SONUCU BİLDİRİMİ]")
            return items
        except:
            return None

    selectors = [
        "a[href*='Grade']", "a[href*='Not']", "a[href*='Sonuc']",
        "li[onclick*='Grade']", "li[onclick*='Not']", "li[onclick*='Sonuc']"
    ]
    return _click_tab_and_parse(driver, selectors, parse_grades)


def scrape_course_assignments(driver: webdriver.Chrome, course: dict) -> list[dict]:
    course_name = course["name"]

    def parse_assignments(d):
        items = []
        rows = d.find_elements(By.CSS_SELECTOR, "table tr")
        if not rows:
            rows = d.find_elements(By.XPATH, "//tr[td]")
        
        if not rows: return []

        headers = [h.text.strip().lower() for h in rows[0].find_elements(By.TAG_NAME, "th")]
        if not headers: headers = [h.text.strip().lower() for h in rows[0].find_elements(By.TAG_NAME, "td")]
        
        if not any(x in "".join(headers) for x in ["ödev", "teslim", "assignment", "due"]):
            return None

        idx_title = 0
        idx_date = 1
        for i, h in enumerate(headers):
            if any(x in h for x in ["başlık", "ödev", "title", "assignment"]): idx_title = i
            if any(x in h for x in ["teslim", "tarih", "due", "date"]): idx_date = i

        for row in rows[1:]:
            cells = row.find_elements(By.TAG_NAME, "td")
            if len(cells) <= max(idx_title, idx_date): continue

            title    = cells[idx_title].text.strip()
            due_date = cells[idx_date].text.strip() if len(cells) > idx_date else ""
            
            if not title or title in ("Ödev Başlığı", "Ödev Adı", "Başlık"): continue
            desc     = cells[2].text.strip() if len(cells) > 2 else ""

            old_data = get_existing_assignment(course_name, title)
            old_due = old_data[0] if old_data else "Yok"
            
            logger.info("  [KONTROL - ÖDEV] %s | %s | Eski Tarih: %s -> Yeni Tarih: %s", 
                        course_name, title, old_due, due_date)

            h = make_hash(course_name, title, due_date)
            if is_new_item("assignments", h):
                save_assignment(course_name, title, due_date, desc, h)
                items.append({"type": "assignment", "course": course_name,
                              "title": title, "due_date": due_date, "description": desc,
                              "old_due_date": old_due})
                logger.info("    └─> [YENİ ÖDEV BİLDİRİMİ]")
        return items

    selectors = ["a[href*='Assignment']", "a[href*='Odev']", "li[onclick*='Assignment']", "li[onclick*='Odev']"]
    return _click_tab_and_parse(driver, selectors, parse_assignments)


def scrape_course_announcements(driver: webdriver.Chrome, course: dict) -> list[dict]:
    course_name = course["name"]

    def parse_annc(d):
        items = []
        rows = d.find_elements(By.CSS_SELECTOR, "table tr")
        if not rows:
            rows = d.find_elements(By.XPATH, "//tr[td]")
        
        if not rows: return []

        for row in rows:
            cells = row.find_elements(By.TAG_NAME, "td")
            if len(cells) < 1: continue
            title   = cells[0].text.strip()
            
            if not title or title in ("Başlık", "Duyuru Başlığı", "Duyuru"): continue
                
            content = cells[1].text.strip() if len(cells) > 1 else ""

            old_data = get_existing_announcement(course_name, title)
            old_content = old_data[0] if old_data else "Yok"
            old_preview = (old_content[:20] + "...") if old_content != "Yok" else "Yok"
            new_preview = (content[:20] + "...") if content else "Yok"

            logger.info("  [KONTROL - DUYURU] %s | %s | Eski İçerik: %s -> Yeni İçerik: %s",
                        course_name, title, old_preview, new_preview)

            h = make_hash(course_name, title, content[:50])
            if is_new_item("announcements", h):
                save_announcement(course_name, title, content, h)
                items.append({"type": "announcement", "course": course_name,
                              "title": title, "content": content,
                              "old_content": old_content})
                logger.info("    └─> [YENİ DUYURU BİLDİRİMİ]")
        return items

    selectors = ["a[href*='Announcement']", "a[href*='Duyuru']", "li[onclick*='Announcement']", "li[onclick*='Duyuru']"]
    return _click_tab_and_parse(driver, selectors, parse_annc)


def scrape_course_contents(driver: webdriver.Chrome, course: dict) -> list[dict]:
    course_name = course["name"]

    def parse_contents(d):
        items = []
        rows = d.find_elements(By.CSS_SELECTOR, "table tr")
        if not rows:
            rows = d.find_elements(By.XPATH, "//tr[td]")

        for row in rows:
            cells = row.find_elements(By.TAG_NAME, "td")
            if len(cells) < 2: continue
            
            c_name = cells[0].text.strip()
            if not c_name or c_name in ("İçerik Adı", "Adı", "Hafta"): continue
                
            c_type = cells[1].text.strip() if len(cells) > 1 else ""
            c_date = cells[2].text.strip() if len(cells) > 2 else ""
            c_desc = cells[3].text.strip() if len(cells) > 3 else ""

            old_data = get_existing_content(course_name, c_name)
            old_type = old_data[0] if old_data else "Yok"

            logger.info("  [KONTROL - İÇERİK] %s | %s | Eski Tip: %s -> Yeni Tip: %s",
                        course_name, c_name, old_type, c_type)

            h = make_hash(course_name, c_name, c_type, c_date)
            if is_new_item("contents", h):
                save_content(course_name, c_name, c_type, c_date, c_desc, h)
                items.append({
                    "type": "content", 
                    "course": course_name,
                    "name": c_name, 
                    "content_type": c_type,
                    "date": c_date,
                    "description": c_desc
                })
                logger.info("    └─> [YENİ DERS İÇERİĞİ BİLDİRİMİ]")
        return items

    selectors = ["a[href*='Content']", "a[href*='Icerik']", "li[onclick*='Content']", "li[onclick*='Icerik']"]
    return _click_tab_and_parse(driver, selectors, parse_contents)


def _save_debug_page(driver: webdriver.Chrome, filename: str):
    try:
        with open(filename, "w", encoding="utf-8") as f:
            f.write(driver.page_source)
        logger.debug("Debug sayfası kaydedildi: %s", filename)
    except Exception:
        pass

def run_scrape() -> list[dict]:
    init_db()
    driver = None
    all_new = []

    try:
        driver = create_driver()

        if not login(driver):
            logger.error("Giriş yapılamadı, tarama iptal.")
            return []

        courses = get_course_list(driver)
        if not courses:
            logger.warning("Ders bulunamadı! debug_student_home.html dosyasını kontrol edin.")
            return []

        logger.info("=" * 60)
        logger.info("%d ders taranıyor...", len(courses))

        for course in courses:
            logger.info("[DERS] %s", course["name"])
            try:
                # Dersi aç (Bir kez) - Tüm sekmeler burada tıklanacak
                driver.get(course["detail_url"])
                time.sleep(2)
                
                all_new += scrape_course_grades(driver, course)
                all_new += scrape_course_assignments(driver, course)
                all_new += scrape_course_announcements(driver, course)
                all_new += scrape_course_contents(driver, course)
            except Exception as e:
                logger.error("Ders hata [%s]: %s", course["name"], e)

        logger.info("Tarama bitti. Yeni içerik: %d", len(all_new))

    except Exception as e:
        logger.error("Genel hata: %s", e, exc_info=True)
    finally:
        if driver:
            driver.quit()

    return all_new


if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s [%(levelname)s] %(message)s",
        handlers=[logging.StreamHandler(sys.stdout)]
    )
    results = run_scrape()
    print(f"\nYeni içerik sayısı: {len(results)}")
    for r in results:
        print(f"  [{r['type'].upper()}] {r['course']}: {r['title']}")
