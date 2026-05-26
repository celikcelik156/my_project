-- Veritabaný Oluþturma
CREATE DATABASE AtikToplama;

USE AtikToplama;

-- 1. ADIM: BAÐIMSIZ TABLOLAR

-- Geri Dönüþüm Tesisleri
CREATE TABLE GeriDonusumTesisi (
    TesisID INT PRIMARY KEY IDENTITY(1,1),
    TesisAdi NVARCHAR(100) NOT NULL,
    Kapasite DECIMAL(18,2)
);

-- Atýk Türleri
CREATE TABLE AtikTurleri (
    AtikID INT PRIMARY KEY IDENTITY(1,1),
    AtikAdi NVARCHAR(50) NOT NULL,
    BirimFiyat DECIMAL(18,2)
);

-- Rota
CREATE TABLE Rota (
    RotaID INT PRIMARY KEY IDENTITY(1,1),
    RotaAdi NVARCHAR(100)
);

-- Araç Tipleri
CREATE TABLE AracTipi (
    AracTipiID INT PRIMARY KEY IDENTITY(1,1),
    TipAdi NVARCHAR(50)
);

-- Durumlar 
CREATE TABLE Durum (
    DurumID INT PRIMARY KEY IDENTITY(1,1),
    DurumAdi NVARCHAR(50)
);

-- Bakým Türleri
CREATE TABLE BakimTur (
    BakimTurID INT PRIMARY KEY IDENTITY(1,1),
    BakimAdi NVARCHAR(100),
    BakimFiyati DECIMAL(18,2)
);

-- Unvanlar
CREATE TABLE Unvan (
    UnvanID INT PRIMARY KEY IDENTITY(1,1),
    UnvanAdi NVARCHAR(50),
    MaasMin DECIMAL(18,2),
    MaasMax DECIMAL(18,2)
);

-- 2. ADIM: ÝLÝÞKÝLÝ TABLOLAR 

-- Araçlar Tablosu
CREATE TABLE Araclar (
    AracSasiID INT PRIMARY KEY IDENTITY(1,1),
    Marka NVARCHAR(50),
    Modeli NVARCHAR(50),
    Kapasite DECIMAL(18,2),
    Plaka NVARCHAR(20) UNIQUE,
    AracTipiID INT,
    RotaID INT,
    AracDurumID INT,
    
    FOREIGN KEY (AracTipiID) REFERENCES AracTipi(AracTipiID),
    FOREIGN KEY (RotaID) REFERENCES Rota(RotaID),
    FOREIGN KEY (AracDurumID) REFERENCES Durum(DurumID)
);

-- Atýk Ýþlem Tipi Tablosu
CREATE TABLE IslemTipleri (
    IslemTipID INT PRIMARY KEY IDENTITY(1,1),
    IslemAdi NVARCHAR(50) NOT NULL
);

-- Atýk Ýþlem Tablosu
CREATE TABLE AtikIslem (
    IslemID INT PRIMARY KEY IDENTITY(1,1),
    TesisID INT,
    IslemTipi NVARCHAR(10) NOT NULL,
    AtikID INT,
    Miktar DECIMAL(18,2),
    IslemTarihi DATETIME DEFAULT GETDATE(),
    ToplamTutar DECIMAL(18,2),

    FOREIGN KEY (IslemTipi) REFERENCES IslemTipleri(IslemTipID),
    FOREIGN KEY (TesisID) REFERENCES GeriDonusumTesisi(TesisID),
    FOREIGN KEY (AtikID) REFERENCES AtikTurleri(AtikID)
);

-- Bakýmlar Tablosu
CREATE TABLE Bakimlar (
    BakimID INT PRIMARY KEY IDENTITY(1,1),
    BakimTurID INT,
    AracSasiID INT,
    BakimTarihi DATETIME,
    
    FOREIGN KEY (BakimTurID) REFERENCES BakimTur(BakimTurID),
    FOREIGN KEY (AracSasiID) REFERENCES Araclar(AracSasiID)
);

-- Personel Tablosu
CREATE TABLE Personel (
    TC INT PRIMARY KEY IDENTITY(1,1),
    Ad NVARCHAR(50),
    Soyad NVARCHAR(50),
    Yas INT,
    DogumTarihi DATE,
    DogumYeri NVARCHAR(50),
    Telefon NVARCHAR(20),
    Email NVARCHAR(100),
    Sifre NVARCHAR(256),
    IsAdmin BIT DEFAULT 0,
    DepartmanYoneticisi BIT DEFAULT 0,
    Adres NVARCHAR(MAX),
    MedeniDurum NVARCHAR(20),
    CocukSayisi INT,
    UnvanID INT,
    Maas DECIMAL(18,2),
    AracSasiID INT DEFAULT NULL,
    
    FOREIGN KEY (UnvanID) REFERENCES Unvan(UnvanID),
    FOREIGN KEY (AracSasiID) REFERENCES Araclar(AracSasiID)
);



