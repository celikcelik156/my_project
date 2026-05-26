import streamlit as st
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import seaborn as sns
import pickle
import sys
import os
from scipy import stats
import warnings
warnings.filterwarnings("ignore")

st.set_page_config(
    page_title="🏠 Emlak Fiyat Tahmin Sistemi",
    page_icon="🏠",
    layout="wide",
    initial_sidebar_state="expanded"
)

PROJE_DIR = r"c:\Users\mehme\OneDrive\Desktop\yapay zekaya giriş dönem sonu projesi"
sys.path.insert(0, os.path.join(PROJE_DIR, "src"))
MODEL_DOSYASI = os.path.join(PROJE_DIR, "data", "modeller.pkl")
VERI_DOSYASI = os.path.join(PROJE_DIR, "data", "emlak_veri.csv")
GRAFIK_DIR = os.path.join(PROJE_DIR, "grafikler")

st.markdown("""
<style>
    @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');
    
    * { font-family: 'Inter', sans-serif; }
    
    .stApp {
        background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%);
    }
    
    .main-header {
        background: linear-gradient(90deg, #6366f1, #8b5cf6, #06b6d4);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        font-size: 2.5rem;
        font-weight: 700;
        text-align: center;
        padding: 1rem 0;
        margin-bottom: 0.5rem;
    }
    
    .sub-header {
        text-align: center;
        color: #94a3b8;
        font-size: 1.1rem;
        margin-bottom: 2rem;
    }
    
    .metric-card {
        background: linear-gradient(135deg, #1e293b, #334155);
        border: 1px solid #475569;
        border-radius: 16px;
        padding: 1.5rem;
        text-align: center;
        box-shadow: 0 4px 20px rgba(99, 102, 241, 0.1);
        transition: transform 0.2s;
    }
    
    .metric-card:hover {
        transform: translateY(-2px);
        border-color: #6366f1;
    }
    
    .metric-value {
        font-size: 2rem;
        font-weight: 700;
        background: linear-gradient(90deg, #6366f1, #8b5cf6);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
    }
    
    .metric-label {
        color: #94a3b8;
        font-size: 0.85rem;
        margin-top: 0.25rem;
    }
    
    .price-result {
        background: linear-gradient(135deg, #312e81, #1e3a8a);
        border: 2px solid #6366f1;
        border-radius: 20px;
        padding: 2rem;
        text-align: center;
        box-shadow: 0 0 30px rgba(99, 102, 241, 0.3);
        margin: 1rem 0;
    }
    
    .price-value {
        font-size: 3rem;
        font-weight: 700;
        background: linear-gradient(90deg, #fbbf24, #f59e0b);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
    }
    
    .section-title {
        color: #e2e8f0;
        font-size: 1.4rem;
        font-weight: 600;
        border-left: 4px solid #6366f1;
        padding-left: 1rem;
        margin: 1.5rem 0 1rem 0;
    }
    
    .info-box {
        background: rgba(99, 102, 241, 0.1);
        border: 1px solid rgba(99, 102, 241, 0.3);
        border-radius: 12px;
        padding: 1rem;
        color: #c7d2fe;
        font-size: 0.9rem;
    }
    
    .stSidebar {
        background: #0f172a;
    }
    
    div[data-testid="stSelectbox"] > div > div {
        background: #1e293b;
        border-color: #475569;
        color: white;
    }
    
    .stButton > button {
        background: linear-gradient(90deg, #6366f1, #8b5cf6);
        color: white;
        border: none;
        border-radius: 12px;
        padding: 0.75rem 2rem;
        font-size: 1rem;
        font-weight: 600;
        width: 100%;
        transition: all 0.3s;
        box-shadow: 0 4px 15px rgba(99, 102, 241, 0.4);
    }
    
    .stButton > button:hover {
        transform: translateY(-2px);
        box-shadow: 0 6px 20px rgba(99, 102, 241, 0.6);
    }
    
    .grafik-container {
        background: #1e293b;
        border-radius: 16px;
        padding: 1rem;
        border: 1px solid #334155;
        margin: 0.5rem 0;
    }
    
    .badge-ekonomik { color: #10b981; background: rgba(16,185,129,0.1); padding: 0.2rem 0.8rem; border-radius: 999px; border: 1px solid #10b981; }
    .badge-orta { color: #6366f1; background: rgba(99,102,241,0.1); padding: 0.2rem 0.8rem; border-radius: 999px; border: 1px solid #6366f1; }
    .badge-luks { color: #f59e0b; background: rgba(245,158,11,0.1); padding: 0.2rem 0.8rem; border-radius: 999px; border: 1px solid #f59e0b; }
</style>
""", unsafe_allow_html=True)

@st.cache_data
def veri_yukle():
    if os.path.exists(VERI_DOSYASI):
        return pd.read_csv(VERI_DOSYASI, encoding="utf-8-sig")
    return None

@st.cache_resource
def modeller_yukle():
    if os.path.exists(MODEL_DOSYASI):
        with open(MODEL_DOSYASI, "rb") as f:
            return pickle.load(f)
    return None

