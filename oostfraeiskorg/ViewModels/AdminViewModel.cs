using System;
using DotVVM.Framework.ViewModel;
using oostfraeiskorg.Services;

namespace oostfraeiskorg.ViewModels;

public class AdminViewModel : MasterPageViewModel
{
    private readonly SitemapService _sitemapService;

    [Bind(Direction.Both)]
    public string Message { get; set; }

    public AdminViewModel(SitemapService sitemapService)
    {
        _sitemapService = sitemapService;
    }

    public void RecreateSitemap()
    {
        try
        {
            SitemapResult result = _sitemapService.Generate();
            Message = $"Sitemaps erstellt: {result.UrlCount} URLs in {result.FileCount} Dateien ({result.DurationMilliseconds} ms)";
        }
        catch (Exception ex)
        {
            Message = "Sitemaps konnten nicht erstellt werden: " + ex.Message;
        }
    }
}
