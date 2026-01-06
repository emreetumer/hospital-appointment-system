# ?? Hýzlý Baþlangýç Kýlavuzu

Bu kýlavuz projeyi hýzlýca çalýþtýrmanýz için adým adým talimatlar içerir.

## ? Hýzlý Kurulum (5 Dakika)

### 1?? SQL Server Kurulumu
Eðer SQL Server kurulu deðilse:
1. [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) indir
2. "Basic" installation seç
3. Varsayýlan ayarlarla kur

### 2?? Veritabanýný Oluþtur

#### Yöntem 1: SSMS ile (Önerilen)
```bash
1. SQL Server Management Studio (SSMS) aç
2. Server name: localhost\SQLEXPRESS (veya kendi instance'ýnýz)
3. Authentication: Windows Authentication
4. Connect
5. Database klasöründeki scriptleri sýrayla çalýþtýr:
   - 01_CreateDatabase.sql
   - 02_CreateTables.sql
   - 03_CreateIndexes.sql
   - 04_InsertSampleData.sql
```

#### Yöntem 2: Command Line ile
```bash
sqlcmd -S localhost\SQLEXPRESS -i Database/01_CreateDatabase.sql
sqlcmd -S localhost\SQLEXPRESS -i Database/02_CreateTables.sql
sqlcmd -S localhost\SQLEXPRESS -i Database/03_CreateIndexes.sql
sqlcmd -S localhost\SQLEXPRESS -i Database/04_InsertSampleData.sql
```

### 3?? Connection String Ayarla

`AppointmentSystem.API/appsettings.json` dosyasýný aç ve güncelle:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HospitalAppointmentDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Önemli:** SQL Server instance adýnýzý kontrol edin!

Bulmak için:
1. SSMS'i aç
2. Connect ederken kullandýðýnýz server name'i kopyala
3. Connection string'de kullan

### 4?? Projeyi Çalýþtýr

```bash
cd AppointmentSystem.API
dotnet restore
dotnet run
```

veya Visual Studio ile:
1. Solution'ý aç
2. F5'e bas (veya Run butonu)

### 5?? Swagger UI'ý Aç

Tarayýcýda otomatik olarak açýlacak:
```
https://localhost:7xxx
```

## ?? Ýlk Testinizi Yapýn

### 1. Login Endpoint'ini Test Et

1. Swagger UI'da `/api/auth/login` bul
2. "Try it out" buton
3. Aþaðýdaki bilgileri gir:

```json
{
  "email": "patient1@email.com",
  "password": "Patient123!"
}
```

4. "Execute" butonuna bas
5. Response'da token'ý kopyala

### 2. Token'ý Authorize Et

1. Sayfanýn üstündeki "Authorize" butonuna týkla
2. `Bearer {token}` formatýnda yapýþtýr
   - Örnek: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. "Authorize" butonuna týkla
4. Kapat

### 3. Doktorlarý Listele

1. `/api/doctors` GET endpoint'ini bul
2. "Try it out" ve "Execute"
3. Aktif doktorlarý görmelisiniz

## ?? Test Kullanýcýlarý

| Rol | Email | Þifre | Açýklama |
|-----|-------|-------|----------|
| Patient | patient1@email.com | Patient123! | Örnek hasta |
| Doctor | dr.mehmet@hospital.com | Doctor123! | Kardiyoloji uzmaný |
| Doctor | dr.ayse@hospital.com | Doctor123! | Nöroloji uzmaný |
| Admin | admin@hospital.com | Admin123! | Sistem yöneticisi |

## ?? Örnek API Ýstekleri

### Yeni Hasta Kaydý
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "yeni@email.com",
  "password": "Sifre123!",
  "firstName": "Ahmet",
  "lastName": "Yýlmaz",
  "phoneNumber": "5551234567",
  "dateOfBirth": "1990-05-15",
  "gender": "Male",
  "address": "Ýstanbul",
  "bloodType": "A+"
}
```

### Randevu Oluþtur
```http
POST /api/appointments
Authorization: Bearer {token}
Content-Type: application/json

