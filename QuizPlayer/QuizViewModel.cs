using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuizPlayer
{
  public class QuizFlowViewModel : BindableBase, ISelectionChanging
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
    public QuizFlowView View => view is null ? view = new(this) : view;

    public QuestionViewModel CurrentQuestion => (QuestionViewModel)QuizModel.CurrentQuestion;
    public string QuestionNumberText => CurrentQuestion.QuestionNumber + ".";
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
      RaisePropertyChanged(nameof(QuestionNumberText));
    }

    public void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
      var answer = (sender as ListBox).SelectedItem as AnswerViewModel;
      if (answer is null)
        return;
      answer.UserAnswer = !answer.UserAnswer;
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
    public bool AllRight => QuestionCount == RightAnsweredQuestionCount;
    public string YouResultClassify => SuccessPassedQuiz ? "You are passed" : "You are failed";
    public string TestResults => $"{RightAnsweredQuestionCount}/{QuestionCount} ({RightAnsweredPercent}%)";
    public IEnumerable<string> WrongQuestionList =>
      QuizModel
      .WrongQuestionList
      .Cast<QuestionViewModel>()
      .Select(q => $"{q.QuestionNumber}({q.BaseQuestionNumber}). {q.Text}");
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
