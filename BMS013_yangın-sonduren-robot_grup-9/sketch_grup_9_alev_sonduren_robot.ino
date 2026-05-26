#include <Servo.h>

// ==========================================
// MOTOR SÜRÜCÜ PİNLERİ (L298N)
// ==========================================
#define enA 11 
#define in1 9  
#define in2 8  
#define in3 7  
#define in4 6  
#define enB 5  

#define ir_R A0
#define ir_F A1
#define ir_L A2

#define servoPin A4
#define pumpPin A5

Servo myServo;

// HIZ AYARLARI
int Speed = 200;      // Hedefe kilitlendikten sonra üstüne atılma hızı

int s_right, s_front, s_left;

// ==========================================
// EŞİK VE TOLERANS DEĞERLERİ
// ==========================================
const int FIRE_DISTANCE_THRESHOLD = 40;  // 20'nin altına inince DUR ve SIK
const int FIRE_DETECT_THRESHOLD = 500;   // Ateşi görme sınırı
const int TOLERANCE = 75; // Motor titremesini önleyen tolerans payı (Gerekirse artır)

void setup() {
  Serial.begin(9600);

  pinMode(ir_R, INPUT);
  pinMode(ir_F, INPUT);
  pinMode(ir_L, INPUT);

  pinMode(enA, OUTPUT);
  pinMode(in1, OUTPUT);
  pinMode(in2, OUTPUT);
  pinMode(in3, OUTPUT);
  pinMode(in4, OUTPUT);
  pinMode(enB, OUTPUT);
  pinMode(pumpPin, OUTPUT);

  myServo.attach(servoPin);

  Serial.println("Avci Modu (Kiyaslama Mantigi) Hazir...");
  myServo.write(90); 
  digitalWrite(pumpPin, LOW);
  Stop();
  delay(1000); 
}

void loop() {
  s_right = analogRead(ir_R);
  s_front = analogRead(ir_F);
  s_left = analogRead(ir_L);

  // =====================================================================
  // SERİ MONİTÖR EKRANINA YAZDIRMA KISMI 
  // =====================================================================
  Serial.print("S: "); Serial.print(s_right);
  Serial.print(" | O: "); Serial.print(s_front);
  Serial.print(" | L: "); Serial.println(s_left);

  // =====================================================================
  // DURUM 1: HEDEF TAM MERKEZDE VE DİBİMİZDE (20'DEN KÜÇÜK)
  // =====================================================================
  if (s_front < FIRE_DISTANCE_THRESHOLD) {
    Stop();              
    delay(200);          
    digitalWrite(pumpPin, HIGH); 
    sweepServo(70, 110); 
    delay(100);
    }
  
  // =====================================================================
  // DURUM 2: KALİBRASYON (ORTALAMA) VE YAKLAŞMA
  // (Herhangi bir sensör ateşi gördüyse)
  // =====================================================================
  else if (s_front < FIRE_DETECT_THRESHOLD || s_right < FIRE_DETECT_THRESHOLD || s_left < FIRE_DETECT_THRESHOLD) {
    digitalWrite(pumpPin, LOW); 

    // DOĞRUDAN KIYASLAMA MANTIĞI:
    // Eğer ön sensör, sağ ve sol sensörden tolerans payı kadar daha iyi veya eşitse:
    if (s_front <= (s_right + TOLERANCE) && s_front <= (s_left + TOLERANCE)) {
      myServo.write(90); 
      forword(); 
      delay(100);
    }
    // Eğer SAĞ sensör en düşük sayıya (en güçlü ateşe) sahipse
    else if (s_right < s_left && s_right < s_front) {
      myServo.write(60);  
      turnRight(); 
      delay(100);
    }
    // Eğer SOL sensör en düşük sayıya (en güçlü ateşe) sahipse
    else if (s_left < s_right && s_left < s_front) {
      myServo.write(120); 
      turnLeft(); 
      delay(100);
    }
  }
  
  // =====================================================================
  // DURUM 3: ATEŞ YOK
  // =====================================================================
  else {
    digitalWrite(pumpPin, LOW);
    Stop();
    myServo.write(90); 
    delay(100);
  }

  delay(20); 
}

void sweepServo(int startAngle, int endAngle) {
  for (int i = startAngle; i <= endAngle; i += 4) {
    myServo.write(i);
    delay(25);
  }
  for (int i = endAngle; i >= startAngle; i -= 4) {
    myServo.write(i);
    delay(25);
  }
}

// =======================================================
// HAREKET FONKSİYONLARI
// =======================================================

void forword() {
  analogWrite(enA, Speed); analogWrite(enB, Speed); 
  digitalWrite(in1, LOW); digitalWrite(in2, HIGH);  // Sağ İleri
  digitalWrite(in3, HIGH);  digitalWrite(in4, LOW); // Sol İleri
}

void turnRight() {
  analogWrite(enA, Speed); analogWrite(enB, Speed);
  digitalWrite(in1, LOW); digitalWrite(in2, HIGH);  
  digitalWrite(in3, LOW); digitalWrite(in4, HIGH);  
}

void turnLeft() {
  analogWrite(enA, Speed); analogWrite(enB, Speed);
  digitalWrite(in1, HIGH);  digitalWrite(in2, LOW); 
  digitalWrite(in3, HIGH);  digitalWrite(in4, LOW); 
}

void Stop() {
  analogWrite(enA, 0); analogWrite(enB, 0); 
  digitalWrite(in1, LOW); digitalWrite(in2, LOW);
  digitalWrite(in3, LOW); digitalWrite(in4, LOW);
}

