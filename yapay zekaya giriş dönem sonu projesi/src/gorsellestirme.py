
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import seaborn as sns
import numpy as np
import pandas as pd
from scipy import stats
import os

plt.rcParams["font.family"] = "DejaVu Sans"
plt.rcParams["figure.dpi"] = 120
RENK_PALETI = ["#6366f1", "#8b5cf6", "#06b6d4", "#10b981", "#f59e0b", "#ef4444"]
GRAFIK_DIR = r"c:\Users\mehme\OneDrive\Desktop\yapay zekaya giriş dönem sonu projesi\grafikler"

def grafik_kaydet(fig, isim):
    os.makedirs(GRAFIK_DIR, exist_ok=True)
    yol = os.path.join(GRAFIK_DIR, f"{isim}.png")
    fig.savefig(yol, bbox_inches="tight", dpi=150, facecolor=fig.get_facecolor())
    plt.close(fig)
    print(f"   💾 Kaydedildi: {isim}.png")
    return yol

def fiyat_dagilim_grafigi(df):
    fig, axes = plt.subplots(1, 3, figsize=(18, 5))
    fig.patch.set_facecolor("#0f172a")
    
    for ax in axes:
        ax.set_facecolor("#1e293b")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
    
    axes[0].hist(df["fiyat"] / 1_000_000, bins=40, color="#6366f1", edgecolor="#4338ca", alpha=0.9)
    axes[0].set_title("Fiyat Dağılımı", color="white", fontsize=13, fontweight="bold")
    axes[0].set_xlabel("Fiyat (Milyon TL)", color="#94a3b8")
    axes[0].set_ylabel("Konut Sayısı", color="#94a3b8")
    
    sehir_ort = df.groupby("sehir")["fiyat"].mean().sort_values(ascending=True) / 1_000_000
    bars = axes[1].barh(sehir_ort.index, sehir_ort.values, color=RENK_PALETI[:len(sehir_ort)])
    axes[1].set_title("Şehir Bazlı Ortalama Fiyat", color="white", fontsize=13, fontweight="bold")
    axes[1].set_xlabel("Ortalama Fiyat (Milyon TL)", color="#94a3b8")
    
    kategori_sayilari = df["kategori"].value_counts()
    wedges, texts, autotexts = axes[2].pie(
        kategori_sayilari.values,
        labels=kategori_sayilari.index,
        autopct="%1.1f%%",
        colors=["#10b981", "#6366f1", "#f59e0b"],
        textprops={"color": "white"},
        startangle=90
    )
    axes[2].set_title("Fiyat Kategorisi Dağılımı", color="white", fontsize=13, fontweight="bold")
    
    plt.suptitle("Türkiye Emlak Veri Seti - Fiyat Analizi", 
                 color="white", fontsize=15, fontweight="bold", y=1.02)
    plt.tight_layout()
    return grafik_kaydet(fig, "01_fiyat_dagilim")

def korelasyon_heatmap(df):
    sayisal_df = df[["fiyat", "metrekare", "oda_sayisi", "bina_yasi", 
                      "bulundugu_kat", "toplam_kat", "ulasim_skoru",
                      "balkon", "esyali", "site_icinde"]].copy()
    
    fig, axes = plt.subplots(1, 2, figsize=(20, 8))
    fig.patch.set_facecolor("#0f172a")
    
    pearson_corr = sayisal_df.corr(method="pearson")
    sns.heatmap(pearson_corr, annot=True, fmt=".2f", cmap="RdYlGn",
                ax=axes[0], center=0, vmin=-1, vmax=1,
                cbar_kws={"shrink": 0.8},
                annot_kws={"size": 9})
    axes[0].set_title("Pearson Korelasyon Matrisi", color="white", fontsize=13, fontweight="bold", pad=15)
    axes[0].set_facecolor("#1e293b")
    axes[0].tick_params(colors="white", labelsize=9)
    
    spearman_corr = sayisal_df.corr(method="spearman")
    sns.heatmap(spearman_corr, annot=True, fmt=".2f", cmap="RdYlGn",
                ax=axes[1], center=0, vmin=-1, vmax=1,
                cbar_kws={"shrink": 0.8},
                annot_kws={"size": 9})
    axes[1].set_title("Spearman Korelasyon Matrisi", color="white", fontsize=13, fontweight="bold", pad=15)
    axes[1].set_facecolor("#1e293b")
    axes[1].tick_params(colors="white", labelsize=9)
    
    plt.suptitle("Değişkenler Arası Korelasyon Analizi (Pearson & Spearman)",
                 color="white", fontsize=15, fontweight="bold")
    plt.tight_layout()
    return grafik_kaydet(fig, "02_korelasyon_heatmap")

