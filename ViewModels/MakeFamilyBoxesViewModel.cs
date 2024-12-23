
using Autodesk.Revit.DB;
using MakeFamilyBoxes.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MakeFamilyBoxes.ViewModels
{

    public class MakeFamilyBoxesViewModel() : ViewModelBase
    {
        private bool _isAutoPlacementEnabled = false;
        private bool _isManualPlacementEnabled = false;
        private bool _isChoosingById = false;
        private bool _isCreateSpecification = false;
        private bool _isCombineBoxes = false;
        private string _minSizeOfRoundBox = string.Empty;
        private string _minSizeOfSquareBox = string.Empty;
        private string _boxId = string.Empty;
        private string _offsetFromCuttingEdge = string.Empty;

        public bool IsAutoPlacementEnabled
        {
            get => _isAutoPlacementEnabled;
            set
            {
                if (_isAutoPlacementEnabled != value)
                {
                    _isAutoPlacementEnabled = value;
                    OnPropertyChanged(nameof(IsAutoPlacementEnabled));
                }
            }
        }
        public bool IsManualPlacementEnabled
        {
            get => _isManualPlacementEnabled;
            set
            {
                if (_isManualPlacementEnabled != value)
                {
                    _isManualPlacementEnabled = value;
                    OnPropertyChanged(nameof(IsManualPlacementEnabled));
                }
            }
        }
        public bool IsChoosingById
        {
            get => _isChoosingById;
            set
            {
                if (_isChoosingById != value)
                {
                    _isChoosingById = value;
                    OnPropertyChanged(nameof(IsChoosingById));
                }
            }
        }
        public bool IsCreateSpecification
        {
            get => _isCreateSpecification;
            set
            {
                if (_isCreateSpecification != value)
                {
                    _isCreateSpecification = value;
                    OnPropertyChanged(nameof(IsCreateSpecification));
                }
            }
        }
        public bool IsCombineBoxes
        {
            get => _isCombineBoxes;
            set
            {
                if (_isCombineBoxes != value)
                {
                    _isCombineBoxes = value;
                    OnPropertyChanged(nameof(IsCombineBoxes));
                }
            }
        }
        public string MinSizeOfSquareBox
        {
            get => _minSizeOfSquareBox;
            set
            {
                if (_minSizeOfSquareBox != value)
                {
                    _minSizeOfSquareBox = value;
                    OnPropertyChanged(nameof(MinSizeOfSquareBox));
                }
            }
        }
        public string MinSizeOfRoundBox
        {
            get => _minSizeOfRoundBox;
            set
            {
                if (_minSizeOfRoundBox != value)
                {
                    _minSizeOfRoundBox = value;
                    OnPropertyChanged(nameof(MinSizeOfRoundBox));
                }
            }
        }
        public string OffsetFromCuttingEdge
        {
            get => _offsetFromCuttingEdge;
            set
            {
                if (_offsetFromCuttingEdge != value)
                {
                    _offsetFromCuttingEdge = value;
                    OnPropertyChanged(nameof(OffsetFromCuttingEdge));
                }
            }
        }
        public string BoxId
        {
            get => _boxId;
            set
            {
                if (_boxId != value)
                {
                    _boxId = value;
                    OnPropertyChanged(nameof(BoxId));
                }
            }
        }

    }
}