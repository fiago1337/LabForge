using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
namespace LabForge.Services;

public class MethodologyReader
{
    public string ReadWordDocument(string filePath)
    {
        using WordprocessingDocument document =
            WordprocessingDocument.Open(filePath, false);

        Body? body = document.MainDocumentPart?.Document?.Body;

        if (body == null)
        {
            return "У документі не знайдено тексту.";
        }

        return body.InnerText;
    }
}