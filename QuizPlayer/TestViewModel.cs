using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;

namespace QuizPlayer
{
  class TestViewModel : BindableBase
  {
    private TestModel TestModel { get; }

    public TestViewModel(string testCaption, TestModel testModel)
    {
      Caption = $"QuizPlayer: {testCaption}";
      TestModel = testModel;
    }

    public string Caption { get; }

#region Use on question stage
    public Question CurrentQuestion => TestModel.CurrentQuestion;
    public bool ShowCurrentQuestionResult { get; set; }
#endregion

    public bool CompletedTest => TestModel.CompletedTest;

#region Use on completed test stage
    public int QuestionCount => TestModel.QuestionCount;
    public int RightAnsweredQuestionCount => TestModel.RightAnsweredQuestionCount;
    public int RightAnsweredPercent => TestModel.RightAnsweredPercent;
    public IEnumerable<string> WrongQuestionList => TestModel.WrongQuestionList;
#endregion

    public DelegateCommand AnswerQuestion => new(() =>
    {
      ShowCurrentQuestionResult = true;
      RaisePropertyChanged(nameof(ShowCurrentQuestionResult));
    });

    public DelegateCommand NextQuestion => new(() =>
    {
      ShowCurrentQuestionResult = false;
      TestModel.NextQuestion();
      RaisePropertyChanged(nameof(ShowCurrentQuestionResult));
      RaisePropertyChanged(nameof(CurrentQuestion));
      RaisePropertyChanged(nameof(CompletedTest));
    });

  }
}
