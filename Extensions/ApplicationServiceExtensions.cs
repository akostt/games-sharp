using GamesSharp.Services;

namespace GamesSharp.Extensions
{
    /// <summary>
    /// Расширения для регистрации приложения и сервисов
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Регистрирует все сервисы приложения
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Регистрация кеша в памяти
            services.AddMemoryCache();

            // Регистрация бизнес-сервисов
            services.AddScoped<IReferenceDataService, ReferenceDataService>();

            // Добавить CORS поддержку если нужна API
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://localhost:5000")
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }

        /// <summary>
        /// Настраивает middleware безопасности и общие параметры
        /// </summary>
        public static WebApplication UseApplicationMiddleware(this WebApplication app)
        {
            // Настройка обработки ошибок
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // Детальная страница ошибок в разработке
                app.UseDeveloperExceptionPage();
            }

            // Безопасность
            app.UseHttpsRedirection();
            
            // Статическое содержимое
            app.UseRouting();
            app.MapStaticAssets();

            // Авторизация
            app.UseAuthorization();

            // Маршруты контроллеров
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            return app;
        }
    }
}
