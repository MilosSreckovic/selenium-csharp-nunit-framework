
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutomationExercise.Base
{
    public abstract class BaseTest
    {
        protected IWebDriver? driver;

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new();

            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-extensions");

            bool isCI = Environment.GetEnvironmentVariable("CI") == "true";

            if (isCI)
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
            }

            driver = new ChromeDriver(options);

            if (!isCI)
            {
                driver.Manage().Window.Maximize();
            }
        }


        protected void CloseGoogleAdIfPresent()
        {
            if (driver == null)
                return;

            driver.SwitchTo().DefaultContent();

            foreach (IWebElement frame in driver.FindElements(
                By.CssSelector("iframe[title='Advertisement']")))
            {
                try
                {
                    driver.SwitchTo().Frame(frame);

                    var closeButtons = driver.FindElements(
                        By.CssSelector("[aria-label='Close ad']"));

                    if (closeButtons.Count > 0 && closeButtons[0].Displayed)
                    {
                        closeButtons[0].Click();
                        TestContext.Out.WriteLine("Google Ad popup closed.");
                        return;
                    }
                }
                catch (WebDriverException)
                {
                    // The advertisement may disappear while being inspected.
                }
                finally
                {
                    driver.SwitchTo().DefaultContent();
                    TestContext.Out.WriteLine(
                        $"Advertisement iframes found: {driver.FindElements(By.CssSelector("iframe[title='Advertisement']")).Count}");

                }
            }
        }


        [TearDown]
        public void TearDown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome.Status
                    == TestStatus.Failed && driver is ITakesScreenshot screenshotDriver)
                {
                    try
                    {
                        string screenshotDirectory = Path.Combine(
                            TestContext.CurrentContext.WorkDirectory,
                            "Screenshots"
                        );

                        Directory.CreateDirectory(screenshotDirectory);

                        string testName = TestContext.CurrentContext.Test.ID;
                        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");

                        string screenshotPath = Path.Combine(
                            screenshotDirectory,
                            $"{testName}_{timestamp}.png"
                        );

                        Screenshot screenshot = screenshotDriver.GetScreenshot();
                        screenshot.SaveAsFile(screenshotPath);

                        TestContext.AddTestAttachment(
                            screenshotPath,
                            "Screenshot on test failure"
                        );

                        TestContext.Progress.WriteLine(
                            $"Failure screenshot saved: {screenshotPath}"
                        );
                    }
                    catch (Exception ex)
                    {
                        TestContext.Progress.WriteLine(
                            $"Could not capture screenshot: {ex.Message}"
                        );
                    }
                }
            }
            finally
            {
                try
                {
                    driver?.Quit();
                }
                finally
                {
                    driver?.Dispose();
                    driver = null;
                }
            }
        }
    }
}
