using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;

namespace QuizPlayer
{
  public interface IAnswerChanged
  {
    void AnswerChanged();
  }

  class QuizViewModel : BindableBase, IAnswerChanged
  {
    private QuizModel QuizModel { get; }

    public QuizViewModel(string quizCaption, QuizModel quizModel)
    {
      Caption = $"QuizPlayer: {quizCaption}";
      QuizModel = quizModel;
    }

    public string Caption { get; }

#region Use on question stage
    public Question CurrentQuestion => QuizModel.CurrentQuestion;
    public string QuestionNumber => QuizModel.QuestionNumber + ".";
    public bool ShowCurrentQuestionResult { get; set; }
#endregion

    public bool CompletedQuiz => QuizModel.CompletedQuiz;

#region Use on completed quiz stage
    public int QuestionCount => QuizModel.QuestionCount;
    public int RightAnsweredQuestionCount => QuizModel.RightAnsweredQuestionCount;
    public int RightAnsweredPercent => QuizModel.RightAnsweredPercent;
    public IEnumerable<string> WrongQuestionList => QuizModel.WrongQuestionList;
    #endregion

    public string ButtonCaption => ShowCurrentQuestionResult ? "Next Question" : "Answer";

    public DelegateCommand ButtonCommand => new(() =>
    {
      if (ShowCurrentQuestionResult)
        NextQuestion();
      else
        AnswerQuestion();
    },
    () =>
    {
      return CurrentQuestion.UserAnswered;
    });

    public void AnswerChanged()
    {
      RaisePropertyChanged(nameof(ButtonCommand));
    }

    private void AnswerQuestion()
    {
      ShowCurrentQuestionResult = true;
      RaisePropertyChanged(nameof(ShowCurrentQuestionResult));
      RaisePropertyChanged(nameof(ButtonCaption));
    }

    private void NextQuestion()
    {
      ShowCurrentQuestionResult = false;
      QuizModel.NextQuestion();
      RaisePropertyChanged(nameof(ShowCurrentQuestionResult));
      RaisePropertyChanged(nameof(ButtonCaption));
      RaisePropertyChanged(nameof(CurrentQuestion));
      RaisePropertyChanged(nameof(QuestionNumber));
      RaisePropertyChanged(nameof(CompletedQuiz));
    }

  }
}
