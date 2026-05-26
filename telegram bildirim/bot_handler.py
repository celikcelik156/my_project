import time
import logging
import requests
from config import TELEGRAM_BOT_TOKEN
from data_manager import update_user_registration, get_user_by_id, load_previous_state
from menu_scraper import scrape_daily_menu
from datetime import datetime, timedelta

def bot_poll_updates():
    if not TELEGRAM_BOT_TOKEN:
        logging.error("TELEGRAM_BOT_TOKEN ayarlanmamış!")
        return

    logging.info("Bot dinleyici başlatıldı...")
    offset = 0
    retry_delay = 5
    
    while True:
        try:
            url = f"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/getUpdates"
            params = {"offset": offset, "timeout": 30}
            response = requests.get(url, params=params, timeout=40)
            
            if response.status_code == 200:
                retry_delay = 5
                data = response.json()
                if data.get("ok"):
                    for result in data.get("result", []):
                        update_id = result.get("update_id")
                        offset = update_id + 1
                        
                        message = result.get("message")
                        if message:
                            handle_message(message)
            else:
                logging.error(f"Bot dinleyici hatası: {response.status_code}")
                time.sleep(retry_delay)
                retry_delay = min(retry_delay * 2, 60)
                
        except requests.exceptions.Timeout:
            logging.warning(f"Bot dinleyici zaman aşımı (Timeout). {retry_delay} saniye sonra yeniden bağlanılacak...")
            time.sleep(retry_delay)
            retry_delay = min(retry_delay * 2, 60)
            
        except requests.exceptions.ConnectionError:
            logging.warning(f"Bot dinleyici bağlantı hatası (İnternet veya DNS sorunu olabilir). {retry_delay} saniye sonra yeniden denenecek...")
            time.sleep(retry_delay)
            retry_delay = min(retry_delay * 2, 60)
            
        except Exception as e:
            err_msg = str(e)
            if TELEGRAM_BOT_TOKEN and TELEGRAM_BOT_TOKEN in err_msg:
                err_msg = err_msg.replace(TELEGRAM_BOT_TOKEN, "[GİZLİ_TOKEN]")
            logging.error(f"Bot dinleme döngüsünde hata: {err_msg}")
            time.sleep(retry_delay)
            retry_delay = min(retry_delay * 2, 60)

