# 🏠 Türkiye Emlak Fiyat Tahmin Sistemi

\---

## 👋 Sayın Sait Hocam,

Merhabalar. Ben Mehmet Çelik, öğrenci numaram **230007028**. Yapay Zekaya Giriş dersinin dönem sonu projesi kapsamında geliştirdiğim bu çalışmayı sizinle paylaşmaktan memnuniyet duyuyorum.

Proje konusu olarak **Türkiye konut piyasasında fiyat tahmini**ni seçtim. Gerçek hayatta sıkça karşılaşılan bu problem üzerinden makine öğrenmesinin gücünü göstermeye çalıştım. Sistem; konum, metrekare, bina yaşı, oda sayısı ve çeşitli konfor özellikleri gibi girdileri alarak bir konutun tahmini piyasa değerini hesaplıyor. Aynı zamanda evi "Ekonomik", "Orta Segment" veya "Lüks" olarak sınıflandırıyor ve benzer konutları K-Means ile kümeleştiriyor.

Projeyi hem **Python** ortamında hem de **MATLAB** ortamında çalışacak şekilde geliştirdim. Arayüz için ise Streamlit kullanarak tarayıcı tabanlı, görsel açıdan zengin bir web uygulaması oluşturdum.

\---

## 📋 Proje Bilgileri

|||
|-|-|
|**Öğrenci Adı Soyadı**|Mehmet Çelik|
|**Öğrenci Numarası**|230007028|
|**Ders**|Yapay Zekaya Giriş|
|**Proje Konusu**|Türkiye Emlak Fiyat Tahmin Sistemi|
|**Kullanılan Ortamlar**|Python + MATLAB|

\---

## 🚀 Programı Çalıştırmak İçin

### ► En Kolay Yol: Çift Tıkla Aç

Proje klasöründeki **`UYGULAMAYI\_AC.bat`** dosyasına **çift tıkla** — gerisini program halleder:

* Modeller yoksa otomatik eğitir (\~1-2 dakika)
* Web arayüzünü başlatır
* Tarayıcıyı otomatik açar → `http://localhost:8501`

### ► Manuel Çalıştırma (Komut Satırı)

```bash
# 1. Bağımlılıkları kur
pip install -r requirements.txt

# 2. Modelleri eğit ve grafikleri oluştur
python -X utf8 main.py

# 3. Web arayüzünü başlat
python -X utf8 -m streamlit run ui/app.py
```

\---

## 📁 Proje Klasör Yapısı

```
yapay zekaya giriş dönem sonu projesi/
│
├── UYGULAMAYI\_AC.bat          ← Buraya çift tıkla, her şey otomatik açılır
├── main.py                    ← Ana program: veri + model eğitimi + grafikler
├── requirements.txt           ← Gerekli Python kütüphaneleri
│
├── data/
│   ├── emlak\_veri.csv         ← 1.200 konut kaydından oluşan veri seti
│   └── modeller.pkl           ← Eğitilmiş makine öğrenmesi modelleri
│
├── src/
│   ├── veri\_olustur.py        ← Türkiye'ye özgü gerçekçi veri üretici
│   ├── on\_isleme.py           ← Veri temizleme, encoding, normalizasyon
│   ├── modeller.py            ← Tüm ML algoritmalarının tanımları
│   └── gorsellestirme.py      ← Grafik oluşturma fonksiyonları
│
├── ui/
│   └── app.py                 ← Streamlit web arayüzü (5 sayfa)
│
├── matlab/
│   └── emlak\_analiz.m         ← MATLAB versiyonu (3 algoritma + grafikler)
│
├── rapor/
│   └── teknik\_rapor.md        ← Teknik rapor (teorik açıklamalar dahil)
│
└── grafikler/                 ← main.py tarafından üretilen PNG grafikler
    ├── 01\_fiyat\_dagilim.png
    ├── 02\_korelasyon\_heatmap.png
    ├── 03\_confusion\_matrix\_\*.png
    ├── 04\_regresyon\_karsilastirma.png
    ├── 05\_siniflandirma\_karsilastirma.png
    ├── 06\_kmeans\_kumeleme.png
    ├── 07\_degisken\_iliskileri.png
    ├── 08\_gercek\_tahmin\_\*.png
    └── 09\_feature\_importance\_\*.png
```

\---

## 🗂️ Veri Seti

Veri seti, Türkiye'nin 10 büyük şehrindeki (İstanbul, Ankara, İzmir, Bursa, Antalya vb.) gerçek emlak ilanlarının özelliklerine dayalı olarak oluşturulmuştur. **1.200 konut kaydı** içermektedir.

