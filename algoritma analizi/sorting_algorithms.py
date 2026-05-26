import random
import time
import tracemalloc
import copy
import sys
import os

import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
import numpy as np

sys.setrecursionlimit(200000)

def bubble_sort(arr):
    n = len(arr)
    for i in range(n):
        swapped = False
        for j in range(0, n - i - 1):
            if arr[j] > arr[j + 1]:
                arr[j], arr[j + 1] = arr[j + 1], arr[j]
                swapped = True
        if not swapped:
            break


def selection_sort(arr):
    n = len(arr)
    for i in range(n):
        min_idx = i
        for j in range(i + 1, n):
            if arr[j] < arr[min_idx]:
                min_idx = j
        arr[i], arr[min_idx] = arr[min_idx], arr[i]


def insertion_sort(arr):
    for i in range(1, len(arr)):
        key = arr[i]
        j = i - 1
        while j >= 0 and arr[j] > key:
            arr[j + 1] = arr[j]
            j -= 1
        arr[j + 1] = key


def merge_sort(arr):
    if len(arr) > 1:
        mid = len(arr) // 2
        left_half = arr[:mid]
        right_half = arr[mid:]

        merge_sort(left_half)
        merge_sort(right_half)

        i = j = k = 0
        while i < len(left_half) and j < len(right_half):
            if left_half[i] <= right_half[j]:
                arr[k] = left_half[i]
                i += 1
            else:
                arr[k] = right_half[j]
                j += 1
            k += 1

        while i < len(left_half):
            arr[k] = left_half[i]
            i += 1
            k += 1

        while j < len(right_half):
            arr[k] = right_half[j]
            j += 1
            k += 1


def _partition(arr, low, high):
    mid = (low + high) // 2
    if arr[low] > arr[mid]:
        arr[low], arr[mid] = arr[mid], arr[low]
    if arr[low] > arr[high]:
        arr[low], arr[high] = arr[high], arr[low]
    if arr[mid] > arr[high]:
        arr[mid], arr[high] = arr[high], arr[mid]
    pivot = arr[mid]
    arr[mid], arr[high - 1] = arr[high - 1], arr[mid]
    i = low
    j = high - 1
    while True:
        i += 1
        while arr[i] < pivot:
            i += 1
        j -= 1
        while arr[j] > pivot:
            j -= 1
        if i >= j:
            break
        arr[i], arr[j] = arr[j], arr[i]
    arr[i], arr[high - 1] = arr[high - 1], arr[i]
    return i


def _quick_sort_helper(arr, low, high):
    if high - low < 2:
        return
    if high - low < 10:
        for i in range(low + 1, high + 1):
            key = arr[i]
            j = i - 1
            while j >= low and arr[j] > key:
                arr[j + 1] = arr[j]
                j -= 1
            arr[j + 1] = key
        return
    pi = _partition(arr, low, high)
    _quick_sort_helper(arr, low, pi - 1)
    _quick_sort_helper(arr, pi + 1, high)


def quick_sort(arr):
    if len(arr) > 1:
        _quick_sort_helper(arr, 0, len(arr) - 1)


def _heapify(arr, n, i):
    largest = i
    left = 2 * i + 1
    right = 2 * i + 2

    if left < n and arr[left] > arr[largest]:
        largest = left
    if right < n and arr[right] > arr[largest]:
        largest = right

    if largest != i:
        arr[i], arr[largest] = arr[largest], arr[i]
        _heapify(arr, n, largest)


