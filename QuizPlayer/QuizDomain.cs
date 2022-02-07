using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace QuizPlayer
{
  public class Answer
  {
    public string Text { get; set; }

    public bool RightAnswer { get; set; }

    [JsonIgnore]
    public bool UserAnswer { get; set; }

    [JsonIgnore]
    public bool UserRightAnswered => UserAnswer == RightAnswer;
  }

  public class Question
  {
    public string Text { get; set; }

    public List<Answer> Answers { get; set; }

    [JsonIgnore]
    public bool UserRightAnswered => Answers.All(a => a.UserRightAnswered);

    [JsonIgnore]
    public bool UserAnswered => Answers.Any(a => a.UserAnswer);
  }

  public class Quiz
  {
    public string QuizCaption { get; set; }
    public List<Question> Questions { get; set; }

    [JsonIgnore]
    public List<Question> RandomizedQuestions => Questions.Select(q => new Question { Text = q.Text, Answers = q.Answers.Shuffle() }).Shuffle();
  }
}
