
using System;
using System.Windows;
using LabForge.Services;

namespace LabForge
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Title = "Выберите файл с методичкой";
            dialog.Filter = "Документы (*.docx;*.pdf)|*.docx;*.pdf";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string fileName = System.IO.Path.GetFileName(dialog.FileName);
                SelectedFileText.Text = "Выбран файл: " + fileName;
                DocumentText.Text = "Загрузка текста...";
                StatusText.Text = "Чтение документа...";

                try
                {
                    string extension = System.IO.Path.GetExtension(dialog.FileName);
                    string text;

                    if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
                    {
                        MethodologyReader reader = new MethodologyReader();
                        text = reader.ReadWordDocument(dialog.FileName);
                    }
                    else if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        PdfReaderService reader = new PdfReaderService();
                        text = reader.ReadPdfDocument(dialog.FileName);
                    }
                    else
                    {
                        text = "";
                    }

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        DocumentText.Text = "В документе не найдено текста.";
                        StatusText.Text = "Текст не найден.";
                    }
                    else
                    {
                        DocumentText.Text = text;
                        StatusText.Text = "Документ успешно прочитан.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Не удалось прочитать документ:\n" + ex.Message,
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    StatusText.Text = "Ошибка при чтении документа.";
                }
            }
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileText.Text = "Методичка не выбрана";
            DocumentText.Text = "Здесь появится содержимое документа.";
            StatusText.Text = "Ожидание выбора методички";
        }
    }
}
