# UBYS WhatsApp Bildirim Sistemi

GİBTÜ UBYS portalını otomatik olarak izleyen ve **yeni sınav duyurusu** veya **ödev yüklemesi** olduğunda belirtilen WhatsApp grubuna mesaj gönderen sistem.

\---

## 📁 Klasör Yapısı

```
whatsapp bildirim oluşturucu/
├── scraper/
│   ├── ubys\_scraper.py       ← UBYS tarayıcı
│   ├── db\_manager.py         ← Veritabanı yöneticisi
│   ├── whatsapp\_sender.py    ← WhatsApp mesaj gönderici
│   └── scheduler.py          ← Ana program döngüsü
├── config/
│   └── .env                  ← Ayar dosyası (buraya bilgilerinizi girin)
├── data/
│   ├── notifications.db      ← Veri tabanı (otomatik oluşur)
│   └── ubys\_notifier.log     ← Program günlüğü (otomatik oluşur)
├── setup.bat                 ← Bir kere çalıştır (kurulum)
├── start.bat                 ← Programı başlat
├── test\_login.bat            ← Sadece giriş testi yap
└── requirements.txt          ← Python paket listesi
```

\---

## 🚀 Kurulum (İlk Kez)

### 1\. Ayar dosyasını düzenleyin

`config/.env` dosyasını Not Defteri ile açın ve şu alanları doldurun:

```
UBYS\_USERNAME=
UBYS\_PASSWORD=

WHATSAPP\_GROUP\_NAME=Grubunuzun Adı
CHECK\_INTERVAL\_MINUTES=15
```

> \*\*❗ WHATSAPP\_GROUP\_NAME\*\*: WhatsApp'ta grubun tam adını yazın (büyük/küçük harf önemli).

### 2\. Kurulum scriptini çalıştırın

`setup.bat` dosyasına çift tıklayın. Python paketleri otomatik yüklenir.

### 3\. Test edin

`test\_login.bat` çalıştırın → UBYS'e giriş yapılır ve dersler listelenir.

\---

## ▶️ Kullanım

`start.bat` dosyasına çift tıklayın.

### İlk Çalıştırmada:

1. Tarayıcıda **WhatsApp Web** açılır
2. Telefonunuzla **QR kodunu taratın**
3. Sistem her 15 dakikada bir UBYS'i kontrol etmeye başlar
4. Yeni sınav/ödev bulunduğunda otomatik mesaj gönderilir

### Sistem Çalışırken:

* Pencereyi **kapatmayın** (arka planda çalışır)
* Durdurmak için pencereye tıklayıp **Ctrl+C** basın
* Log dosyası: `data/ubys\_notifier.log`

\---

## 📩 Mesaj Örnekleri

**Sınav Duyurusu:**

```
📢 Sınav Duyurusu 📢

📚 Ders: Veri Madenciliği
📝 Sınav: Vize Sınavı
🏷️ Tür: Vize
📅 Tarih: 25.04.2026 10:00

⏰ Bildirim: 11.04.2026 20:15
```

**Yeni Ödev:**

```
📝 Yeni Ödev 📝

📚 Ders: Proje Yönetimi
📌 Ödev: Hafta 5 Ödevi
⏳ Son Teslim: 18.04.2026 23:59

⏰ Bildirim: 11.04.2026 20:15
```

\---

## ⚙️ Ayarlar (`config/.env`)

|Ayar|Açıklama|Varsayılan|
|-|-|-|
|`UBYS\_USERNAME`|Öğrenci numaranız|-|
|`UBYS\_PASSWORD`|UBYS şifreniz|-|
|`WHATSAPP\_GROUP\_NAME`|WhatsApp grup adı|Ders Grubu|
|`CHECK\_INTERVAL\_MINUTES`|Kontrol aralığı (dakika)|15|
|`HEADLESS`|Tarayıcı görünür mü?|True|
|`LOG\_LEVEL`|Log seviyesi|INFO|

\---

## ❓ Sorun Giderme

**"Giriş başarısız" hatası:**

* `config/.env` dosyasındaki kullanıcı adı/şifreyi kontrol edin

**"Ders bulunamadı" uyarısı:**

* `debug\_courses.html` dosyası oluşturulur, içini kontrol edin
* Tarayıcıyı görünür yapmak için `.env` dosyasında `HEADLESS=False` yapın

**WhatsApp mesajı gitmiyor:**

* `WHATSAPP\_GROUP\_NAME` değerinin grup adıyla birebir aynı olduğunu kontrol edin
* WhatsApp Web'de oturum açık mı kontrol edin

