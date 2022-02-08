using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizPlayer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public void AnswerChanged(object sender, RoutedEventArgs e)
    {
      if (DataContext is IAnswerChanged eventProvider)
        eventProvider.AnswerChanged();
    }
    public MainWindow()
    {
      InitializeComponent();
    }
  }
}
