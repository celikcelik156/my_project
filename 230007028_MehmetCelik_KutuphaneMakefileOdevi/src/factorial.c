/*
 * factorial.c
 * Bir sayının faktöriyelini hesaplayan fonksiyon
 */

#include "mathlib.h"

long long faktoriyel(int sayi) {
    /* Negatif sayılar için hata kodu */
    if (sayi < 0) {
        return -1;
    }
    /* 0! = 1 tanımı */
    if (sayi == 0) {
        return 1;
    }

    long long sonuc = 1;
    for (int i = 1; i <= sayi; i++) {
        sonuc *= i;
    }
    return sonuc;
}
