import socket
import threading
from protocol import send_json, receive_json

HOST = '0.0.0.0'
PORT = 12345

connected_users = {}
lock = threading.Lock()


def handle_client(client_socket, client_address):
    print(f"[YENİ BAĞLANTI] {client_address} bağlandı.")
    username = None

    try:
        request = receive_json(client_socket)
        if request is None or request.get("type") != "LOGIN":
            client_socket.close()
            return

        username = request.get("username")

        with lock:
            if username in connected_users:
                send_json(client_socket, {"type": "ERROR", "message": "Bu kullanıcı adı kullanımda."})
                client_socket.close()
                return
            connected_users[username] = client_socket

        print(f"[GİRİŞ] {username} sisteme dahil oldu.")

        user_list = list(connected_users.keys())
        send_json(client_socket, {"type": "USER_LIST", "users": user_list})

        broadcast({"type": "USER_JOIN", "username": username}, exclude_user=username)

        while True:
            msg = receive_json(client_socket)
            if msg is None:
                break

            if msg.get("type") != "FILE_CHUNK":
                print(f"[{username}] Gelen: {msg}")

            if msg.get("type") == "GET_USER_LIST":
                user_list = list(connected_users.keys())
                send_json(client_socket, {"type": "USER_LIST", "users": user_list})

            ROUTABLE_TYPES = ["CHAT_MSG", "FILE_OFFER", "FILE_RESPONSE", "FILE_CHUNK", "FILE_DONE"]

            if msg.get("type") in ROUTABLE_TYPES:
                target_user = msg.get("to")

                target_sock = None
                with lock:
                    target_sock = connected_users.get(target_user)

                if target_sock:
                    msg["from"] = username
                    try:
                        send_json(target_sock, msg)
                        if msg.get("type") != "FILE_CHUNK":
                            print(f"[İLETİLDİ] {username} -> {target_user} ({msg.get('type')})")
                    except:
                        pass
                else:
                    if msg.get("type") in ["CHAT_MSG", "FILE_OFFER"]:
                        print(f"[UYARI] Hedef '{target_user}' bulunamadı.")
                        send_json(client_socket, {
                            "type": "ERROR",
                            "message": f"'{target_user}' çevrimdışı."
                        })

    except Exception as e:
        print(f"[HATA] {username}: {e}")

    finally:
        if username:
            print(f"[ÇIKIŞ] {username} ayrıldı.")
            with lock:
                if username in connected_users:
                    del connected_users[username]
            broadcast({"type": "USER_LEAVE", "username": username})
        client_socket.close()


def broadcast(message, exclude_user=None):
    with lock:
        for user, sock in list(connected_users.items()):
            if user == exclude_user:
                continue
            try:
                send_json(sock, message)
            except:
                pass


def start_server():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((HOST, PORT))
    server.listen()
    print(f"--- Sunucu {HOST}:{PORT} üzerinde çalışıyor ---")

    while True:
        client_sock, addr = server.accept()
        threading.Thread(target=handle_client, args=(client_sock, addr)).start()


if __name__ == "__main__":
    start_server()