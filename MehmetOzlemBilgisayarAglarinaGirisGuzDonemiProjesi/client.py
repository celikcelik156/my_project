import socket
import threading
import json
import sys
import os
import base64
import time
from datetime import datetime
from protocol import send_json, receive_json

HOST = 'localhost'
PORT = 12345

DOWNLOAD_DIR = "indirilenler"
if not os.path.exists(DOWNLOAD_DIR):
    os.makedirs(DOWNLOAD_DIR)

offered_files = {}


def get_timestamp():
    return datetime.now().isoformat(timespec='seconds')


def listen_for_messages(sock):
    while True:
        try:
            msg = receive_json(sock)
            if msg is None:
                print("\n[BİLGİ] Sunucu bağlantısı koptu.")
                sys.exit()
                break

            typ = msg.get("type")
            sender = msg.get("from", "Bilinmeyen")
            ts = msg.get("timestamp", "")
            time_display = f"[{ts.split('T')[-1]}]" if ts else ""

            if typ == "CHAT_MSG":
                print(f"\n{time_display} [{sender}]: {msg['text']}")
                print(">> ", end="", flush=True)

            elif typ == "USER_LIST":
                print(f"\n[SİSTEM] Kullanıcılar: {msg['users']}")
                print(">> ", end="", flush=True)

            elif typ == "USER_JOIN":
                print(f"\n[SİSTEM] {msg['username']} katıldı.")
                print(">> ", end="", flush=True)

            elif typ == "USER_LEAVE":
                print(f"\n[SİSTEM] {msg['username']} ayrıldı.")
                print(">> ", end="", flush=True)

            elif typ == "FILE_OFFER":
                filename = msg['filename']
                filesize = msg['filesize']
                print(f"\n[DOSYA] {sender} size '{filename}' ({filesize} bayt) göndermek istiyor.")
                print(f"Kabul için:  kabul:{sender}:{filename}")
                print(f"Red için:    red:{sender}:{filename}")
                print(">> ", end="", flush=True)

            elif typ == "FILE_RESPONSE":
                if msg.get("status") == "accepted":
                    filename = msg['filename']
                    print(f"\n[BİLGİ] {sender} kabul etti. Gönderim başlıyor...")

                    full_path = offered_files.get(filename)
                    if full_path and os.path.exists(full_path):
                        threading.Thread(target=send_file_content, args=(sock, sender, full_path)).start()
                    else:
                        print(f"\n[HATA] Dosya yolu hafızada bulunamadı: {filename}")
                else:
                    print(f"\n[BİLGİ] {sender} dosya teklifini REDDETTİ.")
                print(">> ", end="", flush=True)

            elif typ == "FILE_CHUNK":
                filename = msg['filename']
                chunk_data = base64.b64decode(msg['data'])
                filepath = os.path.join(DOWNLOAD_DIR, filename)
                with open(filepath, 'ab') as f:
                    f.write(chunk_data)

            elif typ == "FILE_DONE":
                filename = msg['filename']
                print(f"\n[BAŞARILI] '{filename}' indi.")
                print(">> ", end="", flush=True)

            elif typ == "ERROR":
                print(f"\n[HATA] {msg['message']}")
                print(">> ", end="", flush=True)

        except Exception as e:
            print(f"\n[HATA] Dinleme hatası: {e}")
            break


def send_file_content(sock, target_user, filepath):
    try:
        filename = os.path.basename(filepath)
        with open(filepath, 'rb') as f:
            chunk_seq = 0
            while True:
                bytes_read = f.read(4096)
                if not bytes_read: break

                encoded_data = base64.b64encode(bytes_read).decode('utf-8')
                chunk_msg = {
                    "type": "FILE_CHUNK",
                    "to": target_user,
                    "filename": filename,
                    "seq": chunk_seq,
                    "data": encoded_data
                }
                send_json(sock, chunk_msg)
                chunk_seq += 1
                time.sleep(0.02)

        done_msg = {
            "type": "FILE_DONE",
            "to": target_user,
            "filename": filename,
            "timestamp": get_timestamp()
        }
        send_json(sock, done_msg)
        print(f"\n[BİLGİ] Transfer tamamlandı: {filename}")
        print(">> ", end="", flush=True)

    except Exception as e:
        print(f"\n[HATA] Dosya okunurken hata: {e}")


