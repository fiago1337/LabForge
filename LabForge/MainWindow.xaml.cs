
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
            Microsoft.Win32.OpenFileDialog dialog =
                new Microsoft.Win32.OpenFileDialog();

            dialog.Title = "Выберите файл с методичкой";
            dialog.Filter = "Документы Word (*.docx)|*.docx";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                SelectedFileText.Text = "Выбран файл: " + dialog.FileName;
                StatusText.Text = "Чтение документа...";

                try
                {
                    MethodologyReader reader = new MethodologyReader();
                    string text = reader.ReadWordDocument(dialog.FileName);
                    if(string.IsNullOrWhiteSpace(text))
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
    }
}
