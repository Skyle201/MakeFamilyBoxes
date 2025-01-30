using MakeFamilyBoxes.ViewModels;
using MakeFamilyBoxes.ViewModels.Command;

namespace MakeFamilyBoxes.Views
{
    public sealed partial class MakeFamilyBoxesView
    {
        public MakeFamilyBoxesView(MakeFamilyBoxesViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.CloseRequest = Close;
            viewModel.SaveJson = viewModel.SaveVM;
            InitializeComponent();
            Closed += (sender, args) =>
            {
                viewModel.SaveJson?.Invoke();
            };
        }
    }
}