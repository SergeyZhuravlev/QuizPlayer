using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace QuizPlayer
{
  public class Answer
  {
    public string Text { get; set; }

    public bool RightAnswer { get; set; }
  }

  public class Question
  {
    public string Text { get; set; }

    public List<Answer> Answers { get; set; }

    [JsonIgnore]
    public int BaseQuestionNumber { get; set; }
  }

  // todo: добавить минимальный процент для успешного прохождения
  public class Quiz
  {
    public string QuizCaption { get; set; }
    public List<Question> Questions { get; set; }

    [JsonIgnore]
    public List<Question> RandomizedQuestions => Questions.Select((question, index) => new Question { BaseQuestionNumber = index, Text = question.Text, Answers = question.Answers.Shuffle() }).Shuffle();
  }
}
