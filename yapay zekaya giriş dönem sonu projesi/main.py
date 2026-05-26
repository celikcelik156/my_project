import sys
import os

PROJE_DIR = r"c:\Users\mehme\OneDrive\Desktop\yapay zekaya giriş dönem sonu projesi"
sys.path.insert(0, os.path.join(PROJE_DIR, "src"))

from veri_olustur import uret_veri
from on_isleme import veri_on_isleme, ozellik_muhendisligi, veri_hazirla
from modeller import (
    linear_regression_egit, decision_tree_reg_egit, random_forest_reg_egit,
    knn_reg_egit, svm_reg_egit,
    decision_tree_clf_egit, random_forest_clf_egit, knn_clf_egit, logistic_regression_egit,
    kmeans_egit, cross_validation_uygula, anova_testi
)
from gorsellestirme import (
    fiyat_dagilim_grafigi, korelasyon_heatmap, confusion_matrix_grafigi,
    algoritma_karsilastirma_reg, algoritma_karsilastirma_clf,
    kmeans_gorsellestir, metrekare_fiyat_iliskisi,
    gercek_tahmin_grafigi, onem_grafigi, anova_gorsellestir
)
import pandas as pd
import numpy as np
import pickle
import warnings
warnings.filterwarnings("ignore")

VERI_DOSYASI = os.path.join(PROJE_DIR, "data", "emlak_veri.csv")
MODEL_DOSYASI = os.path.join(PROJE_DIR, "data", "modeller.pkl")

