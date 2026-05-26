import socket
import struct
import json

HEADER_FORMAT = '>I'
HEADER_SIZE = 4


def send_json(sock, message_dict):

    json_data = json.dumps(message_dict, ensure_ascii=False).encode('utf-8')
    data_length = len(json_data)
    header = struct.pack(HEADER_FORMAT, data_length)
    sock.sendall(header + json_data)


def receive_json(sock):

    header_data = _recv_all(sock, HEADER_SIZE)
    if not header_data:
        return None

    message_length = struct.unpack(HEADER_FORMAT, header_data)[0]

    json_body = _recv_all(sock, message_length)
    if not json_body:
        return None

    return json.loads(json_body.decode('utf-8'))


def _recv_all(sock, length):
    data = b''
    while len(data) < length:
        packet = sock.recv(length - len(data))
        if not packet:
            return None
        data += packet
    return data