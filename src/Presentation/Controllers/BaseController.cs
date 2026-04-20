using Microsoft.AspNetCore.Mvc;
using GamesSharp.Data;
using GamesSharp.Helpers;

namespace GamesSharp.Controllers
{
    /// <summary>
    /// Базовый контроллер с общей логикой для всех контроллеров
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext Context;
        protected readonly ILogger Logger;

        protected BaseController(ApplicationDbContext context, ILogger logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Проверка валидности ID
        /// </summary>
        protected bool IsValidId(int? id)
        {
            return id.HasValue && id.Value > 0;
        }

        /// <summary>
        /// Возврат NotFound с логированием
        /// </summary>
        protected IActionResult NotFoundWithLogging(string entityName, int? id)
        {
            Logger.LogWarning("Попытка доступа к несуществующему {EntityName} с ID: {Id}", entityName, id);
            SetErrorMessage(Constants.ErrorMessages.RecordNotFound);
            return NotFound();
        }

        /// <summary>
        /// Установка сообщения об успехе
        /// </summary>
        protected void SetSuccessMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Сообщение не может быть пустым", nameof(message));
            
            TempData[Constants.Parameters.SuccessMessage] = message;
        }

        /// <summary>
        /// Установка сообщения об ошибке
        /// </summary>
        protected void SetErrorMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Сообщение не может быть пустым", nameof(message));
            
            TempData[Constants.Parameters.ErrorMessage] = message;
        }

        /// <summary>
        /// Обработка и логирование исключений с более информативным сообщением
        /// </summary>
        protected IActionResult HandleException(Exception ex, string action, string? entityName = null)
        {
            var errorDetails = GetErrorDetails(ex);
            
            Logger.LogError(ex, 
                "Ошибка в действии {Action} контроллера {ControllerName}. {ErrorDetails}", 
                action, 
                GetControllerName(), 
                errorDetails);

            // Выбор сообщения в зависимости от типа ошибки
            string errorMessage = ex switch
            {
                InvalidOperationException => "Операция не может быть выполнена. Проверьте данные.",
                ArgumentException => "Некорректные входные параметры.",
                TimeoutException => "Превышено время ожидания. Попробуйте позже.",
                _ => Constants.ErrorMessages.UnexpectedError
            };

            SetErrorMessage(errorMessage);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Получает информацию об ошибке для логирования
        /// </summary>
        private static string GetErrorDetails(Exception ex)
        {
            return ex.InnerException != null 
                ? $"{ex.Message} -> {ex.InnerException.Message}" 
                : ex.Message;
        }

        /// <summary>
        /// Получает имя контроллера
        /// </summary>
        private string GetControllerName()
        {
            return GetType().Name.Replace("Controller", "");
        }
    }
}
