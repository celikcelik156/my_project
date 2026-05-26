
clc; clear; close all;

fprintf('===========================================\n');
fprintf('  EMLAK FİYAT TAHMİN SİSTEMİ - MATLAB\n');
fprintf('  Geliştirici: Mehmet Çelik (230007028)\n');
fprintf('===========================================\n\n');

fprintf('[1/5] Veri oluşturuluyor...\n');

rng(42);
N = 500;

sehir_carpanlari = [3.2, 1.6, 2.0, 1.3, 1.8, 0.9, 0.95, 0.85];
sehir_etiketleri = {'İstanbul','Ankara','İzmir','Bursa','Antalya','Adana','Konya','Kayseri'};

sehir_indeks = randi(length(sehir_carpanlari), N, 1);
carpan = sehir_carpanlari(sehir_indeks)';

metrekare   = round(normrnd(100, 30, N, 1));
metrekare   = max(40, min(350, metrekare));

oda_sayisi  = randi(4, N, 1) + 1;
bina_yasi   = randi(40, N, 1);
kat         = randi(10, N, 1);
site        = randi(2, N, 1) - 1;
esyali      = randi(2, N, 1) - 1;
ulasim      = randi(10, N, 1);

baz = 8000;

yas_carpan = ones(N, 1);
yas_carpan(bina_yasi == 0) = 1.35;
yas_carpan(bina_yasi >= 1 & bina_yasi <= 5) = 1.20;
yas_carpan(bina_yasi >= 6 & bina_yasi <= 10) = 1.10;
yas_carpan(bina_yasi > 20) = max(0.6, 1.0 - bina_yasi(bina_yasi > 20) * 0.015);

konfor_carpan = 1 + site * 0.12 + esyali * 0.08 + ulasim/100;

gurultu = normrnd(1.0, 0.08, N, 1);
fiyat = baz .* metrekare .* carpan .* yas_carpan .* konfor_carpan .* gurultu;
fiyat = round(fiyat / 1000) * 1000;

fprintf('   ✓ %d konut verisi oluşturuldu\n', N);
fprintf('   ✓ Fiyat aralığı: %.2f M - %.2f M TL\n', ...
    min(fiyat)/1e6, max(fiyat)/1e6);


fprintf('\n[2/5] Özellik mühendisliği uygulanıyor...\n');

amortisman  = exp(-bina_yasi / 20);
konfor_skor = site*0.3 + esyali*0.3 + ulasim/10*0.4;

X = [metrekare, oda_sayisi, bina_yasi, kat, site, esyali, ...
     ulasim, sehir_indeks, amortisman, konfor_skor];

ozellik_adlari = {'Metrekare', 'Oda Sayisi', 'Bina Yasi', 'Kat', ...
                  'Site', 'Esyali', 'Ulasim', 'Sehir', ...
                  'Amortisman', 'Konfor Skoru'};
y = fiyat / 1e6;

fprintf('   ✓ %d özellik hazırlandı\n', size(X, 2));


fprintf('\n[3/5] Train/Test ayrımı yapılıyor...\n');

test_orani = 0.20;
n_test = round(N * test_orani);
n_train = N - n_test;

perm = randperm(N);
train_idx = perm(1:n_train);
test_idx  = perm(n_train+1:end);

X_train = X(train_idx, :);
X_test  = X(test_idx, :);
y_train = y(train_idx);
y_test  = y(test_idx);

fprintf('   ✓ Eğitim seti: %d örnek\n', n_train);
fprintf('   ✓ Test seti: %d örnek\n', n_test);

[X_train_norm, mu, sigma] = zscore(X_train);
X_test_norm = (X_test - mu) ./ sigma;


fprintf('\n[4/5] Modeller eğitiliyor...\n');

fprintf('   → Lineer Regresyon eğitiliyor...\n');
mdl_lr = fitlm(X_train_norm, y_train);
y_pred_lr = predict(mdl_lr, X_test_norm);

mae_lr  = mean(abs(y_test - y_pred_lr));
mse_lr  = mean((y_test - y_pred_lr).^2);
rmse_lr = sqrt(mse_lr);
ss_res  = sum((y_test - y_pred_lr).^2);
ss_tot  = sum((y_test - mean(y_test)).^2);
r2_lr   = 1 - ss_res/ss_tot;

fprintf('      MAE: %.4f M TL | RMSE: %.4f M TL | R²: %.4f\n', ...
    mae_lr, rmse_lr, r2_lr);

