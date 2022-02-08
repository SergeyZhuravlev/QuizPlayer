using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizPlayer
{
  [ValueConversion(typeof(bool), typeof(bool))]
  public class InverseBooleanConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(bool))
        throw new InvalidOperationException("The target must be a nullable boolean");
      return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !(bool)value;
    }
  }

  [ValueConversion(typeof(Answer), typeof(SolidColorBrush))]
  class AnswerToColorBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is AnswerViewModel answer)
      {
        if (!answer.UserAnswered)
          return new SolidColorBrush(Colors.Transparent);
        return answer.UserRightAnswered ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
      }

      return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
