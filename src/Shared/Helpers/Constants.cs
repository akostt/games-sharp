namespace GamesSharp.Helpers
{
    /// <summary>
    /// Класс для хранения констант приложения
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Статусы игровых сессий
        /// </summary>
        public static class SessionStatus
        {
            public const int ScheduledId = 1;
            public const int InProgressId = 2;
            public const int CompletedId = 3;
            public const int CancelledId = 4;

            public const string ScheduledCode = "scheduled";
            public const string InProgressCode = "in_progress";
            public const string CompletedCode = "completed";
            public const string CancelledCode = "cancelled";
        }

        /// <summary>
        /// Сообщения об ошибках
        /// </summary>
        public static class ErrorMessages
        {
            public const string RecordNotFound = "Запись не найдена";
            public const string InvalidId = "Некорректный идентификатор";
            public const string DatabaseError = "Ошибка базы данных. Попробуйте позже.";
            public const string ValidationError = "Ошибка валидации данных";
            public const string UnexpectedError = "Произошла непредвиденная ошибка. Пожалуйста, обратитесь в поддержку.";
            public const string ConcurrencyError = "Запись была изменена другим пользователем. Пожалуйста, обновите страницу и попробуйте снова.";
            public const string DeleteError = "Невозможно удалить запись. Возможно, на нее ссылаются другие записи.";
        }

        /// <summary>
        /// Сообщения об успехе
        /// </summary>
        public static class SuccessMessages
        {
            public const string RecordCreated = "Запись успешно создана";
            public const string RecordUpdated = "Запись успешно обновлена";
            public const string RecordDeleted = "Запись успешно удалена";
        }

        /// <summary>
        /// Названия параметров
        /// </summary>
        public static class Parameters
        {
            public const string Id = "id";
            public const string ErrorMessage = "ErrorMessage";
            public const string SuccessMessage = "SuccessMessage";
        }

        /// <summary>
        /// Валидационные константы
        /// </summary>
        public static class Validation
        {
            // Длины строк
            public const int MaxNameLength = 100;
            public const int MaxDescriptionLength = 1000;
            public const int MaxCityLength = 50;
            public const int MaxGenreLength = 50;

            // Диапазоны чисел
            public const int MinPlayers = 1;
            public const int MaxPlayers = 100;
            public const int MinDuration = 1;
            public const int MaxDuration = 1000;
            public const int MinComplexity = 1;
            public const int MaxComplexity = 10;
            public const int MinAge = 0;
            public const int MaxAge = 99;
            public const int MaxYearPublished = 2100;

            // Пагинация
            public const int DefaultPageSize = 10;
            public const int MaxPageSize = 100;
            public const int MinPageSize = 1;
        }

        /// <summary>
        /// Названия категорий логирования
        /// </summary>
        public static class LogCategories
        {
            public const string Controllers = "Controllers";
            public const string Data = "Data";
            public const string Services = "Services";
        }
    }
}
