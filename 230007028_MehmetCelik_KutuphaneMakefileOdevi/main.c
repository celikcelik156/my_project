/*
 * main.c
 * Ana program - mathlib kütüphanesindeki fonksiyonları kullanan örnek program
 */

#include <stdio.h>
#include "mathlib.h"

int main() {
    printf("========================================\n");
    printf("   MathLib Kutuphanesi - Test Programi  \n");
    printf("========================================\n\n");

    /* --- KARE HESAPLAMA --- */
    printf("--- Kare Hesaplama ---\n");
    double sayilar[] = {3.0, 5.0, 7.5, -4.0};
    int n = sizeof(sayilar) / sizeof(sayilar[0]);
    for (int i = 0; i < n; i++) {
        printf("  kare(%.1f) = %.2f\n", sayilar[i], kare(sayilar[i]));
    }

    /* --- KÜP HESAPLAMA --- */
    printf("\n--- Kup Hesaplama ---\n");
    double sayilar2[] = {2.0, 3.0, 4.0, -2.0};
    int n2 = sizeof(sayilar2) / sizeof(sayilar2[0]);
    for (int i = 0; i < n2; i++) {
        printf("  kup(%.1f) = %.2f\n", sayilar2[i], kup(sayilar2[i]));
    }

    /* --- FAKTÖRİYEL HESAPLAMA --- */
    printf("\n--- Faktoriyel Hesaplama ---\n");
    int faktSayilar[] = {0, 1, 5, 7, 10, 12};
    int nf = sizeof(faktSayilar) / sizeof(faktSayilar[0]);
    for (int i = 0; i < nf; i++) {
        long long sonuc = faktoriyel(faktSayilar[i]);
        printf("  faktoriyel(%d) = %lld\n", faktSayilar[i], sonuc);
    }

    /* --- ASAL SAYI KONTROLÜ --- */
    printf("\n--- Asal Sayi Kontrolu ---\n");
    int asalSayilar[] = {1, 2, 3, 4, 7, 11, 15, 17, 20, 23};
    int na = sizeof(asalSayilar) / sizeof(asalSayilar[0]);
    for (int i = 0; i < na; i++) {
        int sonuc = asal_mi(asalSayilar[i]);
        printf("  %2d  -> %s\n", asalSayilar[i], sonuc ? "ASAL" : "asal degil");
    }

    printf("\n========================================\n");
    printf("           Program tamamlandi.          \n");
    printf("========================================\n");

    return 0;
}
