import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np
import pandas as pd
import os

PROJE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
GRAFIK_DIR = os.path.join(PROJE_DIR, "grafikler")
RENKLER = ["#4f46e5", "#7c3aed", "#0891b2", "#10b981", "#f59e0b"]


def grafik_kaydet(fig, isim):
    os.makedirs(GRAFIK_DIR, exist_ok=True)
    yol = os.path.join(GRAFIK_DIR, f"{isim}.png")
    fig.savefig(yol, bbox_inches="tight", dpi=120)
    plt.close(fig)
    print(f"  Grafik kaydedildi: {isim}.png")
    return yol


def algoritma_karsilastirma(metrikler_listesi):
    df_m = pd.DataFrame(metrikler_listesi)
    fig, axes = plt.subplots(1, 3, figsize=(18, 5))
    renkler = RENKLER[: len(df_m)]
    for ax, (metrik, baslik) in zip(axes, [("R2", "R2 Skoru"), ("RMSE", "RMSE"), ("MAE", "MAE")]):
        bars = ax.bar(df_m["model"], df_m[metrik], color=renkler)
        ax.set_title(baslik)
        ax.tick_params(axis="x", rotation=20)
        for bar, val in zip(bars, df_m[metrik]):
            label = f"{val:.3f}" if metrik == "R2" else f"{val/1e6:.2f}M"
            ax.text(bar.get_x() + bar.get_width() / 2, bar.get_height(), label, ha="center", va="bottom", fontsize=7)
    plt.suptitle("5 Algoritma Performans Karsilastirmasi")
    plt.tight_layout()
    return grafik_kaydet(fig, "01_algoritma_karsilastirma")


def gercek_tahmin_grafigi(y_gercek, y_tahmin, model_adi):
    fig, ax = plt.subplots(figsize=(6, 5))
    y_g = np.array(y_gercek) / 1e6
    y_t = np.array(y_tahmin) / 1e6
    ax.scatter(y_g, y_t, alpha=0.5, s=20)
    min_val = min(y_g.min(), y_t.min())
    max_val = max(y_g.max(), y_t.max())
    ax.plot([min_val, max_val], [min_val, max_val], "r--", label="Ideal cizgi")
    ax.set_xlabel("Gercek Fiyat (Milyon TL)")
    ax.set_ylabel("Tahmin (Milyon TL)")
    ax.set_title(f"{model_adi}: Gercek vs Tahmin")
    ax.legend()
    plt.tight_layout()
    isim = f"02_gercek_tahmin_{model_adi.replace(' ', '_').lower()}"
    return grafik_kaydet(fig, isim)


def korelasyon_grafigi(df):
    sayisal = df.select_dtypes(include="number")
    fig, ax = plt.subplots(figsize=(8, 6))
    sns.heatmap(sayisal.corr(method="pearson"), annot=True, fmt=".2f", cmap="RdYlGn", ax=ax, center=0)
    ax.set_title("Pearson Korelasyon Matrisi")
    plt.tight_layout()
    return grafik_kaydet(fig, "03_korelasyon")
