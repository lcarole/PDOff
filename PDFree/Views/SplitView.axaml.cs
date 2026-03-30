using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using PDFree.ViewModels;

namespace PDFree.Views;

public partial class SplitView : UserControl
{
    public SplitView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not SplitViewModel vm) return;
        if (!e.DataTransfer.Contains(DataFormat.File)) return;

        var files = e.DataTransfer.TryGetFiles();
        if (files is null) return;

        foreach (var item in files)
        {
            if (item is not IStorageFile file) continue;
            var path = file.TryGetLocalPath();
            if (path is not null &&
                path.EndsWith(".pdf", System.StringComparison.OrdinalIgnoreCase))
            {
                vm.SelectedFile = path;
                break;
            }
        }
    }
}
