using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizPlayer
{
  public class QuizModel
  {
    private IReadOnlyCollection<IQuestion> Questions { get; }
    private readonly IEnumerator<IQuestion> questionEnumerator;
    private readonly int minimalAnsweredQuestionsPercentForQuizSuccess;

    public QuizModel(IReadOnlyCollection<IQuestion> questions, int minimalAnsweredQuestionsPercentForQuizSuccess)
    {
      Questions = questions;
      this.minimalAnsweredQuestionsPercentForQuizSuccess = minimalAnsweredQuestionsPercentForQuizSuccess;
      questionEnumerator = Questions.AsEnumerable().GetEnumerator();
      NextQuestion();
    }

    public IQuestion CurrentQuestion { get; private set; }

    public void NextQuestion()
    {
      if (questionEnumerator.MoveNext())
        CurrentQuestion = questionEnumerator.Current;
      else
        CompletedQuiz = true;
    }

    public bool CompletedQuiz { get; private set; }
    public int QuestionCount => Questions.Count;
    public int RightAnsweredQuestionCount => Questions.Count(q => q.UserRightAnswered);
    public int RightAnsweredPercent => Math.Min(Convert.ToInt32(100.0 * RightAnsweredQuestionCount / QuestionCount), 100);
    public bool SuccessPassedQuiz => RightAnsweredPercent >= minimalAnsweredQuestionsPercentForQuizSuccess;
    public IEnumerable<IQuestion> WrongQuestionList => Questions.Where(q => !q.UserRightAnswered);
  }
}
