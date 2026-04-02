using GamesSharp.Models;
using ClosedXML.Excel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Excel = Microsoft.Office.Interop.Excel;

namespace GamesSharp.Services
{
    public sealed class ExcelExportResult
    {
        public string StoredFilePath { get; init; } = string.Empty;
        public string DownloadFileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }

    public interface IExcelExportService
    {
        Task<ExcelExportResult> ExportGamesAsync(IReadOnlyCollection<Game> games, CancellationToken cancellationToken = default);
        Task<ExcelExportResult> ExportPlayersAsync(IReadOnlyCollection<Player> players, CancellationToken cancellationToken = default);
    }

    public class ExcelExportService : IExcelExportService
    {
        private static readonly SemaphoreSlim ExportLock = new(1, 1);
        private static readonly TimeSpan ExportFileTtl = TimeSpan.FromHours(12);

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ExcelExportService> _logger;

        public ExcelExportService(IWebHostEnvironment environment, ILogger<ExcelExportService> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<ExcelExportResult> ExportGamesAsync(IReadOnlyCollection<Game> games, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(games);

            var headers = new[]
            {
                "ID",
                "Название",
                "Издатель",
                "Категории",
                "Мин. игроков",
                "Макс. игроков",
                "Длительность (мин)",
                "Сложность",
                "Мин. возраст",
                "Год издания"
            };

            var rows = games.Select(game => new object[]
            {
                game.Id,
                EscapeForExcel(game.Name),
                EscapeForExcel(game.Publisher?.Name),
                EscapeForExcel(string.Join(", ", game.GameCategoryAssignments
                    .Select(x => x.GameCategory?.Name)
                    .Where(x => !string.IsNullOrWhiteSpace(x)))),
                game.MinPlayers,
                game.MaxPlayers,
                game.AverageDuration,
                game.Complexity?.ToString() ?? "-",
                game.MinAge?.ToString() ?? "-",
                game.YearPublished?.ToString() ?? "-"
            }).ToList();

            return ExportToExcelAsync("Игры", "games", headers, rows, cancellationToken);
        }

        public Task<ExcelExportResult> ExportPlayersAsync(IReadOnlyCollection<Player> players, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(players);

            var headers = new[]
            {
                "ID",
                "Имя",
                "Email",
                "Телефон",
                "Город",
                "Любимый жанр",
                "Дата рождения",
                "Дата регистрации"
            };

            var rows = players.Select(player => new object[]
            {
                player.Id,
                EscapeForExcel(player.Name),
                EscapeForExcel(player.Email),
                EscapeForExcel(player.Phone),
                EscapeForExcel(player.City),
                EscapeForExcel(player.FavoriteGenre),
                player.BirthDate?.ToString("dd.MM.yyyy") ?? "-",
                player.RegisteredDate.ToString("dd.MM.yyyy HH:mm")
            }).ToList();

            return ExportToExcelAsync("Игроки", "players", headers, rows, cancellationToken);
        }

        private async Task<ExcelExportResult> ExportToExcelAsync(
            string sheetName,
            string filePrefix,
            IReadOnlyList<string> headers,
            IReadOnlyList<object[]> rows,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var exportDirectory = EnsureExportDirectory();
            CleanupOldExports(exportDirectory);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
            var physicalFileName = $"{filePrefix}_{timestamp}_{uniqueSuffix}.xlsx";
            var physicalPath = Path.Combine(exportDirectory, physicalFileName);
            var downloadName = $"{filePrefix}_export_{timestamp}.xlsx";

            if (!OperatingSystem.IsWindows())
            {
                await ExportWithClosedXmlAsync(physicalPath, sheetName, headers, rows, cancellationToken);

                _logger.LogInformation("Excel-файл сохранен (ClosedXML): {PhysicalPath}", physicalPath);
                return new ExcelExportResult
                {
                    StoredFilePath = physicalPath,
                    DownloadFileName = downloadName
                };
            }

            await ExportLock.WaitAsync(cancellationToken);

            Excel.Application? excelApp = null;
            Excel.Workbook? workbook = null;
            Excel.Worksheet? worksheet = null;
            Excel.Range? headerRange = null;
            Excel.Range? dataRange = null;

            try
            {
                excelApp = new Excel.Application
                {
                    DisplayAlerts = false,
                    Visible = false,
                    ScreenUpdating = false
                };

                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                worksheet.Name = NormalizeSheetName(sheetName);

                for (var col = 0; col < headers.Count; col++)
                {
                    worksheet.Cells[1, col + 1] = headers[col];
                }

                if (rows.Count > 0)
                {
                    var values = new object[rows.Count, headers.Count];
                    for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var row = rows[rowIndex];
                        for (var col = 0; col < headers.Count; col++)
                        {
                            values[rowIndex, col] = col < row.Length ? row[col] : string.Empty;
                        }
                    }

                    dataRange = worksheet.Range[
                        worksheet.Cells[2, 1],
                        worksheet.Cells[rows.Count + 1, headers.Count]];
                    dataRange.Value2 = values;
                }

                headerRange = worksheet.Range[
                    worksheet.Cells[1, 1],
                    worksheet.Cells[1, headers.Count]];

                headerRange.Font.Bold = true;
                headerRange.Interior.Color = 0xD9E1F2;
                headerRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                var usedRange = worksheet.UsedRange;
                usedRange.Columns.AutoFit();
                ReleaseComObject(usedRange);

                workbook.SaveAs(physicalPath, Excel.XlFileFormat.xlOpenXMLWorkbook);

                _logger.LogInformation("Excel-файл сохранен: {PhysicalPath}", physicalPath);

                return new ExcelExportResult
                {
                    StoredFilePath = physicalPath,
                    DownloadFileName = downloadName
                };
            }
            catch (COMException ex)
            {
                _logger.LogError(ex, "Ошибка COM при экспорте в Excel");
                throw new InvalidOperationException("Не удалось создать Excel-файл. Проверьте наличие установленного Microsoft Excel.", ex);
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close(false);
                }

                if (excelApp != null)
                {
                    excelApp.Quit();
                }

                ReleaseComObject(dataRange);
                ReleaseComObject(headerRange);
                ReleaseComObject(worksheet);
                ReleaseComObject(workbook);
                ReleaseComObject(excelApp);

                ExportLock.Release();
            }
        }

