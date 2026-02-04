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
            Context = context;
            Logger = logger;
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
            Logger.LogWarning($"{entityName} с ID {id} не найден");
            TempData[Constants.Parameters.ErrorMessage] = Constants.ErrorMessages.RecordNotFound;
            return NotFound();
        }

        /// <summary>
        /// Установка сообщения об успехе
        /// </summary>
        protected void SetSuccessMessage(string message)
        {
            TempData[Constants.Parameters.SuccessMessage] = message;
        }

        /// <summary>
        /// Установка сообщения об ошибке
        /// </summary>
        protected void SetErrorMessage(string message)
        {
            TempData[Constants.Parameters.ErrorMessage] = message;
        }

        /// <summary>
        /// Логирование и возврат ошибки
        /// </summary>
        protected IActionResult HandleException(Exception ex, string action)
        {
            Logger.LogError(ex, $"Ошибка при выполнении действия: {action}");
            SetErrorMessage(Constants.ErrorMessages.UnexpectedError);
            return RedirectToAction("Index");
        }
    }
}
