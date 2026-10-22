using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); //makes endpoints available for API tools such as Swagger
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); // do i need these? why tutorial project doesn't have it? as AI these swagger and scalar work by visualizing openAPI code


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // do i need these? why tutorial project doesn't have it? as AI these swagger and scalar work by visualizing openAPI code
    app.MapScalarApiReference();
    // // Can I use both Scalar and swagger? Make swagger work as well if so!
    // app.UseCors("DevCors");
    // app.UseSwagger();
    // app.UseSwaggerUI();
}
// // Do I need these? what are these?
// else
// {
//     app.UseCors("ProdCors");
//     app.UseHttpsRedirection();
    
// }

app.UseHttpsRedirection(); // point of this?

app.UseAuthorization();

app.MapControllers(); // point of this?

app.Run();