def confusion_matrix_grafigi(cm, model_adi, sinif_etiketleri=["Ekonomik", "Orta Segment", "Lüks"]):
    fig, ax = plt.subplots(figsize=(8, 6))
    fig.patch.set_facecolor("#0f172a")
    ax.set_facecolor("#1e293b")
    
    sns.heatmap(cm, annot=True, fmt="d", cmap="Blues",
                xticklabels=sinif_etiketleri,
                yticklabels=sinif_etiketleri,
                ax=ax, linewidths=0.5,
                annot_kws={"size": 14, "weight": "bold"})
    
    ax.set_title(f"Confusion Matrix - {model_adi}", 
                 color="white", fontsize=13, fontweight="bold", pad=15)
    ax.set_xlabel("Tahmin Edilen", color="#94a3b8", fontsize=11)
    ax.set_ylabel("Gerçek Değer", color="#94a3b8", fontsize=11)
    ax.tick_params(colors="white")
    
    plt.tight_layout()
    isim = f"03_confusion_matrix_{model_adi.replace(' ', '_').lower()}"
    return grafik_kaydet(fig, isim)

def algoritma_karsilastirma_reg(metrikler_listesi):
    df_m = pd.DataFrame(metrikler_listesi)
    
    fig, axes = plt.subplots(1, 3, figsize=(18, 6))
    fig.patch.set_facecolor("#0f172a")
    
    renkler = RENK_PALETI[:len(df_m)]
    
    metrik_bilgi = [
        ("R2", "R² Skoru (Yüksek = İyi)", True),
        ("RMSE", "RMSE (Düşük = İyi)", False),
        ("MAE", "MAE (Düşük = İyi)", False),
    ]
    
    for ax, (metrik, baslik, yuksek_iyi) in zip(axes, metrik_bilgi):
        ax.set_facecolor("#1e293b")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
        
        bars = ax.bar(df_m["model"], df_m[metrik], color=renkler, edgecolor="#334155", linewidth=0.5)
        ax.set_title(baslik, color="white", fontsize=12, fontweight="bold")
        ax.set_xticklabels(df_m["model"], rotation=15, ha="right", color="white", fontsize=9)
        
        # En iyi değeri vurgula
        if yuksek_iyi:
            en_iyi_idx = df_m[metrik].idxmax()
        else:
            en_iyi_idx = df_m[metrik].idxmin()
        bars[en_iyi_idx].set_edgecolor("#fbbf24")
        bars[en_iyi_idx].set_linewidth(3)
        
        # Değerleri bar üzerine yaz
        for bar, val in zip(bars, df_m[metrik]):
            if metrik == "R2":
                label = f"{val:.3f}"
            else:
                label = f"{val/1e6:.2f}M"
            ax.text(bar.get_x() + bar.get_width()/2, bar.get_height() * 1.01,
                   label, ha="center", va="bottom", color="white", fontsize=9, fontweight="bold")
    
    plt.suptitle("🏆 Regresyon Algoritmaları Performans Karşılaştırması",
                 color="white", fontsize=14, fontweight="bold")
    plt.tight_layout()
    return grafik_kaydet(fig, "04_regresyon_karsilastirma")