|Özellik|Tür|Açıklama|
|-|-|-|
|`fiyat`|Sayısal (hedef)|Konut satış fiyatı (TL)|
|`sehir`|Kategorik|İstanbul, Ankara, İzmir...|
|`ilce`|Kategorik|İlçe bilgisi|
|`metrekare`|Sayısal|Brüt kullanım alanı (m²)|
|`oda\_sayisi`|Sayısal (ayrık)|1+1, 2+1, 3+1...|
|`bina\_yasi`|Sayısal|Binanın yaşı (yıl)|
|`bulundugu\_kat`|Sayısal|Dairenin bulunduğu kat|
|`toplam\_kat`|Sayısal|Binanın toplam kat sayısı|
|`isinma`|Kategorik|Doğalgaz, Kombi, Merkezi...|
|`balkon`|İkili (0/1)|Balkon var mı?|
|`esyali`|İkili (0/1)|Eşyalı mı?|
|`site\_icinde`|İkili (0/1)|Site içinde mi?|
|`ulasim\_skoru`|Sayısal (1-10)|Metro/otobüs erişim puanı|
|`kategori`|Kategorik (hedef)|Ekonomik / Orta Segment / Lüks|

\---

## 🔧 Veri Ön İşleme \& Özellik Mühendisliği

`main.py` çalıştırıldığında aşağıdaki adımlar sırasıyla uygulanır:

1. **Eksik veri doldurma** — Sayısallarda medyan, kategoriklerde mod
2. **Aykırı değer temizleme** — IQR yöntemiyle %1–%99 dışı kayıtlar çıkarılır
3. **Label Encoding** — Şehir, ilçe, ısınma tipi sayısal koda dönüştürülür
4. **Standardizasyon** — Z-score normalizasyonu uygulanır
5. **Yeni özellikler türetme:**

   * `amortisman\_skoru` = exp(−bina\_yaşı / 20) → Bina değer kaybı
   * `kat\_orani` = bulundugu\_kat / (toplam\_kat + 1) → Kat pozisyonu
   * `konfor\_skoru` = Balkon + eşyalı + site + ulaşım ağırlıklı ortalaması
   * `buyukluk\_kategori` = Metrekareye göre 4 gruba ayırma

\---

## 🤖 Kullanılan Algoritmalar

### 📉 Regresyon — Fiyat Tahmini

|#|Algoritma|Açıklama|Sonuç (R²)|
|-|-|-|-|
|1|**Linear Regression**|Doğrusal ilişki modeli|\~0.51|
|2|**Decision Tree**|Karar ağacı regresyon|\~0.81|
|3|**Random Forest** ⭐|Çoklu ağaç topluluğu|**\~0.94**|
|4|**KNN**|K-en yakın komşu|\~0.48|
|5|**SVM**|Destek vektör makinesi|\~0.00\*|

> \*SVM bu veri ölçeğinde ön işleme olmadan düşük performans göstermiştir — bu durum raporda tartışılmıştır.

### 🔵 Sınıflandırma — Fiyat Kategorisi (Ekonomik / Orta / Lüks)

|Algoritma|Accuracy|
|-|-|
|**Decision Tree**|%91.5|
|**Random Forest**|%91.5|
|**KNN**|%77.9|
|**Logistic Regression**|%80.9|

### 🔴 Kümeleme

|Algoritma|Açıklama|
|-|-|
|**K-Means (K=3)**|Benzer konutları otomatik olarak 3 gruba ayırır|

\---

## 📊 Değerlendirme Metrikleri

|Metrik Türü|Kullanılan Metrikler|
|-|-|
|**Regresyon**|MAE, MSE, RMSE, R²|
|**Sınıflandırma**|Accuracy, Precision, Recall, F1-Score|
|**Görsel**|Confusion Matrix, Feature Importance, Gerçek vs Tahmin|
|**Korelasyon**|Pearson Korelasyon, Spearman Korelasyon|
|**Doğrulama**|K-Fold Cross Validation|

\---

## 🖥️ Web Arayüzü Sayfaları

Uygulama **5 sayfadan** oluşmaktadır:

|Sayfa|İçerik|
|-|-|
|🏠 **Ana Sayfa**|Proje özeti, kullanılan algoritmalar|
|🤖 **Fiyat Tahmini**|Ev özelliklerini gir → anlık fiyat tahmini al, 5 modeli karşılaştır|
|📊 **Veri Analizi**|Pearson \& Spearman korelasyon heatmap, şehir bazlı fiyatlar|
|🏆 **Model Karşılaştırma**|Regresyon \& sınıflandırma tabloları, Confusion Matrix|
|📈 **Grafikler**|main.py tarafından üretilen tüm PNG grafikler|

\---

## 🛠️ Kullanılan Teknolojiler

|Araç|Kullanım Amacı|
|-|-|
|**Python 3.x**|Ana programlama dili|
|**Scikit-learn**|Tüm ML algoritmaları|
|**Pandas \& NumPy**|Veri işleme ve matris hesaplamaları|
|**Matplotlib \& Seaborn**|Grafik ve görselleştirme|
|**Streamlit**|Web tabanlı kullanıcı arayüzü|
|**SciPy**|Pearson \& Spearman korelasyon analizleri|
|**MATLAB**|Alternatif platform uygulaması|

\---

> Saygılarımla,  
> \*\*Mehmet Çelik — 230007028\*\*