@st.cache_data
def ozellik_muhendisligi_uygula(df):
    df = df.copy()
    df["metrekare_basi_fiyat"] = df["fiyat"] / df["metrekare"]
    df["kat_orani"] = df["bulundugu_kat"] / (df["toplam_kat"] + 1)
    df["amortisman_skoru"] = np.exp(-df["bina_yasi"] / 20)
    df["konfor_skoru"] = (
        df["balkon"] * 0.2 + df["esyali"] * 0.3 +
        df["site_icinde"] * 0.3 + df["ulasim_skoru"] / 10 * 0.2
    )
    return df

df = veri_yukle()
kayit = modeller_yukle()

with st.sidebar:
    st.markdown("""
    <div style='text-align:center; padding: 1rem 0;'>
        <div style='font-size: 3rem'>🏠</div>
        <div style='color: white; font-size: 1.2rem; font-weight: 700; margin-top: 0.5rem'>
            Emlak AI
        </div>
        <div style='color: #64748b; font-size: 0.8rem'>Fiyat Tahmin Sistemi</div>
    </div>
    """, unsafe_allow_html=True)
    
    st.markdown("---")
    
    sayfa = st.selectbox(
        "📌 Sayfa Seç",
        ["🏠 Ana Sayfa", "🤖 Fiyat Tahmini", "📊 Veri Analizi", 
         "🏆 Model Karşılaştırma", "📈 Grafikler", "ℹ️ Hakkında"]
    )
    
    st.markdown("---")
    
    if df is not None:
        st.markdown("### 📊 Veri Seti")
        st.metric("Toplam Konut", f"{len(df):,}")
        st.metric("Ortalama Fiyat", f"{df['fiyat'].mean()/1e6:.2f}M TL")
        st.metric("Şehir Sayısı", df['sehir'].nunique())
    
    st.markdown("---")
    st.markdown("""
    <div style='background:linear-gradient(135deg,#1e1b4b,#1e3a5f);border:1px solid #4f46e5;border-radius:12px;padding:0.9rem 1rem;'>
        <div style='display:flex;align-items:center;gap:0.6rem'>
            <div style='background:#6366f1;border-radius:50%;width:36px;height:36px;display:flex;align-items:center;justify-content:center;font-size:1rem;flex-shrink:0'>👤</div>
            <div>
                <div style='color:#a5b4fc;font-size:0.65rem;text-transform:uppercase;letter-spacing:0.08em'>Öğrenci</div>
                <div style='color:white;font-size:0.95rem;font-weight:700;line-height:1.2'>Mehmet Çelik</div>
            </div>
        </div>
        <div style='color:#64748b;font-size:0.7rem;margin-top:0.6rem;text-align:center'>
            🎓 Yapay Zekaya Giriş · Dönem Sonu
        </div>
    </div>
    """, unsafe_allow_html=True)

if sayfa == "🏠 Ana Sayfa":
    st.markdown('<div class="main-header">🏠 Emlak Fiyat Tahmin Sistemi</div>', unsafe_allow_html=True)
    st.markdown('<div class="sub-header">Makine Öğrenmesi ile Türkiye Konut Piyasası Analizi</div>', unsafe_allow_html=True)
    
    if kayit is None:
        st.error("⚠️ Modeller henüz eğitilmedi! Lütfen önce `python main.py` komutunu çalıştırın.")
    else:
        st.success("✅ Modeller hazır! Fiyat tahmini ve analizlere başlayabilirsiniz.")
    
    st.markdown("---")
    
    col1, col2, col3, col4 = st.columns(4)
    
    with col1:
        st.markdown("""
        <div class="metric-card">
            <div style="font-size: 2.5rem">🤖</div>
            <div class="metric-value">5</div>
            <div class="metric-label">ML Algoritması</div>
        </div>
        """, unsafe_allow_html=True)
    
    with col2:
        st.markdown("""
        <div class="metric-card">
            <div style="font-size: 2.5rem">📊</div>
            <div class="metric-value">14</div>
            <div class="metric-label">Özellik / Feature</div>
        </div>
        """, unsafe_allow_html=True)
    
    with col3:
        st.markdown("""
        <div class="metric-card">
            <div style="font-size: 2.5rem">📈</div>
            <div class="metric-value">10+</div>
            <div class="metric-label">Analiz Grafiği</div>
        </div>
        """, unsafe_allow_html=True)
    
    with col4:
        total = len(df) if df is not None else 1200
        st.markdown(f"""
        <div class="metric-card">
            <div style="font-size: 2.5rem">🏘️</div>
            <div class="metric-value">{total:,}</div>
            <div class="metric-label">Konut Verisi</div>
        </div>
        """, unsafe_allow_html=True)
    
    st.markdown("---")
    
    st.markdown('<div class="section-title">🤖 Kullanılan Algoritmalar</div>', unsafe_allow_html=True)
    
    col1, col2 = st.columns(2)
    
    with col1:
        st.markdown("**📉 Regresyon (Fiyat Tahmini)**")
        algoritmalar_reg = [
            ("1️⃣", "Linear Regression", "Doğrusal ilişki modeli"),
            ("2️⃣", "Decision Tree", "Karar ağacı regresyon"),
            ("3️⃣", "Random Forest", "Çoklu ağaç topluluğu"),
            ("4️⃣", "KNN", "K-en yakın komşu"),
            ("5️⃣", "SVM", "Destek vektör makinesi"),
        ]
        for num, ad, aciklama in algoritmalar_reg:
            st.markdown(f"""
            <div style='background:#1e293b;border:1px solid #334155;border-radius:8px;padding:0.6rem 1rem;margin:0.3rem 0;display:flex;align-items:center;gap:0.5rem'>
                <span>{num}</span>
                <span style='color:#e2e8f0;font-weight:600'>{ad}</span>
                <span style='color:#64748b;font-size:0.85rem'>— {aciklama}</span>
            </div>
            """, unsafe_allow_html=True)
    
    with col2:
        st.markdown("**🔵 Sınıflandırma & Kümeleme**")
        algoritmalar_clf = [
            ("🌳", "Decision Tree Clf", "Ekonomik / Orta / Lüks"),
            ("🌲", "Random Forest Clf", "Güçlü topluluk modeli"),
            ("📍", "KNN Classifier", "Benzerlik tabanlı"),
            ("📊", "Logistic Regression", "Çok sınıflı sınıflandırma"),
            ("🔵", "K-Means (K=3)", "Benzer evleri grupla"),
        ]
        for num, ad, aciklama in algoritmalar_clf:
            st.markdown(f"""
            <div style='background:#1e293b;border:1px solid #334155;border-radius:8px;padding:0.6rem 1rem;margin:0.3rem 0;display:flex;align-items:center;gap:0.5rem'>
                <span>{num}</span>
                <span style='color:#e2e8f0;font-weight:600'>{ad}</span>
                <span style='color:#64748b;font-size:0.85rem'>— {aciklama}</span>
            </div>
            """, unsafe_allow_html=True)

