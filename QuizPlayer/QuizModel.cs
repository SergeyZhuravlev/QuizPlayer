using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizPlayer
{
  public class QuizModel
  {
    private IReadOnlyCollection<IQuestion> Questions { get; }
    private readonly IEnumerator<IQuestion> questionEnumerator;

    public QuizModel(IReadOnlyCollection<IQuestion> questions)
    {
      Questions = questions;
      questionEnumerator = Questions.AsEnumerable().GetEnumerator();
      NextQuestion();
    }

    public IQuestion CurrentQuestion { get; private set; }

    public void NextQuestion()
    {
      if (questionEnumerator.MoveNext())
      {
        CurrentQuestion = questionEnumerator.Current;
        ++QuestionNumber;
      }
      else
      {
        CompletedQuiz = true;
      }
    }

    public bool CompletedQuiz { get; private set; }
    public int QuestionNumber { get; private set; }
    public int QuestionCount => Questions.Count;
    public int RightAnsweredQuestionCount => Questions.Count(q => q.UserRightAnswered);
    public int RightAnsweredPercent => Convert.ToInt32(100.0 * RightAnsweredQuestionCount / QuestionCount);
    public IEnumerable<IQuestion> WrongQuestionList => Questions.Where(q => !q.UserRightAnswered);
  }
}
