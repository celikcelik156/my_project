import os

UBYS_USERNAME = ""
UBYS_PASSWORD = ""

TELEGRAM_BOT_TOKEN = ""
TELEGRAM_CHAT_ID = ""

UBYS_LOGIN_URL = "https://ubys.gibtu.edu.tr/"
UBYS_STUDENT_HOME_URL = "https://ubys.gibtu.edu.tr/AIS/Student/Home/Index"

HEADLESS_MODE = True 
DATABASE_PATH = os.path.join(os.path.dirname(__file__), "ubys_data.db")
LOG_FILE_PATH = os.path.join(os.path.dirname(__file__), "app.log")

SLEEP_SHORT = 2
SLEEP_MEDIUM = 5
SLEEP_LONG = 10
