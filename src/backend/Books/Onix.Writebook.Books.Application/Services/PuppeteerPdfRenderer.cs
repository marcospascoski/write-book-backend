using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Onix.Writebook.Books.Application.Services;

internal static class PuppeteerPdfRenderer
{
    private static bool _initialized;
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    public static async Task<byte[]> RenderAsync(string html, CancellationToken cancellationToken = default)
    {
        await EnsureBrowserAsync(cancellationToken).ConfigureAwait(false);

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-setuid-sandbox"]
        }).ConfigureAwait(false);

        await using var page = await browser.NewPageAsync().ConfigureAwait(false);
        await page.SetContentAsync(html, new NavigationOptions { WaitUntil = [WaitUntilNavigation.Networkidle0] }).ConfigureAwait(false);

        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            PreferCSSPageSize = true
        }).ConfigureAwait(false);
    }

    private static async Task EnsureBrowserAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
            return;

        await _initLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_initialized)
                return;

            var fetcher = new BrowserFetcher();
            await fetcher.DownloadAsync().ConfigureAwait(false);
            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }
}