{
  "patientId": 1,
  "doctorId": 1,
  "appointmentDate": "2024-01-25",
  "appointmentTime": "10:00:00",
  "notes": "Kontrol muayenesi"
}
```

### Hasta Randevularýný Görüntüle
```http
GET /api/appointments/patient/1
Authorization: Bearer {token}
```

## ? Yaygýn Sorunlar ve Çözümleri

### Sorun 1: SQL Server'a baðlanamýyorum
**Çözüm:**
1. SQL Server Service'in çalýþtýðýný kontrol et:
   - Windows + R ? `services.msc`
   - "SQL Server (SQLEXPRESS)" servisini bul
   - Çalýþmýyorsa "Start" et

2. TCP/IP protokolünü aktif et:
   - SQL Server Configuration Manager aç
   - SQL Server Network Configuration ? Protocols
   - TCP/IP'yi Enable et
   - SQL Server'ý restart et

### Sorun 2: Connection string hatasý
**Çözüm:**
Connection string formatýný kontrol et:
```
Server=.\SQLEXPRESS;Database=HospitalAppointmentDB;Trusted_Connection=True;TrustServerCertificate=True;
```

Alternatif formatlar:
- `localhost\SQLEXPRESS`
- `(localdb)\MSSQLLocalDB` (LocalDB için)
- `.` (default instance için)

### Sorun 3: Database bulunamýyor
**Çözüm:**
1. SSMS'de veritabanýnýn oluþup oluþmadýðýný kontrol et
2. Script'leri tekrar çalýþtýr
3. Veritabaný adýný kontrol et: `HospitalAppointmentDB`

### Sorun 4: JWT token çalýþmýyor
**Çözüm:**
1. Token'ýn süresi dolmuþ olabilir (1440 dakika = 1 gün)
2. Yeniden login ol
3. Swagger'da "Authorize" yaparken `Bearer ` prefix'ini unutma

### Sorun 5: Build hatasý
**Çözüm:**
```bash
dotnet clean
dotnet restore
dotnet build
```

## ?? Postman Collection

Postman kullanmayý tercih ediyorsanýz:

1. Postman'i aç
2. New Collection oluþtur: "Hospital Appointment System"
3. Environment variable ekle:
   - Variable: `baseUrl`
   - Initial Value: `https://localhost:7xxx`
   - Variable: `token`
   - Initial Value: (login'den sonra dolacak)

4. Login request oluþtur:
```
POST {{baseUrl}}/api/auth/login
Body: raw JSON
{
  "email": "patient1@email.com",
  "password": "Patient123!"
}
```

5. Response'dan token'ý al ve Tests tab'ine ekle:
```javascript
pm.environment.set("token", pm.response.json().data.token);
```

6. Diðer request'lerde Authorization kullan:
```
Type: Bearer Token
Token: {{token}}
```

## ?? Sonraki Adýmlar

Projeyi çalýþtýrdýktan sonra:

1. ? README.md dosyasýný oku
2. ? Kod yapýsýný incele (Clean Architecture)
3. ? Dapper kullanýmýný öðren (Repository'lere bak)
4. ? CQRS pattern'i anla (Features klasörüne bak)
5. ? Kendi feature'larýný ekle

## ?? Öðrenme Yolu

### Baþlangýç Seviyesi
1. API'yi Swagger'dan test et
2. Veritabaný tablolarýný SSMS'de incele
3. Entity'leri (Domain/Entities) oku
4. Controller'larý incele

### Orta Seviye
1. Repository Pattern'i anla
2. MediatR Handler'larý incele
3. FluentValidation kullanýmýný öðren
4. JWT Authentication flow'unu anla

### Ýleri Seviye
1. Clean Architecture prensiplerine odaklan
2. CQRS pattern'i derinlemesine öðren
3. Dapper ile complex query'ler yaz
4. Kendi feature'larýný ekle

## ?? Yardým

Sorun mu yaþýyorsun?

1. README.md dosyasýný dikkatlice oku
2. Error mesajlarýný kontrol et
3. Serilog log dosyalarýna bak (`logs/` klasörü)
4. Google'da ara: "Dapper + [hata mesajý]"
5. Stack Overflow'da sor

## ? Checklist

Kurulumu doðrula:

- [ ] SQL Server kurulu ve çalýþýyor
- [ ] Veritabaný oluþturuldu (HospitalAppointmentDB)
- [ ] Tablolar oluþturuldu (8 tablo)
- [ ] Örnek veriler eklendi
- [ ] Connection string doðru
- [ ] Proje build oluyor
- [ ] Swagger UI açýlýyor
- [ ] Login çalýþýyor ve token alýnabiliyor
- [ ] Token ile diðer endpoint'ler çalýþýyor

Tümü ? ise hazýrsýnýz! ??

---

**Ýyi çalýþmalar! Sorularýnýz için README.md dosyasýna bakýn.**
