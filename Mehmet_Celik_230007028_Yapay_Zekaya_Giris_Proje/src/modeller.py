import numpy as np
from sklearn.linear_model import LinearRegression
from sklearn.tree import DecisionTreeRegressor
from sklearn.ensemble import RandomForestRegressor
from sklearn.neighbors import KNeighborsRegressor
from sklearn.svm import SVR
from sklearn.metrics import mean_absolute_error, mean_squared_error, r2_score


def regresyon_metrikleri(y_gercek, y_tahmin, model_adi):
    mae = mean_absolute_error(y_gercek, y_tahmin)
    mse = mean_squared_error(y_gercek, y_tahmin)
    rmse = np.sqrt(mse)
    r2 = r2_score(y_gercek, y_tahmin)
    print(f"\n{model_adi} - MAE: {mae:,.0f} TL | RMSE: {rmse:,.0f} TL | R2: {r2:.4f}")
    return {"model": model_adi, "MAE": mae, "MSE": mse, "RMSE": rmse, "R2": r2}


def linear_regression_egit(X_train, X_test, y_train, y_test):
    model = LinearRegression()
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    return model, y_pred, regresyon_metrikleri(y_test, y_pred, "Linear Regression")


def decision_tree_reg_egit(X_train, X_test, y_train, y_test):
    model = DecisionTreeRegressor(max_depth=8, random_state=42)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    return model, y_pred, regresyon_metrikleri(y_test, y_pred, "Decision Tree")


def random_forest_reg_egit(X_train, X_test, y_train, y_test):
    model = RandomForestRegressor(n_estimators=100, max_depth=10, random_state=42)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    return model, y_pred, regresyon_metrikleri(y_test, y_pred, "Random Forest")


def knn_reg_egit(X_train, X_test, y_train, y_test):
    model = KNeighborsRegressor(n_neighbors=7, weights="distance")
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    return model, y_pred, regresyon_metrikleri(y_test, y_pred, "KNN")


def svm_reg_egit(X_train, X_test, y_train, y_test):
    model = SVR(kernel="rbf", C=1000, gamma=0.1, epsilon=0.1)
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    return model, y_pred, regresyon_metrikleri(y_test, y_pred, "SVM")
