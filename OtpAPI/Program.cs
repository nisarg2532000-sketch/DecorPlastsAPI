using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OtpAPI.BAL;
using OtpAPI.Data;
using OtpAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<OtpBAL>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OtpService>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("OtpPolicy", opt =>
    {
        opt.PermitLimit = 3; // 3 requests
        opt.Window = TimeSpan.FromMinutes(10);
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();
app.UseRateLimiter();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
