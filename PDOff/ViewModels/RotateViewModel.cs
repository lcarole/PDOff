using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PDOff.Helpers;
using PDOff.Services;

namespace PDOff.ViewModels;

public partial class RotateViewModel : ViewModelBase
{
    private readonly IPdfRotateService _rotateService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RotateCommand))]
    private string? _selectedFile;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Is90CW), nameof(Is90CCW), nameof(Is180))]
    private int _angleIndex; // 0=90CW, 1=90CCW, 2=180

    public bool Is90CW
    {
        get => AngleIndex == 0;
        set { if (value) AngleIndex = 0; }
    }
    public bool Is90CCW
    {
        get => AngleIndex == 1;
        set { if (value) AngleIndex = 1; }
    }
    public bool Is180
    {
        get => AngleIndex == 2;
        set { if (value) AngleIndex = 2; }
    }

    [ObservableProperty]
    private bool _allPages = true;

    [ObservableProperty]
    private string _pageRange = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RotateCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isSuccess;

    public RotateViewModel(IPdfRotateService rotateService)
    {
        _rotateService = rotateService;
    }

    [RelayCommand]
    private async Task SelectFile()
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider is null) return;

        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Lang.Instance["RotateDialogTitle"],
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("PDF") { Patterns = new[] { "*.pdf" } } }
        });

        if (files.Count > 0)
            SelectedFile = files[0].TryGetLocalPath();
    }

    private bool CanRotate() => SelectedFile is not null && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanRotate))]
    private async Task Rotate()
    {
        if (SelectedFile is null) return;

        var storageProvider = GetStorageProvider();
        if (storageProvider is null)
        {
            IsSuccess = false;
            StatusMessage = Lang.Instance["StorageUnavailable"];
            return;
        }

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Lang.Instance["RotateSaveTitle"],
            DefaultExtension = "pdf",
            SuggestedFileName = Path.GetFileNameWithoutExtension(SelectedFile) + Lang.Instance["RotateSuggestedSuffix"] + ".pdf",
            FileTypeChoices = new[] { new FilePickerFileType("PDF") { Patterns = new[] { "*.pdf" } } }
        });

        if (file is null) return;
        var outputPath = file.TryGetLocalPath();
        if (outputPath is null) return;

        int angle = AngleIndex switch { 1 => 270, 2 => 180, _ => 90 };

        IReadOnlyList<int>? pages = null;
        if (!AllPages && !string.IsNullOrWhiteSpace(PageRange))
            pages = PageRangeParser.Parse(PageRange);

        IsBusy = true;
        StatusMessage = null;

        try
        {
            var result = await Task.Run(() => _rotateService.Rotate(SelectedFile, outputPath, angle, pages));
            IsSuccess = result.Success;
            StatusMessage = result.Success ? Lang.Instance["RotateSuccess"] : result.ErrorMessage;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