def algoritma_karsilastirma_clf(metrikler_listesi):
    df_m = pd.DataFrame(metrikler_listesi)
    
    metrikler = ["Accuracy", "Precision", "Recall", "F1"]
    x = np.arange(len(df_m))
    width = 0.2
    
    fig, ax = plt.subplots(figsize=(14, 6))
    fig.patch.set_facecolor("#0f172a")
    ax.set_facecolor("#1e293b")
    ax.tick_params(colors="white")
    for spine in ax.spines.values():
        spine.set_edgecolor("#334155")
    
    for i, metrik in enumerate(metrikler):
        bars = ax.bar(x + i * width, df_m[metrik], width, 
                      label=metrik, color=RENK_PALETI[i], alpha=0.9)
    
    ax.set_xticks(x + width * 1.5)
    ax.set_xticklabels(df_m["model"], rotation=10, ha="right", color="white")
    ax.set_ylim(0, 1.15)
    ax.set_ylabel("Skor", color="#94a3b8")
    ax.set_title("🏆 Sınıflandırma Algoritmaları Karşılaştırması",
                 color="white", fontsize=13, fontweight="bold")
    
    legend = ax.legend(facecolor="#334155", edgecolor="#475569", labelcolor="white")
    ax.axhline(y=1.0, color="#64748b", linestyle="--", alpha=0.5)
    
    plt.tight_layout()
    return grafik_kaydet(fig, "05_siniflandirma_karsilastirma")

def kmeans_gorsellestir(df, kume_etiketleri):
    fig, axes = plt.subplots(1, 2, figsize=(16, 6))
    fig.patch.set_facecolor("#0f172a")
    
    renkler_kume = ["#6366f1", "#10b981", "#f59e0b"]
    kume_renk = [renkler_kume[i] for i in kume_etiketleri]
    
    df_viz = df.copy()
    df_viz["kume"] = kume_etiketleri
    
    for ax in axes:
        ax.set_facecolor("#1e293b")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
    
    scatter = axes[0].scatter(df_viz["metrekare"], df_viz["fiyat"] / 1e6,
                               c=kume_renk, alpha=0.7, s=40)
    axes[0].set_xlabel("Metrekare (m²)", color="#94a3b8")
    axes[0].set_ylabel("Fiyat (Milyon TL)", color="#94a3b8")
    axes[0].set_title("K-Means Kümeleme: Metrekare vs Fiyat",
                      color="white", fontsize=12, fontweight="bold")
    
    axes[1].scatter(df_viz["bina_yasi"], df_viz["fiyat"] / 1e6,
                    c=kume_renk, alpha=0.7, s=40)
    axes[1].set_xlabel("Bina Yaşı (yıl)", color="#94a3b8")
    axes[1].set_ylabel("Fiyat (Milyon TL)", color="#94a3b8")
    axes[1].set_title("K-Means Kümeleme: Bina Yaşı vs Fiyat",
                      color="white", fontsize=12, fontweight="bold")
    
    patches = [mpatches.Patch(color=renkler_kume[i], label=f"Küme {i+1}") for i in range(3)]
    fig.legend(handles=patches, loc="upper right", facecolor="#334155", 
               edgecolor="#475569", labelcolor="white")
    
    plt.suptitle("🔵 K-Means Kümeleme Analizi (K=3)",
                 color="white", fontsize=14, fontweight="bold")
    plt.tight_layout()
    return grafik_kaydet(fig, "06_kmeans_kumeleme")

