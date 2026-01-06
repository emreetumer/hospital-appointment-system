# ?? Öðrenme Rehberi

Bu doküman projede kullanýlan her bir teknoloji ve pattern'i detaylýca açýklar.

## Ýçindekiler
1. [Clean Architecture](#1-clean-architecture)
2. [Dapper (Micro ORM)](#2-dapper-micro-orm)
3. [CQRS Pattern](#3-cqrs-pattern)
4. [MediatR](#4-mediatr)
5. [Repository Pattern](#5-repository-pattern)
6. [Result Pattern](#6-result-pattern)
7. [FluentValidation](#7-fluentvalidation)
8. [JWT Authentication](#8-jwt-authentication)
9. [Dependency Injection](#9-dependency-injection)
10. [Serilog Logging](#10-serilog-logging)

---

## 1. Clean Architecture

### Nedir?
Clean Architecture, yazýlýmý baðýmsýz, test edilebilir ve sürdürülebilir katmanlara ayýran bir mimari yaklaþýmdýr.

### Katmanlar

#### ?? Domain Layer (En Ýç Katman)
**Sorumluluðu:** Business entities ve business rules

```csharp
// Entity örneði
public class User : BaseEntity
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
}
```

**Özellikleri:**
- ? Hiçbir katmana baðýmlý deðil
- ? Pure C# classes
- ? Business logic içerir

#### ?? Application Layer
**Sorumluluðu:** Use cases, business logic orchestration

```csharp
// Command örneði
public class CreateAppointmentCommand : IRequest<Result<int>>
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
}

// Handler örneði
public class CreateAppointmentCommandHandler 
    : IRequestHandler<CreateAppointmentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(...)
    {
        // Business logic here
    }
}
```

**Özellikleri:**
- ? Interface'ler tanýmlar (IRepository, IService)
- ? CQRS Commands/Queries
- ? Validation rules
- ? Database implementation yok

#### ?? Infrastructure Layer
**Sorumluluðu:** External concerns (database, file system, email, etc.)

```csharp
// Repository implementation
public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;
    
    public async Task<User> GetByIdAsync(int id)
    {
        var query = "SELECT * FROM Users WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
    }
}
```

**Özellikleri:**
- ? Database access (Dapper)
- ? External API calls
- ? File operations
- ? Email/SMS services

#### ?? Presentation Layer (API)
**Sorumluluðu:** HTTP endpoints, user interface

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
```

**Özellikleri:**
- ? HTTP request/response handling
- ? Authentication/Authorization
- ? Validation
- ? Business logic yok (delegated to Application layer)

### Baðýmlýlýk Yönü
```
API ? Infrastructure ? Application ? Domain
```

**Altýn Kural:** Ýç katmanlar dýþ katmanlarý bilmez!

---

## 2. Dapper (Micro ORM)

### Nedir?
Dapper, hafif ve performanslý bir Micro ORM'dir. Entity Framework'e göre daha hýzlýdýr çünkü minimal abstraction kullanýr.

### Neden Dapper?
? **Performans:** EF Core'dan ~3x daha hýzlý  
? **Control:** Raw SQL kontrolü  
? **Lightweight:** Minimal overhead  
? **Database First:** Mevcut DB'yi kullanmak kolay  

### Temel Kullaným

#### 1. Connection Oluþturma
```csharp
public class DapperContext
{
    private readonly string _connectionString;
    
    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
```

#### 2. Query (SELECT)
```csharp
// Single entity
var user = await connection.QuerySingleOrDefaultAsync<User>(
    "SELECT * FROM Users WHERE Id = @Id", 
    new { Id = id }
);

// Multiple entities
var users = await connection.QueryAsync<User>(
    "SELECT * FROM Users WHERE IsActive = @IsActive",
    new { IsActive = true }
);
```

#### 3. Execute (INSERT/UPDATE/DELETE)
```csharp
// INSERT with SCOPE_IDENTITY
var query = @"
    INSERT INTO Users (Email, FirstName, LastName)
    VALUES (@Email, @FirstName, @LastName);
    SELECT CAST(SCOPE_IDENTITY() as int)";

var id = await connection.QuerySingleAsync<int>(query, user);

// UPDATE
var updateQuery = @"
    UPDATE Users 
    SET FirstName = @FirstName, LastName = @LastName
    WHERE Id = @Id";

var rowsAffected = await connection.ExecuteAsync(updateQuery, user);
```

#### 4. Multi-Mapping (JOIN)
```csharp
// JOIN example - Doctor with User and Department
var query = @"
    SELECT d.*, u.*, dep.*
    FROM Doctors d
    INNER JOIN Users u ON d.UserId = u.Id
    INNER JOIN Departments dep ON d.DepartmentId = dep.Id
    WHERE d.Id = @Id";

var doctor = await connection.QueryAsync<Doctor, User, Department, Doctor>(
    query,
    (doctor, user, department) =>
    {
        doctor.User = user;
        doctor.Department = department;
        return doctor;
    },
    new { Id = id },
    splitOn: "Id,Id" // Split columns by Id
);
```

**splitOn Açýklamasý:**
- Dapper hangi sütundan itibaren yeni entity'nin baþladýðýný bilmeli
- Örnek: `d.* (Doctor), u.* (User), dep.* (Department)`
- Ýlk `Id` ? User baþlangýcý
- Ýkinci `Id` ? Department baþlangýcý

### Dapper vs Entity Framework

| Özellik | Dapper | EF Core |
|---------|--------|---------|
| Performance | ??? Çok Hýzlý | ? Orta |
| SQL Control | ? Full Control | ? Limited |
| Learning Curve | ? Kolay | ? Zor |
| Change Tracking | ? Yok | ? Var |
| Lazy Loading | ? Yok | ? Var |
| Database First | ?? Mükemmel | ?? Karýþýk |

---

## 3. CQRS Pattern

### Nedir?
**CQRS** = Command Query Responsibility Segregation

Ýþlemleri 2 kategoriye ayýrýr:
- **Commands:** Veri deðiþtiren iþlemler (Create, Update, Delete)
- **Queries:** Veri okuyan iþlemler (Get, List)

### Neden CQRS?
? **Separation of Concerns:** Okuma/yazma ayrý  
? **Scalability:** Okuma ve yazma ayrý scale edilebilir  
? **Optimization:** Her iþlem için farklý optimizasyon  
? **Maintainability:** Kod daha temiz ve anlaþýlýr  

### Command Örneði

```csharp
// 1. Command (Request)
public class CreateAppointmentCommand : IRequest<Result<int>>
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
}

// 2. Validator
public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.DoctorId).GreaterThan(0);
        RuleFor(x => x.AppointmentDate).GreaterThanOrEqualTo(DateTime.Today);
    }
}

// 3. Handler (Business Logic)
public class CreateAppointmentCommandHandler 
    : IRequestHandler<CreateAppointmentCommand, Result<int>>
{
    private readonly IAppointmentRepository _repository;
    
    public async Task<Result<int>> Handle(
        CreateAppointmentCommand request, 
        CancellationToken cancellationToken)
    {
        // Validation
        var isAvailable = await _repository.IsTimeSlotAvailableAsync(...);
        if (!isAvailable)
            return Result<int>.Failure("Time slot not available");
        
        // Create
        var appointment = new Appointment { ... };
        var id = await _repository.CreateAsync(appointment);
        
        return Result<int>.Success(id, "Appointment created");
    }
}
```

### Query Örneði

```csharp
// 1. Query (Request)
public class GetAllDoctorsQuery : IRequest<Result<IEnumerable<Doctor>>>
{
}

// 2. Handler
public class GetAllDoctorsQueryHandler 
    : IRequestHandler<GetAllDoctorsQuery, Result<IEnumerable<Doctor>>>
{
    private readonly IDoctorRepository _repository;
    
    public async Task<Result<IEnumerable<Doctor>>> Handle(
        GetAllDoctorsQuery request, 
        CancellationToken cancellationToken)
    {
        var doctors = await _repository.GetAllAsync();
        return Result<IEnumerable<Doctor>>.Success(doctors);
    }
}
```

### Controller'da Kullaným

```csharp
[HttpPost]
public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
{
    var result = await _mediator.Send(command); // MediatR magic!
    return result.IsSuccess ? Ok(result) : BadRequest(result);
}

[HttpGet]
public async Task<IActionResult> GetDoctors()
{
    var query = new GetAllDoctorsQuery();
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

---

## 4. MediatR

### Nedir?
MediatR, Mediator Pattern implementasyonudur. Request/Response iþlemlerini handle eder.

### Nasýl Çalýþýr?

```
Controller ? MediatR ? Handler ? Repository ? Database
```

1. Controller bir `Command` veya `Query` oluþturur
2. `_mediator.Send(command)` ile gönderir
3. MediatR ilgili `Handler`'ý bulur
4. Handler business logic'i çalýþtýrýr
5. Sonuç Controller'a döner

### Avantajlarý
? **Loose Coupling:** Controller handler'ý direkt bilmiyor  
? **Single Responsibility:** Her handler tek bir iþ yapar  
? **Testability:** Handler'lar kolayca test edilebilir  
? **Pipeline Behaviors:** Validation, logging gibi cross-cutting concerns  

### Kurulum

```csharp
// Application/DependencyInjection.cs
services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
```

### Pipeline Behaviors (Opsiyonel)

```csharp
// Validation Behavior - Tüm command'lar için otomatik validation
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();
        
        if (failures.Count != 0)
            throw new ValidationException(failures);
        
        return await next();
    }
}
```

---

## 5. Repository Pattern

### Nedir?
Repository Pattern, data access logic'i business logic'ten ayýrýr.

### Neden Kullanýlýr?
? **Abstraction:** Business logic database'i direkt bilmiyor  
? **Testability:** Mock repository ile test kolay  
? **Maintainability:** Database deðiþirse sadece repository deðiþir  
? **Reusability:** Ayný repository farklý use case'lerde kullanýlabilir  

### Yapýsý

```csharp
// 1. Interface (Application Layer)
public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<int> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}

// 2. Implementation (Infrastructure Layer)
public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;
    
    public UserRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<User> GetByIdAsync(int id)
    {
        var query = "SELECT * FROM Users WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
    }
    
    // ... other methods
}

// 3. Registration (Infrastructure/DependencyInjection.cs)
services.AddScoped<IUserRepository, UserRepository>();
```

### Generic Repository (Opsiyonel)

```csharp
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly DapperContext _context;
    private readonly string _tableName;
    
    public GenericRepository(DapperContext context, string tableName)
    {
        _context = context;
        _tableName = tableName;
    }
    
    public async Task<T> GetByIdAsync(int id)
    {
        var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
    }
}
```

---

## 6. Result Pattern

### Nedir?
Result Pattern, iþlem sonuçlarýný standardize eder. Exception yerine Result<T> döner.

### Neden Kullanýlýr?
? **Predictable:** Her zaman bir sonuç var (success veya failure)  
? **Type Safe:** Compile-time type checking  
? **No Exceptions:** Exception handling overhead'i yok  
? **Readable:** Kod daha okunabilir  

### Implementasyon

```csharp
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public string Message { get; private set; }
    public List<string> Errors { get; private set; }
    
    private Result(bool isSuccess, T data, string message, List<string> errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        Errors = errors ?? new List<string>();
    }
    
    public static Result<T> Success(T data, string message = "Success")
        => new Result<T>(true, data, message, null);
    
    public static Result<T> Failure(string message, List<string> errors = null)
        => new Result<T>(false, default, message, errors);
}

// Non-generic version
public class Result
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public List<string> Errors { get; private set; }
    
    public static Result Success(string message = "Success")
        => new Result(true, message, null);
    
    public static Result Failure(string message, List<string> errors = null)
        => new Result(false, message, errors);
}
```

### Kullaným

```csharp
// Handler'da
public async Task<Result<int>> Handle(...)
{
    if (user == null)
        return Result<int>.Failure("User not found");
    
    var id = await _repository.CreateAsync(user);
    return Result<int>.Success(id, "User created successfully");
}

// Controller'da
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
{
    var result = await _mediator.Send(command);
    
    if (!result.IsSuccess)
        return BadRequest(result);
    
    return Ok(result);
}

// Response örneði (Success)
{
  "isSuccess": true,
  "data": 123,
  "message": "User created successfully",
  "errors": []
}

// Response örneði (Failure)
{
  "isSuccess": false,
  "data": null,
  "message": "User not found",
  "errors": ["Invalid user ID", "User inactive"]
}
```

---

## 7. FluentValidation

### Nedir?
FluentValidation, input validation için kullanýlan bir library.

### Neden Kullanýlýr?
? **Readable:** Validation rules çok okunabilir  
? **Testable:** Validation logic kolayca test edilebilir  
? **Reusable:** Ayný validator farklý yerlerde kullanýlabilir  
? **Complex Rules:** Karmaþýk validation rules kolay  

### Kullaným

```csharp
// Validator
public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase")
            .Matches(@"[0-9]").WithMessage("Password must contain number");
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[0-9]{10}$").WithMessage("Phone number must be 10 digits")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber)); // Conditional validation
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old");
    }
    
    private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
        return age >= 18;
    }
}

// Registration
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

### Advanced Validation

```csharp
// Custom validator
RuleFor(x => x.Email)
    .MustAsync(async (email, cancellation) => 
    {
        var exists = await _userRepository.ExistsAsync(email);
        return !exists;
    })
    .WithMessage("Email already exists");

// Dependent rules
RuleFor(x => x.ConfirmPassword)
    .Equal(x => x.Password)
    .When(x => !string.IsNullOrEmpty(x.Password))
    .WithMessage("Passwords must match");
```

---

## 8. JWT Authentication

### Nedir?
JWT (JSON Web Token), stateless authentication için kullanýlýr.

### Nasýl Çalýþýr?

```
1. User login ? Server validates credentials
2. Server generates JWT token
3. Token returned to client
4. Client stores token (localStorage, cookie)
5. Client sends token in every request (Authorization header)
6. Server validates token
```

### JWT Yapýsý

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

Header.Payload.Signature
```

**Header:**
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload (Claims):**
```json
{
  "sub": "123",
  "email": "user@email.com",
  "role": "Patient",
  "exp": 1234567890
}
```

**Signature:**
```
HMACSHA256(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  secret
)
```

### Implementation

```csharp
// Token Generation
public Task<string> GenerateJwtTokenAsync(int userId, string email, string role)
{
    var jwtSettings = _configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    
    var token = new JwtSecurityToken(
        issuer: jwtSettings["Issuer"],
        audience: jwtSettings["Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(1440),
        signingCredentials: credentials
    );
    
    return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
}

// Token Validation (Program.cs)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
```

### Usage in Controllers

```csharp
// Require authentication
[Authorize]
[HttpGet]
public async Task<IActionResult> GetProfile()
{
    // Get current user ID from claims
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    // ...
}

// Require specific role
[Authorize(Roles = "Doctor")]
[HttpGet("appointments")]
public async Task<IActionResult> GetDoctorAppointments()
{
    // ...
}

// Multiple roles
[Authorize(Roles = "Admin,Doctor")]
[HttpPost("medical-record")]
public async Task<IActionResult> CreateMedicalRecord()
{
    // ...
}
```

---

## 9. Dependency Injection

### Nedir?
Dependency Injection (DI), Inversion of Control (IoC) pattern'idir. Dependencies class içinde oluþturulmaz, dýþarýdan inject edilir.

### Neden Kullanýlýr?
? **Loose Coupling:** Classes birbirlerine baðýmlý deðil  
? **Testability:** Mock dependencies kolayca inject edilebilir  
? **Maintainability:** Implementation deðiþirse sadece bir yerde deðiþir  
? **Flexibility:** Runtime'da implementation deðiþtirilebilir  

### Lifetime'lar

#### 1. Transient
Her istek için yeni instance

```csharp
services.AddTransient<IEmailService, EmailService>();
```

**Ne zaman kullanýlýr:** Stateless, lightweight services

#### 2. Scoped
Her HTTP request için bir instance

```csharp
services.AddScoped<IUserRepository, UserRepository>();
```

**Ne zaman kullanýlýr:** Repository'ler, DbContext

#### 3. Singleton
Uygulama boyunca tek instance

```csharp
services.AddSingleton<DapperContext>();
```

**Ne zaman kullanýlýr:** Configuration, caching, logging

### Registration

```csharp
// Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    return services;
}

// Infrastructure/DependencyInjection.cs
public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    services.AddSingleton<DapperContext>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IDoctorRepository, DoctorRepository>();
    services.AddScoped<IAuthService, AuthService>();
    return services;
}

// Program.cs
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
```

### Constructor Injection

```csharp
public class CreateAppointmentCommandHandler 
    : IRequestHandler<CreateAppointmentCommand, Result<int>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    
    // DI Container automatically injects dependencies
    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }
    
    public async Task<Result<int>> Handle(...)
    {
        // Use injected dependencies
    }
}
```

---

## 10. Serilog Logging

### Nedir?
Serilog, structured logging library'sidir.

### Neden Kullanýlýr?
? **Structured:** JSON formatýnda log  
? **Sinks:** Farklý yerlere log yazabilir (file, console, database)  
? **Filtering:** Log level filtering  
? **Enrichment:** Contextual information ekleme  

### Configuration

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

### Usage

```csharp
private readonly ILogger<CreateAppointmentCommandHandler> _logger;

public async Task<Result<int>> Handle(...)
{
    _logger.LogInformation("Creating appointment for patient {PatientId}", request.PatientId);
    
    try
    {
        // ...
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating appointment");
        return Result<int>.Failure("An error occurred");
    }
}
```

### Log Levels

```csharp
_logger.LogTrace("Very detailed logs");
_logger.LogDebug("Debug information");
_logger.LogInformation("General information");
_logger.LogWarning("Warning message");
_logger.LogError(exception, "Error message");
_logger.LogCritical(exception, "Critical error");
```

---

## ?? Sonuç

Bu projede modern .NET development için gerekli tüm pattern'leri öðrendiniz:

1. ? **Clean Architecture** - Katmanlý mimari
2. ? **Dapper** - Performanslý data access
3. ? **CQRS** - Command/Query separation
4. ? **MediatR** - Request/Response handling
5. ? **Repository Pattern** - Data access abstraction
6. ? **Result Pattern** - Standardized responses
7. ? **FluentValidation** - Input validation
8. ? **JWT** - Secure authentication
9. ? **Dependency Injection** - IoC pattern
10. ? **Serilog** - Structured logging

**Sonraki Adýmlar:**
- Kendi feature'larýnýzý ekleyin
- Unit test yazýn
- Integration test yazýn
- Performance optimization yapýn
- Production'a deploy edin

**Ýyi çalýþmalar! ??**
