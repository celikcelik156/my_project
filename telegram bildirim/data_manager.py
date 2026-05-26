import json
import sqlite3
import os
from config import DATABASE_PATH

def get_db_connection():
    conn = sqlite3.connect(DATABASE_PATH, timeout=30)
    conn.row_factory = sqlite3.Row
    conn.execute('''
        CREATE TABLE IF NOT EXISTS users (
            chat_id TEXT PRIMARY KEY,
            username TEXT,
            password TEXT,
            registration_state TEXT DEFAULT 'COMPLETED',
            last_scrape DATETIME,
            last_status TEXT,
            last_status_time DATETIME,
            is_active INTEGER DEFAULT 1
        )
    ''')
    conn.execute('''
        CREATE TABLE IF NOT EXISTS course_states (
            chat_id TEXT,
            course_code TEXT,
            course_name TEXT,
            state_json TEXT,
            last_updated DATETIME DEFAULT CURRENT_TIMESTAMP,
            PRIMARY KEY (chat_id, course_code)
        )
    ''')
    conn.execute('''
        CREATE TABLE IF NOT EXISTS system_state (
            key TEXT PRIMARY KEY,
            value TEXT
        )
    ''')
    
    try:
        cursor = conn.cursor()
        cursor.execute("PRAGMA table_info(users)")
        columns = [row['name'] for row in cursor.fetchall()]
        
        if 'last_status' not in columns:
            conn.execute("ALTER TABLE users ADD COLUMN last_status TEXT")
            print("Veritabanı güncellendi: last_status kolonu eklendi.")
            
        if 'last_status_time' not in columns:
            conn.execute("ALTER TABLE users ADD COLUMN last_status_time DATETIME")
            print("Veritabanı güncellendi: last_status_time kolonu eklendi.")
            
        conn.commit()
    except Exception as e:
        print(f"Migrasyon hatası: {e}")

    conn.commit()
    return conn

def get_all_active_users():
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM users WHERE is_active = 1 AND registration_state = 'COMPLETED'")
        users = [dict(row) for row in cursor.fetchall()]
        conn.close()
        return users
    except Exception as e:
        print(f"Kullanıcılar listelenirken hata: {e}")
        return []

def get_user_by_id(chat_id):
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM users WHERE chat_id = ?", (str(chat_id),))
        row = cursor.fetchone()
        conn.close()
        return dict(row) if row else None
    except Exception as e:
        print(f"Kullanıcı bulunurken hata: {e}")
        return None

def update_user_registration(chat_id, **kwargs):
    try:
        conn = get_db_connection()
        chat_id = str(chat_id)
        
        # Mevcut kullanıcıyı kontrol et
        cursor = conn.cursor()
        cursor.execute("SELECT 1 FROM users WHERE chat_id = ?", (chat_id,))
        if not cursor.fetchone():
            conn.execute("INSERT INTO users (chat_id) VALUES (?)", (chat_id,))
        
        if kwargs:
            set_clause = ", ".join([f"{k} = ?" for k in kwargs.keys()])
            values = list(kwargs.values()) + [chat_id]
            conn.execute(f"UPDATE users SET {set_clause} WHERE chat_id = ?", values)
        
        conn.commit()
        conn.close()
        return True
    except Exception as e:
        print(f"Kullanıcı güncellenirken hata: {e}")
        return False

def update_user_status(chat_id, status):
    try:
        conn = get_db_connection()
        conn.execute(
            "UPDATE users SET last_status = ?, last_status_time = CURRENT_TIMESTAMP WHERE chat_id = ?",
            (status, str(chat_id))
        )
        conn.commit()
        conn.close()
    except Exception as e:
        print(f"Durum güncellenirken hata: {e}")

def get_user_status(chat_id):
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT last_status, last_status_time FROM users WHERE chat_id = ?", (str(chat_id),))
        row = cursor.fetchone()
        conn.close()
        return dict(row) if row else None
    except Exception as e:
        print(f"Durum okunurken hata: {e}")
        return None

def get_system_setting(key):
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT value FROM system_state WHERE key = ?", (key,))
        row = cursor.fetchone()
        conn.close()
        return row['value'] if row else None
    except Exception as e:
        print(f"Sistem ayarı okunurken hata: {e}")
        return None

def set_system_setting(key, value):
    try:
        conn = get_db_connection()
        conn.execute('''
            INSERT INTO system_state (key, value) VALUES (?, ?)
            ON CONFLICT(key) DO UPDATE SET value = excluded.value
        ''', (key, value))
        conn.commit()
        conn.close()
        return True
    except Exception as e:
        print(f"Sistem ayarı kaydedilirken hata: {e}")
        return False

