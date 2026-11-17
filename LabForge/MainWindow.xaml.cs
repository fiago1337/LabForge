using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LabForge.Services;
using Microsoft.Win32;

namespace LabForge
{
    public partial class MainWindow : Window
    {
        private const string NoFileText = "Методичка не выбрана";
        private const string PlaceholderText = "Здесь появится содержимое документа.";
        private const string IdleStatus = "Ожидание выбора методички";

        private string _documentText = string.Empty;

        // Сервис создаётся один раз при первом использовании.
        private GeminiService? _geminiService;

        public MainWindow()
        {
            InitializeComponent();
            ResetUi();
        }

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_documentText))
            {
                ShowMessage(
                    "Пожалуйста, выберите методичку и убедитесь, что текст документа загружен.",
                    "Нет данных",
                    MessageBoxImage.Warning);
                return;
            }

            var button = (Button)sender;
            button.IsEnabled = false; // защита от повторных нажатий во время запроса
            StatusText.Text = "Анализ документа...";

            try
            {
                string result = await GetGeminiService().GenerateTextAsync(_documentText);
                DocumentText.Text = result;
                StatusText.Text = "Анализ завершён.";
            }
            catch (Exception ex)
            {
                ShowMessage(
                    "Произошла ошибка при анализе документа:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxImage.Error);
                StatusText.Text = "Ошибка при анализе документа.";
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите файл с методичкой",
                Filter = "Документы (*.docx;*.pdf)|*.docx;*.pdf"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var button = (Button)sender;
            button.IsEnabled = false;

            string path = dialog.FileName;
            SelectedFileText.Text = "Выбран файл: " + Path.GetFileName(path);
            DocumentText.Text = "Загрузка текста...";
            StatusText.Text = "Чтение документа...";

            try
            {
                // Чтение файла — в фоновом потоке, чтобы окно не зависало на больших документах.
                string text = await Task.Run(() => ReadDocument(path));

                if (string.IsNullOrWhiteSpace(text))
                {
                    _documentText = string.Empty;
                    DocumentText.Text = "В документе не найдено текста.";
                    StatusText.Text = "Текст не найден.";
                }
                else
                {
                    _documentText = text;
                    DocumentText.Text = text;
                    StatusText.Text = "Документ успешно прочитан.";
                }
            }
            catch (Exception ex)
            {
                _documentText = string.Empty;
                DocumentText.Text = PlaceholderText;
                ShowMessage(
                    "Не удалось прочитать документ:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxImage.Error);
                StatusText.Text = "Ошибка при чтении документа.";
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUi();
        }

        private static string ReadDocument(string path)
        {
            string extension = Path.GetExtension(path);

            if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            {
                return new MethodologyReader().ReadWordDocument(path);
            }

            if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return new PdfReaderService().ReadPdfDocument(path);
            }

            throw new NotSupportedException($"Формат \"{extension}\" не поддерживается.");
        }

        private GeminiService GetGeminiService()
        {
            if (_geminiService is null)
            {
                // Ключ берётся из api.json / переменных окружения, а не из кода.
                ApiConfig config = new ApiConfigLoader().Load();
                _geminiService = new GeminiService(config.ApiKey, config.Model);
            }

            return _geminiService;
        }

        private void ResetUi()
        {
            _documentText = string.Empty;
            SelectedFileText.Text = NoFileText;
            DocumentText.Text = PlaceholderText;
            StatusText.Text = IdleStatus;
        }

        private void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            MessageBox.Show(this, message, title, MessageBoxButton.OK, icon);
        }
    }
}