-- 1. ADIM: SABÝT VERÝLERÝN EKLENMESÝ

-- Geri Dönüþüm Tesisi
INSERT INTO GeriDonusumTesisi (TesisAdi, Kapasite) VALUES 
('Merkez', 50000);

-- Atýk Türleri ve Fiyatlarý
INSERT INTO AtikTurleri (AtikAdi, BirimFiyat) VALUES 
('Organik', 0.5),
('Plastik', 4.0),
('Metal', 6.0),
('Kaðýt', 2.0),
('Cam', 1.0);

-- Rotalar
INSERT INTO Rota (RotaAdi) VALUES 
('Merkez'), 
('Doðu'), 
('Batý'), 
('Kuzey'), 
('Güney');

-- Araç Tipleri
INSERT INTO AracTipi (TipAdi) VALUES 
('Kamyon'), 
('Otomobil');

-- Durumlar
INSERT INTO Durum (DurumAdi) VALUES 
('Aktif'), 
('Pasif'), 
('Hasarlý'), 
('Bakýmda');

-- Bakým Türleri
INSERT INTO BakimTur (BakimAdi, BakimFiyati) VALUES 
('Standart', 5000),
('Aðýr', 12000);

-- Unvanlar ve Maaþ Aralýklarý
INSERT INTO Unvan (UnvanAdi, MaasMin, MaasMax) VALUES 
('Þöför', 12000, 20000),                -- ID: 1
('Toplama Personeli', 10000, 17000),    -- ID: 2
('Geri Dönüþüm Operatörü', 11000, 18000),-- ID: 3
('Bakým Teknisyeni', 13000, 22000),     -- ID: 4
('Muhasebe', 14000, 23000),             -- ID: 5
('Departman Yöneticisi', 18000, 30000); -- ID: 6

-- Eðer tablo boþsa bu kodla içeriyi doldurun
INSERT INTO IslemTipleri (IslemAdi) VALUES ('Atýk Giriþi');
INSERT INTO IslemTipleri (IslemAdi) VALUES ('Geri Dönüþüm Çýkýþý');

-- 2. ADIM: ÝLÝÞKÝLÝ VERÝLERÝN EKLENMESÝ (Örnek Senaryolar)

-- Araçlar
-- Not: AracTipiID: 1=Kamyon, 2=Otomobil
-- RotaID: 1=Merkez, 2=Doðu...
-- DurumID: 1=Aktif, 4=Bakýmda...
INSERT INTO Araclar (Marka, Modeli, Kapasite, Plaka, AracTipiID, RotaID, AracDurumID) VALUES 
('Ford', 'Cargo', 15000, '27 GA 1001', 1, 2, 1),  -- Doðu Rotasýnda Aktif Kamyon
('Mercedes', 'Axor', 18000, '27 GA 1002', 1, 3, 1), -- Batý Rotasýnda Aktif Kamyon
('Isuzu', 'NPR', 8000, '27 GA 1003', 1, 4, 4),    -- Kuzey Rotasýnda Bakýmda Kamyon
('Fiat', 'Egea', 0, '27 GA 9001', 2, 1, 1),       -- Merkezde Hizmet Aracý (Otomobil)
('BMC', 'Pro', 20000, '27 GA 1005', 1, 5, 3),     -- Güney Rotasýnda Hasarlý Kamyon
('Man', 'TGS', 22000, '27 GA 2055', 1, 4, 1),      -- Kuzey Rotasý, Büyük Kamyon (Aktif)
('Otokar', 'Atlas', 12000, '27 GA 2056', 1, 1, 2), -- Merkez, Yedek Kamyon (Pasif)
('Renault', 'Megane', 0, '27 GA 9050', 2, 1, 1),   -- Yönetici Aracý (Otomobil - Aktif)
('Ford', 'Transit', 5000, '27 GA 3020', 1, 5, 4),  -- Güney Rotasý, Küçük Kamyonet (Bakýmda)
('Mercedes', 'Arocs', 25000, '27 GA 4001', 1, 3, 1), -- Batý Rotasý, Çöp Kamyonu (Aktif)
('Volvo', 'FMX', 24000, '27 GA 4002', 1, 2, 3);    -- Doðu Rotasý, Kamyon (Hasarlý - Kaza yapmýþ)

