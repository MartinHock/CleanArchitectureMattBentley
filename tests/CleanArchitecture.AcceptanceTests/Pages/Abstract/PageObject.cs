namespace CleanArchitecture.AcceptanceTests.Pages.Abstract
{
    public abstract class PageObject
    {
        public readonly IPage Page;

        protected PageObject(IPage page)
        {
            Page = page;
        }

        public TPage As<TPage>() where TPage : PageObject
        {
            return (TPage)this;
        }

        public async Task RefreshAsync()
        {
            await Page.ReloadAsync();
        }

        public async Task<bool> WaitForConditionAsync(Func<Task<bool>> condition, bool waitForValue = true,
            int checkDelayMs = 100, int numberOfChecks = 300)
        {
            bool value = !waitForValue;
            for (int i = 0; i < numberOfChecks; i++)
            {
                value = await condition();
                if (value == waitForValue)
                {
                    break;
                }

                await Task.Delay(checkDelayMs);
            }

            return value;
        }
    }
}