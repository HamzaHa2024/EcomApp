using BuyerApp.Interfaces;
using BuyerApp.Mapper;
using BuyerApp.Services;
using OrderCloud.SDK;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IOrderCloudClient>(provider =>
    new OrderCloudClient(new OrderCloudClientConfig
    {
        ClientId = builder.Configuration["OrderCloud:ClientId"],
        ClientSecret = builder.Configuration["OrderCloud:ClientSecret"],
        ApiUrl = builder.Configuration["OrderCloud:ApiUrl"],
        AuthUrl = builder.Configuration["OrderCloud:AuthUrl"],
        Username = builder.Configuration["OrderCloud:username"],
        Password = builder.Configuration["OrderCloud:password"],
        GrantType = GrantType.Password,
    })
);

builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.AddTransient<IOrderService, OrderService>();
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
