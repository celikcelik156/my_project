# ============================================================
# Makefile - MathLib Projesi
# ============================================================
# Degiskenler
CC      = gcc
CFLAGS  = -Wall -Wextra -fPIC -Iinclude
LDFLAGS = -Llib -lmathlib -Wl,-rpath,$(shell pwd)/lib

SRC_DIR     = src
INC_DIR     = include
LIB_DIR     = lib
OBJ_DIR     = obj

# Kaynak dosyalar ve onlara karsilik gelen nesne dosyalari
SRCS = $(SRC_DIR)/square.c \
       $(SRC_DIR)/cube.c \
       $(SRC_DIR)/factorial.c \
       $(SRC_DIR)/isprime.c

OBJS = $(OBJ_DIR)/square.o \
       $(OBJ_DIR)/cube.o \
       $(OBJ_DIR)/factorial.o \
       $(OBJ_DIR)/isprime.o

LIB_NAME  = $(LIB_DIR)/libmathlib.so
EXEC_NAME = program

# ============================================================
# all: Tum projeyi derle (kutuphanesi + ana program)
# ============================================================
all: $(LIB_NAME) $(EXEC_NAME)

# ============================================================
# lib: Paylasilan kutuphanesi olustur
# ============================================================
lib: $(LIB_NAME)

$(LIB_NAME): $(OBJS) | $(LIB_DIR)
	$(CC) -shared -o $@ $(OBJS)
	@echo "[OK] Paylasilan kutuphane olusturuldu: $@"

# Nesne dosyasi olusturma kurali
$(OBJ_DIR)/%.o: $(SRC_DIR)/%.c | $(OBJ_DIR)
	$(CC) $(CFLAGS) -c $< -o $@
	@echo "[OK] Nesne dosyasi olusturuldu: $@"

# ============================================================
# main: Ana programi derle ve kutuphaneyh bagla
# ============================================================
main: $(EXEC_NAME)

$(EXEC_NAME): main.c $(LIB_NAME)
	$(CC) -Wall -Iinclude -o $@ main.c $(LDFLAGS)
	@echo "[OK] Calistirilabilir dosya olusturuldu: $@"

# ============================================================
# run: Programi calistir
# ============================================================
run: $(EXEC_NAME)
	@echo ""
	@echo "--- Program calistiriliyor ---"
	@echo ""
	./$(EXEC_NAME)

# ============================================================
# clean: Gecici ve cikti dosyalarini temizle
# ============================================================
clean:
	rm -rf $(OBJ_DIR) $(LIB_DIR) $(EXEC_NAME)
	@echo "[OK] Temizleme tamamlandi."

# ============================================================
# install / uninstall (isteğe bağlı)
# ============================================================
install: $(LIB_NAME)
	cp $(LIB_NAME) /usr/local/lib/
	ldconfig
	@echo "[OK] Kutuphane sisteme kuruldu."

uninstall:
	rm -f /usr/local/lib/libmathlib.so
	ldconfig
	@echo "[OK] Kutuphane sistemden kaldirildi."

# ============================================================
# Klasor olusturma kurallari
# ============================================================
$(OBJ_DIR):
	mkdir -p $(OBJ_DIR)

$(LIB_DIR):
	mkdir -p $(LIB_DIR)

# Bu hedefler dosya degil, islem temsil eder
.PHONY: all lib main run clean install uninstall