def main():
    print("=" * 60)
    print("🏠 EMLAK FİYAT TAHMİN SİSTEMİ - Ana Program")
    print("🎓 Öğrenci: Mehmet Çelik (230007028)")
    print("=" * 60)
    
    print("\n📊 1. VERİ HAZIRLANIYOR...")
    if not os.path.exists(VERI_DOSYASI):
        print("   Veri seti bulunamadı, oluşturuluyor...")
        df_ham = uret_veri(1200)
        df_ham.to_csv(VERI_DOSYASI, index=False, encoding="utf-8-sig")
        print(f"   ✅ {len(df_ham)} satır veri oluşturuldu!")
    else:
        df_ham = pd.read_csv(VERI_DOSYASI, encoding="utf-8-sig")
        print(f"   ✅ Mevcut veri yüklendi: {len(df_ham)} satır")
    
    print("\n🔧 2. VERİ ÖN İŞLEME...")
    df, encoders = veri_on_isleme(df_ham)
    df = ozellik_muhendisligi(df)
    print(f"   ✅ İşlendi: {len(df)} satır")
    
    print("\n📦 3. TRAIN/TEST SPLIT (%80/%20)...")
    veri = veri_hazirla(df)
    X_tr = veri["X_train_scaled"]
    X_te = veri["X_test_scaled"]
    y_fiyat_tr = veri["y_fiyat_train"]
    y_fiyat_te = veri["y_fiyat_test"]
    y_kat_tr = veri["y_kat_train"]
    y_kat_te = veri["y_kat_test"]
    print(f"   Train: {len(X_tr)} | Test: {len(X_te)}")
    
    print("\n📈 4. VERİ ANALİZİ GRAFİKLERİ...")
    fiyat_dagilim_grafigi(df)
    korelasyon_heatmap(df)
    metrekare_fiyat_iliskisi(df)
    
    print("\n📊 4.2 İSTATİSTİKSEL ANOVA ANALİZİ...")
    f_val, p_val = anova_testi(df_ham, grup_sutunu="sehir", deger_sutunu="fiyat")
    anova_gorsellestir(df_ham, f_val, p_val)
    
    print("\n🤖 5. REGRESYON MODELLERİ EĞİTİLİYOR...")
    
    reg_modeller = {}
    reg_metrikler = []
    
    print("\n[1/5] Linear Regression")
    lr_m, lr_pred, lr_met = linear_regression_egit(X_tr, X_te, y_fiyat_tr, y_fiyat_te)
    reg_modeller["linear_regression"] = lr_m
    reg_metrikler.append(lr_met)
    gercek_tahmin_grafigi(y_fiyat_te, lr_pred, "Linear Regression")
    
    print("\n[2/5] Decision Tree Regressor")
    dt_m, dt_pred, dt_met = decision_tree_reg_egit(X_tr, X_te, y_fiyat_tr, y_fiyat_te)
    reg_modeller["decision_tree"] = dt_m
    reg_metrikler.append(dt_met)
    onem_grafigi(dt_m, veri["ozellikler"], "Decision Tree")
    
    print("\n[3/5] Random Forest Regressor")
    rf_m, rf_pred, rf_met = random_forest_reg_egit(X_tr, X_te, y_fiyat_tr, y_fiyat_te)
    reg_modeller["random_forest"] = rf_m
    reg_metrikler.append(rf_met)
    onem_grafigi(rf_m, veri["ozellikler"], "Random Forest")
    gercek_tahmin_grafigi(y_fiyat_te, rf_pred, "Random Forest")
    
    print("\n[4/5] KNN Regressor")
    knn_m, knn_pred, knn_met = knn_reg_egit(X_tr, X_te, y_fiyat_tr, y_fiyat_te)
    reg_modeller["knn"] = knn_m
    reg_metrikler.append(knn_met)
    
    print("\n[5/5] SVM Regressor")
    svm_m, svm_pred, svm_met = svm_reg_egit(X_tr, X_te, y_fiyat_tr, y_fiyat_te)
    reg_modeller["svm"] = svm_m
    reg_metrikler.append(svm_met)
    
    algoritma_karsilastirma_reg(reg_metrikler)
    
    print("\n🔵 6. SINIFLANDIRMA MODELLERİ EĞİTİLİYOR...")
    
    sinif_etikletleri = list(encoders["kategori"].classes_)
    clf_metrikler = []
    
    print("\n[1/4] Decision Tree Classifier")
    dtc_m, dtc_pred, dtc_met = decision_tree_clf_egit(X_tr, X_te, y_kat_tr, y_kat_te)
    clf_metrikler.append(dtc_met)
    confusion_matrix_grafigi(dtc_met["confusion_matrix"], "Decision Tree", sinif_etikletleri)
    
    print("\n[2/4] Random Forest Classifier")
    rfc_m, rfc_pred, rfc_met = random_forest_clf_egit(X_tr, X_te, y_kat_tr, y_kat_te)
    clf_metrikler.append(rfc_met)
    confusion_matrix_grafigi(rfc_met["confusion_matrix"], "Random Forest", sinif_etikletleri)
    
    print("\n[3/4] KNN Classifier")
    knnc_m, knnc_pred, knnc_met = knn_clf_egit(X_tr, X_te, y_kat_tr, y_kat_te)
    clf_metrikler.append(knnc_met)
    
    print("\n[4/4] Logistic Regression")
    log_m, log_pred, log_met = logistic_regression_egit(X_tr, X_te, y_kat_tr, y_kat_te)
    clf_metrikler.append(log_met)
    confusion_matrix_grafigi(log_met["confusion_matrix"], "Logistic Regression", sinif_etikletleri)
    
    algoritma_karsilastirma_clf(clf_metrikler)
    
    print("\n🔵 7. K-MEANS KÜMELEME (K=3)...")
    ozellik_kmeans = ["metrekare", "fiyat", "bina_yasi", "oda_sayisi", "ulasim_skoru"]
    from sklearn.preprocessing import StandardScaler
    scaler_km = StandardScaler()
    X_km = scaler_km.fit_transform(df[ozellik_kmeans])
    km_m, kume_etiketleri = kmeans_egit(X_km, n_clusters=3)
    kmeans_gorsellestir(df, kume_etiketleri)
    print("   ✅ K-Means tamamlandı!")
    
    print("\n💾 8. MODELLER KAYDEDİLİYOR...")
    kayit = {
        "reg_modeller": reg_modeller,
        "clf_modeller": {
            "decision_tree": dtc_m,
            "random_forest": rfc_m,
            "knn": knnc_m,
            "logistic_regression": log_m
        },
        "kmeans": km_m,
        "scaler": veri["scaler"],
        "scaler_km": scaler_km,
        "encoders": encoders,
        "ozellikler": veri["ozellikler"],
        "reg_metrikler": reg_metrikler,
        "clf_metrikler": clf_metrikler,
        "anova_results": {
            "f_val": f_val,
            "p_val": p_val
        }
    }
    
    with open(MODEL_DOSYASI, "wb") as f:
        pickle.dump(kayit, f)
    print(f"   ✅ Modeller kaydedildi: {MODEL_DOSYASI}")
    
    print("\n" + "=" * 60)
    print("🎉 TÜM İŞLEMLER TAMAMLANDI!")
    print(f"   📁 Grafikler: {PROJE_DIR}\\grafikler\\")
    print(f"   💾 Modeller:  {MODEL_DOSYASI}")
    print("=" * 60)
    
    return kayit

if __name__ == "__main__":
    main()
