import pandas as pd
from sklearn.model_selection import train_test_split

OZELLIKLER = [
    "metrekare", "oda_sayisi", "bina_yasi", "bulundugu_kat",
    "toplam_kat", "balkon", "esyali", "site_icinde", "ulasim_skoru"
]


def veri_yukle(dosya_yolu):
    df = pd.read_csv(dosya_yolu, encoding="utf-8-sig")
    print(f"Veri yuklendi: {len(df)} satir")
    return df


def veri_hazirla(df, test_orani=0.2):
    X = df[OZELLIKLER]
    y = df["fiyat"]
    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=test_orani, random_state=42
    )
    return {
        "X_train": X_train,
        "X_test": X_test,
        "y_train": y_train,
        "y_test": y_test,
        "ozellikler": OZELLIKLER,
    }
