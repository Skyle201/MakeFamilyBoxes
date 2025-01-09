using MakeFamilyBoxes.ViewModels;

namespace MakeFamilyBoxes.Views
{
    public sealed partial class MakeFamilyBoxesView
    {
        public MakeFamilyBoxesView(MakeFamilyBoxesViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.CloseRequest = Close;
            InitializeComponent();
        }
    }
}