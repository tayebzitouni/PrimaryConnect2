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
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect;
using PrimaryConnect.Data;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("conection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChatHub>();


builder.Services.AddDistributedMemoryCache(); // Or another distributed cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // Session timeout
    options.Cookie.HttpOnly = true; // Important for security
    options.Cookie.IsEssential = true; // Makes the session cookie essential
});
var app = builder.Build();

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
