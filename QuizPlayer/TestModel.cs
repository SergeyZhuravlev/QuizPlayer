using System.Collections.Generic;
using System.Linq;

namespace QuizPlayer
{
  class TestModel
  {
    private List<Question> Questions { get; }
    private IEnumerator<Question> questionEnumerator;

    public TestModel(List<Question> questions)
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
        CompletedTest = true;
      }
    }

    public bool CompletedTest { get; set; }
    public int QuestionCount => Questions.Count;
    public int RightAnsweredQuestionCount => Questions.Count(q => q.UserRightAnswered);
    public int RightAnsweredPercent => (int)(100.0 * RightAnsweredQuestionCount / QuestionCount);
    public IEnumerable<string> WrongQuestionList => Questions.Where(q => !q.UserRightAnswered).Select(q => q.Text);
  }
}