def handle_message(message):
    chat_id = message.get("chat", {}).get("id")
    text = message.get("text", "").strip()
    
    if not chat_id or not text:
        return

    user = get_user_by_id(chat_id)
    
    if text == "/start":
        update_user_registration(chat_id, registration_state="WAITING_USERNAME")
        send_reply(chat_id, "👋 Merhaba! GİBTÜ UBYS bildirim sistemine hoş geldiniz.\n\nLütfen UBYS **Kullanıcı Adınızı** (Öğrenci No) yazın:")
        return

    if not user:
        send_reply(chat_id, "Lütfen sistemi başlatmak için /start komutunu kullanın.")
        return

    state = user.get("registration_state")

    if state == "WAITING_USERNAME":
        clean_username = text.replace(" ", "").strip()
        update_user_registration(chat_id, username=clean_username, registration_state="WAITING_PASSWORD")
        send_reply(chat_id, "Teşekkürler! Şimdi lütfen UBYS **Şifrenizi** yazın:\n(Not: Şifreniz veritabanında güvenli bir şekilde saklanacaktır.)")
        
    elif state == "WAITING_PASSWORD":
        clean_password = text.replace(" ", "").strip()
        update_user_registration(chat_id, password=clean_password, registration_state="COMPLETED", is_active=1)
        send_reply(chat_id, "✅ Kaydınız başarıyla tamamlandı!\n\nArtık UBYS sisteminizde bir değişiklik (not, ödev, duyuru) olduğunda size buradan bildirim göndereceğim.\n\nSistem her 20 dakikada bir otomatik tarama yapacaktır.")
        logging.info(f"Yeni kullanıcı kaydedildi: {chat_id}")
        
    elif text == "/durum":
        send_reply(chat_id, f"Sistem şu an aktif. ✅\nKullanıcı: {user.get('username')}")

    elif text == "/notlar":
        state = load_previous_state(chat_id)
        if not state:
            send_reply(chat_id, "Henüz kaydedilmiş bir notunuz bulunmuyor. İlk taramanın yapılmasını bekleyin.")
            return
        
        msg = "<b>📊 Güncel Notlarınız</b>\n\n"
        found = False
        for code, data in state.items():
            grades = data.get("grades", {})
            if grades:
                found = True
                msg += f"🔹 <b>{data.get('name', code)}</b>\n"
                for exam, grade in grades.items():
                    msg += f"   - {exam}: {grade}\n"
                msg += "\n"
        
        if not found:
            msg = "Sistemde henüz açıklanmış bir notunuz bulunmuyor."
        send_reply(chat_id, msg, parse_mode="HTML")

    elif text == "/duyurular":
        state = load_previous_state(chat_id)
        if not state:
            send_reply(chat_id, "Henüz kayıtlı veri yok.")
            return
        
        msg = "<b>📢 Son Duyurular</b>\n\n"
        found = False
        for code, data in state.items():
            anns = data.get("announcements", [])
            if anns:
                found = True
                msg += f"🔹 <b>{data.get('name', code)}</b>\n"
                for ann in anns[:2]:
                    title = ann.get("title", "İsimsiz")
                    date = ann.get("date", "")
                    msg += f"   • {date} - {title}\n"
                msg += "\n"
        
        if not found:
            msg = "Henüz bir duyuru bulunmuyor."
        send_reply(chat_id, msg, parse_mode="HTML")

    elif text == "/yemek":
        send_reply(chat_id, "🍴 Yemek listesi getiriliyor, lütfen bekleyin...")
        menu = scrape_daily_menu()
        if menu:
            msg = f"<b>🍴 Günün Yemek Listesi</b>\n📅 {datetime.now().strftime('%d.%m.%Y')}\n\n{menu}"
        else:
            msg = "Maalesef şu an yemek listesine ulaşılamıyor."
        send_reply(chat_id, msg, parse_mode="HTML")

    elif text == "/yarinyemek":
        send_reply(chat_id, "🍴 Yarınki yemek listesi getiriliyor, lütfen bekleyin...")
        tomorrow = datetime.now() + timedelta(days=1)
        menu = scrape_daily_menu(tomorrow)
        if menu:
            msg = f"<b>🍴 Yarınki Yemek Listesi</b>\n📅 {tomorrow.strftime('%d.%m.%Y')}\n\n{menu}"
        else:
            msg = "Yarın için henüz yemek listesi yayınlanmamış veya ulaşılamıyor."
        send_reply(chat_id, msg, parse_mode="HTML")

    elif text == "/odevler":
        state = load_previous_state(chat_id)
        if not state:
            send_reply(chat_id, "Henüz kayıtlı veri yok.")
            return
        
        msg = "<b>📝 Güncel Ödevleriniz</b>\n\n"
        found = False
        for code, data in state.items():
            assignments = data.get("assignments", [])
            if assignments:
                found = True
                msg += f"🔹 <b>{data.get('name', code)}</b>\n"
                for odev in assignments:
                    title = odev.get("title", "İsimsiz Ödev")
                    due_date = odev.get("due_date", "Tarih Belirtilmemiş")
                    msg += f"   • {title}\n"
                    msg += f"     📅 Son Teslim: {due_date}\n"
                msg += "\n"
        
        if not found:
            msg = "Şu an için kayıtlı bir ödeviniz bulunmuyor."
        send_reply(chat_id, msg, parse_mode="HTML")

    elif text == "/yardim":
        msg = (
            "<b>🤖 Yardım Menüsü</b>\n\n"
            "Aşağıdaki komutları kullanarak bilgi alabilirsiniz:\n"
            "/notlar - Tüm derslerinizdeki güncel notları listeler.\n"
            "/duyurular - Derslerinizdeki son duyuruları gösterir.\n"
            "/odevler - Güncel ödevlerinizi ve teslim tarihlerini listeler.\n"
            "/yemek - Günün yemek menüsünü getirir.\n"
            "/yarinyemek - Yarınki yemek menüsünü getirir.\n"
            "/durum - Kayıtlı bilgilerinizi ve sistem durumunu gösterir.\n"
            "/yardim - Bu menüyü gösterir.\n\n"
            "<i>Sistem her 20 dakikada bir otomatik tarama yapar ve değişiklik olduğunda size haber verir.</i>"
        )
        send_reply(chat_id, msg, parse_mode="HTML")
    
    else:
        send_reply(chat_id, "Anlayamadım. 🤔\nKomutları görmek için /yardim yazabilirsiniz.")

def send_reply(chat_id, text, parse_mode="Markdown"):
    url = f"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendMessage"
    payload = {
        "chat_id": chat_id,
        "text": text,
        "parse_mode": parse_mode
    }
    try:
        requests.post(url, json=payload)
    except Exception as e:
        err_msg = str(e)
        if TELEGRAM_BOT_TOKEN and TELEGRAM_BOT_TOKEN in err_msg:
            err_msg = err_msg.replace(TELEGRAM_BOT_TOKEN, "[GİZLİ_TOKEN]")
        logging.error(f"Mesaj yanıtlama hatası: {err_msg}")

if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO)
    bot_poll_updates()
