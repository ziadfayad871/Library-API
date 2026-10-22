using LibraryWeb.Services.IServices;
using LibraryWeb.Services.Services;

namespace LibraryWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            // http client for API call
            builder.Services.AddHttpClient<IBaseService, BaseService>();

            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBookCopyService, BookCopyService>();
            

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
