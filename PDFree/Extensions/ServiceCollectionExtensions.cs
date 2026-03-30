using Microsoft.Extensions.DependencyInjection;
using PDFree.Services;
using PDFree.ViewModels;

namespace PDFree.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IPdfMergeService, PdfMergeService>();
        collection.AddSingleton<IPdfSplitService, PdfSplitService>();
        collection.AddSingleton<IPdfCompressService, PdfCompressService>();
        collection.AddSingleton<IPdfRotateService, PdfRotateService>();
    }

    public static void AddViewModels(this IServiceCollection collection)
    {
        collection.AddTransient<MergeViewModel>();
        collection.AddTransient<SplitViewModel>();
        collection.AddTransient<CompressViewModel>();
        collection.AddTransient<RotateViewModel>();
        collection.AddSingleton<MainWindowViewModel>();
    }
}