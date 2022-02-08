using System.Windows;

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