fprintf('   → Karar Ağacı Regresyon eğitiliyor...\n');
mdl_tree = fitrtree(X_train_norm, y_train, 'MaxNumSplits', 20);
y_pred_tree = predict(mdl_tree, X_test_norm);

mae_tree  = mean(abs(y_test - y_pred_tree));
rmse_tree = sqrt(mean((y_test - y_pred_tree).^2));
r2_tree   = 1 - sum((y_test - y_pred_tree).^2) / sum((y_test - mean(y_test)).^2);

fprintf('      MAE: %.4f M TL | RMSE: %.4f M TL | R²: %.4f\n', ...
    mae_tree, rmse_tree, r2_tree);

fprintf('   → KNN Regresyon eğitiliyor...\n');
mdl_knn = fitcknn(X_train_norm, round(y_train), 'NumNeighbors', 7);
y_pred_knn_raw = predict(mdl_knn, X_test_norm);
y_pred_knn = double(y_pred_knn_raw);

mae_knn  = mean(abs(y_test - y_pred_knn));
rmse_knn = sqrt(mean((y_test - y_pred_knn).^2));
r2_knn   = 1 - sum((y_test - y_pred_knn).^2) / sum((y_test - mean(y_test)).^2);

fprintf('      MAE: %.4f M TL | RMSE: %.4f M TL | R²: %.4f\n', ...
    mae_knn, rmse_knn, r2_knn);


fprintf('\n[5/5] Grafikler oluşturuluyor...\n');

figure('Name', 'Algoritma Karşılaştırması', 'Position', [100, 100, 900, 400]);

subplot(1, 3, 1);
modeller = {'Lineer Reg.', 'Karar Ağacı', 'KNN'};
r2_vals = [r2_lr, r2_tree, r2_knn];
bar(r2_vals, 'FaceColor', 'flat', 'CData', [0.39 0.40 0.95; 0.55 0.36 0.97; 0.02 0.71 0.63]);
set(gca, 'XTickLabel', modeller, 'XTickLabelRotation', 10);
ylabel('R² Skoru');
title('R² Karşılaştırması');
ylim([0 1.1]);
grid on; box off;

subplot(1, 3, 2);
rmse_vals = [rmse_lr, rmse_tree, rmse_knn];
bar(rmse_vals, 'FaceColor', 'flat', 'CData', [0.39 0.40 0.95; 0.55 0.36 0.97; 0.02 0.71 0.63]);
set(gca, 'XTickLabel', modeller, 'XTickLabelRotation', 10);
ylabel('RMSE (Milyon TL)');
title('RMSE Karşılaştırması');
grid on; box off;

subplot(1, 3, 3);
mae_vals = [mae_lr, mae_tree, mae_knn];
bar(mae_vals, 'FaceColor', 'flat', 'CData', [0.39 0.40 0.95; 0.55 0.36 0.97; 0.02 0.71 0.63]);
set(gca, 'XTickLabel', modeller, 'XTickLabelRotation', 10);
ylabel('MAE (Milyon TL)');
title('MAE Karşılaştırması');
grid on; box off;

sgtitle('MATLAB - Algoritma Performans Karşılaştırması', 'FontSize', 14, 'FontWeight', 'bold');

figure('Name', 'Gerçek vs Tahmin', 'Position', [200, 200, 800, 400]);

subplot(1, 2, 1);
scatter(y_test, y_pred_lr, 30, [0.39 0.40 0.95], 'filled', 'MarkerFaceAlpha', 0.7);
hold on;
min_val = min([y_test; y_pred_lr]);
max_val = max([y_test; y_pred_lr]);
plot([min_val, max_val], [min_val, max_val], 'r--', 'LineWidth', 2);
xlabel('Gerçek Fiyat (Milyon TL)');
ylabel('Tahmin Edilen Fiyat (Milyon TL)');
title('Lineer Regresyon: Gerçek vs Tahmin');
legend('Tahminler', 'İdeal Çizgi', 'Location', 'best');
grid on; box off;

subplot(1, 2, 2);
hatalar_lr = y_pred_lr - y_test;
histogram(hatalar_lr, 30, 'FaceColor', [0.55 0.36 0.97], 'EdgeColor', 'white', 'FaceAlpha', 0.8);
xline(0, 'r--', 'LineWidth', 2);
xlabel('Tahmin Hatası (Milyon TL)');
ylabel('Frekans');
title('Hata Dağılımı');
grid on; box off;

