import sqlite3
import os

DATABASE_PATH = os.path.join(os.path.dirname(__file__), "ubys_data.db")

def migrate():
    conn = sqlite3.connect(DATABASE_PATH)
    cursor = conn.cursor()
    
    cursor.execute("PRAGMA table_info(course_states)")
    columns = [row[1] for row in cursor.fetchall()]
    
    if 'chat_id' not in columns:
        print("Eski veritabanı yapısı tespit edildi. Migration başlatılıyor...")
        
        cursor.execute("ALTER TABLE course_states RENAME TO course_states_old")
        
        cursor.execute('''
            CREATE TABLE course_states (
                chat_id TEXT,
                course_code TEXT,
                course_name TEXT,
                state_json TEXT,
                last_updated DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (chat_id, course_code)
            )
        ''')
        
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS users (
                chat_id TEXT PRIMARY KEY,
                username TEXT,
                password TEXT,
                registration_state TEXT DEFAULT 'COMPLETED',
                last_scrape DATETIME,
                is_active INTEGER DEFAULT 1
            )
        ''')
        
        print("Migration tamamlandı. Eski tablo 'course_states_old' olarak saklandı.")
    else:
        print("Veritabanı yapısı güncel.")
    
    conn.commit()
    conn.close()

if __name__ == "__main__":
    migrate()
