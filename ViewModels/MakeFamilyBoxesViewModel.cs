﻿using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Models;
using MakeFamilyBoxes.Services;
using MakeFamilyBoxes.ViewModels.Command;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

namespace MakeFamilyBoxes.ViewModels
{

    public class MakeFamilyBoxesViewModel : ViewModelBase
    {
        private readonly GetRevitDocuments _getRevitDocuments;
        private bool _isAutoPlacementEnabled = true;
        private bool _isManualPlacementEnabled = false;
        private bool _isChoosingById = false;
        private bool _isCreateSpecification = false;
        private bool _isCombineBoxes = false;
        private string _minSizeOfRoundBox = string.Empty;
        private string _minSizeOfSquareBox = string.Empty;
        private string _boxId = string.Empty;
         private string _offsetFromCuttingEdge = string.Empty;
        private List<DocumentEntity> _documentEntities = [];
        private DocumentEntity _selectedHubDocument;
        private DocumentEntity _selectedEngineersDocument;
        private DocumentEntity _selectedModelDocument;
        private List<FamilyEntity> _familyEntities = [];
        private FamilyEntity _selectedFamilySquareBox;
        private FamilyEntity _selectedFamilyRoundBox;
        private RevitTask _revitTask = new();

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
                SelectedHubDocument = _documentEntities.FirstOrDefault(x => x.Title == _getRevitDocuments.activeUIDoc.Document.Title);
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
                SelectedHubDocument = _documentEntities.FirstOrDefault(x => x.Title == _getRevitDocuments.activeUIDoc.Document.Title);
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
        public List<DocumentEntity> DocumentEntities
        {
            get => _documentEntities;
            set
            {
                _documentEntities = value;
                OnPropertyChanged(nameof(DocumentEntities));
            }
        }
        public DocumentEntity SelectedHubDocument
        {
            get => _selectedHubDocument;
            set
            {
                if (value != null)
                GetFamilyEntities(value);
                _selectedHubDocument = value;
                OnPropertyChanged(nameof(SelectedHubDocument));
            }
        }
        public DocumentEntity SelectedEngineersDocument
        {
            get => _selectedEngineersDocument;
            set
            {
                _selectedEngineersDocument = value;
                OnPropertyChanged(nameof(SelectedEngineersDocument));
            }
        }
        public DocumentEntity SelectedModelDocument
        {
            get => _selectedModelDocument;
            set
            {
                _selectedModelDocument = value;
                OnPropertyChanged(nameof(SelectedModelDocument));
            }
        }
        public List<FamilyEntity> FamilyEntities
        {
            get => _familyEntities;
            set
            {
                _familyEntities = value;
                OnPropertyChanged(nameof(FamilyEntities));
            }
        }
        public FamilyEntity SelectedFamilySquareBox
        {
            get => _selectedFamilySquareBox;
            set
            {
                _selectedFamilySquareBox = value;
                OnPropertyChanged(nameof(SelectedFamilySquareBox));
            }
        }
        public FamilyEntity SelectedFamilyRoundBox
        {
            get => _selectedFamilyRoundBox;
            set
            {

                _selectedFamilyRoundBox = value;
                OnPropertyChanged(nameof(SelectedFamilyRoundBox));
            }
        }
        public MakeFamilyBoxesViewModel(GetRevitDocuments getRevitDocuments)
        {
            InitOperationCommand = new RelayCommand(InitOperation, null);
            _getRevitDocuments = getRevitDocuments;
            GetDocumentEntities();
        }
        private void GetFamilyEntities(DocumentEntity hubdoc)
        {
            FamilyEntities = new GetFamilyGenericBox(_getRevitDocuments).GetFamilyEntities(hubdoc);
        }
        private void GetDocumentEntities()
        {
            _documentEntities = _getRevitDocuments.GetRevitDocs();
        }
        private async void InitOperation(object obj)
        {
            List<bool> bools = [IsAutoPlacementEnabled, IsManualPlacementEnabled, IsChoosingById, IsCreateSpecification, IsCombineBoxes];
            if (bools.Contains(true))
            {
                if (IsAutoPlacementEnabled)
                {
                    try
                    {
                        if (SelectedFamilyRoundBox == null && SelectedFamilySquareBox == null)
                        {
                            MessageBox.Show("Не выбраны семейства боксов");
                        }
                        else
                        {
                            FindIntersectsService findIntersectsService = new();
                            CreateBoxesService createBoxesService = new();
                            List<IntersectionEntity> intersections = findIntersectsService.FindIntersects(_getRevitDocuments, SelectedEngineersDocument, SelectedModelDocument, MinSizeOfSquareBox, MinSizeOfRoundBox);
                            var instance = await _revitTask.Run((uiApp) => createBoxesService.CreateBoxes(_getRevitDocuments, SelectedHubDocument, intersections, SelectedFamilySquareBox, SelectedFamilyRoundBox, OffsetFromCuttingEdge));
                            MessageBox.Show($"Боксы успешно созданы в количестве {createBoxesService.counter} штук");
                        }
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
                }


                else if (IsManualPlacementEnabled)
                {
                    try
                    {
                    ManualBoxPlacementService manualBoxPlacementService = new();
                        FindIntersectsService findIntersectsService = new();
                        CreateBoxesService createBoxesService = new();
                        List<IntersectionEntity> intersections = manualBoxPlacementService.ActivateManualBoxPlacement(_getRevitDocuments, MinSizeOfSquareBox, MinSizeOfRoundBox);
                        var instance =await _revitTask.Run((uiApp) => createBoxesService.CreateBoxes(_getRevitDocuments, SelectedHubDocument, intersections, SelectedFamilySquareBox, SelectedFamilyRoundBox, OffsetFromCuttingEdge));
                        MessageBox.Show($"Боксы успешно созданы в количестве {createBoxesService.counter} штук");

                    }
                    catch (Exception ex) {  MessageBox.Show($"Ошибка: {ex.Message}"); }

                }

                else if (IsChoosingById)
                {
                    SelectByIdService selectByIdService = new();
                    selectByIdService.SelectElementInCurrentDocument(_getRevitDocuments, BoxId);
                }

                else if (IsCreateSpecification)
                {
                    MessageBox.Show("IsCreateSpecification");
                    // CreateSpecification();
                }

                else if (IsCombineBoxes)
                {
                    try
                    {
                        CombineBoxService combineBoxService = new();
                        var instance = await _revitTask.Run((uiApp) => combineBoxService.FindBoxToCombine(_getRevitDocuments,SelectedFamilySquareBox));
                        MessageBox.Show("Выделенные боксы объединены");
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}"); }
                }
            }
            else
            {
                MessageBox.Show("Не выбраны функции", "Ошибка");
            }
        }
        public ICommand InitOperationCommand { get; }
        public ICommand CancelCommand => new RelayCommand(obj =>
        {
            CloseRequest.Invoke();
        });
        public ICommand FactoryResetCommand => new RelayCommand(FactoryReset, null);
        public void SaveVM()
        {
            var VMSaver = new VMSaver();
            VMSaver.SaveVM(this);
        }
        private void FactoryReset(object obj)
        {
            var VMSaver = new VMSaver();
            VMSaver.FactoryReset(this);
        }
    }

    }
