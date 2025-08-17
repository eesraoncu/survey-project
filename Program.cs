using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Services;
using AutoMapper;
using SurveyApp.Application.Profiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);   

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// CORS ayarlarını ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Ayarları
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Google Auth Ayarları
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

// Google AI Ayarları
builder.Services.Configure<GoogleAISettings>(builder.Configuration.GetSection("GoogleAI"));

// MongoDB DI kayıtları (IMongoClient, IMongoDatabase)
var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
if (mongoSettings is not null && !string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
{
    // MongoDB Settings'i DI container'a ekle
    builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
    
    builder.Services.AddSingleton<IMongoClient>(_ => 
    {
        var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        
        // TLS 1.2'yi zorunlu tut
        settings.SslSettings = new SslSettings
        {
            EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
            CheckCertificateRevocation = false
        };
        
        // Timeout ayarları
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(60);
        settings.ConnectTimeout = TimeSpan.FromSeconds(60);
        settings.SocketTimeout = TimeSpan.FromSeconds(60);
        settings.HeartbeatTimeout = TimeSpan.FromSeconds(60);
        
        return new MongoClient(settings);
    });
    
    builder.Services.AddSingleton<IMongoDatabase>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(mongoSettings.DatabaseName);
    });
    
    // AutoIncrementService'i ekle
    builder.Services.AddScoped<AutoIncrementService>();
}

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings != null)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
}

// Trello Auth ayarlarını yapılandır
builder.Services.Configure<TrelloAuthSettings>(builder.Configuration.GetSection("TrelloAuth"));

// Jira Auth ayarlarını yapılandır
builder.Services.Configure<JiraAuthSettings>(builder.Configuration.GetSection("JiraAuth"));

// HttpClient'ı ekle (Jira API için)
builder.Services.AddHttpClient("JiraClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "SurveyApp/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// HttpClient'ı ekle (Google AI API için)
builder.Services.AddHttpClient<IAIService, AIService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "SurveyApp-AI/1.0");
    client.Timeout = TimeSpan.FromSeconds(60);
});

// Service'leri kaydet
builder.Services.AddScoped<IAdresService, AdresService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<ITrelloAuthService, TrelloAuthService>();
builder.Services.AddScoped<IJiraAuthService, JiraAuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<MigrationService>();

// Repository'leri kaydet
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS middleware'ini ekle (UseAuthentication'dan önce)
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Migration'ı çalıştır (sadece bir kez)
if (app.Environment.IsDevelopment())
{
    try
    {
        var migrationService = app.Services.GetRequiredService<MigrationService>();
        await migrationService.MigrateUsersToNewRoleSystem();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration hatası: {ex.Message}");
    }
}

app.Run(); 