        private static Task ExportWithClosedXmlAsync(
            string physicalPath,
            string sheetName,
            IReadOnlyList<string> headers,
            IReadOnlyList<object[]> rows,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(NormalizeSheetName(sheetName));

            for (var col = 0; col < headers.Count; col++)
            {
                worksheet.Cell(1, col + 1).Value = headers[col];
                worksheet.Cell(1, col + 1).Style.Font.Bold = true;
                worksheet.Cell(1, col + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E1F2");
            }

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                for (var col = 0; col < headers.Count; col++)
                {
                    var value = col < row.Length ? row[col]?.ToString() ?? string.Empty : string.Empty;
                    worksheet.Cell(rowIndex + 2, col + 1).Value = value;
                }
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(physicalPath);

            return Task.CompletedTask;
        }

        private string EnsureExportDirectory()
        {
            var path = Path.Combine(_environment.ContentRootPath, "App_Data", "Exports");
            Directory.CreateDirectory(path);
            return path;
        }

        private void CleanupOldExports(string exportDirectory)
        {
            try
            {
                var threshold = DateTime.UtcNow.Subtract(ExportFileTtl);
                var files = Directory.EnumerateFiles(exportDirectory, "*.xlsx", SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTimeUtc < threshold)
                    {
                        fileInfo.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось очистить устаревшие Excel-файлы");
            }
        }

        private static string EscapeForExcel(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            var trimmed = value.Trim();
            return trimmed.Length > 0 && "=+-@".Contains(trimmed[0])
                ? $"'{trimmed}"
                : trimmed;
        }

        private static string NormalizeSheetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Лист1";
            }

            var invalid = new[] { '\\', '/', '?', '*', '[', ']', ':' };
            var normalized = value;
            foreach (var ch in invalid)
            {
                normalized = normalized.Replace(ch, '_');
            }

            return normalized.Length > 31
                ? normalized[..31]
                : normalized;
        }

        [SupportedOSPlatform("windows")]
        private static void ReleaseComObject(object? comObject)
        {
            if (comObject == null)
            {
                return;
            }

            try
            {
                Marshal.FinalReleaseComObject(comObject);
            }
            catch
            {
                // Игнорируем ошибки очистки COM-объектов.
            }
        }
    }
}
