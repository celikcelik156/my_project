import pandas as pd
import numpy as np
from sklearn.preprocessing import LabelEncoder, StandardScaler, MinMaxScaler
from sklearn.model_selection import train_test_split

def veri_yukle(dosya_yolu):
    df = pd.read_csv(dosya_yolu, encoding="utf-8-sig")
    print(f"✅ Veri yüklendi: {df.shape[0]} satır, {df.shape[1]} sütun")
    return df

def eksik_veri_analiz(df):
    eksik = df.isnull().sum()
    eksik_yuzde = (eksik / len(df)) * 100
    return pd.DataFrame({"Eksik Sayı": eksik, "Yüzde": eksik_yuzde})[eksik > 0]

def veri_on_isleme(df):
    df = df.copy()
    
    sayisal_sutunlar = df.select_dtypes(include=[np.number]).columns
    for col in sayisal_sutunlar:
        if df[col].isnull().sum() > 0:
            df[col].fillna(df[col].median(), inplace=True)
    
    kategorik_sutunlar = df.select_dtypes(include=["object"]).columns
    for col in kategorik_sutunlar:
        if df[col].isnull().sum() > 0:
            df[col].fillna(df[col].mode()[0], inplace=True)
    
    Q1 = df["fiyat"].quantile(0.01)
    Q3 = df["fiyat"].quantile(0.99)
    df = df[(df["fiyat"] >= Q1) & (df["fiyat"] <= Q3)]
    
    le_sehir = LabelEncoder()
    le_ilce = LabelEncoder()
    le_isinma = LabelEncoder()
    le_kategori = LabelEncoder()
    
    df["sehir_kod"] = le_sehir.fit_transform(df["sehir"])
    df["ilce_kod"] = le_ilce.fit_transform(df["ilce"])
    df["isinma_kod"] = le_isinma.fit_transform(df["isinma"])
    df["kategori_kod"] = le_kategori.fit_transform(df["kategori"])
    
    encoders = {
        "sehir": le_sehir,
        "ilce": le_ilce,
        "isinma": le_isinma,
        "kategori": le_kategori
    }
    
    return df, encoders

def ozellik_muhendisligi(df):
    df = df.copy()
    
    df["metrekare_basi_fiyat"] = df["fiyat"] / df["metrekare"]
    
    df["kat_orani"] = df["bulundugu_kat"] / (df["toplam_kat"] + 1)
    
    df["amortisman_skoru"] = np.exp(-df["bina_yasi"] / 20)
    
    df["konfor_skoru"] = (
        df["balkon"] * 0.2 +
        df["esyali"] * 0.3 +
        df["site_icinde"] * 0.3 +
        df["ulasim_skoru"] / 10 * 0.2
    )
    
    df["buyukluk_kategori"] = pd.cut(
        df["metrekare"],
        bins=[0, 75, 120, 200, 999],
        labels=[0, 1, 2, 3]
    ).astype(int)
    
    return df

def veri_hazirla(df, hedef="fiyat", test_orani=0.2):
    ozellikler = [
        "metrekare", "oda_sayisi", "bina_yasi", "bulundugu_kat",
        "toplam_kat", "balkon", "esyali", "site_icinde",
        "sehir_kod", "ilce_kod", "isinma_kod", "ulasim_skoru",
        "kat_orani", "amortisman_skoru", "konfor_skoru", "buyukluk_kategori"
    ]
    
    X = df[ozellikler]
    y_fiyat = df["fiyat"]
    y_kategori = df["kategori_kod"]
    
    X_train, X_test, y_fiyat_train, y_fiyat_test = train_test_split(
        X, y_fiyat, test_size=test_orani, random_state=42
    )
    _, _, y_kat_train, y_kat_test = train_test_split(
        X, y_kategori, test_size=test_orani, random_state=42
    )
    
    scaler = StandardScaler()
    X_train_scaled = scaler.fit_transform(X_train)
    X_test_scaled = scaler.transform(X_test)
    
    return {
        "X_train": X_train, "X_test": X_test,
        "X_train_scaled": X_train_scaled, "X_test_scaled": X_test_scaled,
        "y_fiyat_train": y_fiyat_train, "y_fiyat_test": y_fiyat_test,
        "y_kat_train": y_kat_train, "y_kat_test": y_kat_test,
        "scaler": scaler, "ozellikler": ozellikler
    }