def heap_sort(arr):
    n = len(arr)
    for i in range(n // 2 - 1, -1, -1):
        _heapify(arr, n, i)
    for i in range(n - 1, 0, -1):
        arr[0], arr[i] = arr[i], arr[0]
        _heapify(arr, i, 0)


def _counting_sort_by_digit(arr, exp):
    n = len(arr)
    output = [0] * n
    count = [0] * 10

    for num in arr:
        index = (num // exp) % 10
        count[index] += 1

    for i in range(1, 10):
        count[i] += count[i - 1]

    for i in range(n - 1, -1, -1):
        index = (arr[i] // exp) % 10
        output[count[index] - 1] = arr[i]
        count[index] -= 1

    for i in range(n):
        arr[i] = output[i]


def radix_sort(arr):
    if not arr:
        return
    max_val = max(arr)
    exp = 1
    while max_val // exp > 0:
        _counting_sort_by_digit(arr, exp)
        exp *= 10



def measure_performance(sort_func, data):
    arr = copy.copy(data)

    tracemalloc.start()
    start_time = time.perf_counter()

    sort_func(arr)

    end_time = time.perf_counter()
    current, peak = tracemalloc.get_traced_memory()
    tracemalloc.stop()

    elapsed = end_time - start_time
    peak_kb = peak / 1024

    return elapsed, peak_kb


MIN_VAL = 10_000_000_000
MAX_VAL = 99_999_999_999

ALL_SIZES = [100, 500, 1000, 2000, 5000]

FAST_SIZES = [10_000, 50_000, 100_000]

SLOW_ALGOS = {
    "Bubble Sort": bubble_sort,
    "Selection Sort": selection_sort,
    "Insertion Sort": insertion_sort,
}

FAST_ALGOS = {
    "Merge Sort": merge_sort,
    "Quick Sort": quick_sort,
    "Heap Sort": heap_sort,
    "Radix Sort": radix_sort,
}

ALL_ALGOS = {**SLOW_ALGOS, **FAST_ALGOS}

ALGO_COLORS = {
    "Bubble Sort":    "#e74c3c",
    "Selection Sort": "#e67e22",
    "Insertion Sort": "#f1c40f",
    "Merge Sort":     "#2ecc71",
    "Quick Sort":     "#3498db",
    "Heap Sort":      "#9b59b6",
    "Radix Sort":     "#1abc9c",
}


def run_tests():
    print("=" * 70)
    print("  SIRALAMA ALGORİTMALARI PERFORMANS TESTİ")
    print("=" * 70)

    all_sizes = sorted(set(ALL_SIZES + FAST_SIZES))
    datasets = {}
    print("\n[1/3] Veri setleri oluşturuluyor...")
    for size in all_sizes:
        datasets[size] = [random.randint(MIN_VAL, MAX_VAL) for _ in range(size)]
        print(f"  n={size:>7,} -> {size} adet 11 basamaklı tam sayı üretildi.")

    results = []  # (algo_name, size, time_s, mem_kb)

    print("\n[2/3] Algoritmalar test ediliyor...\n")

    for algo_name, algo_func in ALL_ALGOS.items():
        is_slow = algo_name in SLOW_ALGOS
        test_sizes = ALL_SIZES if is_slow else all_sizes
        complexity = "O(n²)" if is_slow else "O(n log n)"
        print(f"  ── {algo_name} ({complexity}) ──")

        for size in test_sizes:
            data = datasets[size]
            elapsed, peak_kb = measure_performance(algo_func, data)
            results.append({
                "Algoritma": algo_name,
                "Veri Boyutu": size,
                "Çalışma Süresi (s)": round(elapsed, 6),
                "Bellek Kullanımı (KB)": round(peak_kb, 2),
            })
            tag = ""
            if size > 5000:
                tag = "  [hızlı algo]"
            print(f"    n={size:>7,}  ->  {elapsed:10.6f} s  |  {peak_kb:10.2f} KB{tag}")

        print()

    return results


def plot_time_comparison(results, output_dir):
    """Grafik 1: Veri boyutuna göre çalışma süresi"""
    fig, axes = plt.subplots(1, 2, figsize=(16, 7))
    fig.suptitle(
        "Veri Boyutuna Göre Çalışma Süresi Karşılaştırması",
        fontsize=16, fontweight="bold", y=1.01
    )

    ax1 = axes[0]
    for name, func in SLOW_ALGOS.items():
        data = [(r["Veri Boyutu"], r["Çalışma Süresi (s)"])
                for r in results if r["Algoritma"] == name]
        data.sort()
        xs, ys = zip(*data)
        ax1.plot(xs, ys, marker="o", label=name,
                 color=ALGO_COLORS[name], linewidth=2, markersize=6)

    ax1.set_title("O(n²) Algoritmalar", fontsize=13, pad=10)
    ax1.set_xlabel("Veri Boyutu (n)", fontsize=11)
    ax1.set_ylabel("Çalışma Süresi (saniye)", fontsize=11)
    ax1.legend(fontsize=10)
    ax1.grid(True, alpha=0.3)
    ax1.xaxis.set_major_formatter(ticker.FuncFormatter(lambda x, _: f"{int(x):,}"))

    ax2 = axes[1]
    all_sizes_fast = sorted(set(ALL_SIZES + FAST_SIZES))
    for name, func in FAST_ALGOS.items():
        data = [(r["Veri Boyutu"], r["Çalışma Süresi (s)"])
                for r in results if r["Algoritma"] == name]
        data.sort()
        xs, ys = zip(*data)
        ax2.plot(xs, ys, marker="o", label=name,
                 color=ALGO_COLORS[name], linewidth=2, markersize=6)

    ax2.set_title("O(n log n) / O(d·n) Algoritmalar", fontsize=13, pad=10)
    ax2.set_xlabel("Veri Boyutu (n)", fontsize=11)
    ax2.set_ylabel("Çalışma Süresi (saniye)", fontsize=11)
    ax2.legend(fontsize=10)
    ax2.grid(True, alpha=0.3)
    ax2.xaxis.set_major_formatter(ticker.FuncFormatter(lambda x, _: f"{int(x):,}"))

    plt.tight_layout()
    path = os.path.join(output_dir, "grafik1_sure_karsilastirma.png")
    plt.savefig(path, dpi=150, bbox_inches="tight")
    plt.close()
    print(f"  [OK] Grafik 1 kaydedildi: {path}")
    return path


def plot_memory_comparison(results, output_dir):
    fig, ax = plt.subplots(figsize=(12, 7))

    target_size = 5000
    names, mems = [], []
    for name in ALL_ALGOS.keys():
        row = next((r for r in results
                    if r["Algoritma"] == name and r["Veri Boyutu"] == target_size), None)
        if row:
            names.append(name)
            mems.append(row["Bellek Kullanımı (KB)"])

    colors = [ALGO_COLORS[n] for n in names]
    bars = ax.bar(names, mems, color=colors, edgecolor="white", linewidth=1.2)

    for bar, val in zip(bars, mems):
        ax.text(bar.get_x() + bar.get_width() / 2, bar.get_height() + 0.5,
                f"{val:.1f} KB", ha="center", va="bottom", fontsize=10, fontweight="bold")

    ax.set_title(f"Algoritmalara Göre Bellek Kullanımı (n = {target_size:,})",
                 fontsize=14, fontweight="bold", pad=15)
    ax.set_xlabel("Algoritma", fontsize=12)
    ax.set_ylabel("Tepe Bellek Kullanımı (KB)", fontsize=12)
    ax.set_xticklabels(names, rotation=20, ha="right", fontsize=11)
    ax.grid(True, axis="y", alpha=0.3)
    ax.set_ylim(0, max(mems) * 1.2)

    plt.tight_layout()
    path = os.path.join(output_dir, "grafik2_bellek_karsilastirma.png")
    plt.savefig(path, dpi=150, bbox_inches="tight")
    plt.close()
    print(f"  [OK] Grafik 2 kaydedildi: {path}")
    return path


def plot_log_time(results, output_dir):
    fig, ax = plt.subplots(figsize=(14, 7))

    for name in ALL_ALGOS.keys():
        data = [(r["Veri Boyutu"], r["Çalışma Süresi (s)"])
                for r in results if r["Algoritma"] == name]
        data.sort()
        xs, ys = zip(*data)
        ax.plot(xs, ys, marker="o", label=name,
                color=ALGO_COLORS[name], linewidth=2, markersize=6)

    ax.set_xscale("log")
    ax.set_yscale("log")
    ax.set_title("Tüm Algoritmalar – Logaritmik Ölçek (Süre vs Boyut)",
                 fontsize=14, fontweight="bold", pad=15)
    ax.set_xlabel("Veri Boyutu (n) – log ölçek", fontsize=12)
    ax.set_ylabel("Çalışma Süresi (s) – log ölçek", fontsize=12)
    ax.legend(fontsize=10, loc="upper left")
    ax.grid(True, which="both", alpha=0.3)

    plt.tight_layout()
    path = os.path.join(output_dir, "grafik3_logaritmik.png")
    plt.savefig(path, dpi=150, bbox_inches="tight")
    plt.close()
    print(f"  [OK] Grafik 3 kaydedildi: {path}")
    return path



def print_table(results):
    print("\n" + "=" * 70)
    print("  SONUÇ TABLOSU")
    print("=" * 70)
    header = f"{'Algoritma':<20} {'Veri Boyutu':>12} {'Süre (s)':>14} {'Bellek (KB)':>14}"
    print(header)
    print("-" * 70)
    prev_algo = None
    for r in results:
        if prev_algo and prev_algo != r["Algoritma"]:
            print()
        print(f"{r['Algoritma']:<20} {r['Veri Boyutu']:>12,} "
              f"{r['Çalışma Süresi (s)']:>14.6f} "
              f"{r['Bellek Kullanımı (KB)']:>14.2f}")
        prev_algo = r["Algoritma"]
    print("=" * 70)


def save_csv(results, output_dir):
    path = os.path.join(output_dir, "sonuclar.csv")
    with open(path, "w", encoding="utf-8-sig") as f:
        f.write("Algoritma,Veri Boyutu,Çalışma Süresi (s),Bellek Kullanımı (KB)\n")
        for r in results:
            f.write(f"{r['Algoritma']},{r['Veri Boyutu']},"
                    f"{r['Çalışma Süresi (s)']},{r['Bellek Kullanımı (KB)']}\n")
    print(f"  [OK] CSV kaydedildi: {path}")

def main():
    output_dir = os.path.dirname(os.path.abspath(__file__))

    results = run_tests()

    print_table(results)

    print("\n[3/3] Grafikler oluşturuluyor...")
    g1 = plot_time_comparison(results, output_dir)
    g2 = plot_memory_comparison(results, output_dir)
    g3 = plot_log_time(results, output_dir)

    save_csv(results, output_dir)

    print("\n" + "=" * 70)
    print("  TÜM TESTLER TAMAMLANDI!")
    print(f"  Çıktı klasörü: {output_dir}")
    print("=" * 70)


if __name__ == "__main__":
    main()