elif sayfa == "🤖 Fiyat Tahmini":
    st.markdown('<div class="main-header">🤖 Akıllı Fiyat Tahmini</div>', unsafe_allow_html=True)
    st.markdown('<div class="sub-header">Ev özelliklerini girin, yapay zeka fiyatı tahmin etsin</div>', unsafe_allow_html=True)
    
    if kayit is None:
        st.error("⚠️ Önce `python main.py` çalıştırın!")
        st.stop()
    
    st.markdown("---")
    
    col1, col2, col3 = st.columns(3)
    
    with col1:
        st.markdown("### 📍 Konum")
        sehirler = ["İstanbul", "Ankara", "İzmir", "Bursa", "Antalya", "Adana", "Konya", "Kayseri", "Trabzon", "Gaziantep"]
        sehir = st.selectbox("Şehir", sehirler)
        
        ilce_map = {
            "İstanbul": ["Kadıköy", "Beşiktaş", "Şişli", "Üsküdar", "Ataşehir", "Maltepe", "Bağcılar", "Esenyurt", "Pendik", "Sarıyer"],
            "Ankara": ["Çankaya", "Keçiören", "Mamak", "Etimesgut", "Yenimahalle", "Sincan", "Pursaklar", "Gölbaşı"],
            "İzmir": ["Konak", "Karşıyaka", "Bornova", "Bayraklı", "Buca", "Çiğli", "Gaziemir", "Balçova"],
            "Bursa": ["Nilüfer", "Osmangazi", "Yıldırım", "Mudanya", "Gürsu"],
            "Antalya": ["Muratpaşa", "Kepez", "Konyaaltı", "Döşemealtı", "Alanya"],
            "Adana": ["Seyhan", "Çukurova", "Yüreğir", "Sarıçam"],
            "Konya": ["Selçuklu", "Karatay", "Meram", "Beyşehir"],
            "Kayseri": ["Melikgazi", "Kocasinan", "Talas", "Develi"],
            "Trabzon": ["Ortahisar", "Akçaabat", "Araklı", "Yomra"],
            "Gaziantep": ["Şehitkamil", "Şahinbey", "Nizip", "Islahiye"],
        }
        ilce = st.selectbox("İlçe", ilce_map.get(sehir, ["Merkez"]))
        ulasim_skoru = st.slider("Ulaşım Skoru (1-10)", 1, 10, 7)
    
    with col2:
        st.markdown("### 🏗️ Bina Özellikleri")
        metrekare = st.number_input("Metrekare (m²)", min_value=30, max_value=500, value=120, step=5)
        oda_sayisi = st.selectbox("Oda Sayısı", [1, 2, 3, 4, 5])
        bina_yasi = st.number_input("Bina Yaşı (yıl)", min_value=0, max_value=60, value=5)
        isinma = st.selectbox("Isınma Tipi", ["Doğalgaz", "Kombi", "Merkezi Sistem", "Soba", "Klima", "Jeotermal"])
    
    with col3:
        st.markdown("### 🏠 Daire Özellikleri")
        bulundugu_kat = st.number_input("Bulunduğu Kat", min_value=0, max_value=30, value=3)
        toplam_kat = st.number_input("Binanın Toplam Katı", min_value=1, max_value=35, value=8)
        balkon = st.checkbox("🌿 Balkon Var", value=True)
        esyali = st.checkbox("🛋️ Eşyalı", value=False)
        site_icinde = st.checkbox("🏘️ Site İçinde", value=True)
        
        st.markdown("### 🤖 Model Seç")
        model_sec = st.selectbox(
            "Tahmin Modeli",
            ["random_forest", "decision_tree", "linear_regression", "knn", "svm"],
            format_func=lambda x: {
                "random_forest": "🌲 Random Forest (Önerilen)",
                "decision_tree": "🌳 Decision Tree",
                "linear_regression": "📏 Linear Regression",
                "knn": "📍 KNN",
                "svm": "⚡ SVM"
            }.get(x, x)
        )
    
    st.markdown("---")
    
    if st.button("🚀 Fiyat Tahmin Et!", use_container_width=True):

        encoders = kayit["encoders"]
        scaler = kayit["scaler"]
        ozellikler = kayit["ozellikler"]
        
        try:
            sehir_kod = list(encoders["sehir"].classes_).index(sehir)
        except:
            sehir_kod = 0
        try:
            ilce_kod = list(encoders["ilce"].classes_).index(ilce)
        except:
            ilce_kod = 0
        try:
            isinma_kod = list(encoders["isinma"].classes_).index(isinma)
        except:
            isinma_kod = 0
        
        kat_orani = bulundugu_kat / (toplam_kat + 1)
        amortisman_skoru = np.exp(-bina_yasi / 20)
        konfor_skoru = (
            int(balkon) * 0.2 + int(esyali) * 0.3 +
            int(site_icinde) * 0.3 + ulasim_skoru / 10 * 0.2
        )
        buyukluk_kategori = 0 if metrekare <= 75 else (1 if metrekare <= 120 else (2 if metrekare <= 200 else 3))
        
        X_input = pd.DataFrame([[
            metrekare, oda_sayisi, bina_yasi, bulundugu_kat,
            toplam_kat, int(balkon), int(esyali), int(site_icinde),
            sehir_kod, ilce_kod, isinma_kod, ulasim_skoru,
            kat_orani, amortisman_skoru, konfor_skoru, buyukluk_kategori
        ]], columns=ozellikler)
        
        X_scaled = scaler.transform(X_input)
        
        model = kayit["reg_modeller"][model_sec]
        tahmin = model.predict(X_scaled)[0]
        
        clf_model = kayit["clf_modeller"]["random_forest"]
        kat_pred = clf_model.predict(X_scaled)[0]
        kat_ad = encoders["kategori"].inverse_transform([kat_pred])[0]
        
        col1, col2 = st.columns([2, 1])
        
        with col1:
            st.markdown(f"""
            <div class="price-result">
                <div style="color: #c7d2fe; font-size: 1rem; margin-bottom: 0.5rem">
                    🤖 {model_sec.replace('_', ' ').title()} Tahmini
                </div>
                <div class="price-value">
                    {tahmin/1_000_000:.2f} Milyon TL
                </div>
                <div style="color: #94a3b8; font-size: 0.9rem; margin-top: 0.5rem">
                    {tahmin:,.0f} TL
                </div>
                <div style="margin-top: 1rem; font-size: 1.2rem">
                    Kategori: <span class="badge-{'ekonomik' if kat_ad=='Ekonomik' else ('orta' if kat_ad=='Orta Segment' else 'luks')}">
                    {'🟢' if kat_ad=='Ekonomik' else ('🔵' if kat_ad=='Orta Segment' else '🌟')} {kat_ad}
                    </span>
                </div>
            </div>
            """, unsafe_allow_html=True)
        
        with col2:
            st.markdown("""
            <div class="metric-card" style="margin-top: 1rem">
                <div style="color: #94a3b8; font-size: 0.9rem">Benzer Evler Aralığı</div>
            </div>
            """, unsafe_allow_html=True)
            
            alt = tahmin * 0.90
            ust = tahmin * 1.10
            st.metric("Alt Sınır", f"{alt/1e6:.2f}M TL")
            st.metric("Üst Sınır", f"{ust/1e6:.2f}M TL")
            st.metric("m² Fiyatı", f"{tahmin/metrekare:,.0f} TL/m²")
        
        st.markdown("---")
        st.markdown('<div class="section-title">🔄 Tüm Modellerin Karşılaştırması</div>', unsafe_allow_html=True)
        
        tahminler = {}
        for m_adi, m in kayit["reg_modeller"].items():
            tahminler[m_adi] = m.predict(X_scaled)[0]
        
        cols = st.columns(len(tahminler))
        renk_map = {"linear_regression": "🔵", "decision_tree": "🌳", "random_forest": "🌲", "knn": "📍", "svm": "⚡"}
        
        for col, (m_adi, m_tahmin) in zip(cols, tahminler.items()):
            with col:
                vurgu = "border: 2px solid #fbbf24;" if m_adi == model_sec else ""
                st.markdown(f"""
                <div class="metric-card" style="{vurgu}">
                    <div style="font-size: 1.5rem">{renk_map.get(m_adi, '🤖')}</div>
                    <div style="color: #94a3b8; font-size: 0.75rem">{m_adi.replace('_', ' ').title()}</div>
                    <div style="color: #e2e8f0; font-weight: 700; font-size: 1.1rem">{m_tahmin/1e6:.2f}M TL</div>
                </div>
                """, unsafe_allow_html=True)

