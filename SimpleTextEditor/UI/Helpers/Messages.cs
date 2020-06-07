using System;
using System.Text;
using System.Windows;

namespace SimpleTextEditor.UI
{
    public static class InformationMessage
    {
        public static void Show(string message, Window owner = null)
        {
            if (owner == null)
                MessageBox.Show(message ?? string.Empty, "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(owner, message ?? string.Empty, "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public static class WarningMessage
    {
        public static void Show(string message, Window owner = null)
        {
            if(owner == null)
                MessageBox.Show(message ?? string.Empty, "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MessageBox.Show(owner, message ?? string.Empty, "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    public static class ErrorMessage
    {
        public static void Show(Exception exception, Window owner = null)
        {
            if (exception == null)
                return;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Wystąpił nieoczekiwany błąd w aplikacji i musi ona zostać zamknięta. Przepraszamy za kłopoty.");
            builder.AppendLine();
            builder.AppendLine($"Wystąpił wyjątek: {exception.GetType().FullName}");
            if ((exception.Message?.Length ?? 0) > 0)
                builder.AppendLine($"Wiadomość: {exception.Message}");
            builder.Append(exception.StackTrace ?? string.Empty);

            if(owner == null)
                MessageBox.Show(builder.ToString(), "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(owner, builder.ToString(), "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void Show(string customMessage, Window owner = null)
        {
            if(owner == null)
                MessageBox.Show(customMessage, "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(owner, customMessage, "Prosty edytor tekstu", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