figure('Name', 'Korelasyon Analizi', 'Position', [300, 300, 700, 600]);

tum_veri = [X(:, 1:7), y];
col_isimler = {'Metrekare', 'Oda', 'Bina Yaşı', 'Kat', 'Site', 'Eşyalı', 'Ulaşım', 'Fiyat'};

corr_matrisi = corr(tum_veri, 'Type', 'Pearson');
heatmap(col_isimler, col_isimler, round(corr_matrisi, 2), ...
    'Colormap', parula, 'ColorLimits', [-1, 1]);
title('Pearson Korelasyon Matrisi');

figure('Name', 'Fiyat Dağılımı', 'Position', [400, 100, 900, 400]);

subplot(1, 2, 1);
histogram(y, 40, 'FaceColor', [0.39 0.40 0.95], 'EdgeColor', 'white', 'FaceAlpha', 0.9);
xlabel('Fiyat (Milyon TL)');
ylabel('Konut Sayısı');
title('Fiyat Dağılım Histogramı');
grid on; box off;

subplot(1, 2, 2);
sehir_fiyatlar = cell(length(sehir_etiketleri), 1);
for i = 1:length(sehir_etiketleri)
    sehir_fiyatlar{i} = y(sehir_indeks == i);
end

for i = 1:length(sehir_etiketleri)
    if ~isempty(sehir_fiyatlar{i})
        boxplot_data(i) = {sehir_fiyatlar{i}};
    end
end

boxplot(y, sehir_indeks, 'Labels', sehir_etiketleri);
xlabel('Şehir');
ylabel('Fiyat (Milyon TL)');
title('Şehir Bazlı Fiyat Dağılımı');
grid on; box off;
xtickangle(30);

figure('Name', 'Metrekare-Fiyat', 'Position', [100, 400, 700, 400]);

scatter(metrekare, y, 30, sehir_indeks, 'filled', 'MarkerFaceAlpha', 0.7);
colorbar;
colormap(jet(8));
xlabel('Metrekare (m²)');
ylabel('Fiyat (Milyon TL)');
title('Metrekare – Fiyat İlişkisi (Renk: Şehir)');
grid on; box off;

hold on;
p = polyfit(metrekare, y, 1);
x_fit = linspace(min(metrekare), max(metrekare), 100);
y_fit = polyval(p, x_fit);
plot(x_fit, y_fit, 'r-', 'LineWidth', 2.5);
legend('Konutlar', 'Lineer Trend');

fprintf('\n===========================================\n');
fprintf('           SONUÇ TABLOSU\n');
fprintf('===========================================\n');
fprintf('%-20s %-10s %-10s %-10s\n', 'Model', 'R²', 'MAE(M TL)', 'RMSE(M TL)');
fprintf('%-20s %-10.4f %-10.4f %-10.4f\n', 'Linear Regression', r2_lr, mae_lr, rmse_lr);
fprintf('%-20s %-10.4f %-10.4f %-10.4f\n', 'Karar Agaci', r2_tree, mae_tree, rmse_tree);
fprintf('%-20s %-10.4f %-10.4f %-10.4f\n', 'KNN', r2_knn, mae_knn, rmse_knn);
fprintf('===========================================\n');

[best_r2, best_idx] = max([r2_lr, r2_tree, r2_knn]);
model_adlari = {'Linear Regression', 'Karar Agaci', 'KNN'};
fprintf('\n🏆 En iyi model: %s (R² = %.4f)\n', model_adlari{best_idx}, best_r2);

fprintf('\n===========================================\n');
fprintf('     İSTATİSTİKSEL ANOVA ANALİZİ (MATLAB)\n');
fprintf('===========================================\n');
[p_anova, tbl_anova, ~] = anova1(y, sehir_indeks, 'off');
f_anova = tbl_anova{2, 5};
fprintf('Grup Değişkeni : Şehir\n');
fprintf('F-İstatistiği  : %.4f\n', f_anova);
fprintf('p-değeri       : %.4e\n', p_anova);
if p_anova < 0.05
    fprintf('Yorum          : Şehirler arasında konut fiyatları açısından anlamlı bir fark vardır (p < 0.05).\n');
else
    fprintf('Yorum          : Şehirler arasında konut fiyatları açısından anlamlı bir fark yoktur (p >= 0.05).\n');
end
fprintf('===========================================\n');

fprintf('\n✅ MATLAB analizi tamamlandı!\n');