elif sayfa == "📊 Veri Analizi":
    st.markdown('<div class="main-header">📊 Veri Analizi</div>', unsafe_allow_html=True)
    
    if df is None:
        st.error("Veri bulunamadı. Önce main.py çalıştırın.")
        st.stop()
    
    df_oz = ozellik_muhendisligi_uygula(df)
    
    st.markdown('<div class="section-title">📈 Özet İstatistikler</div>', unsafe_allow_html=True)
    
    col1, col2, col3, col4 = st.columns(4)
    
    istatistikler = [
        (col1, "💰 Ortalama Fiyat", f"{df['fiyat'].mean()/1e6:.2f}M TL"),
        (col2, "📐 Ort. Metrekare", f"{df['metrekare'].mean():.0f} m²"),
        (col3, "🏗️ Ort. Bina Yaşı", f"{df['bina_yasi'].mean():.1f} yıl"),
        (col4, "🏘️ En Pahalı Şehir", df.groupby("sehir")["fiyat"].mean().idxmax()),
    ]
    
    for col, baslik, deger in istatistikler:
        with col:
            st.metric(baslik, deger)
    
    st.markdown("---")
    
    col1, col2 = st.columns(2)
    
    with col1:
        st.markdown("**📊 Şehir Bazlı Fiyat Dağılımı**")
        sehir_ort = df.groupby("sehir")["fiyat"].mean().sort_values(ascending=False) / 1e6
        
        fig, ax = plt.subplots(figsize=(8, 5))
        fig.patch.set_facecolor("#1e293b")
        ax.set_facecolor("#1e293b")
        
        renkler = plt.cm.plasma(np.linspace(0.3, 0.9, len(sehir_ort)))
        bars = ax.bar(sehir_ort.index, sehir_ort.values, color=renkler)
        ax.set_xticklabels(sehir_ort.index, rotation=45, ha="right", color="white", fontsize=8)
        ax.set_ylabel("Ortalama Fiyat (Milyon TL)", color="#94a3b8")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
        ax.set_title("Şehir Bazlı Ortalama Fiyat", color="white", fontsize=11, fontweight="bold")
        
        for bar, val in zip(bars, sehir_ort.values):
            ax.text(bar.get_x() + bar.get_width()/2, bar.get_height() + 0.05,
                   f"{val:.1f}M", ha="center", va="bottom", color="white", fontsize=7)
        
        plt.tight_layout()
        st.pyplot(fig, use_container_width=True)
    
    with col2:
        st.markdown("**📊 Fiyat vs Metrekare İlişkisi**")
        
        fig, ax = plt.subplots(figsize=(8, 5))
        fig.patch.set_facecolor("#1e293b")
        ax.set_facecolor("#1e293b")
        
        kat_renk = {"Ekonomik": "#10b981", "Orta Segment": "#6366f1", "Lüks": "#f59e0b"}
        for kat, renk in kat_renk.items():
            alt = df[df["kategori"] == kat]
            ax.scatter(alt["metrekare"], alt["fiyat"]/1e6, color=renk, label=kat, alpha=0.6, s=20)
        
        ax.set_xlabel("Metrekare (m²)", color="#94a3b8")
        ax.set_ylabel("Fiyat (Milyon TL)", color="#94a3b8")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
        ax.set_title("Metrekare - Fiyat İlişkisi", color="white", fontsize=11, fontweight="bold")
        legend = ax.legend(facecolor="#334155", edgecolor="#475569", labelcolor="white")
        
        plt.tight_layout()
        st.pyplot(fig, use_container_width=True)
    
    st.markdown('<div class="section-title">🔥 Korelasyon Analizi (Pearson & Spearman)</div>', unsafe_allow_html=True)
    
    sayisal_df = df[["fiyat", "metrekare", "oda_sayisi", "bina_yasi", 
                      "bulundugu_kat", "ulasim_skoru", "balkon", "esyali", "site_icinde"]]
    
    tab1, tab2, tab3 = st.tabs(["Pearson Korelasyon", "Spearman Korelasyon", "ANOVA (Varyans Analizi)"])
    
    for tab, metod in [(tab1, "pearson"), (tab2, "spearman")]:
        with tab:
            fig, ax = plt.subplots(figsize=(10, 7))
            fig.patch.set_facecolor("#1e293b")
            ax.set_facecolor("#1e293b")
            
            corr = sayisal_df.corr(method=metod)
            sns.heatmap(corr, annot=True, fmt=".2f", cmap="RdYlGn",
                       ax=ax, center=0, vmin=-1, vmax=1,
                       annot_kws={"size": 10})
            ax.set_title(f"{metod.title()} Korelasyon Matrisi", 
                        color="white", fontsize=12, fontweight="bold")
            ax.tick_params(colors="white")
            
            plt.tight_layout()
            st.pyplot(fig, use_container_width=True)
            
    with tab3:
        st.markdown("**📊 Şehirler Arası Fiyat Farklarının ANOVA Testi ile Karşılaştırılması**")
        st.markdown("""
        Varyans Analizi (ANOVA), üç veya daha fazla bağımsız grubun ortalamaları arasında istatistiksel olarak anlamlı bir fark olup olmadığını belirlemek için kullanılan parametrik bir hipotez testidir.
        Bu projede, **farklı şehirlerdeki konutların ortalama fiyatlarının birbirlerinden anlamlı düzeyde ayrışıp ayrışmadığı** test edilmiştir:
        - **H₀ (Boş Hipotez):** Tüm şehirlerdeki konutların ortalama fiyatları birbirine eşittir.
        - **H₁ (Alternatif Hipotez):** En az bir şehrin ortalama fiyatı diğerlerinden farklıdır.
        """)
        
        if kayit is not None and "anova_results" in kayit:
            f_val = kayit["anova_results"]["f_val"]
            p_val = kayit["anova_results"]["p_val"]
        else:
            import scipy.stats as stats
            gruplar = df["sehir"].unique()
            grup_verileri = [df[df["sehir"] == g]["fiyat"].values for g in gruplar]
            f_val, p_val = stats.f_oneway(*grup_verileri)
            
        col1, col2 = st.columns(2)
        with col1:
            st.metric("ANOVA F-İstatistiği (F-Value)", f"{f_val:.4f}")
        with col2:
            st.metric("p-değeri (p-value)", f"{p_val:.4e}")
            
        if p_val < 0.05:
            st.success("🌟 **İstatistiksel Sonuç:** p-değeri < 0.05 olduğu için Boş Hipotez (H₀) reddedilir. Farklı şehirlerdeki konut fiyatlarının ortalamaları arasında **istatistiksel olarak son derece anlamlı bir fark vardır**.")
        else:
            st.warning("⚠️ **İstatistiksel Sonuç:** p-değeri >= 0.05 olduğu için Boş Hipotez (H₀) reddedilemez. Şehirler arasında konut fiyatları açısından anlamlı bir fark tespit edilememiştir.")
            
        anova_grafik_yol = os.path.join(GRAFIK_DIR, "10_anova_analizi.png")
        if os.path.exists(anova_grafik_yol):
            st.image(anova_grafik_yol, caption="Şehir Bazlı ANOVA Kutu Grafiği (Boxplot)", use_container_width=True)
    
    st.markdown('<div class="section-title">📋 Ham Veri (İlk 20 Satır)</div>', unsafe_allow_html=True)
    st.dataframe(
        df.head(20).style.background_gradient(subset=["fiyat"], cmap="Blues"),
        use_container_width=True,
        height=400
    )
