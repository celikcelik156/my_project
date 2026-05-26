import sqlite3
import os
import logging
from datetime import datetime

logger = logging.getLogger(__name__)

DB_PATH = os.path.join(os.path.dirname(os.path.dirname(__file__)), "data", "notifications.db")


def init_db():
    os.makedirs(os.path.dirname(DB_PATH), exist_ok=True)
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()

    cursor.execute("""
        CREATE TABLE IF NOT EXISTS grades (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            course_name TEXT NOT NULL,
            exam_name   TEXT NOT NULL,
            score       TEXT NOT NULL,
            letter      TEXT,
            first_seen  TEXT NOT NULL,
            hash_key    TEXT UNIQUE NOT NULL
        )
    """)

    cursor.execute("""
        CREATE TABLE IF NOT EXISTS assignments (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            course_name TEXT NOT NULL,
            assignment_title TEXT NOT NULL,
            due_date TEXT,
            description TEXT,
            first_seen TEXT NOT NULL,
            hash_key TEXT UNIQUE NOT NULL
        )
    """)

    cursor.execute("""
        CREATE TABLE IF NOT EXISTS announcements (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            course_name TEXT NOT NULL,
            title TEXT NOT NULL,
            content TEXT,
            first_seen TEXT NOT NULL,
            hash_key TEXT UNIQUE NOT NULL
        )
    """)

    cursor.execute("""
        CREATE TABLE IF NOT EXISTS contents (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            course_name TEXT NOT NULL,
            name        TEXT NOT NULL,
            type        TEXT,
            date        TEXT,
            description TEXT,
            first_seen  TEXT NOT NULL,
            hash_key    TEXT UNIQUE NOT NULL
        )
    """)

    cursor.execute("""
        CREATE TABLE IF NOT EXISTS system_state (
            key TEXT PRIMARY KEY,
            value TEXT
        )
    """)

    conn.commit()
    conn.close()
    logger.info("Veritabanı başlatıldı: %s", DB_PATH)


def get_system_setting(key: str) -> str | None:
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.execute("SELECT value FROM system_state WHERE key = ?", (key,))
    result = cursor.fetchone()
    conn.close()
    return result[0] if result else None


def set_system_setting(key: str, value: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    try:
        cursor.execute(
            "INSERT INTO system_state (key, value) VALUES (?, ?) "
            "ON CONFLICT(key) DO UPDATE SET value = excluded.value",
            (key, value)
        )
        conn.commit()
    except Exception as e:
        logger.error("Ayar kaydetme hatası: %s", e)
    finally:
        conn.close()


def is_new_item(table: str, hash_key: str) -> bool:
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.execute(f"SELECT 1 FROM {table} WHERE hash_key = ?", (hash_key,))
    result = cursor.fetchone()
    conn.close()
    return result is None


def save_grade(course_name: str, exam_name: str, score: str, letter: str, hash_key: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    try:
        cursor.execute(
            "INSERT OR IGNORE INTO grades (course_name, exam_name, score, letter, first_seen, hash_key) "
            "VALUES (?, ?, ?, ?, ?, ?)",
            (course_name, exam_name, score, letter, datetime.now().isoformat(), hash_key)
        )
        conn.commit()
        logger.info("Not kaydedildi: %s - %s -> %s", course_name, exam_name, score)
    except Exception as e:
        logger.error("Not kaydetme hatası: %s", e)
    finally:
        conn.close()


def save_assignment(course_name: str, title: str, due_date: str, description: str, hash_key: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    try:
        cursor.execute(
            "INSERT OR IGNORE INTO assignments (course_name, assignment_title, due_date, description, first_seen, hash_key) VALUES (?, ?, ?, ?, ?, ?)",
            (course_name, title, due_date, description, datetime.now().isoformat(), hash_key)
        )
        conn.commit()
        logger.info("Ödev kaydedildi: %s - %s", course_name, title)
    except Exception as e:
        logger.error("Ödev kaydetme hatası: %s", e)
    finally:
        conn.close()


def save_announcement(course_name: str, title: str, content: str, hash_key: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    try:
        cursor.execute(
            "INSERT OR IGNORE INTO announcements (course_name, title, content, first_seen, hash_key) VALUES (?, ?, ?, ?, ?)",
            (course_name, title, content, datetime.now().isoformat(), hash_key)
        )
        conn.commit()
        logger.info("Duyuru kaydedildi: %s - %s", course_name, title)
    except Exception as e:
        logger.error("Duyuru kaydetme hatası: %s", e)
    finally:
        conn.close()

def save_content(course_name: str, name: str, type: str, date: str, description: str, hash_key: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    try:
        cursor.execute(
            "INSERT OR IGNORE INTO contents (course_name, name, type, date, description, first_seen, hash_key) VALUES (?, ?, ?, ?, ?, ?, ?)",
            (course_name, name, type, date, description, datetime.now().isoformat(), hash_key)
        )
        conn.commit()
        logger.info("İçerik kaydedildi: %s - %s", course_name, name)
    except Exception as e:
        logger.error("İçerik kaydetme hatası: %s", e)
    finally:
        conn.close()

def get_existing_grade(course_name: str, exam_name: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.execute("SELECT score, letter FROM grades WHERE course_name = ? AND exam_name = ? ORDER BY id DESC", (course_name, exam_name))
    result = cursor.fetchone()
    conn.close()
    return result

def get_existing_assignment(course_name: str, title: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.execute("SELECT due_date, description FROM assignments WHERE course_name = ? AND assignment_title = ? ORDER BY id DESC", (course_name, title))
    result = cursor.fetchone()
    conn.close()
    return result

def get_existing_announcement(course_name: str, title: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.execute("SELECT content FROM announcements WHERE course_name = ? AND title = ? ORDER BY id DESC", (course_name, title))
    result = cursor.fetchone()
    conn.close()
    return result

def get_existing_content(course_name: str, name: str):
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.execute("SELECT type, date, description FROM contents WHERE course_name = ? AND name = ? ORDER BY id DESC", (course_name, name))
    result = cursor.fetchone()
    conn.close()
    return result
