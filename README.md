# Selenium C# NUnit E2E Automation Framework

[![Selenium E2E Tests](https://github.com/MilosSreckovic/selenium-csharp-nunit-framework/actions/workflows/selenium-tests.yml/badge.svg)](https://github.com/MilosSreckovic/selenium-csharp-nunit-framework/actions/workflows/selenium-tests.yml)

A UI test automation framework built with **C#**, **Selenium WebDriver**, **NUnit**, and **.NET 10**.

The project demonstrates maintainable Page Object Model (POM) architecture, reusable browser setup, explicit wait utilities, automated failure screenshots, and continuous integration through GitHub Actions.

**Application under test:** [Automation Exercise](https://automationexercise.com/)

## Tech Stack

- C# / .NET 10
- Selenium WebDriver
- NUnit
- Page Object Model (POM)
- GitHub Actions

## Test Coverage

The framework contains three automated UI tests organized by application feature.

| Test Class              | Focus                                     |
| ----------------------- | ----------------------------------------- |
| `LoginTests.cs`         | Login-related scenarios                   |
| `SearchAndCartTests.cs` | Product search and shopping cart          |
| `BrandTests.cs`         | Brand navigation and product verification |

The `ViewAndCartBrandProducts` scenario verifies brand navigation, including the Polo and H&M brand pages, their headings, and corresponding URLs.

## Project Structure

```text
.
├── .github/
│   └── workflows/
│       └── selenium-tests.yml
├── Base/
│   └── BaseTest.cs
├── Pages/
│   ├── AccountDeletedPage.cs
│   ├── CartPage.cs
│   ├── HomePage.cs
│   ├── LoginPage.cs
│   └── ProductsPage.cs
├── Tests/
│   ├── BrandTests.cs
│   ├── LoginTests.cs
│   └── SearchAndCartTests.cs
├── Utilities/
│   ├── ApiHelper.cs
│   └── WaitHelper.cs
├── .gitignore
├── SeleniumNUnitFramework.csproj
├── SeleniumNUnitFramework.slnx
└── README.md
```

Generated directories such as `bin/` and `obj/` are excluded.

## Framework Design

### Page Object Model

Page-specific locators and interactions are encapsulated in dedicated Page Object classes, separating test logic from UI implementation details.

### Reusable Test Setup

`BaseTest.cs` provides centralized WebDriver initialization and cleanup using NUnit `[SetUp]` and `[TearDown]`.

Chrome runs in headed mode locally and headless mode in GitHub Actions.

### Explicit Wait Utilities

`WaitHelper.cs` provides reusable methods for waiting for visible and clickable elements and handling transient Selenium interaction exceptions.

### Screenshot on Failure

When an NUnit test fails, the framework automatically attempts to:

1. Capture a browser screenshot.
2. Save it in the `Screenshots/` directory under the NUnit working directory.
3. Attach it to the NUnit test result.
4. Log the screenshot location.
5. Close and dispose of the browser session.

Screenshots are generated only for failed tests and require an active WebDriver session.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Google Chrome
- Git

### Clone Repository

```bash
git clone https://github.com/MilosSreckovic/selenium-csharp-nunit-framework.git
cd selenium-csharp-nunit-framework
```

### Restore Dependencies

```bash
dotnet restore SeleniumNUnitFramework.slnx
```

### Build Project

```bash
dotnet build SeleniumNUnitFramework.slnx
```

### Run All Tests

```bash
dotnet test SeleniumNUnitFramework.slnx
```

### Run Specific Tests

```bash
dotnet test --filter "FullyQualifiedName~BrandTests"
```

### Generate TRX Results

```bash
dotnet test --logger "trx;LogFileName=test-results.trx" --results-directory TestResults
```

## Continuous Integration

GitHub Actions automatically executes the Selenium NUnit suite on:

- Pushes to `main`
- Pull requests targeting `main`
- Manual workflow dispatch

The CI pipeline:

1. Checks out the repository.
2. Installs .NET 10.
3. Configures Google Chrome.
4. Restores dependencies and builds the project.
5. Executes Selenium tests in headless mode.
6. Uploads TRX results and available failure screenshots.

Test artifacts are retained for 14 days.

[View GitHub Actions workflow](https://github.com/MilosSreckovic/selenium-csharp-nunit-framework/actions/workflows/selenium-tests.yml)

## Reliability Notes

UI automation may be affected by dynamic content, advertisements, and differences between local and CI environments.

The brand navigation test has previously encountered an intermittent element timeout. Screenshot capture improves failure diagnostics, but does not eliminate potential test instability.

## Future Improvements

- Investigate intermittent UI interaction failures.
- Extend browser coverage.
- Improve failure diagnostics and reporting.
- Expand test scenarios as needed.

## Disclaimer

This is an independent QA portfolio project using the public Automation Exercise demo application. It is not affiliated with the application provider.