elif sayfa == "🏆 Model Karşılaştırma":
    st.markdown('<div class="main-header">🏆 Model Performans Karşılaştırması</div>', unsafe_allow_html=True)
    
    if kayit is None:
        st.error("Önce main.py çalıştırın!")
        st.stop()
    
    tab1, tab2, tab3 = st.tabs(["📉 Regresyon Metrikleri", "🔵 Sınıflandırma Metrikleri", "🗺️ Confusion Matrix"])
    
    with tab1:
        st.markdown('<div class="section-title">📉 Regresyon Model Performansları</div>', unsafe_allow_html=True)
        
        reg_df = pd.DataFrame(kayit["reg_metrikler"])
        
        en_iyi_r2 = reg_df.loc[reg_df["R2"].idxmax()]
        en_iyi_rmse = reg_df.loc[reg_df["RMSE"].idxmin()]
        
        col1, col2 = st.columns(2)
        with col1:
            st.success(f"🏆 En Yüksek R² → **{en_iyi_r2['model']}** ({en_iyi_r2['R2']:.4f})")
        with col2:
            st.success(f"🏆 En Düşük RMSE → **{en_iyi_rmse['model']}** ({en_iyi_rmse['RMSE']/1e6:.3f}M TL)")
        
        goster_df = reg_df.copy()
        goster_df["MAE"] = goster_df["MAE"].apply(lambda x: f"{x/1e6:.3f}M TL")
        goster_df["RMSE"] = goster_df["RMSE"].apply(lambda x: f"{x/1e6:.3f}M TL")
        goster_df["R2"] = goster_df["R2"].apply(lambda x: f"{x:.4f}")
        goster_df = goster_df.drop("MSE", axis=1)
        
        st.dataframe(goster_df, use_container_width=True, hide_index=True)
        
        fig, axes = plt.subplots(1, 2, figsize=(14, 5))
        fig.patch.set_facecolor("#1e293b")
        
        reg_df_orig = pd.DataFrame(kayit["reg_metrikler"])
        renkler = ["#6366f1", "#8b5cf6", "#06b6d4", "#10b981", "#f59e0b"]
        
        for ax in axes:
            ax.set_facecolor("#1e293b")
            ax.tick_params(colors="white")
            for spine in ax.spines.values():
                spine.set_edgecolor("#334155")
        
        bars = axes[0].bar(reg_df_orig["model"], reg_df_orig["R2"], color=renkler)
        axes[0].set_title("R² Skoru Karşılaştırması", color="white", fontweight="bold")
        axes[0].set_xticklabels(reg_df_orig["model"], rotation=15, ha="right", color="white")
        axes[0].set_ylabel("R² Skoru", color="#94a3b8")
        
        bars2 = axes[1].bar(reg_df_orig["model"], reg_df_orig["RMSE"]/1e6, color=renkler)
        axes[1].set_title("RMSE Karşılaştırması (Milyon TL)", color="white", fontweight="bold")
        axes[1].set_xticklabels(reg_df_orig["model"], rotation=15, ha="right", color="white")
        axes[1].set_ylabel("RMSE (Milyon TL)", color="#94a3b8")
        
        plt.tight_layout()
        st.pyplot(fig, use_container_width=True)
    
    with tab2:
        st.markdown('<div class="section-title">🔵 Sınıflandırma Model Performansları</div>', unsafe_allow_html=True)
        
        clf_data = [{k: v for k, v in m.items() if k != "confusion_matrix"} for m in kayit["clf_metrikler"]]
        clf_df = pd.DataFrame(clf_data)
        
        en_iyi_acc = clf_df.loc[clf_df["Accuracy"].idxmax()]
        st.success(f"🏆 En Yüksek Accuracy → **{en_iyi_acc['model']}** ({en_iyi_acc['Accuracy']:.4f})")
        
        goster_clf = clf_df.copy()
        for col in ["Accuracy", "Precision", "Recall", "F1"]:
            goster_clf[col] = goster_clf[col].apply(lambda x: f"{x:.4f}")
        
        st.dataframe(goster_clf, use_container_width=True, hide_index=True)
        
        metrikler = ["Accuracy", "Precision", "Recall", "F1"]
        x = np.arange(len(clf_df))
        width = 0.2
        
        fig, ax = plt.subplots(figsize=(12, 5))
        fig.patch.set_facecolor("#1e293b")
        ax.set_facecolor("#1e293b")
        ax.tick_params(colors="white")
        for spine in ax.spines.values():
            spine.set_edgecolor("#334155")
        
        for i, metrik in enumerate(metrikler):
            ax.bar(x + i * width, clf_df[metrik], width, label=metrik, color=renkler[i], alpha=0.9)
        
        ax.set_xticks(x + width * 1.5)
        ax.set_xticklabels(clf_df["model"], rotation=10, ha="right", color="white")
        ax.set_ylim(0, 1.2)
        ax.set_ylabel("Skor", color="#94a3b8")
        ax.set_title("Sınıflandırma Algoritmaları Karşılaştırması", color="white", fontweight="bold")
        legend = ax.legend(facecolor="#334155", edgecolor="#475569", labelcolor="white")
        
        plt.tight_layout()
        st.pyplot(fig, use_container_width=True)
    
    with tab3:
        st.markdown('<div class="section-title">🗺️ Confusion Matrix Analizi</div>', unsafe_allow_html=True)
        
        secilen_model = st.selectbox(
            "Model Seç",
            [m["model"] for m in kayit["clf_metrikler"]]
        )
        
        seçilen = next(m for m in kayit["clf_metrikler"] if m["model"] == secilen_model)
        cm = seçilen["confusion_matrix"]
        
        encoders = kayit["encoders"]
        sinif_etiketleri = list(encoders["kategori"].classes_)
        
        fig, ax = plt.subplots(figsize=(8, 6))
        fig.patch.set_facecolor("#1e293b")
        ax.set_facecolor("#1e293b")
        
        sns.heatmap(cm, annot=True, fmt="d", cmap="Blues",
                   xticklabels=sinif_etiketleri,
                   yticklabels=sinif_etiketleri,
                   ax=ax, linewidths=1,
                   annot_kws={"size": 16, "weight": "bold"})
        
        ax.set_title(f"Confusion Matrix – {secilen_model}", color="white", fontsize=13, fontweight="bold")
        ax.set_xlabel("Tahmin Edilen Sınıf", color="#94a3b8", fontsize=11)
        ax.set_ylabel("Gerçek Sınıf", color="#94a3b8", fontsize=11)
        ax.tick_params(colors="white")
        
        plt.tight_layout()
        st.pyplot(fig, use_container_width=True)
        
        st.markdown("""
        <div class="info-box">
        📌 <strong>Confusion Matrix Nasıl Okunur?</strong><br><br>
        • <strong>Köşegen elemanlar</strong> (sol üstten sağ alta) → Doğru tahminler<br>
        • <strong>Köşegen dışı elemanlar</strong> → Yanlış sınıflandırmalar<br>
        • Satırlar <strong>gerçek sınıfları</strong>, sütunlar <strong>tahmin edilen sınıfları</strong> gösterir
        </div>
        """, unsafe_allow_html=True)

