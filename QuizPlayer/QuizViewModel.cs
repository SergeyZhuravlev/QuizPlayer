using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QuizPlayer
{
  public class QuizFlowViewModel : BindableBase
  {
    private QuizModel QuizModel { get; }
    private QuizViewModel QuizViewModel { get; }

    private bool ShowCurrentQuestionResult { get; set; }

    public QuizFlowViewModel(QuizModel quizModel, QuizViewModel quizViewModel)
    {
      QuizModel = quizModel;
      QuizViewModel = quizViewModel;
    }

    private QuizFlowView view;
    public QuizFlowView View => view is null ? view = new() : view;

    public QuestionViewModel CurrentQuestion => (QuestionViewModel)QuizModel.CurrentQuestion;
    public string QuestionNumber => QuizModel.QuestionNumber + ".";
    public string ButtonCaption => ShowCurrentQuestionResult ? "Next Question" : "Answer";

    public DelegateCommand ButtonCommand => new(() =>
    {
      if (ShowCurrentQuestionResult)
      {
        NextQuestion();
        if (QuizModel.CompletedQuiz)
          QuizViewModel.CurrentViewModel = new QuizResultsViewModel(QuizModel);
      }
      else
      {
        AnswerQuestion();
      }
    });

    private void AnswerQuestion()
    {
      ShowCurrentQuestionResult = true;
      CurrentQuestion.UserAnswered();
      RaisePropertyChanged(nameof(CurrentQuestion));
      RaisePropertyChanged(nameof(ButtonCaption));
    }

    private void NextQuestion()
    {
      ShowCurrentQuestionResult = false;
      QuizModel.NextQuestion();
      RaisePropertyChanged(nameof(ButtonCaption));
      RaisePropertyChanged(nameof(CurrentQuestion));
      RaisePropertyChanged(nameof(QuestionNumber));
    }
  }

  public class QuizResultsViewModel : BindableBase
  {
    private QuizModel QuizModel { get; }

    public QuizResultsViewModel(QuizModel quizModel)
    {
      QuizModel = quizModel;
    }

    private QuizResultsView view;
    public QuizResultsView View => view is null ? view = new() : view;

    public int QuestionCount => QuizModel.QuestionCount;
    public int RightAnsweredQuestionCount => QuizModel.RightAnsweredQuestionCount;
    public int RightAnsweredPercent => QuizModel.RightAnsweredPercent;
    public bool SuccessPassedQuiz => QuizModel.SuccessPassedQuiz;
    public IEnumerable<QuestionViewModel> WrongQuestionList => QuizModel.WrongQuestionList.Cast<QuestionViewModel>();
  }

  public class QuizViewModel : BindableBase
  {
    public QuizViewModel(string quizCaption, QuizModel quizModel)
    {
      Caption = $"QuizPlayer: {quizCaption}";
      CurrentViewModel = new QuizFlowViewModel(quizModel, this);
    }

    public string Caption { get; }

    private object currentViewModel;
    public object CurrentViewModel
    {
      get => currentViewModel;
      set
      {
        currentViewModel = value;
        RaisePropertyChanged(nameof(CurrentViewModel));
      }
    }

  }
}