def start_client():
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.connect((HOST, PORT))
    except:
        print("Sunucuya bağlanılamadı. server.py açık mı?")
        return

    username = input("Kullanıcı Adı: ").strip()
    send_json(sock, {"type": "LOGIN", "username": username})

    threading.Thread(target=listen_for_messages, args=(sock,), daemon=True).start()

    print(f"\n--- Hoşgeldin {username}! ---")
    print("1. Liste:   list")
    print("2. Sohbet:  Ayşe:Selam")
    print("3. Dosya:   dosya:Ayşe:C:\\resim.jpg")
    print("4. Kabul:   kabul:Ali:resim.jpg")
    print("5. Red:     red:Ali:resim.jpg")
    print("-" * 40)

    while True:
        try:
            user_input = input(">> ").strip()
            if not user_input: continue
            if user_input.lower() == 'exit': break

            if user_input.lower() == 'list':
                send_json(sock, {"type": "GET_USER_LIST"})

            elif user_input.startswith("dosya:"):
                try:
                    parts = user_input.split(":", 2)
                    if len(parts) < 3: raise ValueError
                    target = parts[1].strip()
                    filepath = parts[2].strip()
                    if os.path.exists(filepath):
                        filesize = os.path.getsize(filepath)
                        filename = os.path.basename(filepath)
                        offered_files[filename] = filepath
                        offer_msg = {
                            "type": "FILE_OFFER",
                            "to": target,
                            "filename": filename,
                            "filesize": filesize,
                            "timestamp": get_timestamp()
                        }
                        send_json(sock, offer_msg)
                        print(f"[BİLGİ] Teklif gönderildi...")
                    else:
                        print(f"[HATA] Dosya bulunamadı: {filepath}")
                except ValueError:
                    print("[UYARI] Format: dosya:Kime:DosyaYolu")

            elif user_input.startswith("kabul:"):
                try:
                    parts = user_input.split(":", 2)
                    if len(parts) < 3: raise ValueError
                    sender = parts[1].strip()
                    filename = parts[2].strip()
                    save_path = os.path.join(DOWNLOAD_DIR, filename)
                    if os.path.exists(save_path): os.remove(save_path)
                    response_msg = {
                        "type": "FILE_RESPONSE",
                        "to": sender,
                        "filename": filename,
                        "status": "accepted",
                        "timestamp": get_timestamp()
                    }
                    send_json(sock, response_msg)
                    print(f"[BİLGİ] Kabul edildi, bekleniyor...")
                except ValueError:
                    print("[UYARI] Format: kabul:Gönderen:DosyaAdı")

            elif user_input.startswith("red:"):
                try:
                    parts = user_input.split(":", 2)
                    if len(parts) < 3: raise ValueError
                    sender = parts[1].strip()
                    filename = parts[2].strip()
                    response_msg = {
                        "type": "FILE_RESPONSE",
                        "to": sender,
                        "filename": filename,
                        "status": "rejected",
                        "timestamp": get_timestamp()
                    }
                    send_json(sock, response_msg)
                    print(f"[BİLGİ] {filename} reddedildi.")
                except ValueError:
                    print("[UYARI] Format: red:Gönderen:DosyaAdı")

            elif ":" in user_input:
                parts = user_input.split(":", 1)
                target = parts[0].strip()
                text = parts[1].strip()
                chat_msg = {
                    "type": "CHAT_MSG",
                    "to": target,
                    "text": text,
                    "timestamp": get_timestamp()
                }
                send_json(sock, chat_msg)
            else:
                print(f"[UYARI] Hatalı format! 'list' veya 'isim:mesaj' yazın.")

        except KeyboardInterrupt:
            break
        except Exception as e:
            print(f"Hata: {e}")
            break

    sock.close()


if __name__ == "__main__":
    start_client()