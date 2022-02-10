using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace QuizPlayer
{
  public class Answer
  {
    public string Text { get; set; }

    public bool RightAnswer { get; set; }

    public bool MustBeLastAnswer { get; set; }
  }

  public class Question
  {
    public string Text { get; set; }

    public bool? OnlyOneRightAnswerPerQuestion { get; set; }

    public List<Answer> Answers { get; set; }

    [JsonIgnore]
    public int BaseQuestionNumber { get; set; }

    [JsonIgnore]
    public int QuestionNumber { get; set; }
  }

  public class Quiz
  {
    public string QuizCaption { get; set; }

    public bool? OnlyOneRightAnswerPerQuestionByDefault { get; set; }

    public int? QuestionsPerQuiz { get; set; }

    // 0..100
    public int? MinimalAnsweredQuestionsPercentForQuizSuccess { get; set; }

    public List<Question> Questions { get; set; }

    [JsonIgnore]
    public List<Question> RandomizedQuestions => Questions.Select(question => new Question
    {
      Text = question.Text,
      BaseQuestionNumber = question.BaseQuestionNumber,
      OnlyOneRightAnswerPerQuestion = question.OnlyOneRightAnswerPerQuestion,
      Answers = question.Answers.Shuffle()
    })
    .Shuffle()
    .Select((question, index) =>
    {
      question.QuestionNumber = index + 1;
      return question;
    })
    .ToList();
  }
}
