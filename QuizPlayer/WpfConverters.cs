using System;
using System.Globalization;
using System.Linq;
using System.Windows;
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
        throw new InvalidOperationException("The target must be a boolean");
      return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !(bool)value;
    }
  }

  [ValueConversion(typeof(Answer), typeof(SolidColorBrush))]
  class AnswerStateToColorBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is AnswerState state)
      {
        switch (state)
        {
          case AnswerState.NotAnswered:
            return new SolidColorBrush(Colors.Transparent);
          case AnswerState.RightAnswered:
            return new SolidColorBrush(Colors.Green);
          case AnswerState.WrongAnswered:
            return new SolidColorBrush(Colors.Red);
        }
      }

      return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
  class BooleanToARGBColorBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool boolean)
        return BoolToColor(boolean, parameter);

      return Binding.DoNothing;
    }

    private SolidColorBrush BoolToColor(bool value, object parameter)
    {
      var error = nameof(parameter) + " must be AARRGGBB|AARRGGBB (for false|for true)";
      var colors = ((string)parameter).Split("|");
      if (colors.Length != 2 || colors[0].Length != 8 || colors[1].Length != 8)
        throw new ArgumentException(error);
      var colorString = value ? colors[1] : colors[0];
      var argb = colorString.ChunkSplit(2).Select(v => byte.Parse(v, NumberStyles.HexNumber)).ToArray();
      var color = Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
      return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class BooleanToVisibilityConverter : IValueConverter
  {
    enum Parameters
    {
      TrueToVisible,
      FalseToVisible
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var boolValue = (bool)value;
      var direction = Enum.Parse<Parameters>((string)parameter);

      if (direction == Parameters.TrueToVisible)
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
      return !boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
