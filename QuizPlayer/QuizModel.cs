using System.Collections.Generic;
using System.Linq;

namespace QuizPlayer
{
  class QuizModel
  {
    private List<Question> Questions { get; }
    private IEnumerator<Question> questionEnumerator;

    public QuizModel(List<Question> questions)
    {
      Questions = questions;
      questionEnumerator = Questions.AsEnumerable().GetEnumerator();
      NextQuestion();
    }

    public Question CurrentQuestion { get; set; }

    public void NextQuestion()
    {
      if (questionEnumerator.MoveNext())
      {
        CurrentQuestion = questionEnumerator.Current;
      }
      else
      {
        CurrentQuestion = new() { Text = string.Empty, Answers = new() { new() { Text = string.Empty } } };
        CompletedQuiz = true;
      }
    }

    public bool CompletedQuiz { get; set; }
    public int QuestionCount => Questions.Count;
    public int RightAnsweredQuestionCount => Questions.Count(q => q.UserRightAnswered);
    public int RightAnsweredPercent => (int)(100.0 * RightAnsweredQuestionCount / QuestionCount);
    public IEnumerable<string> WrongQuestionList => Questions.Where(q => !q.UserRightAnswered).Select(q => q.Text);
  }
}