-- Personeller 
INSERT INTO Personel (Ad, Soyad, Yas, DogumTarihi, Telefon, Email, Sifre, UnvanID, Maas, IsAdmin, DepartmanYoneticisi, AracSasiID, Adres) VALUES 
-- Yönetici (ID:6)
('Ahmet', 'Yýlmaz', 45, '1980-05-15', '05321000001', 'admin', 'admin', 6, 28000, 1, 1, NULL, 'Gaziantep Þehitkamil'),
-- Þoför (ID:1) - Araca baðlý (AracSasiID: 1)
('Mehmet', 'Demir', 35, '1990-02-20', '05321000002', 'mehmet@sirket.com', '1234', 1, 15000, 0, 0, 1, 'Gaziantep Þahinbey'),
-- Þoför (ID:1) - Araca baðlý (AracSasiID: 2)
('Veli', 'Kaya', 32, '1993-11-10', '05321000003', 'veli@sirket.com', '1234', 1, 14500, 0, 0, 2, 'Gaziantep Karataþ'),
-- Toplama Personeli (ID:2)
('Ayþe', 'Çelik', 28, '1997-08-30', '05321000004', 'ayse@sirket.com', '1234', 2, 11000, 0, 0, NULL, 'Gaziantep Ýbrahimli'),
-- Muhasebe (ID:5)
('Fatma', 'Sönmez', 30, '1995-04-12', '05321000005', 'fatma@sirket.com', '1234', 5, 16000, 0, 0, NULL, 'Gaziantep Üniversite'),
-- Bakým Teknisyeni (ID:4)
('Hasan', 'Usta', 50, '1975-01-01', '05321000006', 'hasan@sirket.com', '1234', 4, 18000, 0, 0, NULL, 'Gaziantep Sanayi'),
-- GERÝ DÖNÜÞÜM OPERATÖRLERÝ (Tesis içinde çalýþanlar, araçlarý yok)
('Murat', 'Koç', 29, '1996-03-12', '05441112233', 'murat@sirket.com', '1234', 3, 12500, 0, 0, NULL, 'Gaziantep Gazikent'),
('Sinan', 'Yýldýz', 40, '1985-07-22', '05441112234', 'sinan@sirket.com', '1234', 3, 13000, 0, 0, NULL, 'Gaziantep Düztepe'),
('Elif', 'Polat', 26, '1999-09-09', '05441112235', 'elif@sirket.com', '1234', 3, 11500, 0, 0, NULL, 'Gaziantep Karataþ'),

-- TOPLAMA PERSONELLERÝ (Kamyon arkasýnda çalýþanlar)
('Kemal', 'Sunal', 33, '1992-01-01', '05334445566', 'kemal@sirket.com', '1234', 2, 10500, 0, 0, NULL, 'Gaziantep Þehitkamil'),
('Halil', 'Sezai', 38, '1987-05-14', '05334445567', 'halil@sirket.com', '1234', 2, 10800, 0, 0, NULL, 'Gaziantep Þahinbey'),

-- ÞOFÖRLER (Yeni eklenen araçlara atandýlar)
-- Araca atanan personel (Tahmini AracID: 6 -> Man TGS)
('Burak', 'Özçivit', 36, '1989-12-20', '05556667788', 'burak@sirket.com', '1234', 1, 16000, 0, 0, 6, 'Gaziantep Ýbrahimli'),
-- Araca atanan personel (Tahmini AracID: 10 -> Mercedes Arocs)
('Kenan', 'Ýmir', 42, '1983-04-10', '05556667789', 'kenan@sirket.com', '1234', 1, 17500, 0, 0, 10, 'Gaziantep Binevler'),

-- MUHASEBE VE ÝDARÝ ÝÞLER
('Selin', 'Gür', 27, '1998-02-28', '05051112233', 'selin@sirket.com', '1234', 5, 15500, 0, 0, NULL, 'Gaziantep Üniversite Bulvarý'),
('Cem', 'Yýlmaz', 48, '1977-10-10', '05051112234', 'cem@sirket.com', 'admin123', 6, 29000, 1, 1, 8, 'Gaziantep Prime Mall Civarý'); -- Yönetici ve Renault Megane (ID:8) kullanýyor

-- Atýk Ýþlemleri
-- Örnek: Plastik (ID:2, Fiyat:4) -> 100 kg * 4 = 400 TL
INSERT INTO AtikIslem (TesisID, AtikID, Miktar, ToplamTutar) VALUES 
(1, 2, 100, 400),  -- 100 kg Plastik
(1, 3, 50, 300),   -- 50 kg Metal (6 TL'den)
(1, 4, 200, 400),  -- 200 kg Kaðýt (2 TL'den)
(1, 1, 1000, 500); -- 1000 kg Organik (0.5 TL'den)

-- Bakýmlar
-- Araç 3 (Bakýmdaki Kamyon) için Aðýr Bakým
INSERT INTO Bakimlar (BakimTurID, AracSasiID, BakimTarihi) VALUES 
(2, 3, '2025-12-20'),
-- Araç 1 için Standart Bakým (Geçmiþ tarihli)
(1, 1, '2025-11-15');


select * from Araclar

select * from Bakimlar

select * from personel
