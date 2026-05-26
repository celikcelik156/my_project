import numpy as np
import pandas as pd
import random

np.random.seed(42)
random.seed(42)

SEHIRLER = {
    "İstanbul": {"carpan": 3.2, "ilceler": ["Kadıköy", "Beşiktaş", "Şişli", "Üsküdar", "Ataşehir", "Maltepe", "Bağcılar", "Esenyurt", "Pendik", "Sarıyer"]},
    "Ankara":   {"carpan": 1.6, "ilceler": ["Çankaya", "Keçiören", "Mamak", "Etimesgut", "Yenimahalle", "Sincan", "Pursaklar", "Gölbaşı"]},
    "İzmir":    {"carpan": 2.0, "ilceler": ["Konak", "Karşıyaka", "Bornova", "Bayraklı", "Buca", "Çiğli", "Gaziemir", "Balçova"]},
    "Bursa":    {"carpan": 1.3, "ilceler": ["Nilüfer", "Osmangazi", "Yıldırım", "Mudanya", "Gürsu"]},
    "Antalya":  {"carpan": 1.8, "ilceler": ["Muratpaşa", "Kepez", "Konyaaltı", "Döşemealtı", "Alanya"]},
    "Adana":    {"carpan": 0.9, "ilceler": ["Seyhan", "Çukurova", "Yüreğir", "Sarıçam"]},
    "Konya":    {"carpan": 0.95, "ilceler": ["Selçuklu", "Karatay", "Meram", "Beyşehir"]},
    "Kayseri":  {"carpan": 0.85, "ilceler": ["Melikgazi", "Kocasinan", "Talas", "Develi"]},
    "Trabzon":  {"carpan": 1.0, "ilceler": ["Ortahisar", "Akçaabat", "Araklı", "Yomra"]},
    "Gaziantep":{"carpan": 0.88, "ilceler": ["Şehitkamil", "Şahinbey", "Nizip", "Islahiye"]},
}

ISINMA = ["Doğalgaz", "Kombi", "Merkezi Sistem", "Soba", "Klima", "Jeotermal"]
ISINMA_KOD = {v: i for i, v in enumerate(ISINMA)}

def uret_veri(n=1000):
    veriler = []
    
    for _ in range(n):
        sehir = random.choice(list(SEHIRLER.keys()))
        ilce = random.choice(SEHIRLER[sehir]["ilceler"])
        carpan = SEHIRLER[sehir]["carpan"]
        
        oda_sayisi = random.choice([1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 5])
        metrekare = int(np.random.normal(55 + oda_sayisi * 25, 20))
        metrekare = max(35, min(400, metrekare))
        
        bina_yasi = random.randint(0, 40)
        bulundugu_kat = random.randint(0, 20)
        toplam_kat = max(bulundugu_kat + 1, bulundugu_kat + random.randint(1, 5))
        toplam_kat = min(toplam_kat, 25)
        
        balkon = random.choice([0, 1])
        esyali = random.choice([0, 0, 1])
        site_icinde = random.choice([0, 1])
        
        isinma = random.choice(ISINMA)
        ulasim_skoru = random.randint(1, 10)
        
        baz_fiyat = 8000
        
        baz_fiyat *= carpan
        
        baz_fiyat *= (0.8 + metrekare / 250)
        
        if bina_yasi == 0:
            yas_carpan = 1.35
        elif bina_yasi <= 5:
            yas_carpan = 1.20
        elif bina_yasi <= 10:
            yas_carpan = 1.10
        elif bina_yasi <= 20:
            yas_carpan = 1.0
        else:
            yas_carpan = max(0.6, 1.0 - bina_yasi * 0.015)
        baz_fiyat *= yas_carpan
        
        kat_carpan = 1.0
        if bulundugu_kat == 0: kat_carpan = 0.90  # zemin
        elif bulundugu_kat >= toplam_kat - 1: kat_carpan = 1.05  # çatı katı
        baz_fiyat *= kat_carpan
        
        if site_icinde: baz_fiyat *= 1.12
        if esyali: baz_fiyat *= 1.08
        if balkon: baz_fiyat *= 1.03
        
        baz_fiyat *= (0.9 + ulasim_skoru * 0.02)
        
        if isinma in ["Doğalgaz", "Kombi"]: baz_fiyat *= 1.05
        elif isinma == "Merkezi Sistem": baz_fiyat *= 1.02
        elif isinma == "Soba": baz_fiyat *= 0.90
        
        baz_fiyat *= np.random.normal(1.0, 0.08)
        
        toplam_fiyat = int(baz_fiyat * metrekare / 1000) * 1000
        toplam_fiyat = max(300000, toplam_fiyat)
        
        if toplam_fiyat < 2_000_000:
            kategori = "Ekonomik"
        elif toplam_fiyat < 5_000_000:
            kategori = "Orta Segment"
        else:
            kategori = "Lüks"
        
        veriler.append({
            "fiyat": toplam_fiyat,
            "sehir": sehir,
            "ilce": ilce,
            "metrekare": metrekare,
            "oda_sayisi": oda_sayisi,
            "bina_yasi": bina_yasi,
            "bulundugu_kat": bulundugu_kat,
            "toplam_kat": toplam_kat,
            "balkon": balkon,
            "esyali": esyali,
            "site_icinde": site_icinde,
            "isinma": isinma,
            "ulasim_skoru": ulasim_skoru,
            "kategori": kategori
        })
    
    df = pd.DataFrame(veriler)
    return df

if __name__ == "__main__":
    df = uret_veri(1200)
    cikis = r"c:\Users\mehme\OneDrive\Desktop\yapay zekaya giriş dönem sonu projesi\data\emlak_veri.csv"
    df.to_csv(cikis, index=False, encoding="utf-8-sig")
    print(f"✅ {len(df)} satır veri oluşturuldu!")
    print(f"📊 Sütunlar: {list(df.columns)}")
    print(f"\n📈 Fiyat istatistikleri:")
    print(df["fiyat"].describe())
    print(f"\n🏘️ Kategori dağılımı:")
    print(df["kategori"].value_counts())
