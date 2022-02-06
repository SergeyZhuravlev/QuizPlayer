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
    private static readonly TestTool tool = new TestTool();
    private void Application_Startup(object sender, StartupEventArgs ea)
    {
      /*JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, WriteIndented = true };
      var a = new Test() { TestCaption = "TestCaption", Questions = Enumerable.Repeat(new Question() { Text = "QuestionText", Answers = Enumerable.Repeat(new Answer() { Text = "AnswerText", RightAnswer = false }, 1).ToList() }, 1).ToList() };
      File.WriteAllText(TestTool.TestFileNameWithoutExtension+".json", JsonSerializer.Serialize(a, options));
      Current.Shutdown();
      int i = 5;
      if (i == 5)
        return;*/
      try
      {
        var convertResult = tool.TryConvertTest();
        switch (convertResult)
        {
          case ConvertType.CompiledTest:
            ShowMessageAndExit(MessageType.Success, $"Test '{tool.TestFileSource}' successfuly compiled to '{tool.TestFileCompiled}'");
            return;
          case ConvertType.DecompiledTest:
            ShowMessageAndExit(MessageType.Success, $"Test '{tool.TestFileSourceForDecompile}' successfuly decompiled to '{tool.TestFileSource}'");
            return;
          case ConvertType.NothingToConvert:
            break;
        }
      } catch(Exception e)
      {
        ShowMessageAndExit(MessageType.Error, $"Test compiling / decompiling error: '{e.Message}' for file '{TestTool.TestFileNameWithoutExtension}.*'");
        return;
      }
      Test test;
      try
      {
        test = tool.GetTest();
      }
      catch (System.IO.FileNotFoundException)
      {
        ShowMessageAndExit(MessageType.Error, $"Test file not found error. {Environment.NewLine}Supported file names: test.json, test.bin, test.decompile. {Environment.NewLine}For example: '{tool.TestFileCompiled}'");
        return;
      }
      catch (Exception e)
      {
        ShowMessageAndExit(MessageType.Error, $"Test opening error: '{e.Message}'");
        return;
      }
      var testModel = new TestModel(test.RandomizedQuestions);
      var testViewModel= new TestViewModel(test.TestCaption, testModel);
      var window = new MainWindow();
      window.DataContext = testViewModel;
      window.Show();
    }

    private void ShowMessageAndExit(MessageType type, string message)
    {
      MessageBox.Show(message, "QuizPlayer", MessageBoxButton.OK, type == MessageType.Success ? MessageBoxImage.Information : MessageBoxImage.Error);
      Current.Shutdown();
    }
  }
}
