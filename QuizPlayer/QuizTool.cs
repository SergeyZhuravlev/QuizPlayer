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

  public static class StringExtension
  {
    public static IEnumerable<string> ChunkSplit(this string str, int chunkSize)
    {
      return Enumerable.Range(0, str.Length / chunkSize)
          .Select(i => str.Substring(i * chunkSize, chunkSize));
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
        throw new($"File '{QuizFileSourceForDecompile}' not contain compiled .bin quiz file. It file broken or wrongly renamed from *.json file. You should rename only *.bin file to *.decompile file");
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
      FillQuiz(quiz);
      CheckQuiz(quiz);
      return quiz;
    }

    private static void FillQuiz(Quiz quiz)
    {
      foreach (var (question, index) in quiz.Questions.Select((question, index) => (question, index)))
      {
        question.BaseQuestionNumber = index;
        if (!question.OnlyOneRightAnswerPerQuestion.HasValue)
          question.OnlyOneRightAnswerPerQuestion = quiz.OnlyOneRightAnswerPerQuestionByDefault;
        if (!question.OnlyOneRightAnswerPerQuestion.HasValue)
          question.OnlyOneRightAnswerPerQuestion = false;
      }
      if (!quiz.QuestionsPerQuiz.HasValue)
        quiz.QuestionsPerQuiz = quiz.Questions is null ? 0 : quiz.Questions.Count;
      if (!quiz.MinimalAnsweredQuestionsPercentForQuizSuccess.HasValue)
        quiz.MinimalAnsweredQuestionsPercentForQuizSuccess = 0;
    }

    private static void CheckQuiz(Quiz quiz)
    {
      if (string.IsNullOrWhiteSpace(quiz.QuizCaption))
        throw new("Empty 'QuizCaption' field in root");
      if (quiz.Questions is null || quiz.Questions.Count == 0)
        throw new("Empty 'Questions' list in root");
      if (quiz.QuestionsPerQuiz < 0 || quiz.QuestionsPerQuiz > quiz.Questions.Count)
        throw new("Field 'QuestionsPerQuiz' in root is wrong");
      if (quiz.MinimalAnsweredQuestionsPercentForQuizSuccess < 0 || quiz.MinimalAnsweredQuestionsPercentForQuizSuccess > 100)
        throw new("Field 'MinimalAnsweredQuestionsPercentForQuizSuccess' in root is wrong. Should be in 0-100 range.");
      {
        if (quiz.Questions.FirstOrDefault(q => string.IsNullOrWhiteSpace(q.Text)) is Question failed)
            throw new($"Empty question 'Text' field in {failed.BaseQuestionNumber} question");
      }
      {
        if (quiz.Questions.FirstOrDefault(q => q.Answers is null || q.Answers.Count < 2) is Question failed)
          throw new($"Empty or small amount in 'Answers' list in {failed.BaseQuestionNumber} question: '{failed.Text}'");
      }
      {
        if (quiz.Questions.FirstOrDefault(q => q.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text)))
          is Question failed)
            throw new($"Empty answer 'Text' field in some of answer in {failed.BaseQuestionNumber} question: '{failed.Text}'");
      }
      {
        if (quiz.Questions.FirstOrDefault(q => q.Answers.All(a => a.RightAnswer)) is Question failed)
            throw new($"See 'RightAnswer' field. All answers is right in {failed.BaseQuestionNumber} question: '{failed.Text}'");
      }
      {
        if (quiz.Questions.FirstOrDefault(q => q.Answers.All(a => !a.RightAnswer)) is Question failed)
            throw new($"See 'RightAnswer' field. All answers is not right in {failed.BaseQuestionNumber} question: '{failed.Text}'");
      }
      {
        if (quiz.Questions
          .Where(q => q.OnlyOneRightAnswerPerQuestion.Value)
          .FirstOrDefault(q => q.Answers.Count(a => a.RightAnswer) != 1)
          is Question failed)
          throw new($"See 'RightAnswer' field. All answers is right in {failed.BaseQuestionNumber} question: '{failed.Text}'");
      }
    }
  }
}
