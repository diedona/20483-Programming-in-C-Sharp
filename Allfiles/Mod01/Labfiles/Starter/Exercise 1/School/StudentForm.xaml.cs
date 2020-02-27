using System;
using System.Windows;
using System.Windows.Data;

namespace School
{
    /// <summary>
    /// Interaction logic for StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        #region Predefined code

        public StudentForm()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(DateTime))]
    class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                return dateTime.ToString("dd/MM/yyyy");
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,System.Globalization.CultureInfo culture)
        {
            if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                return dateTime;
            else
                return new DateTime();
        }
    }
}
