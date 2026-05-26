import logging
import requests
from config import TELEGRAM_BOT_TOKEN, LOG_FILE_PATH

# Loglama ayarları
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler(LOG_FILE_PATH, encoding='utf-8'),
        logging.StreamHandler() # Terminalde göstermek için
    ]
)

def send_telegram_message(message_text, chat_id):
    if not TELEGRAM_BOT_TOKEN or not chat_id or TELEGRAM_BOT_TOKEN == "YOUR_TELEGRAM_BOT_TOKEN":
        logging.warning(f"Telegram Bot Token veya Chat ID ({chat_id}) ayarlanmamış. Mesaj gönderilmedi.")
        print(f"[YAZDIRILAN MESAJ - {chat_id}]: {message_text}")
        return False

    url = f"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendMessage"
    payload = {
        "chat_id": str(chat_id),
        "text": message_text,
        "parse_mode": "HTML"
    }
    
    try:
        response = requests.post(url, json=payload)
        if response.status_code == 200:
            logging.info(f"Mesaj başarıyla Telegram'a gönderildi: {message_text}")
            return True
        else:
            logging.error(f"Telegram mesajı gönderilemedi. Hata Kodu: {response.status_code}, Detay: {response.text}")
            return False
    except Exception as e:
        err_msg = str(e)
        if TELEGRAM_BOT_TOKEN and TELEGRAM_BOT_TOKEN in err_msg:
            err_msg = err_msg.replace(TELEGRAM_BOT_TOKEN, "[GİZLİ_TOKEN]")
        logging.error(f"Telegram isteği sırasında hata oluştu: {err_msg}")
        return False

def process_and_notify(changes):
    if not changes:
        logging.info("Herhangi bir değişiklik tespit edilmedi.")
        return

    for change in changes:
        try:
            msg = ""
            c_type = change.get("type")
            course_code = change.get('course_code', 'Bilinmeyen Kod')
            course_name = change.get('course_name', 'Bilinmeyen Ders')
            course_info = f"<b>{course_code} - {course_name}</b>"
            
            if c_type == "grade":
                exam_type_original = change.get('exam_type', '')
                exam_type_lower = str(exam_type_original).lower()
                grade = change.get('grade', '')
                average = change.get('average', '')
                
                if "vize" in exam_type_lower:
                    exam_label = "Vize"
                elif "final" in exam_type_lower:
                    exam_label = "Final"
                elif "bütünleme" in exam_type_lower:
                    exam_label = "Bütünleme"
                else:
                    exam_label = exam_type_original
                
                msg = (
                    f"<b>🎓 Sınav Notu Açıklandı</b>\n"
                    f"{course_info}\n\n"
                    f"<b>Sınav:</b> {exam_label}\n"
                    f"<b>Notunuz:</b> {grade}\n"
                    f"<b>Sınıf Ortalaması:</b> {average}"
                )
            elif c_type == "announcement":
                ann_data = change.get("announcement_data")
                if isinstance(ann_data, dict):
                    ann_title = ann_data.get("title", "İsimsiz Duyuru")
                    ann_date = ann_data.get("date", "")
                    ann_content = ann_data.get("content", "")
                    
                    msg = f"<b>📢 Yeni Duyuru Eklendi</b>\n{course_info}\n\n"
                    if ann_date:
                        msg += f"<b>Tarih:</b> {ann_date}\n"
                    msg += f"<b>Başlık:</b> {ann_title}\n\n"
                    msg += f"<b>İçerik:</b>\n<i>{ann_content}</i>"
                else:
                    msg = (
                        f"<b>📢 Yeni Duyuru Eklendi</b>\n"
                        f"{course_info}\n\n"
                        f"{change.get('content', '')}"
                    )
            elif c_type == "assignment":
                title = change.get('title', 'İsimsiz Ödev')
                msg = (
                    f"<b>📝 Yeni Ödev Eklendi</b>\n"
                    f"{course_info}\n\n"
                    f"<b>Ödev Tanımı:</b> {title}"
                )
                due_date = change.get('due_date')
                if due_date and due_date != "Belirtilmemiş":
                    msg += f"\n<b>Son Teslim Tarihi:</b> {due_date}"
                    
            elif c_type == "content":
                content_data = change.get('content_data')
                
                if isinstance(content_data, dict):
                    c_name = content_data.get("name", "Bilinmeyen İçerik")
                    c_type_str = content_data.get("type", "")
                    c_date = content_data.get("date", "")
                    c_desc = content_data.get("desc", "")
                    
                    msg = f"<b>📚 Yeni Ders İçeriği Eklendi</b>\n{course_info}\n\n"
                    msg += f"<b>İçerik Adı:</b> {c_name}\n"
                    
                    if c_type_str:
                        msg += f"<b>İçerik Tipi:</b> {c_type_str}\n"
                    if c_date:
                        msg += f"<b>Yayınlanma Tarihi:</b> {c_date}\n"
                    if c_desc:
                        msg += f"\n<b>Açıklama:</b>\n<i>{c_desc}</i>"
                else:
                    # Geriye dönük uyumluluk (eski tip string gelirse)
                    msg = (
                        f"<b>📚 Yeni Ders İçeriği Eklendi</b>\n"
                        f"{course_info}\n"
                        f"{change.get('content_name', '')}"
                    )
            
            if msg:
                chat_id = change.get("chat_id")
                if chat_id:
                    send_telegram_message(msg, chat_id)
        except Exception as e:
            logging.error(f"Bildirim hazırlanırken hata oluştu: {e}")
