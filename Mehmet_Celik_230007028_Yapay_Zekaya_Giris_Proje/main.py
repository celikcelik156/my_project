import os
import pickle
import warnings
import sys

warnings.filterwarnings("ignore")

PROJE_DIR = os.path.dirname(os.path.abspath(__file__))
VERI_DOSYASI = os.path.join(PROJE_DIR, "data", "emlak_veri.csv")
MODEL_DOSYASI = os.path.join(PROJE_DIR, "data", "modeller.pkl")

sys.path.insert(0, os.path.join(PROJE_DIR, "src"))

from veri_hazirla import veri_yukle, veri_hazirla
from modeller import (
    linear_regression_egit, decision_tree_reg_egit, random_forest_reg_egit,
    knn_reg_egit, svm_reg_egit,
)
from gorsellestirme import algoritma_karsilastirma, gercek_tahmin_grafigi, korelasyon_grafigi


def main():
    print("=" * 50)
    print("EMLAK FIYAT TAHMIN - Mehmet Celik (230007028)")
    print("=" * 50)

    print("\n1. Veri yukleniyor (on isleme YOK)...")
    df = veri_yukle(VERI_DOSYASI)

    print("\n2. Train/Test ayrimi (%80 / %20)...")
    veri = veri_hazirla(df)
    X_tr, X_te = veri["X_train"], veri["X_test"]
    y_tr, y_te = veri["y_train"], veri["y_test"]
    print(f"   Egitim: {len(X_tr)} | Test: {len(X_te)}")

    print("\n3. 5 model egitiliyor...")
    reg_modeller = {}
    reg_metrikler = []

    lr_m, lr_pred, lr_met = linear_regression_egit(X_tr, X_te, y_tr, y_te)
    reg_modeller["linear_regression"] = lr_m
    reg_metrikler.append(lr_met)

    dt_m, dt_pred, dt_met = decision_tree_reg_egit(X_tr, X_te, y_tr, y_te)
    reg_modeller["decision_tree"] = dt_m
    reg_metrikler.append(dt_met)

    rf_m, rf_pred, rf_met = random_forest_reg_egit(X_tr, X_te, y_tr, y_te)
    reg_modeller["random_forest"] = rf_m
    reg_metrikler.append(rf_met)

    knn_m, knn_pred, knn_met = knn_reg_egit(X_tr, X_te, y_tr, y_te)
    reg_modeller["knn"] = knn_m
    reg_metrikler.append(knn_met)

    svm_m, svm_pred, svm_met = svm_reg_egit(X_tr, X_te, y_tr, y_te)
    reg_modeller["svm"] = svm_m
    reg_metrikler.append(svm_met)

    en_iyi = max(reg_metrikler, key=lambda m: m["R2"])

    print("\n4. Grafikler olusturuluyor...")
    algoritma_karsilastirma(reg_metrikler)
    gercek_tahmin_grafigi(y_te, rf_pred, "Random Forest")
    korelasyon_grafigi(df)

    kayit = {
        "reg_modeller": reg_modeller,
        "ozellikler": veri["ozellikler"],
        "reg_metrikler": reg_metrikler,
    }

    with open(MODEL_DOSYASI, "wb") as f:
        pickle.dump(kayit, f)

    print(f"\nModeller kaydedildi: {MODEL_DOSYASI}")
    print(f"En iyi R2: {en_iyi['model']} ({en_iyi['R2']:.4f})")
    print("=" * 50)
    return kayit


if __name__ == "__main__":
    main()
