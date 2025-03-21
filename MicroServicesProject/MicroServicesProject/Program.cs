using MicroServicesProject.Infrastructure;
using MicroServicesProject.Contract;

var builder = WebApplication.CreateBuilder(args);

// Register Mediator for All Query files
builder.Services.AddMediatorServices();

// Register DAL (Data Access Layer) as Transient service
builder.Services.AddTransient<IConnectionFactory, ConnectionFactory>();

// Register other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