def metrekare_fiyat_iliskisi(df):
    fig, axes = plt.subplots(1, 2, figsize=(16, 6))
    fig.patch.set_facecolor("#0f172a")
    
    for ax in axes:
        ax.set_facecolor("#1e293b")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
    
    kategori_renk = {"Ekonomik": "#10b981", "Orta Segment": "#6366f1", "Lüks": "#f59e0b"}
    
    for kat, renk in kategori_renk.items():
        alt = df[df["kategori"] == kat]
        axes[0].scatter(alt["metrekare"], alt["fiyat"] / 1e6, 
                       label=kat, alpha=0.6, color=renk, s=25)
    
    z = np.polyfit(df["metrekare"], df["fiyat"] / 1e6, 1)
    p = np.poly1d(z)
    x_line = np.linspace(df["metrekare"].min(), df["metrekare"].max(), 100)
    axes[0].plot(x_line, p(x_line), "r--", linewidth=2, label="Trend", alpha=0.8)
    
    axes[0].set_xlabel("Metrekare (m²)", color="#94a3b8")
    axes[0].set_ylabel("Fiyat (Milyon TL)", color="#94a3b8")
    axes[0].set_title("Metrekare – Fiyat İlişkisi", color="white", fontsize=12, fontweight="bold")
    legend = axes[0].legend(facecolor="#334155", edgecolor="#475569", labelcolor="white")
    
    yas_gruplari = pd.cut(df["bina_yasi"], bins=[0, 5, 10, 20, 40], labels=["0-5", "6-10", "11-20", "20+"])
    df_grp = df.copy()
    df_grp["yas_grubu"] = yas_gruplari
    
    gruplar = ["0-5", "6-10", "11-20", "20+"]
    veri_gruplari = [df_grp[df_grp["yas_grubu"] == g]["fiyat"].values / 1e6 for g in gruplar]
    
    bp = axes[1].boxplot(veri_gruplari, labels=gruplar, patch_artist=True,
                          boxprops=dict(facecolor="#6366f1", color="#818cf8"),
                          medianprops=dict(color="#fbbf24", linewidth=2),
                          whiskerprops=dict(color="#94a3b8"),
                          capprops=dict(color="#94a3b8"))
    
    axes[1].set_xlabel("Bina Yaşı Grubu (yıl)", color="#94a3b8")
    axes[1].set_ylabel("Fiyat (Milyon TL)", color="#94a3b8")
    axes[1].set_title("Bina Yaşı – Fiyat İlişkisi", color="white", fontsize=12, fontweight="bold")
    
    plt.suptitle("📊 Temel Değişken İlişkileri Analizi",
                 color="white", fontsize=14, fontweight="bold")
    plt.tight_layout()
    return grafik_kaydet(fig, "07_degisken_iliskileri")

def gercek_tahmin_grafigi(y_gercek, y_tahmin, model_adi):
    fig, axes = plt.subplots(1, 2, figsize=(16, 6))
    fig.patch.set_facecolor("#0f172a")
    
    for ax in axes:
        ax.set_facecolor("#1e293b")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
    
    y_g = np.array(y_gercek) / 1e6
    y_t = np.array(y_tahmin) / 1e6
    
    axes[0].scatter(y_g, y_t, alpha=0.5, color="#6366f1", s=20)
    min_val = min(y_g.min(), y_t.min())
    max_val = max(y_g.max(), y_t.max())
    axes[0].plot([min_val, max_val], [min_val, max_val], "r--", linewidth=2, label="İdeal")
    axes[0].set_xlabel("Gerçek Fiyat (Milyon TL)", color="#94a3b8")
    axes[0].set_ylabel("Tahmin Edilen Fiyat (Milyon TL)", color="#94a3b8")
    axes[0].set_title(f"{model_adi}: Gerçek vs Tahmin", color="white", fontsize=12, fontweight="bold")
    axes[0].legend(facecolor="#334155", edgecolor="#475569", labelcolor="white")
    
    hatalar = y_t - y_g
    axes[1].hist(hatalar, bins=40, color="#8b5cf6", edgecolor="#6d28d9", alpha=0.9)
    axes[1].axvline(x=0, color="#fbbf24", linewidth=2, linestyle="--")
    axes[1].set_xlabel("Tahmin Hatası (Milyon TL)", color="#94a3b8")
    axes[1].set_ylabel("Frekans", color="#94a3b8")
    axes[1].set_title(f"{model_adi}: Hata Dağılımı", color="white", fontsize=12, fontweight="bold")
    
    plt.suptitle(f"📈 {model_adi} - Tahmin Analizi",
                 color="white", fontsize=14, fontweight="bold")
    plt.tight_layout()
    isim = f"08_gercek_tahmin_{model_adi.replace(' ', '_').lower()}"
    return grafik_kaydet(fig, isim)

