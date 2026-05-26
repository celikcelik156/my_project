/*
 * isprime.c
 * Bir sayının asal olup olmadığını kontrol eden fonksiyon
 */

#include "mathlib.h"

int asal_mi(int sayi) {
    /* 0 ve 1 asal değildir */
    if (sayi < 2) {
        return 0;
    }
    /* 2 asal sayıdır */
    if (sayi == 2) {
        return 1;
    }
    /* Çift sayılar (2 hariç) asal değildir */
    if (sayi % 2 == 0) {
        return 0;
    }
    /* Tek sayılar için 3'ten sqrt(sayi)'ye kadar kontrol */
    for (int i = 3; i * i <= sayi; i += 2) {
        if (sayi % i == 0) {
            return 0;
        }
    }
    return 1;
}
