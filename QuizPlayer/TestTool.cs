using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TestPlayer
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
    CompiledTest,
    DecompiledTest,
    NothingToConvert
  }

  public class TestTool
  {
    public static readonly string TestFileNameWithoutExtension = Path.Combine(AppContext.BaseDirectory, "test");

    public readonly string TestFileSource = $"{TestFileNameWithoutExtension}.json";
    public readonly string TestFileCompiled = $"{TestFileNameWithoutExtension}.bin";
    public readonly string TestFileSourceForDecompile = $"{TestFileNameWithoutExtension}.decompile";
    public ConvertType TryConvertTest()
    {
      if (File.Exists(TestFileSource))
      {
        Compile();
        return ConvertType.CompiledTest;
      }
      if (File.Exists(TestFileSourceForDecompile))
      {
        Decompile();
        return ConvertType.DecompiledTest;
      }
      return ConvertType.NothingToConvert;
    }

    private void Decompile()
    {
      var compiled = File.ReadAllBytes(TestFileSourceForDecompile);
      var source = compiled.Recrypt();
      try
      {
        _ = GetTest(source);
      } catch
      {
        throw new Exception($"File '{TestFileSourceForDecompile}' not contain compiled .bin test file. It file broken or wrongly renamed from *.json file. You should rename only *.bin file to *.decompile file");
      }
      File.WriteAllBytes(TestFileSource, source);
      File.Delete(TestFileSourceForDecompile);
    }

    private void Compile()
    {
      var source = File.ReadAllBytes(TestFileSource);
      _ = GetTest(source);
      File.WriteAllBytes(TestFileCompiled, source.Recrypt());
      File.Delete(TestFileSource);
    }

    public Test GetTest()
    {
      if (!File.Exists(TestFileCompiled))
        throw new FileNotFoundException(TestFileCompiled);

      return GetTest(File.ReadAllBytes(TestFileCompiled).Recrypt());
    }

    private static Test GetTest(byte[] utf8Test)
    {
      Utf8JsonReader jsonUtfReader = new(utf8Test, new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
      JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, WriteIndented = true };
      var test = JsonSerializer.Deserialize<Test>(ref jsonUtfReader, options);
      CheckTest(test);
      return test;
    }

    private static void CheckTest(Test test)
    {
      if (string.IsNullOrWhiteSpace(test.TestCaption))
        throw new Exception("Empty 'TestCaption' field");
      if (test.Questions is null || test.Questions.Count == 0)
        throw new Exception("Empty 'Questions' list");
      {
        if (test.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers is null || q.question.Answers.Count < 2)
          is var failed && !(failed is null))
            throw new Exception($"Empty or small amount in 'Answers' list in {failed.index} question");
      }
      { 
        if (test.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => string.IsNullOrWhiteSpace(q.question.Text))
          is var failed && !(failed is null))
            throw new Exception($"Empty question 'Text' field in {failed.index} question");
      }
      {
        if (test.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text)))
          is var failed && !(failed is null))
            throw new Exception($"Empty answer 'Text' field in some of answer in {failed.index} question");
      }
      {
        if (test.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers.All(a => a.RightAnswer))
          is var failed && !(failed is null))
            throw new Exception($"See 'RightAnswer' field. All answers is right in {failed.index} question");
      }
      {
        if (test.Questions
          .Select((question, index) => new { question, index })
          .FirstOrDefault(q => q.question.Answers.All(a => !a.RightAnswer)) 
          is var failed && !(failed is null))
            throw new Exception($"See 'RightAnswer' field. All answers is not right in {failed.index} question");
      }
    }
  }
}
