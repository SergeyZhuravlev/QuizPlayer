using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public AnswerViewModel(Answer answer, bool onlyOneAnswer)
    {
      Text = answer.Text;
      RightAnswer = answer.RightAnswer;
      this.onlyOneAnswer = onlyOneAnswer;
    }

    public string Text { get; private set; }

    public bool RightAnswer { get; private set; }

    #region For reset other answers in radiobuttoned question with only one answer
    public bool UserAnswerLocked { get; private set; }
    public event Action OnUserAnswerSet;
    private readonly bool onlyOneAnswer;
    #endregion

    private bool userAnswer;
    public bool UserAnswer
    {
      get => userAnswer;
      set
      {
        if (userAnswer == value)
          return;
        userAnswer = value;
        if (onlyOneAnswer && userAnswer)
          try
          {
            UserAnswerLocked = true;
            var handler = OnUserAnswerSet;
            handler?.Invoke();
          }
          finally
          {
            UserAnswerLocked = false;
          }
        RaisePropertyChanged(nameof(UserAnswer));
      }
    }

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
      BaseQuestionNumber = question.BaseQuestionNumber;
      OnlyOneRightAnswerPerQuestion = question.OnlyOneRightAnswerPerQuestion.Value;
      Answers = question.Answers.Select(a => new AnswerViewModel(a, OnlyOneRightAnswerPerQuestion)).ToList();
      foreach (var answer in Answers)
      {
        answer.PropertyChanged += OnAnswerChanged;
        if (OnlyOneRightAnswerPerQuestion)
          answer.OnUserAnswerSet += OnUserAnswerSet;
      }
    }

    public string Text { get; private set; }

    public IReadOnlyCollection<IAnswer> AnswersVariance => Answers;
    public IReadOnlyCollection<AnswerViewModel> Answers { get; private set; }

    public bool OnlyOneRightAnswerPerQuestion { get; private set; }

    public int BaseQuestionNumber { get; private set; }

    public bool UserRightAnswered => Answers.All(a => a.UserRightAnswered);

    private void OnAnswerChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(AnswerViewModel.UserAnswer))
        RaisePropertyChanged(nameof(UserAnyAnswered));
    }
    private void OnUserAnswerSet()
    {
      foreach (var answer in Answers.Where(a => !a.UserAnswerLocked))
        answer.UserAnswer = false;
    }
    

    public bool UserAnyAnswered => Answers.Any(a => a.UserAnswer);

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
      MinimalAnsweredQuestionsPercentForQuizSuccess = quiz.MinimalAnsweredQuestionsPercentForQuizSuccess.Value;
    }

    public string QuizCaption { get; private set; }

    // 0..100
    public int MinimalAnsweredQuestionsPercentForQuizSuccess { get; }

    public List<QuestionViewModel> Questions { get; private set; }
  }
}