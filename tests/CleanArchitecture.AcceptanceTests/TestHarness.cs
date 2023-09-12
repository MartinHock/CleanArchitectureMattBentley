using CleanArchitecture.AcceptanceTests.Pages.Abstract;
using CleanArchitecture.AcceptanceTests.Settings;

namespace CleanArchitecture.AcceptanceTests
{
    public class TestHarness
    {
        public const string VideoPrefix = "Feature_";
        private static bool _installed;
        private readonly BrowserSettings _settings;
        private IBrowser _browser;
        private IBrowserContext _browserContext;
        private string _featureName;
        private string _scenarioName;
        private string _scenarioStatus;
        private string _videoPath;

        public TestHarness(BrowserSettings settings)
        {
            _settings = settings;
            CleanupArtifacts();
        }

        public static string VideosDirectory => Path.Join(Directory.GetCurrentDirectory(), "videos");
        public static string TracesDirectory => Path.Join(Directory.GetCurrentDirectory(), "traces");
        public IPage Page { get; private set; }
        public PageObject CurrentPage { get; set; }

        private static void CleanupArtifacts()
        {
            if (Directory.Exists(TracesDirectory))
            {
                Directory.Delete(TracesDirectory, true);
            }

            if (Directory.Exists(VideosDirectory))
            {
                Directory.Delete(VideosDirectory, true);
            }
        }

        public async Task StartAsync()
        {
            await InitializeBrowser();
            Page = await GotoAsync(string.Empty);
            _videoPath = await Page.Video.PathAsync();
        }

        private async Task InitializeBrowser()
        {
            InstallPlaywright();
            IPlaywright playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                { Headless = _settings.Headless, SlowMo = _settings.SlowMoMilliseconds });
            _browserContext = await _browser.NewContextAsync(new BrowserNewContextOptions
                { RecordVideoDir = Path.Join(VideosDirectory) });
            await OpenNewTabAsync();
        }

        private static void InstallPlaywright()
        {
            if (!_installed)
            {
                Console.WriteLine("Installing browser");
                int exitCode = Program.Main(new[] { "install" });
                if (exitCode != 0)
                {
                    Console.WriteLine($"Playwright install exited with code {exitCode}");
                }
                else
                {
                    Console.WriteLine("Browser installation complete");
                }

                _installed = true;
            }
        }

        /// <summary>
        ///     Go to a new url on the active Page. The 'path' will be added on top of the BaseUrl.
        /// </summary>
        /// <returns></returns>
        public async Task<IPage> GotoAsync(string path)
        {
            string url = $"{_settings.BaseUrl}{path}";
            if (Page.Url != url)
            {
                await Page.GotoAsync(url);
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            }

            return Page;
        }

        /// <summary>
        ///     Create a page with provided browser context
        /// </summary>
        /// <returns></returns>
        public async Task<IPage> OpenNewTabAsync()
        {
            IPage page = await _browserContext.NewPageAsync();
            Page = page;
            return page;
        }

        public async Task StartScenarioAsync(string featureName, string scenarioName)
        {
            _featureName = featureName;
            _scenarioName = scenarioName;
            await StartTracing();
        }

        public async Task StopScenarioAsync(string scenarioStatus)
        {
            _scenarioStatus = scenarioStatus;
            string traceFileName = $"{_scenarioStatus}.{_featureName}.{_scenarioName}.zip";
            await StopTracing(traceFileName);
        }

        public async Task StopAsync()
        {
            await _browser.CloseAsync();
            if (!string.IsNullOrEmpty(_videoPath))
            {
                string extension = Path.GetExtension(_videoPath);
                string featureVideoName =
                    Path.ChangeExtension($"{VideoPrefix}{_featureName.Replace(" ", "_")}", extension);
                string featureVideoPath = Path.Combine(VideosDirectory, featureVideoName);
                File.Move(_videoPath, featureVideoPath);
                ClearRedundantFiles();
            }
        }

        private static void ClearRedundantFiles()
        {
            string[] redundantFiles = Directory.GetFiles(VideosDirectory);
            foreach (string filePath in redundantFiles)
            {
                string file = Path.GetFileName(filePath);
                if (!file.StartsWith(VideoPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(filePath);
                }
            }
        }

        private async Task StartTracing()
        {
            await _browserContext.Tracing.StartAsync(new TracingStartOptions { Screenshots = true, Snapshots = true });
        }

        private async Task StopTracing(string traceFileName)
        {
            await _browserContext.Tracing.StopAsync(new TracingStopOptions
                { Path = Path.Combine(TracesDirectory, traceFileName.Replace(" ", "")) });
        }
    }
}