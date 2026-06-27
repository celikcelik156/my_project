clc; clear; close all;

fprintf('EMLAK FIYAT TAHMIN - MATLAB\n');
fprintf('Mehmet Celik (230007028)\n\n');

data = readtable('../data/emlak_veri.csv');

X = [data.metrekare, data.oda_sayisi, data.bina_yasi, data.bulundugu_kat, ...
     data.toplam_kat, data.balkon, data.esyali, data.site_icinde, data.ulasim_skoru];
y = data.fiyat / 1e6;

N = size(X, 1);
rng(42);
n_test = round(N * 0.20);
perm = randperm(N);
train_idx = perm(1:N-n_test);
test_idx = perm(N-n_test+1:end);

X_train = X(train_idx, :);
X_test = X(test_idx, :);
y_train = y(train_idx);
y_test = y(test_idx);

mdl_lr = fitlm(X_train, y_train);
y_pred_lr = predict(mdl_lr, X_test);

mdl_tree = fitrtree(X_train, y_train, 'MaxNumSplits', 100);
y_pred_tree = predict(mdl_tree, X_test);

mdl_rf = TreeBagger(100, X_train, y_train, 'Method', 'regression');
y_pred_rf = predict(mdl_rf, X_test);

k_komsu = 7;
[idx, ~] = knnsearch(X_train, X_test, 'K', k_komsu);
y_pred_knn = zeros(size(X_test, 1), 1);
for i = 1:size(X_test, 1)
    y_pred_knn(i) = mean(y_train(idx(i, :)));
end

mdl_svm = fitrsvm(X_train, y_train, 'KernelFunction', 'gaussian', 'Standardize', false);
y_pred_svm = predict(mdl_svm, X_test);

y_mean = mean(y_test);
ss_tot = sum((y_test - y_mean).^2);

r2_lr = 1 - sum((y_test - y_pred_lr).^2) / ss_tot;
r2_tree = 1 - sum((y_test - y_pred_tree).^2) / ss_tot;
r2_rf = 1 - sum((y_test - y_pred_rf).^2) / ss_tot;
r2_knn = 1 - sum((y_test - y_pred_knn).^2) / ss_tot;
r2_svm = 1 - sum((y_test - y_pred_svm).^2) / ss_tot;

fprintf('Model               R2\n');
fprintf('Linear Regression   %.4f\n', r2_lr);
fprintf('Decision Tree       %.4f\n', r2_tree);
fprintf('Random Forest       %.4f\n', r2_rf);
fprintf('KNN                 %.4f\n', r2_knn);
fprintf('SVM                 %.4f\n', r2_svm);

figure;
bar([r2_lr, r2_tree, r2_rf, r2_knn, r2_svm]);
set(gca, 'XTickLabel', {'LR', 'Tree', 'RF', 'KNN', 'SVM'});
title('MATLAB Algoritma Karsilastirmasi (R2)');
ylabel('R2');
grid on;
