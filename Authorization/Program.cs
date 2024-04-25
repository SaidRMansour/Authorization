using Authorization.Auth;
using Authorization.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<NewsSiteContext>(options =>
    options.UseSqlite("Data Source=newsSite.db"));

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditArticle", policy => policy.RequireRole("Editor", "Journalist")); // DONE
    options.AddPolicy("CanCreateArticle", policy => policy.RequireRole("Editor","Journalist" )); // DONE

    options.AddPolicy("CanDeleteArticle", policy => policy.RequireRole("Editor")); // DONE!
    options.AddPolicy("CanDeleteComment", policy => policy.RequireRole("Editor")); // DONE!
    options.AddPolicy("CanEditComment", policy => policy.RequireRole("Editor")); // DONE!

    options.AddPolicy("CanComment", policy => policy.RequireRole("Subscriber", "Editor", "Journalist")); // DONE!
    options.AddPolicy("CanReadArticle", policy => policy.RequireRole("Guest", "Subscriber", "Editor", "Journalist")); // DONE!

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NewsSiteContext>();
    dbContext.Database.EnsureCreated(); 
    dbContext.Seed(); 
}
app.Run();

