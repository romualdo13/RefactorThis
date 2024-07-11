using RefactorThis.API.Domain.Services;
using RefactorThis.API.Domain.Repositories;
using RefactorThis.API.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IInvoiceRepository, InvoiceRepository>(); // I register the repository and the serivces this is to enable dependency injection throughout the application
builder.Services.AddTransient<InvoiceService>();
//services.AddDbContext<DbContext>(options => options.UseSqlServer("Connection_String")); If you wan't to interact with a database i just put the sample ^_^

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // I used swagger just to test the API endpoints
    app.UseSwaggerUI();
}

app.UseRouting(); //Set up this to handle HTTP Request
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