elif sayfa == "📈 Grafikler":
    st.markdown('<div class="main-header">📈 Analiz Grafikleri</div>', unsafe_allow_html=True)
    
    if not os.path.exists(GRAFIK_DIR):
        st.warning("Grafikler henüz oluşturulmadı. main.py çalıştırın.")
        st.stop()
    
    grafik_dosyalari = sorted([f for f in os.listdir(GRAFIK_DIR) if f.endswith(".png")])
    
    if not grafik_dosyalari:
        st.info("Grafik bulunamadı. Önce `python main.py` çalıştırın.")
        st.stop()
    
    st.success(f"✅ {len(grafik_dosyalari)} grafik bulundu")
    
    for dosya in grafik_dosyalari:
        yol = os.path.join(GRAFIK_DIR, dosya)
        temiz_isim = dosya.replace("_", " ").replace(".png", "").title()
        with st.expander(f"📊 {temiz_isim}", expanded=False):
            st.image(yol, use_container_width=True)
            
elif sayfa == "ℹ️ Hakkında":
    st.markdown('<div class="main-header">ℹ️ Proje Hakkında</div>', unsafe_allow_html=True)
    
    st.markdown("""
    <div style='background:#1e293b;border:1px solid #334155;border-radius:16px;padding:2rem;margin:1rem 0'>
        <h2 style='color:#e2e8f0'>🎓 Yapay Zekaya Giriş – Dönem Sonu Projesi</h2>
        <h3 style='color:#6366f1'>Türkiye Emlak Fiyat Tahmin Sistemi</h3>
        
        <div style='background:linear-gradient(135deg,#312e81,#1e3a8a);border:1px solid #6366f1;border-radius:12px;padding:1.2rem 1.5rem;margin:1.2rem 0;display:flex;align-items:center;gap:1rem'>
            <div style='font-size:2.5rem'>👨‍💻</div>
            <div>
                <div style='color:#c7d2fe;font-size:0.8rem;text-transform:uppercase;letter-spacing:0.1em'>Öğrenci</div>
                <div style='color:white;font-size:1.4rem;font-weight:700'>Mehmet Çelik</div>
            </div>
        </div>
        
        <p style='color:#94a3b8'>
        Bu proje, makine öğrenmesi teknikleri kullanılarak konut fiyatlarının tahmin edilmesi 
        amacıyla geliştirilmiştir. Sistem, gerçek emlak ilanlarına dayanan özellikler kullanarak 
        çoklu algoritmalar ile fiyat tahmini yapar ve karşılaştırmalı analiz sunar.
        </p>
        
        <h4 style='color:#e2e8f0;margin-top:1.5rem'>📚 Kullanılan Teknolojiler</h4>
        <ul style='color:#94a3b8'>
            <li>Python 3.x</li>
            <li>Scikit-learn (ML modelleri)</li>
            <li>Pandas &amp; NumPy (veri işleme)</li>
            <li>Matplotlib &amp; Seaborn (görselleştirme)</li>
            <li>Streamlit (web arayüzü)</li>
            <li>SciPy (istatistiksel analizler)</li>
        </ul>
        
        <h4 style='color:#e2e8f0;margin-top:1.5rem'>🤖 Uygulanan Algoritmalar</h4>
        <ul style='color:#94a3b8'>
            <li><strong style='color:#6366f1'>Regresyon:</strong> Linear Regression, Decision Tree, Random Forest, KNN, SVM</li>
            <li><strong style='color:#8b5cf6'>Sınıflandırma:</strong> Decision Tree, Random Forest, KNN, Logistic Regression</li>
            <li><strong style='color:#06b6d4'>Kümeleme:</strong> K-Means (K=3)</li>
        </ul>
        
        <h4 style='color:#e2e8f0;margin-top:1.5rem'>📊 Değerlendirme Metrikleri</h4>
        <ul style='color:#94a3b8'>
            <li><strong>Regresyon:</strong> MAE, MSE, RMSE, R²</li>
            <li><strong>Sınıflandırma:</strong> Accuracy, Precision, Recall, F1-Score</li>
            <li><strong>Korelasyon:</strong> Pearson, Spearman</li>
        </ul>
    </div>
    """, unsafe_allow_html=True)

# Global footer visible on all pages
st.markdown("---")
st.markdown("""
<div style='text-align: center; color: #94a3b8; font-size: 0.85rem; padding: 1rem 0;'>
    🏠 Türkiye Emlak Fiyat Tahmin Sistemi © 2026 | Geliştirici: <strong>Mehmet Çelik (230007028)</strong> | Yapay Zekaya Giriş Dönem Sonu Projesi
</div>
""", unsafe_allow_html=True)



