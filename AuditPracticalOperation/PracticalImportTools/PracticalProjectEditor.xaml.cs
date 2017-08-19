using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common.Utils;
using System.Globalization;

namespace PracticalImportTools
{
    /// <summary>
    /// PracticalProjectEditor.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalProjectEditor : Window
    {
        private PracticalFile practicalFile;

        public PracticalProjectEditor(PracticalFile practicalFile)
        {
            this.practicalFile = practicalFile;
            InitializeComponent();
            if (!this.IsInDesignMode())
                container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = practicalFile });
        }

        public static RoutedUICommand AddProject = new RoutedUICommand("Add a new Project", "AddProject", typeof(PracticalProjectEditor));

        private void AddProjectExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (practicalFile.Projects.Count > 0)
                practicalFile.Projects[practicalFile.Projects.Count - 1].IsLast = false;

            PracticalProject project = new PracticalProject();
            project.IsLast = true;
            project.Index = practicalFile.Projects.Count;
            practicalFile.Projects.Add(project);
        }

        public static RoutedUICommand DeleteProject = new RoutedUICommand("Delete the Project", "DeleteProject", typeof(PracticalProjectEditor));

        private void DeleteProjectExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            practicalFile.Projects.RemoveAt((int)e.Parameter);
            for (int i = (int)e.Parameter; i < practicalFile.Projects.Count; i++)
            {
                practicalFile.Projects[i].Index = i;
                practicalFile.Projects[i].IsLast = i == practicalFile.Projects.Count - 1;
            }
        }
    }

    public class PracticalProjectNumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
