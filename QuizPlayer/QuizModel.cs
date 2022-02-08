using System.Collections.Generic;
using System.Linq;

namespace QuizPlayer
{
  class QuizModel
  {
    private List<Question> Questions { get; }
    private readonly IEnumerator<Question> questionEnumerator;

    public QuizModel(List<Question> questions)
    {
      Questions = questions;
      questionEnumerator = Questions.AsEnumerable().GetEnumerator();
      NextQuestion();
    }

    public Question CurrentQuestion { get; private set; }

    public void NextQuestion()
    {
      if (questionEnumerator.MoveNext())
      {
        CurrentQuestion = questionEnumerator.Current;
        ++QuestionNumber;
      }
      else
      {
        CurrentQuestion = new() { Text = string.Empty, Answers = new() { new() { Text = string.Empty } } };
        CompletedQuiz = true;
      }
    }

    public bool CompletedQuiz { get; private set; }
    public int QuestionNumber { get; private set; }
    public int QuestionCount => Questions.Count;
    public int RightAnsweredQuestionCount => Questions.Count(q => q.UserRightAnswered);
    public int RightAnsweredPercent => (int)(100.0 * RightAnsweredQuestionCount / QuestionCount);
    public IEnumerable<string> WrongQuestionList => Questions.Where(q => !q.UserRightAnswered).Select(q => q.Text);
  }
}
