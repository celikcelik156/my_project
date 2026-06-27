TURKIYE EMLAK FIYAT TAHMIN SISTEMI

PROJE BILGILERI
Ogrenci Adi Soyadi: Mehmet Celik
Ogrenci Numarasi: 230007028
Ders: Yapay Zekaya Giris
Proje Konusu: Turkiye Emlak Fiyat Tahmin Sistemi
Kullanilan Ortamlar: Python + MATLAB

PROJE OZETI
Bu proje, konut ozelliklerine gore fiyat tahmini yapan basit bir makine ogrenmesi uygulamasidir.
Veri on isleme yapilmadan yalnizca sayisal ozellikler kullanilmistir.
5 regresyon algoritmasi karsilastirilmistir: Linear Regression, Decision Tree, Random Forest, KNN, SVM.
Python (Streamlit arayuz) ve MATLAB ortamlarinda gelistirilmistir.

CALISTIRMA
1. pip install -r requirements.txt
2. python -X utf8 main.py
3. python -X utf8 -m streamlit run ui/app.py
4. MATLAB: matlab/emlak_analiz.m dosyasini calistirin

KLASOR YAPISI
- main.py                 : Model egitimi ve grafik uretimi
- src/veri_hazirla.py     : CSV yukleme ve train/test ayrimi (on isleme YOK)
- src/modeller.py         : 5 regresyon algoritmasi
- src/gorsellestirme.py   : Grafikler
- ui/app.py               : Streamlit arayuzu
- matlab/emlak_analiz.m   : MATLAB versiyonu (5 algoritma)
- rapor/teknik_rapor.docx : Teknik rapor
- data/emlak_veri.csv     : 1200 konut verisi
- grafikler/              : PNG ciktilar

VERI SETI
1200 konut kaydi. Modelde kullanilan sayisal ozellikler:
metrekare, oda_sayisi, bina_yasi, bulundugu_kat, toplam_kat, balkon, esyali, site_icinde, ulasim_skoru
Hedef degisken: fiyat (TL)

NOT: Veri on isleme (normalizasyon, encoding, ozellik muhendisligi) bilincli olarak uygulanmamistir.

ALGORITMALAR
1. Linear Regression
2. Decision Tree Regressor
3. Random Forest Regressor
4. KNN Regressor
5. SVM Regressor

METRIKLER: MAE, MSE, RMSE, R2
