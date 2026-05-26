import numpy as np
import pandas as pd
from sklearn.linear_model import LinearRegression, LogisticRegression
from sklearn.tree import DecisionTreeRegressor, DecisionTreeClassifier
from sklearn.ensemble import RandomForestRegressor, RandomForestClassifier
from sklearn.neighbors import KNeighborsClassifier, KNeighborsRegressor
from sklearn.svm import SVR, SVC
from sklearn.cluster import KMeans
from sklearn.model_selection import cross_val_score, KFold
from sklearn.metrics import (
    mean_absolute_error, mean_squared_error, r2_score,
    accuracy_score, precision_score, recall_score, f1_score,
    confusion_matrix, classification_report
)
import warnings
warnings.filterwarnings("ignore")

def linear_regression_egit(X_train, X_test, y_train, y_test):
    model = LinearRegression()
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = regresyon_metrikleri(y_test, y_pred, "Linear Regression")
    return model, y_pred, metrikler

def decision_tree_reg_egit(X_train, X_test, y_train, y_test):
    model = DecisionTreeRegressor(max_depth=8, min_samples_split=10, random_state=42)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = regresyon_metrikleri(y_test, y_pred, "Decision Tree")
    return model, y_pred, metrikler

def random_forest_reg_egit(X_train, X_test, y_train, y_test):
    model = RandomForestRegressor(n_estimators=100, max_depth=10, random_state=42, n_jobs=-1)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = regresyon_metrikleri(y_test, y_pred, "Random Forest")
    return model, y_pred, metrikler

def knn_reg_egit(X_train, X_test, y_train, y_test):
    model = KNeighborsRegressor(n_neighbors=7, weights="distance")
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = regresyon_metrikleri(y_test, y_pred, "KNN")
    return model, y_pred, metrikler

def svm_reg_egit(X_train, X_test, y_train, y_test):
    model = SVR(kernel="rbf", C=1000, gamma=0.1, epsilon=0.1)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = regresyon_metrikleri(y_test, y_pred, "SVM")
    return model, y_pred, metrikler

def decision_tree_clf_egit(X_train, X_test, y_train, y_test):
    model = DecisionTreeClassifier(max_depth=6, random_state=42)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = siniflandirma_metrikleri(y_test, y_pred, "Decision Tree Clf")
    return model, y_pred, metrikler

def random_forest_clf_egit(X_train, X_test, y_train, y_test):
    model = RandomForestClassifier(n_estimators=100, random_state=42, n_jobs=-1)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = siniflandirma_metrikleri(y_test, y_pred, "Random Forest Clf")
    return model, y_pred, metrikler

def knn_clf_egit(X_train, X_test, y_train, y_test):
    model = KNeighborsClassifier(n_neighbors=7)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = siniflandirma_metrikleri(y_test, y_pred, "KNN Clf")
    return model, y_pred, metrikler

def logistic_regression_egit(X_train, X_test, y_train, y_test):
    model = LogisticRegression(max_iter=1000, random_state=42)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    metrikler = siniflandirma_metrikleri(y_test, y_pred, "Logistic Regression")
    return model, y_pred, metrikler


def kmeans_egit(X, n_clusters=3):
    model = KMeans(n_clusters=n_clusters, random_state=42, n_init=10)
    kume_etiketleri = model.fit_predict(X)
    return model, kume_etiketleri

def regresyon_metrikleri(y_gercek, y_tahmin, model_adi):
    mae = mean_absolute_error(y_gercek, y_tahmin)
    mse = mean_squared_error(y_gercek, y_tahmin)
    rmse = np.sqrt(mse)
    r2 = r2_score(y_gercek, y_tahmin)
    
    print(f"\n📊 {model_adi} - Regresyon Metrikleri:")
    print(f"   MAE  : {mae:,.0f} TL")
    print(f"   MSE  : {mse:,.0f}")
    print(f"   RMSE : {rmse:,.0f} TL")
    print(f"   R²   : {r2:.4f}")
    
    return {"model": model_adi, "MAE": mae, "MSE": mse, "RMSE": rmse, "R2": r2}

def siniflandirma_metrikleri(y_gercek, y_tahmin, model_adi):
    acc = accuracy_score(y_gercek, y_tahmin)
    prec = precision_score(y_gercek, y_tahmin, average="weighted", zero_division=0)
    rec = recall_score(y_gercek, y_tahmin, average="weighted", zero_division=0)
    f1 = f1_score(y_gercek, y_tahmin, average="weighted", zero_division=0)
    cm = confusion_matrix(y_gercek, y_tahmin)
    
    print(f"\n📊 {model_adi} - Sınıflandırma Metrikleri:")
    print(f"   Accuracy  : {acc:.4f}")
    print(f"   Precision : {prec:.4f}")
    print(f"   Recall    : {rec:.4f}")
    print(f"   F1-Score  : {f1:.4f}")
    
    return {
        "model": model_adi, "Accuracy": acc,
        "Precision": prec, "Recall": rec, "F1": f1,
        "confusion_matrix": cm
    }

def cross_validation_uygula(model, X, y, cv=5, scoring="r2"):
    kf = KFold(n_splits=cv, shuffle=True, random_state=42)
    skorlar = cross_val_score(model, X, y, cv=kf, scoring=scoring)
    print(f"   Cross-Val ({cv}-Fold): {skorlar.mean():.4f} ± {skorlar.std():.4f}")
    return skorlar

def anova_testi(df, grup_sutunu="sehir", deger_sutunu="fiyat"):
    import scipy.stats as stats
    gruplar = df[grup_sutunu].unique()
    grup_verileri = [df[df[grup_sutunu] == g][deger_sutunu].values for g in gruplar]
    f_val, p_val = stats.f_oneway(*grup_verileri)
    
    print(f"\n📊 ANOVA Testi (Grup: {grup_sutunu}, Değer: {deger_sutunu}):")
    print(f"   F-İstatistiği: {f_val:.4f}")
    print(f"   p-değeri     : {p_val:.4e}")
    if p_val < 0.05:
        print("   Yorum        : Gruplar arasında istatistiksel olarak anlamlı bir fark VARDIR (p < 0.05).")
    else:
        print("   Yorum        : Gruplar arasında istatistiksel olarak anlamlı bir fark YOKTUR (p >= 0.05).")
        
    return f_val, p_val

