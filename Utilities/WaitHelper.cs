
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomationExercise.Utilities
{
    public class WaitHelper(IWebDriver driver)
    {
        private readonly WebDriverWait wait = new(
            driver,
            TimeSpan.FromSeconds(10)
        );

        public void RecoverFromServerOverload()
        {
            const int maxRetries = 2;
            const string overloadMessage =
                "This website is under heavy load (queue full)";

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                if (!driver.PageSource.Contains(
                    overloadMessage,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (attempt == maxRetries)
                {
                    throw new InvalidOperationException(
                        "Automation Exercise remains overloaded after 3 checks.");
                }

                TestContext.Out.WriteLine(
                    $"Queue full detected. Refresh {attempt + 1}/{maxRetries}");

                Thread.Sleep(3000);
                driver.Navigate().Refresh();
            }
        }

        public IWebElement WaitForElement(By locator)
        {
            RecoverFromServerOverload();

            return wait.Until(d =>
            {
                try
                {
                    IWebElement element = d.FindElement(locator);

                    return element.Displayed ? element : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            })!;
        }

        public IWebElement WaitForClickableElement(By locator)
        {
            return wait.Until(d =>
            {
                try
                {
                    IWebElement element = d.FindElement(locator);

                    return element.Displayed && element.Enabled
                        ? element
                        : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            })!;
        }

        public void ClickElement(By locator)
        {
            RecoverFromServerOverload();

            wait.Until(d =>
            {
                try
                {
                    IWebElement element = d.FindElement(locator);

                    if (!element.Displayed || !element.Enabled)
                        return false;

                    element.Click();
                    return true;
                }
                catch (ElementClickInterceptedException ex)
                {
                    TestContext.Out.WriteLine(
                        $"Click intercepted for {locator}: {ex.Message}");

                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });
        }
    }
}
