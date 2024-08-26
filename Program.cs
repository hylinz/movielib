using MovieLibrary;
using MovieLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using MovieLibrary.Endpoints;
using MovieLibrary.Services;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using MovieLibrary.Entities;
var builder = WebApplication.CreateBuilder(args);


// ------- Services

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection"));
// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["allowedOrigins"]!).AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
// Cache
builder.Services.AddOutputCache();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();

builder.Services.AddTransient<IFileStorage, AzureFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

//

// Create App
var app = builder.Build();

// ------ Middleware

// Active Services
if (builder.Environment.IsDevelopment())
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{

    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;
    var error = new Error();

    error.Date = DateTime.UtcNow;
    error.Message = exception.Message;
    error.StackTrace = exception.StackTrace;

    var repository = context.RequestServices.GetRequiredService<IErrorsRepository>();

    await repository.Create(error);
 
    await Results
    .BadRequest(new 
    { 
        type = "Error", 
        message = "An unexpected exception was encountered", 
        status = 500 
    })
    .ExecuteAsync(context);
}));
app.UseStatusCodePages();
app.UseCors();
app.UseOutputCache();
///////////////////
// --- Routes

app.MapGet("/", () => "Hello world");


app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/comments").MapComments();




// Middleware end
app.Run();


