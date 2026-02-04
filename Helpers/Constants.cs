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
            public const string Scheduled = "Запланирована";
            public const string InProgress = "В процессе";
            public const string Completed = "Завершена";
            public const string Cancelled = "Отменена";
        }

        /// <summary>
        /// Сообщения об ошибках
        /// </summary>
        public static class ErrorMessages
        {
            public const string RecordNotFound = "Запись не найдена";
            public const string InvalidId = "Некорректный идентификатор";
            public const string DatabaseError = "Ошибка базы данных";
            public const string ValidationError = "Ошибка валидации данных";
            public const string UnexpectedError = "Произошла непредвиденная ошибка";
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
