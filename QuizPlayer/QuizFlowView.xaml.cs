using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizPlayer
{
  public interface ISelectionChanging
  {
    public void OnSelectionChanged(object sender, SelectionChangedEventArgs args);
  }

  /// <summary>
  /// Interaction logic for QuizFlowView.xaml
  /// </summary>
  public partial class QuizFlowView : UserControl, ISelectionChanging
  {

    private readonly ISelectionChanging selectionChanging;
    public void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
      selectionChanging.OnSelectionChanged(sender, args);
    }

    public QuizFlowView(ISelectionChanging selectionChanging)
    {
      Debug.Assert(!(selectionChanging is null));
      this.selectionChanging = selectionChanging;
      InitializeComponent();
    }
  }
}
