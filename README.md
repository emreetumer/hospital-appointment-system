<div align="center">

# 🏥 Hospital Appointment System

### Modern Hastane Randevu Yönetim Sistemi

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

*Clean Architecture prensipleri ve modern .NET teknolojileriyle geliştirilmiş, ölçeklenebilir bir hastane randevu yönetim sistemi.*

[Özellikler](#-temel-özellikler) • [Teknolojiler](#-teknoloji-stack) • [Mimari](#-mimari-yapı) • [Kurulum](#-kurulum)

</div>

---

## 📋 İçindekiler

- [Temel Özellikler](#-temel-özellikler)
- [Teknoloji Stack](#-teknoloji-stack)
- [Mimari Yapı](#-mimari-yapı)
- [Proje Yapısı](#-proje-yapısı)
- [Kurulum](#-kurulum)
- [Veritabanı Şeması](#-veritabanı-şeması)
- [API Kullanımı](#-api-kullanımı)
- [Test](#-test)
- [Katkıda Bulunma](#-katkıda-bulunma)

## ✨ Temel Özellikler

### 🔐 Kimlik Doğrulama & Yetkilendirme
- JWT (JSON Web Token) tabanlı güvenli kimlik doğrulama
- Rol bazlı erişim kontrolü (Admin, Doctor, Patient)
- BCrypt ile şifrelenmiş kullanıcı verileri
- Token yenileme mekanizması

### 📅 Randevu Yönetimi
- Kolay randevu oluşturma ve yönetim
- Doktor müsaitlik kontrolü
- Randevu durumu takibi (Beklemede, Onaylandı, İptal, Tamamlandı, Gelmedi)
- Hasta ve doktor bazlı randevu listeleme
- Çakışma kontrolü ve validasyon

### 👨‍⚕️ Doktor & Departman Yönetimi
- Departman bazlı doktor organizasyonu
- Doktor profil ve deneyim bilgileri
- Çalışma saatleri planlaması
- Aktif/pasif doktor durumu yönetimi

### 🏥 Hasta Yönetimi
- Kapsamlı hasta profili
- Tıbbi geçmiş ve alerji kayıtları
- Acil durum iletişim bilgileri
- Kan grubu ve demografik bilgiler

## 🛠 Teknoloji Stack

### Backend Framework
- **.NET 8** - En güncel .NET teknolojisi ile yüksek performans
- **C# 12** - Modern dil özellikleri
- **ASP.NET Core Web API** - RESTful API geliştirme

### Veritabanı & ORM
- **Microsoft SQL Server** - Güvenilir ilişkisel veritabanı
- **Dapper** - Hafif ve performanslı Micro-ORM
- **Database First** yaklaşımı

### Mimari Desenler & Kütüphaneler
- **Clean Architecture** - Katmanlı ve sürdürülebilir mimari
- **CQRS Pattern** - MediatR ile Command-Query ayrımı
- **Repository Pattern** - Veri erişim soyutlaması
- **FluentValidation** - Güçlü ve okunabilir validasyon
- **JWT Authentication** - Güvenli token tabanlı kimlik doğrulama

### Loglama & Dokümantasyon
- **Serilog** - Yapılandırılmış loglama
- **Swagger/OpenAPI** - Otomatik API dokümantasyonu
- **File & Console Logging** - Çoklu log hedefleri

### Test
- **xUnit** - Modern test framework
- **Moq** - Mocking kütüphanesi
- **%100 Unit Test Coverage** - Kapsamlı test senaryoları

## 🏗 Mimari Yapı

Proje **Clean Architecture** prensiplerine göre 4 katmandan oluşur:

```
📦 AppointmentSystem/
├── 🎯 AppointmentSystem.Domain/          # Entity & Business Rules
│   ├── Entities/                          # Domain entities
│   ├── Enums/                             # Business enumerations
│   └── Common/                            # Base classes & Result pattern
│
├── 💼 AppointmentSystem.Application/     # Use Cases & Business Logic
│   ├── Features/                          # CQRS Commands & Queries
│   │   ├── Auth/                          # Authentication features
│   │   ├── Appointments/                  # Appointment management
│   │   ├── Doctors/                       # Doctor operations
│   │   └── Departments/                   # Department operations
│   ├── Contracts/                         # Interfaces
│   │   ├── Repositories/                  # Repository contracts
│   │   └── Services/                      # Service contracts
│   └── DependencyInjection.cs
│
├── 🔧 AppointmentSystem.Infrastructure/  # External Concerns
│   ├── Data/                              # Dapper context & Scripts
│   ├── Repositories/                      # Repository implementations
│   ├── Services/                          # Service implementations
│   └── DependencyInjection.cs
│
└── 🌐 AppointmentSystem.API/             # Presentation Layer
    ├── Controllers/                       # API endpoints
    ├── Middleware/                        # Global exception handling
    └── Program.cs                         # Application startup
```

### 📊 Dependency Flow
```
API → Infrastructure → Application → Domain
```

**Temel Prensipler:**
- Domain katmanı hiçbir katmana bağımlı değil
- Application katmanı sadece Domain'e bağımlı
- Infrastructure ve API dış dünyayı yönetir
- Dependency Injection ile gevşek bağlı (loosely coupled) yapı

## 📂 Proje Yapısı

<details>
<summary>Detaylı klasör yapısını görmek için tıklayın</summary>

```
AppointmentSystem/
│
├── 📁 AppointmentSystem.Domain/
│   ├── Common/
│   │   ├── BaseEntity.cs
│   │   └── Result.cs
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Patient.cs
│   │   ├── Doctor.cs
│   │   ├── Department.cs
│   │   ├── Appointment.cs
│   │   ├── DoctorSchedule.cs
│   │   └── MedicalRecord.cs
│   └── Enums/
│       ├── UserRoles.cs
│       └── AppointmentStatus.cs
│
├── 📁 AppointmentSystem.Application/
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   └── Validators/
│   │   ├── Appointments/
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   └── Validators/
│   │   ├── Doctors/
│   │   │   └── Queries/
│   │   └── Departments/
│   │       └── Queries/
│   └── Contracts/
│       ├── Repositories/
│       └── Services/
│
├── 📁 AppointmentSystem.Infrastructure/
│   ├── Data/
│   │   ├── DapperContext.cs
│   │   └── Scripts/
│   ├── Repositories/
│   │   └── [Repository Implementations]
│   └── Services/
│       ├── JwtTokenService.cs
│       └── PasswordHashService.cs
│
├── 📁 AppointmentSystem.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── AppointmentsController.cs
│   │   ├── DoctorsController.cs
│   │   └── DepartmentsController.cs
│   ├── Middleware/
│   │   └── GlobalExceptionHandlerMiddleware.cs
│   └── Program.cs
│
├── 📁 AppointmentSystem.Tests/
│   ├── Application/
│   ├── Domain/
│   └── Infrastructure/
│
└── 📁 Database/
    ├── 01_CreateDatabase.sql
    ├── 02_CreateTables.sql
    ├── 03_CreateIndexes.sql
    └── 04_InsertSampleData.sql
```

</details>

## 🚀 Kurulum

### ⚡ Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) veya üzeri
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) (veya SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / [VS Code](https://code.visualstudio.com/) / [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Git](https://git-scm.com/)

### 📥 Adım 1: Projeyi Klonlayın
```bash
git clone https://github.com/[kullanıcı-adınız]/hospital-appointment-system.git
cd hospital-appointment-system
```

### 🗄 Adım 2: Veritabanı Kurulumu

#### SQL Server Management Studio (SSMS) ile:

1. SSMS'i açın ve SQL Server'ınıza bağlanın
2. `Database/` klasöründeki SQL scriptlerini **sırasıyla** çalıştırın:

```sql
-- 1. Veritabanını oluştur
Database/01_CreateDatabase.sql

-- 2. Tabloları oluştur
Database/02_CreateTables.sql

-- 3. İndeksleri oluştur
Database/03_CreateIndexes.sql

-- 4. Örnek verileri ekle (Opsiyonel)
Database/04_InsertSampleData.sql
```

### ⚙️ Adım 3: Connection String Ayarlayın

`AppointmentSystem.API/appsettings.json` dosyasını açın ve connection string'i güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=HospitalAppointmentDB;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

**Not:** `YOUR_SERVER_NAME` kısmını kendi SQL Server instance adınızla değiştirin.
- Örnek: `localhost` veya `localhost\\SQLEXPRESS` veya `.\\SQLEXPRESS`

### 🎬 Adım 4: NuGet Paketlerini Geri Yükleyin ve Projeyi Çalıştırın

```bash
# Solution klasörüne gidin
cd AppointmentSystem

# Paketleri geri yükleyin
dotnet restore

# Projeyi derleyin
dotnet build

# API projesini çalıştırın
cd AppointmentSystem.API
dotnet run
```

Ya da **Visual Studio** ile:
1. Solution'ı açın (`.slnx` dosyası)
2. `AppointmentSystem.API` projesini başlangıç projesi olarak ayarlayın
3. F5 tuşuna basın

### 🌐 Uygulamaya Erişim

Proje başlatıldığında Swagger UI otomatik olarak açılacaktır:
```
https://localhost:7xxx/swagger
http://localhost:5xxx/swagger
```

## 🗄️ Veritabanı Şeması

### 📊 Tablolar:
1. **Users** - Tüm kullanıcılar (Admin, Doctor, Patient)
2. **Departments** - Hastane departmanları
3. **Doctors** - Doktor bilgileri ve uzmanlıkları
4. **Patients** - Hasta bilgileri ve tıbbi geçmiş
5. **DoctorSchedules** - Doktor çalışma saatleri
6. **Appointments** - Randevu kayıtları
7. **MedicalRecords** - Tıbbi kayıtlar ve notlar

#### İlişkiler:
```
Users (1) ──→ (*) Doctors
Users (1) ──→ (*) Patients
Departments (1) ──→ (*) Doctors
Doctors (1) ──→ (*) Appointments
Patients (1) ──→ (*) Appointments
Appointments (1) ──→ (1) MedicalRecords
Doctors (1) ──→ (*) DoctorSchedules
```

### 👥 Örnek Kullanıcılar

Script çalıştırıldığında aşağıdaki test kullanıcıları oluşturulur:

| Rol | Email | Şifre | Açıklama |
|-----|-------|-------|----------|
| Admin | admin@hospital.com | Admin123! | Sistem yöneticisi |
| Doctor | dr.mehmet@hospital.com | Doctor123! | Kardiyoloji uzmanı |
| Doctor | dr.ayse@hospital.com | Doctor123! | Ortopedi uzmanı |
| Patient | patient1@email.com | Patient123! | Test hastası 1 |
| Patient | patient2@email.com | Patient123! | Test hastası 2 |

## 📡 API Kullanımı

### 🔐 Authentication

#### 📝 Hasta Kaydı (Register)
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "newpatient@email.com",
  "password": "Password123!",
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "phoneNumber": "5551234567",
  "dateOfBirth": "1990-01-15",
  "gender": "Male",
  "address": "İstanbul, Turkey",
  "bloodType": "A+"
}
```

#### 🔑 Giriş (Login)
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "patient1@email.com",
  "password": "Patient123!"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "userId": 5,
    "email": "patient1@email.com",
    "fullName": "Emre Özkan",
    "role": "Patient",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "message": "Login successful"
}
```

### 📅 Appointments (Randevular)

#### Randevu Oluştur
```http
POST /api/appointments
Authorization: Bearer {token}
Content-Type: application/json

{
  "patientId": 1,
  "doctorId": 1,
  "appointmentDate": "2024-01-20",
  "appointmentTime": "10:00:00",
  "notes": "Baş ağrısı şikayeti"
}
```

#### Hasta Randevularını Listele
```http
GET /api/appointments/patient/{patientId}
Authorization: Bearer {token}
```

#### Doktor Randevularını Listele
```http
GET /api/appointments/doctor/{doctorId}
Authorization: Bearer {token}
```

### 👨‍⚕️ Doctors (Doktorlar)

#### Tüm Doktorları Listele
```http
GET /api/doctors
Authorization: Bearer {token}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "title": "Prof. Dr.",
      "firstName": "Mehmet",
      "lastName": "Yılmaz",
      "departmentName": "Kardiyoloji",
      "experienceYears": 15,
      "licenseNumber": "DOC001"
    }
  ]
}
```

#### Doktor Detayını Getir
```http
GET /api/doctors/{id}
Authorization: Bearer {token}
```

### 🏥 Departments (Departmanlar)

#### Aktif Departmanları Listele
```http
GET /api/departments
Authorization: Bearer {token}
```

## 🧪 Test

Proje kapsamlı unit testler içermektedir (76 test, %100 başarı oranı).

### Testleri Çalıştırma

```bash
# Tüm testleri çalıştır
dotnet test

# Test coverage raporu ile
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Belirli bir test sınıfını çalıştır
dotnet test --filter "FullyQualifiedName~LoginCommandHandlerTests"
```

### Test Kategorileri

- ✅ **Application Layer Tests**
  - Command Handler Tests (CQRS)
  - Query Handler Tests
  - Validator Tests (FluentValidation)

- ✅ **Domain Tests**
  - Entity Tests
  - Business Logic Tests

- ✅ **Infrastructure Tests**
  - Repository Tests
  - Service Tests

Detaylı test raporu için: [TEST_REPORT.md](AppointmentSystem.Tests/TEST_REPORT.md)

## 🎯 Öğrenilecekler

Bu proje aşağıdaki konuları öğrenmek isteyenler için harika bir kaynak:

### Mimari & Design Patterns
- ✅ Clean Architecture implementasyonu
- ✅ CQRS Pattern (MediatR ile)
- ✅ Repository Pattern
- ✅ Result Pattern (Standart response yönetimi)
- ✅ Dependency Injection

### Backend Teknolojiler
- ✅ .NET 8 Web API geliştirme
- ✅ Dapper ile Database First yaklaşım
- ✅ JWT Authentication & Authorization
- ✅ FluentValidation kullanımı
- ✅ Serilog ile structured logging
- ✅ Global Exception Handling Middleware

### Veritabanı
- ✅ SQL Server database design
- ✅ İlişkisel veritabanı tasarımı
- ✅ İndeks optimizasyonu
- ✅ Stored procedure yazımı (opsiyonel)

### Test
- ✅ xUnit ile unit testing
- ✅ Moq ile mocking
- ✅ Test-driven development (TDD)

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Lütfen katkıda bulunmadan önce:

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'feat: Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

### Commit Mesaj Formatı
```
feat: Yeni özellik ekleme
fix: Bug düzeltme
docs: Dokümantasyon değişiklikleri
style: Kod formatı değişiklikleri
refactor: Kod refactoring
test: Test ekleme veya düzeltme
chore: Genel bakım işleri
```


## 🙏 Teşekkürler

Bu projeyi geliştirirken aşağıdaki kaynaklardan ilham alınmıştır:
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft .NET Documentation](https://docs.microsoft.com/dotnet/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)

---

<div align="center">

**⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın! ⭐**

Made with ❤️ using .NET 8

</div>
