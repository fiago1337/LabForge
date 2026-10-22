using System;
using System.IO;
using System.Text;
using UglyToad.PdfPig;

namespace LabForge.Services
{
    public class PdfReaderService
    {
        public string ReadPdfDocument(string filePath)
        {
            if (!File.Exists(filePath))
                return "Файл не найден";

            try
            {
                using PdfDocument document = PdfDocument.Open(filePath);
                var text = new StringBuilder();
                foreach (var page in document.GetPages())
                {
                    text.AppendLine(page.Text);
                }
                if (string.IsNullOrWhiteSpace(text.ToString()))
                {
                    return "Документ пустой";
                }
                return text.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка чтения PDF: {ex.Message}";
            }
        }
    }
}