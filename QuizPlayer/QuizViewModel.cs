using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;

namespace QuizPlayer
{
  class QuizViewModel : BindableBase
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
    });

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
      RaisePropertyChanged(nameof(CompletedQuiz));
    }

  }
}