def load_previous_state(chat_id):
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT course_code, state_json FROM course_states WHERE chat_id = ?", (str(chat_id),))
        rows = cursor.fetchall()
        
        state = {}
        for row in rows:
            state[row['course_code']] = json.loads(row['state_json'])
            
        conn.close()
        return state
    except Exception as e:
        print(f"Eski veri yüklenirken hata oluştu: {e}")
        return {}

def save_current_state(chat_id, state):
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        chat_id = str(chat_id)
        
        for course_code, course_data in state.items():
            course_name = course_data.get("name", "Bilinmeyen Ders")
            state_json = json.dumps(course_data, ensure_ascii=True)
            
            cursor.execute('''
                INSERT INTO course_states (chat_id, course_code, course_name, state_json, last_updated)
                VALUES (?, ?, ?, ?, CURRENT_TIMESTAMP)
                ON CONFLICT(chat_id, course_code) DO UPDATE SET
                    course_name=excluded.course_name,
                    state_json=excluded.state_json,
                    last_updated=CURRENT_TIMESTAMP
            ''', (chat_id, course_code, course_name, state_json))
            
        conn.commit()
        conn.close()
        return True
    except Exception as e:
        print(f"Durum kaydedilirken hata oluştu: {e}")
        return False

def check_for_changes(chat_id, old_state, new_state):
    changes = []
    
    for course_code_raw, new_course_data in new_state.items():
        course_code = course_code_raw.strip()
        old_course_data = old_state.get(course_code)
        
        if not old_course_data:
            old_course_data = {
                "grades": {}, "announcements": [], "assignments": [], "contents": []
            }
            
        course_name = new_course_data.get("name", "Bilinmeyen Ders")
        
        for exam_type, grade in new_course_data.get("grades", {}).items():
            old_grade = old_course_data.get("grades", {}).get(exam_type)
            if old_grade != grade:
                import logging
                logging.info(f"USER[{chat_id}] NOT DEĞİŞİMİ TESPİTİ: {course_code} - {exam_type}: {old_grade} -> {grade}")
                avg = new_course_data.get("averages", {}).get(exam_type, "Bilinmiyor")
                changes.append({
                    "type": "grade",
                    "chat_id": chat_id,
                    "course_code": course_code,
                    "course_name": course_name,
                    "exam_type": exam_type,
                    "grade": grade,
                    "average": avg
                })
                
        old_announcements = old_course_data.get("announcements", [])
        old_ann_keys = set()
        for a in old_announcements:
            if isinstance(a, dict):
                old_ann_keys.add((a.get("date", "").strip(), a.get("title", "").strip()))
        
        for announcement in new_course_data.get("announcements", []):
            if not isinstance(announcement, dict):
                continue
            ann_key = (announcement.get("date", "").strip(), announcement.get("title", "").strip())
            if ann_key not in old_ann_keys and ann_key != ("", ""):
                import logging
                logging.info(f"USER[{chat_id}] YENİ DUYURU TESPİTİ: {ann_key}")
                changes.append({
                    "type": "announcement",
                    "chat_id": chat_id,
                    "course_code": course_code,
                    "course_name": course_name,
                    "announcement_data": announcement
                })
                
        old_assignment_titles = set()
        for a in old_course_data.get("assignments", []):
            if isinstance(a, dict):
                old_assignment_titles.add(a.get("title", "").strip())
        
        for assignment in new_course_data.get("assignments", []):
            if not isinstance(assignment, dict):
                continue
            title = assignment.get("title", "").strip()
            if title and title not in old_assignment_titles:
                import logging
                logging.info(f"USER[{chat_id}] YENİ ÖDEV TESPİTİ: {title}")
                changes.append({
                    "type": "assignment",
                    "chat_id": chat_id,
                    "course_code": course_code,
                    "course_name": course_name,
                    "title": title,
                    "due_date": assignment.get("due_date", "Belirtilmemiş")
                })
                
        old_contents = old_course_data.get("contents", [])
        old_content_keys = set()
        for c in old_contents:
            if isinstance(c, dict):
                old_content_keys.add((c.get("name", "").strip(), c.get("date", "").strip()))
        
        for content in new_course_data.get("contents", []):
            if not isinstance(content, dict):
                continue
            content_key = (content.get("name", "").strip(), content.get("date", "").strip())
            if content_key not in old_content_keys and content_key != ("", ""):
                import logging
                logging.info(f"USER[{chat_id}] YENİ İÇERİK TESPİTİ: {content_key}")
                changes.append({
                    "type": "content",
                    "chat_id": chat_id,
                    "course_code": course_code,
                    "course_name": course_name,
                    "content_data": content
                })
                
    return changes