def onem_grafigi(model, ozellik_adlari, model_adi):
    if not hasattr(model, "feature_importances_"):
        return None
    
    importances = model.feature_importances_
    indices = np.argsort(importances)[::-1]
    
    fig, ax = plt.subplots(figsize=(12, 6))
    fig.patch.set_facecolor("#0f172a")
    ax.set_facecolor("#1e293b")
    ax.tick_params(colors="white")
    for spine in ax.spines.values():
        spine.set_edgecolor("#334155")
    
    renkler = plt.cm.get_cmap("plasma")(np.linspace(0.2, 0.9, len(importances)))
    bars = ax.bar(range(len(importances)), importances[indices], 
                  color=renkler, edgecolor="#334155")
    ax.set_xticks(range(len(importances)))
    ax.set_xticklabels([ozellik_adlari[i] for i in indices], 
                        rotation=45, ha="right", color="white", fontsize=9)
    ax.set_ylabel("Önem Skoru", color="#94a3b8")
    ax.set_title(f"🎯 {model_adi} - Özellik Önem Analizi (Feature Importance)",
                 color="white", fontsize=13, fontweight="bold")
    
    plt.tight_layout()
    isim = f"09_feature_importance_{model_adi.replace(' ', '_').lower()}"
    return grafik_kaydet(fig, isim)

def anova_gorsellestir(df, f_val, p_val, grup_sutunu="sehir", deger_sutunu="fiyat"):
    fig, ax = plt.subplots(figsize=(12, 6))
    fig.patch.set_facecolor("#0f172a")
    ax.set_facecolor("#1e293b")
    ax.tick_params(colors="white")
    for spine in ax.spines.values():
        spine.set_edgecolor("#334155")
        
    df_plot = df.copy()
    df_plot[deger_sutunu] = df_plot[deger_sutunu] / 1e6
    
    order = df_plot.groupby(grup_sutunu)[deger_sutunu].median().sort_values(ascending=False).index
    
    sns.boxplot(x=grup_sutunu, y=deger_sutunu, data=df_plot, order=order,
                ax=ax, palette="plasma", boxprops=dict(alpha=0.85))
    
    ax.set_xticklabels(order, rotation=45, ha="right", color="white")
    ax.set_xlabel("Şehir", color="#94a3b8", fontsize=11)
    ax.set_ylabel("Konut Fiyatı (Milyon TL)", color="#94a3b8", fontsize=11)
    
    title_text = f"📊 Tek Yönlü ANOVA Testi ve Şehir Bazlı Fiyat Dağılımı\n" \
                 f"F-İstatistiği: {f_val:.4f} | p-değeri: {p_val:.4e}"
    ax.set_title(title_text, color="white", fontsize=13, fontweight="bold")
    
    conclusion = "İstatistiksel Fark: Anlamlı (p < 0.05)" if p_val < 0.05 else "İstatistiksel Fark: Anlamlı Değil (p >= 0.05)"
    ax.text(0.95, 0.95, conclusion, transform=ax.transAxes,
            color="#fbbf24" if p_val < 0.05 else "#94a3b8",
            fontsize=10, fontweight="bold", ha="right", va="top",
            bbox=dict(boxstyle="round,pad=0.5", facecolor="#1e293b", edgecolor="#fbbf24" if p_val < 0.05 else "#334155", alpha=0.9))
            
    plt.tight_layout()
    isim = "10_anova_analizi"
    return grafik_kaydet(fig, isim)

