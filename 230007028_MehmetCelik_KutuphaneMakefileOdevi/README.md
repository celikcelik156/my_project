# MathLib - Matematiksel İşlemler Kütüphanesi

## 1. Projenin Amacı

Bu proje, Linux ortamında C dili kullanılarak geliştirilmiş çok dosyalı bir matematik kütüphanesidir.
Paylaşılan kütüphane (.so) formatında derlenerek bir ana program tarafından kullanılmaktadır.
Derleme süreci Makefile ile otomatikleştirilmiştir.

---

## 2. Proje Klasör Yapısı

```
MathLib/
├── include/
│   └── mathlib.h        ← Fonksiyon prototipleri (başlık dosyası)
├── src/
│   ├── square.c         ← Kare hesaplama fonksiyonu
│   ├── cube.c           ← Küp hesaplama fonksiyonu
│   ├── factorial.c      ← Faktöriyel hesaplama fonksiyonu
│   └── isprime.c        ← Asal sayı kontrol fonksiyonu
├── main.c               ← Ana program
├── Makefile             ← Otomatik derleme dosyası
├── README.md            ← Bu dosya
└── video_link.txt       ← Video bağlantısı
```

Derleme sonrası oluşan klasörler:
```
├── obj/                 ← Nesne dosyaları (.o) — Makefile tarafından oluşturulur
├── lib/                 ← Paylaşılan kütüphane (.so) — Makefile tarafından oluşturulur
└── program              ← Çalıştırılabilir dosya — Makefile tarafından oluşturulur
```

---

## 3. Her Dosyanın Görevi

| Dosya | Görev |
|-------|-------|
| `include/mathlib.h` | Tüm fonksiyonların prototiplerini tanımlar |
| `src/square.c` | `kare()` fonksiyonunu içerir |
| `src/cube.c` | `kup()` fonksiyonunu içerir |
| `src/factorial.c` | `faktoriyel()` fonksiyonunu içerir |
| `src/isprime.c` | `asal_mi()` fonksiyonunu içerir |
| `main.c` | Kütüphane fonksiyonlarını çağıran ana program |
| `Makefile` | Derleme, bağlama, çalıştırma ve temizleme işlemleri |

---

## 4. Kütüphanedeki Fonksiyonların Açıklaması

### `double kare(double sayi)`
Verilen sayının karesini hesaplar.  
**Örnek:** `kare(5.0)` → `25.00`

### `double kup(double sayi)`
Verilen sayının küpünü hesaplar.  
**Örnek:** `kup(3.0)` → `27.00`

### `long long faktoriyel(int sayi)`
Verilen tam sayının faktöriyelini hesaplar. Negatif sayılar için `-1` döner.  
**Örnek:** `faktoriyel(5)` → `120`

### `int asal_mi(int sayi)`
Verilen sayının asal olup olmadığını kontrol eder. Asal ise `1`, değilse `0` döner.  
**Örnek:** `asal_mi(7)` → `1 (ASAL)`, `asal_mi(9)` → `0`

---

## 5. Makefile Hedeflerinin Görevleri

| Hedef | Görev |
|-------|-------|
| `make all` | Kütüphane ve ana programı derler (varsayılan) |
| `make lib` | Yalnızca paylaşılan kütüphaneyi (`libmathlib.so`) oluşturur |
| `make main` | Yalnızca ana programı derler |
| `make run` | Programı çalıştırır |
| `make clean` | `obj/`, `lib/` klasörlerini ve `program` dosyasını siler |
| `make install` | Kütüphaneyi sisteme kurar (`/usr/local/lib/`) |
| `make uninstall` | Kütüphaneyi sistemden kaldırır |

---

## 6. Projenin Nasıl Derleneceği

```bash
# Projeyi klonla veya dosyaları MathLib klasörüne kopyala
cd MathLib

# Tüm projeyi derle (kütüphane + ana program)
make

# Yalnızca kütüphaneyi derle
make lib

# Yalnızca ana programı derle
make main
```

---

## 7. Programın Nasıl Çalıştırılacağı

```bash
# make run ile çalıştır
make run

# veya doğrudan çalıştır
./program
```

---

## 8. Beklenen Örnek Çıktı

```
========================================
   MathLib Kutuphanesi - Test Programi  
========================================

--- Kare Hesaplama ---
  kare(3.0) = 9.00
  kare(5.0) = 25.00
  kare(7.5) = 56.25
  kare(-4.0) = 16.00

--- Kup Hesaplama ---
  kup(2.0) = 8.00
  kup(3.0) = 27.00
  kup(4.0) = 64.00
  kup(-2.0) = -8.00

--- Faktoriyel Hesaplama ---
  faktoriyel(0) = 1
  faktoriyel(1) = 1
  faktoriyel(5) = 120
  faktoriyel(7) = 5040
  faktoriyel(10) = 3628800
  faktoriyel(12) = 479001600

--- Asal Sayi Kontrolu ---
   1  -> asal degil
   2  -> ASAL
   3  -> ASAL
   4  -> asal degil
   7  -> ASAL
  11  -> ASAL
  15  -> asal degil
  17  -> ASAL
  20  -> asal degil
  23  -> ASAL

========================================
           Program tamamlandi.          
========================================
```
