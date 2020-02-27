using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using School.Data;
using System.Globalization;

namespace School
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Connection to the School database
        private SchoolDBEntities schoolContext = null;

        // Field for tracking the currently selected teacher
        private Teacher teacher = null;

        // List for tracking the students assigned to the teacher's class
        private IList studentsInfo = null;

        #region Predefined code

        public MainWindow()
        {
            InitializeComponent();
        }

        // Connect to the database and display the list of teachers when the window appears
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.schoolContext = new SchoolDBEntities();
            teachersList.DataContext = this.schoolContext.Teachers;
        }

        // When the user selects a different teacher, fetch and display the students for that teacher
        private void teachersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Find the teacher that has been selected
            this.teacher = teachersList.SelectedItem as Teacher;
            this.schoolContext.LoadProperty<Teacher>(this.teacher, s => s.Students);

            // Find the students for this teacher
            this.studentsInfo = ((IListSource)teacher.Students).GetList();

            // Use databinding to display these students
            studentsList.DataContext = this.studentsInfo;
        }

        #endregion

        // When the user presses a key, determine whether to add a new student to a class, remove a student from a class, or modify the details of a student
        private void studentsList_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: Exercise 1: Task 1a: If the user pressed Enter, edit the details for the currently selected student
            switch (e.Key)
            {
                case Key.Enter:
                    EditCurrentStudent();
                    break;
                case Key.Insert:
                    InsertNewStudent();
                    break;
                case Key.Delete:
                    DeleteCurrentStudent();
                    break;
                default:
                    break;
            }
            // TODO: Exercise 1: Task 2a: Use the StudentsForm to display and edit the details of the student
            // TODO: Exercise 1: Task 2b: Set the title of the form and populate the fields on the form with the details of the student
            // TODO: Exercise 1: Task 3a: Display the form
            // TODO: Exercise 1: Task 3b: When the user closes the form, copy the details back to the student
            // TODO: Exercise 1: Task 3c: Enable saving (changes are not made permanent until they are written back to the database)
        }

        private void DeleteCurrentStudent()
        {
            Student selectedStudent = (studentsList.SelectedItem as Student);
            if (selectedStudent == null)
            {
                return;
            }

            ConfirmDeleteOfStudent(selectedStudent);
        }

        private void ConfirmDeleteOfStudent(Student student)
        {
            string message = $"Confirm delete of {student.FirstName} {student.LastName}?";
            if (MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.teacher.Students.Remove(student);
            }
        }

        private void InsertNewStudent()
        {
            Student newStudent = new Student();
            ConfigureAndShowNewStudentForm(newStudent);
        }

        private void ConfigureAndShowNewStudentForm(Student student)
        {
            StudentForm frmStudent = new StudentForm();
            frmStudent.Title = $"New Student for Class {teacher.Class}";

            if (frmStudent.ShowDialog().GetValueOrDefault())
            {
                CopyData(student, frmStudent);
                saveChanges.IsEnabled = true;
                this.teacher.Students.Add(student);
            }
        }

        private void EditCurrentStudent()
        {
            Student selectedStudent = (studentsList.SelectedItem as Student);
            if(selectedStudent == null)
            {
                return;
            }

            ConfigureAndShowStudentForm(selectedStudent);
        }

        private void ConfigureAndShowStudentForm(Student student)
        {
            StudentForm frmStudent = new StudentForm();
            frmStudent.Title = $"Editing {student.FirstName} {student.LastName}";
            frmStudent.firstName.Text = student.FirstName;
            frmStudent.lastName.Text = student.LastName;
            frmStudent.dateOfBirth.Text = student.DateOfBirth.ToShortDateString();

            if(frmStudent.ShowDialog().GetValueOrDefault())
            {
                CopyData(student, frmStudent);
                saveChanges.IsEnabled = true;
            }
        }

        private void CopyData(Student student, StudentForm frmStudent)
        {
            student.FirstName = frmStudent.firstName.Text;
            student.LastName = frmStudent.lastName.Text;
            if(DateTime.TryParse(frmStudent.dateOfBirth.Text, out DateTime dateTime))
            {
                student.DateOfBirth = dateTime;
            }
        }

        #region Predefined code

        private void studentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
 
        }

        // Save changes back to the database and make them permanent
        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(Decimal))]
    class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
            {
                return string.Empty;
            }

            if(DateTime.TryParse(value.ToString(), out DateTime dateTime))
            {
                TimeSpan ts = DateTime.Now.Subtract(dateTime);
                int years = (int)(ts.Days / 365.25);
                return years.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        #region Predefined code

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
