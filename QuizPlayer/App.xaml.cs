using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.IO;

namespace QuizPlayer
{
  enum MessageType
  {
    Success,
    Error
  }

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private static readonly QuizTool tool = new QuizTool();
    private void Application_Startup(object sender, StartupEventArgs ea)
    {
      /*JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, WriteIndented = true };
      var a = new Quiz() { QuizCaption = "QuizCaption", Questions = Enumerable.Repeat(new Question() { Text = "QuestionText", Answers = Enumerable.Repeat(new Answer() { Text = "AnswerText", RightAnswer = false }, 1).ToList() }, 1).ToList() };
      File.WriteAllText(QuizTool.QuizFileNameWithoutExtension+".json", JsonSerializer.Serialize(a, options));
      Current.Shutdown();
      int i = 5;
      if (i == 5)
        return;*/
      try
      {
        var convertResult = tool.TryConvertQuiz();
        switch (convertResult)
        {
          case ConvertType.CompiledQuiz:
            ShowMessageAndExit(MessageType.Success, $"Quiz '{tool.QuizFileSource}' successfuly compiled to '{tool.QuizFileCompiled}'");
            return;
          case ConvertType.DecompiledQuiz:
            ShowMessageAndExit(MessageType.Success, $"Quiz '{tool.QuizFileSourceForDecompile}' successfuly decompiled to '{tool.QuizFileSource}'");
            return;
          case ConvertType.NothingToConvert:
            break;
        }
      } catch(Exception e)
      {
        ShowMessageAndExit(MessageType.Error, $"Quiz compiling / decompiling error: '{e.Message}' for file '{QuizTool.QuizFileNameWithoutExtension}.*'");
        return;
      }
      Quiz quiz;
      try
      {
        quiz = tool.GetQuiz();
      }
      catch (System.IO.FileNotFoundException)
      {
        ShowMessageAndExit(MessageType.Error, $"Quiz file not found error. {Environment.NewLine}Supported file names: quiz.json, quiz.bin, quiz.decompile. {Environment.NewLine}For example: '{tool.QuizFileCompiled}'");
        return;
      }
      catch (Exception e)
      {
        ShowMessageAndExit(MessageType.Error, $"Quiz opening error: '{e.Message}'");
        return;
      }
      var viewModelData = new QuizDomainViewModel(quiz);
      var quizModel = new QuizModel(viewModelData.Questions);
      var quizViewModel = new QuizViewModel(viewModelData.QuizCaption, quizModel);
      var window = new MainWindow();
      window.DataContext = quizViewModel;
      window.Show();
    }

    private void ShowMessageAndExit(MessageType type, string message)
    {
      MessageBox.Show(message, "QuizPlayer", MessageBoxButton.OK, type == MessageType.Success ? MessageBoxImage.Information : MessageBoxImage.Error);
      Current.Shutdown();
    }
  }
}
