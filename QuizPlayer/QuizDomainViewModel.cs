using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace QuizPlayer
{
  public interface IAnswer
  {
    public string Text { get; }

    public bool RightAnswer { get; }
  }

  public enum AnswerState
  {
    NotAnswered,
    WrongAnswered,
    RightAnswered
  }

  public class AnswerViewModel : BindableBase, IAnswer
  {
    public AnswerViewModel(Answer answer)
    {
      Text = answer.Text;
      RightAnswer = answer.RightAnswer;
    }

    public string Text { get; private set; }

    public bool RightAnswer { get; private set; }

    public bool UserAnswer { get; set; }

    private bool userAnswered;
    public bool UserAnswered
    {
      get => userAnswered;
      set
      {
        userAnswered = value;
        RaisePropertyChanged(nameof(UserAnswered));
        RaisePropertyChanged(nameof(AnswerState));
      }
    }

    public AnswerState AnswerState =>
      (!UserAnswered)
      ? AnswerState.NotAnswered
      : (UserRightAnswered
        ? (UserAnswer ? AnswerState.RightAnswered : AnswerState.NotAnswered)
        : AnswerState.WrongAnswered);

    public bool UserRightAnswered => UserAnswer == RightAnswer;
  }

  public interface IQuestion
  {
    public string Text { get; }

    public IReadOnlyCollection<IAnswer> AnswersVariance { get; }

    public int BaseQuestionNumber { get; }

    public bool UserRightAnswered { get; }
  }

  public class QuestionViewModel: BindableBase, IQuestion
  {
    public QuestionViewModel(Question question)
    {
      Text = question.Text;
      Answers = question.Answers.Select(a => new AnswerViewModel(a)).ToList();
      BaseQuestionNumber = question.BaseQuestionNumber;
    }

    public string Text { get; private set; }

    public IReadOnlyCollection<IAnswer> AnswersVariance => Answers;
    public IReadOnlyCollection<AnswerViewModel> Answers { get; private set; }

    public int BaseQuestionNumber { get; private set; }

    public bool UserRightAnswered => Answers.All(a => a.UserRightAnswered);

    public bool UserAnyAnswered => Answers.Any(a => a.UserAnswer);

    public void RaisePropertyChangedUserAnyAnswered()
    {
      RaisePropertyChanged(nameof(UserAnyAnswered));
    }

    public void UserAnswered()
    {
      foreach (var answer in Answers)
        answer.UserAnswered = true;
    }
  }

  public class QuizDomainViewModel
  {
    public QuizDomainViewModel(Quiz quiz)
    {
      QuizCaption = quiz.QuizCaption;
      Questions = quiz.RandomizedQuestions.Select(q => new QuestionViewModel(q)).ToList();
    }

    public string QuizCaption { get; private set; }

    public List<QuestionViewModel> Questions { get; private set; }
  }
}