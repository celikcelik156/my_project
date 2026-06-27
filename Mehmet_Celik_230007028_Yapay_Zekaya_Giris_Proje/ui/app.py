import streamlit as st
import pandas as pd
import pickle
import os

st.set_page_config(page_title="Emlak Fiyat Tahmin", layout="wide")

PROJE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
MODEL_DOSYASI = os.path.join(PROJE_DIR, "data", "modeller.pkl")
VERI_DOSYASI = os.path.join(PROJE_DIR, "data", "emlak_veri.csv")
GRAFIK_DIR = os.path.join(PROJE_DIR, "grafikler")
MODELLER = ["random_forest", "decision_tree", "linear_regression", "knn", "svm"]

@st.cache_resource
def modeller_yukle():
    if os.path.exists(MODEL_DOSYASI):
        with open(MODEL_DOSYASI, "rb") as f:
            return pickle.load(f)
    return None

@st.cache_data
def veri_yukle():
    return pd.read_csv(VERI_DOSYASI, encoding="utf-8-sig")

kayit = modeller_yukle()
df = veri_yukle()

st.title("Emlak Fiyat Tahmin Sistemi")
st.caption("Mehmet Celik | 230007028 | Yapay Zekaya Giris")

sayfa = st.sidebar.selectbox("Sayfa", ["Ana Sayfa", "Fiyat Tahmini", "Model Karsilastirma", "Grafikler"])

if sayfa == "Ana Sayfa":
    st.write("5 regresyon algoritmasi ile konut fiyat tahmini yapan basit bir uygulamadir.")
    st.write("Veri on isleme yapilmadan dogrudan sayisal ozellikler kullanilmistir.")
    st.write("Algoritmalar: Linear Regression, Decision Tree, Random Forest, KNN, SVM")
    if kayit is None:
        st.warning("Once python main.py calistirin.")
    else:
        st.success("5 model hazir.")
    st.metric("Veri sayisi", len(df))
    st.metric("Ortalama fiyat", f"{df['fiyat'].mean()/1e6:.2f} M TL")

elif sayfa == "Fiyat Tahmini":
    if kayit is None:
        st.error("Once main.py calistirin.")
        st.stop()
    col1, col2 = st.columns(2)
    with col1:
        metrekare = st.number_input("Metrekare", 30, 500, 120)
        oda_sayisi = st.selectbox("Oda sayisi", [1, 2, 3, 4, 5])
        bina_yasi = st.number_input("Bina yasi", 0, 60, 5)
        bulundugu_kat = st.number_input("Bulundugu kat", 0, 30, 3)
    with col2:
        toplam_kat = st.number_input("Toplam kat", 1, 35, 8)
        balkon = st.checkbox("Balkon var", True)
        esyali = st.checkbox("Esyali", False)
        site_icinde = st.checkbox("Site icinde", True)
        ulasim_skoru = st.slider("Ulasim skoru (1-10)", 1, 10, 7)
        model_sec = st.selectbox("Model", MODELLER, format_func=lambda x: x.replace("_", " ").title())
    if st.button("Tahmin Et"):
        oz = kayit["ozellikler"]
        X = pd.DataFrame([[metrekare, oda_sayisi, bina_yasi, bulundugu_kat, toplam_kat, int(balkon), int(esyali), int(site_icinde), ulasim_skoru]], columns=oz)
        tahmin = kayit["reg_modeller"][model_sec].predict(X)[0]
        st.success(f"Tahmini fiyat: {tahmin/1e6:.2f} Milyon TL ({tahmin:,.0f} TL)")
        st.write("Tum modeller:")
        for m in MODELLER:
            t = kayit["reg_modeller"][m].predict(X)[0]
            st.write(f"- {m.replace('_', ' ').title()}: {t/1e6:.2f} M TL")

elif sayfa == "Model Karsilastirma":
    if kayit is None:
        st.error("Once main.py calistirin.")
        st.stop()
    metrik_df = pd.DataFrame(kayit["reg_metrikler"])
    st.dataframe(metrik_df, use_container_width=True)
    en_iyi = metrik_df.loc[metrik_df["R2"].idxmax()]
    st.info(f"En iyi model: {en_iyi['model']} (R2 = {en_iyi['R2']:.4f})")

elif sayfa == "Grafikler":
    if not os.path.exists(GRAFIK_DIR):
        st.warning("Grafikler yok. Once main.py calistirin.")
    else:
        for dosya in sorted(os.listdir(GRAFIK_DIR)):
            if dosya.endswith(".png"):
                st.image(os.path.join(GRAFIK_DIR, dosya), caption=dosya)
