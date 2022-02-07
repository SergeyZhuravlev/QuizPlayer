using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace QuizPlayer
{
  public static class ShuffleExtension
  {
    private static readonly Random rnd = new();

    public static List<T> Shuffle<T>(this IEnumerable<T> source) // Not thread safe with rnd field
    {
      return source.OrderBy(a => rnd.Next()).ToList();
    }
  }
  public static class CryptByteArrayExtension
  {
    private const byte XorCrypto = 0xA5;
    public static byte[] Recrypt(this byte[] source)
    {
      return source.Select(b => (byte)(b ^ XorCrypto)).ToArray();
    }
  }

  public enum ConvertType
  {
    CompiledQuiz,
    DecompiledQuiz,
    NothingToConvert
  }

  public class QuizTool
  {
    public static readonly string QuizFileNameWithoutExtension = Path.Combine(AppContext.BaseDirectory, "quiz");

    public readonly string QuizFileSource = $"{QuizFileNameWithoutExtension}.json";
    public readonly string QuizFileCompiled = $"{QuizFileNameWithoutExtension}.bin";
    public readonly string QuizFileSourceForDecompile = $"{QuizFileNameWithoutExtension}.decompile";
    public ConvertType TryConvertQuiz()
    {
      if (File.Exists(QuizFileSource))
      {
        Compile();
        return ConvertType.CompiledQuiz;
      }
      if (File.Exists(QuizFileSourceForDecompile))
      {
        Decompile();
        return ConvertType.DecompiledQuiz;
      }
      return ConvertType.NothingToConvert;
    }

    private void Decompile()
    {
      var compiled = File.ReadAllBytes(QuizFileSourceForDecompile);
      var source = compiled.Recrypt();
      try
      {
        _ = GetQuiz(source);
      } catch
      {
        throw new Exception($"File '{QuizFileSourceForDecompile}' not contain compiled .bin quiz file. It file broken or wrongly renamed from *.json file. You should rename only *.bin file to *.decompile file");
      }
      File.WriteAllBytes(QuizFileSource, source);
      File.Delete(QuizFileSourceForDecompile);
    }

    private void Compile()
    {
      var source = File.ReadAllBytes(QuizFileSource);
      _ = GetQuiz(source);
      File.WriteAllBytes(QuizFileCompiled, source.Recrypt());
      File.Delete(QuizFileSource);
    }

    public Quiz GetQuiz()
    {
      if (!File.Exists(QuizFileCompiled))
        throw new FileNotFoundException(QuizFileCompiled);

      return GetQuiz(File.ReadAllBytes(QuizFileCompiled).Recrypt());
    }

    private static Quiz GetQuiz(byte[] utf8Quiz)
    {
      Utf8JsonReader jsonUtfReader = new(utf8Quiz, new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
      JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, WriteIndented = true };
      var quiz = JsonSerializer.Deserialize<Quiz>(ref jsonUtfReader, options);
      CheckQuiz(quiz);
      return quiz;
    }

    private static void CheckQuiz(Quiz quiz)
    {
      if (string.IsNullOrWhiteSpace(quiz.QuizCaption))
        throw new Exception("Empty 'QuizCaption' field");
      if (quiz.Questions is null || quiz.Questions.Count == 0)
        throw new Exception("Empty 'Questions' list");
      { 
        if (quiz.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => string.IsNullOrWhiteSpace(q.question.Text))
          is var failed && !(failed is null))
            throw new Exception($"Empty question 'Text' field in {failed.index} question");
      }
      {
        if (quiz.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers is null || q.question.Answers.Count < 2)
          is var failed && !(failed is null))
          throw new Exception($"Empty or small amount in 'Answers' list in {failed.index} question: '{failed.question.Text}'");
      }
      {
        if (quiz.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text)))
          is var failed && !(failed is null))
            throw new Exception($"Empty answer 'Text' field in some of answer in {failed.index} question: '{failed.question.Text}'");
      }
      {
        if (quiz.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers.All(a => a.RightAnswer))
          is var failed && !(failed is null))
            throw new Exception($"See 'RightAnswer' field. All answers is right in {failed.index} question: '{failed.question.Text}'");
      }
      {
        if (quiz.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers.All(a => !a.RightAnswer)) 
          is var failed && !(failed is null))
            throw new Exception($"See 'RightAnswer' field. All answers is not right in {failed.index} question: '{failed.question.Text}'");
      }
    }
  }
}
