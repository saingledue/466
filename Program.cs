using Microsoft.EntityFrameworkCore;
using SEWebApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<fundDBContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("FundDatabase")));
var MyAllowSpecificOrigins = "OriginsAllowed";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*")
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(MyAllowSpecificOrigins);

app.Run();
