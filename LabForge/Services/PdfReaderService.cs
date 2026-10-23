using System;
using System.IO;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace LabForge.Services
{
    public class PdfReaderService
    {
        /// <exception cref="ArgumentException">Путь пустой.</exception>
        /// <exception cref="FileNotFoundException">Файл не найден.</exception>
        /// <exception cref="InvalidDataException">Файл не удалось прочитать как PDF.</exception>
        public string ReadPdfDocument(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не указан.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            try
            {
                using var document = PdfDocument.Open(filePath);
                var text = new StringBuilder();

                foreach (var page in document.GetPages())
                {
                    // ContentOrderTextExtractor сохраняет порядок чтения и пробелы между словами,
                    // в отличие от page.Text, который часто склеивает слова.
                    text.AppendLine(ContentOrderTextExtractor.GetText(page));
                }

                return text.ToString().Trim();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                throw new InvalidDataException($"Ошибка чтения PDF: {ex.Message}", ex);
            }
        }

        public bool TryReadPdfDocument(string filePath, out string result, out string? error)
        {
            try
            {
                result = ReadPdfDocument(filePath);
                if (string.IsNullOrWhiteSpace(result))
                {
                    error = "В документе нет текстового слоя (возможно, это скан — нужен OCR).";
                    return false;
                }

                error = null;
                return true;
            }
            catch (Exception ex)
            {
                result = string.Empty;
                error = ex.Message;
                return false;
            }
        }
    }
}