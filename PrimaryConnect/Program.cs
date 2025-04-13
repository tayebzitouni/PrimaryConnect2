//using Microsoft.EntityFrameworkCore;
//using PrimaryConnect;
//using PrimaryConnect.Data;
//using System;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//        policy => policy.WithOrigins("http://127.0.0.1:5500") // Remplace par ton URL frontend
//                        .AllowAnyMethod()
//                        .AllowAnyHeader()
//                        .AllowCredentials()); // ✅ Important pour SignalR
//});


//// Add services to the container.
//// 🔥 Ajout nécessaire pour SignalR

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddSignalR();

//builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("conection")));

//builder.Services.AddDistributedMemoryCache(); // Or another distributed cache
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(120); // Session timeout
//    options.Cookie.HttpOnly = true; // Important for security
//    options.Cookie.IsEssential = true; // Makes the session cookie essential
//});
//var app = builder.Build();
//app.UseCors("AllowSpecificOrigin");

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.UseSession();

//app.MapControllers();
//app.MapHub<ChatHub>("/ChatHub"); // 🔥 Assure-toi que ChatHub est bien enregistré

//app.Run();
using FirebaseAdmin;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect;
using PrimaryConnect.Data;
using Microsoft.OpenApi.Models;
using  Swashbuckle.AspNetCore.SwaggerGen;
using System.Configuration;
using Microsoft.AspNetCore.Http.Features;
using System.Reflection;


var options = new WebApplicationOptions
{
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
};




var builder = WebApplication.CreateBuilder(options);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

// Set WebRootPath manually if not set
// Ajouter les services au conteneur
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500") // URL frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddSingleton<IFileUploadPathProvider>(provider =>
    new FileUploadPathProvider(builder.Environment.WebRootPath));
//builder.Services.AddSingleton<IFileUploadPathProvider>(new FileUploadPathProvider(builder.Environment.WebRootPath));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Enable enum string values in Swagger UI
    c.UseInlineDefinitionsForEnums();

    // Include XML comments (optional but recommended)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.OperationFilter<FileUploadOperation>(); // ✅ هذا هو المهم
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<ChatHub>();




builder.Services.AddDistributedMemoryCache(); // Or another distributed cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // Session timeout
    options.Cookie.HttpOnly = true; // Important for security
    options.Cookie.IsEssential = true; // Makes the session cookie essential
});
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}
// ✅ Swagger doit être activé correctement
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // ✅ Afficher les erreurs détaillées
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Appliquer CORS avant Authorization
app.UseCors("AllowSpecificOrigin");


FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("C:\\primaryconnect-572064dabd4d.json")
});


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
