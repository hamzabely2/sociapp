using Api.Service;
using Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajoutez votre DbContext avant de construire l'application
builder.Services.AddDbContext<Context>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ConnectionDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConnectionDB"))
));

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    try
    {
        var dbContext = serviceProvider.GetRequiredService<Context>();
        dbContext.Database.CanConnect(); 
    }
    catch (Exception ex)
    {
        throw new Exception("Erreur de connexion à la base de données. Vérifiez la configuration.", ex);
    }
}

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var reactApp = builder.Configuration["Cors:url"];

string[] origins = new string[] { reactApp, "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "ReactLocal",
      policy => policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()
    );
});

var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

builder.Configuration.AddEnvironmentVariables();

//app.UseHttpsRedirection();
builder.Services.AddHttpContextAccessor();

app.UseAuthorization();
app.UseCors("ReactLocal");
app.UseCookiePolicy();
app.UseAuthentication();

app.MapControllers();

app.Run();
