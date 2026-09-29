# ZigWheels Capstone Project

A C# QA Automation Capstone Project demonstrating functional, performance, accessibility and security testing against the public [ZigWheels](https://www.zigwheels.com/) website.

The project combines Selenium, Playwright, Reqnroll BDD, NUnit, Apache JMeter, axe-core, OWASP ZAP, Docker and Allure reporting within one structured test framework.

---

## Project Status

**Completed**

The final automated functional regression completed successfully:

| Framework | Passed | Failed | Skipped |
|---|---:|---:|---:|
| Selenium BDD | 2 | 0 | 0 |
| Playwright BDD | 7 | 0 | 0 |
| **Total** | **9** | **0** | **0** |

**Final Allure result: 9/9 tests passed — 100%**

---

## Technology Stack

- C#
- .NET 8
- NUnit
- Reqnroll / BDD
- Selenium WebDriver
- Microsoft Playwright
- Allure
- Deque axe-core
- Apache JMeter
- OWASP ZAP
- Docker
- Git / GitHub
- Git Bash

---

# Test Coverage

## Functional Testing

The project automates the main ZigWheels user flows defined by the Capstone brief.

### Upcoming Honda Bikes

Implemented using **Selenium + Reqnroll BDD**.

The scenario:

- Opens ZigWheels
- Navigates to upcoming Honda bikes
- Extracts upcoming bike information
- Filters bikes below ₹4 lakh
- Validates:
  - manufacturer is Honda
  - price is below ₹4 lakh
  - bike name is available
  - price is available
  - expected launch date is available

Example information captured during execution includes:

- Honda XR 300L
- Honda XR 300 Rally
- Honda CRF300L
- Honda Shine Electric

The live data may change because ZigWheels is an external production website.

---

## Used Cars in Chennai

Implemented using **Playwright + Reqnroll BDD**.

The scenario:

- Opens the ZigWheels Used Cars Chennai page
- Collects popular used-car models
- Converts the results into structured test data
- Confirms that valid results are returned

---

## Google Login Error Handling

Implemented using **Selenium + Reqnroll BDD**.

The scenario demonstrates:

- Opening the ZigWheels login interface
- Selecting Google authentication
- Handling a separate authentication window
- Entering a non-production invalid account identifier
- Capturing the resulting authentication error
- Closing the authentication window
- Returning safely to the original ZigWheels window

This scenario demonstrates **multi-window browser handling**.

No genuine account credentials are used.

---

## Menu Navigation and Browser Back

Implemented using **Playwright**.

The scenario:

1. Opens the ZigWheels homepage
2. Handles the cookie-consent dialog when displayed
3. Opens the `NEW BIKES` menu
4. Selects the `Upcoming Bikes` collection
5. Verifies the destination page
6. Uses browser history to navigate back
7. Verifies return to the ZigWheels homepage

This covers:

- menu navigation
- collection navigation
- browser back navigation

---

## Form Validation

Implemented using **Playwright**.

The ZigWheels On-Road Price form is used to verify required-field validation.

The scenario:

1. Opens the On-Road Price page
2. Leaves the required vehicle make unselected
3. Attempts to submit the form
4. Verifies the validation message:

```text
Please select make
```

No genuine personal information is entered or submitted.

---

## iframe Handling

Implemented using **Playwright**.

A ZigWheels video review page is used to demonstrate real iframe handling.

The scenario:

1. Opens the video review page
2. Handles cookie consent
3. Locates the large video preview
4. Activates the video Play control
5. Waits for the YouTube iframe to load
6. Locates the child frame
7. Accesses content inside the iframe
8. Returns to the main-page context
9. Verifies that the ZigWheels page is still available

---

# Parallel Execution

The Playwright BDD suite supports parallel feature execution using NUnit.

Configuration:

```csharp
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]
```

Each scenario uses an isolated Playwright browser context to prevent shared page state.

Parallel execution was verified using the complete Playwright suite:

```text
Total: 7
Passed: 7
Failed: 0
Skipped: 0
```

Overlapping scenario-log timestamps were also observed during execution, confirming concurrent test activity.

---

# Page Object Model

Both Selenium and Playwright tests use Page Object Model principles.

Page objects are responsible for:

- locators
- browser interactions
- navigation
- page-specific behaviour
- extraction of test data

Step definitions remain focused on BDD behaviour and assertions.

This separation improves:

- readability
- reuse
- maintainability
- debugging

---

# Test Reporting and Failure Evidence

## Allure

Both Selenium and Playwright write results to the shared directory:

```text
artifacts/allure-results/
```

This allows the final Allure dashboard to combine both frameworks.

The final clean regression produced:

```text
9 test cases
100% passed
```

Generate the Allure report:

```bash
allure generate artifacts/allure-results \
  --clean \
  -o artifacts/allure-report
```

Open the report:

```bash
allure open artifacts/allure-report
```

Generated Allure reports are excluded from Git.

---

## Failure Artefacts

The framework automatically captures useful debugging evidence when a scenario fails.

Depending on the framework, evidence can include:

- screenshots
- Playwright traces
- scenario logs
- Allure attachments

Artefacts are organised using date, framework, scenario and timestamp information.

Example structure:

```text
artifacts/
├── logs/
├── screenshots/
├── traces/
├── allure-results/
└── allure-report/
```

Generated runtime artefacts are excluded from source control.

---

# Accessibility Testing

Accessibility testing is implemented using:

- Playwright
- Reqnroll
- Deque axe-core

Automated accessibility scans cover:

- Homepage
- Upcoming Honda Bikes
- Used Cars Chennai

The accessibility BDD suite verifies that:

- the scan completes successfully
- findings are captured
- accessibility evidence is written to JSON reports

The scans identified real accessibility observations such as:

- controls without accessible names
- select elements without accessible names
- ARIA naming issues

Findings can include serious or critical axe-core impact classifications.

A successful accessibility test means that the **scan and evidence-generation process completed successfully**. It does not mean that the external ZigWheels website contains no accessibility issues.

Generated evidence is stored under:

```text
accessibility/reports/
```

A manual accessibility checklist is also included under:

```text
accessibility/manual-checks/
```

It covers areas including:

- keyboard navigation
- visible focus
- labels and accessible names
- image alternative text
- heading structure
- colour and contrast
- zoom
- forms and errors
- links and controls
- reading order

---

# Performance Testing

Apache JMeter is used for lightweight performance testing.

Test plans are provided for:

```text
performance/jmeter/test-plans/
```

The covered flows are:

- ZigWheels Homepage
- Upcoming Honda Bikes
- Used Cars Chennai

Each plan includes:

- HTTP requests
- response-code assertions
- response-duration assertions
- test-result listeners for development/debugging

The performance tests were also executed using JMeter's non-GUI mode.

Generated output includes:

- `.jtl` results
- HTML dashboards

Runtime performance reports are stored under:

```text
performance/jmeter/reports/
```

and are excluded from Git.

For detailed JMeter instructions, see:

```text
performance/jmeter/README.md
```

---

# Security Testing

Security testing is implemented using **OWASP ZAP** through Docker.

The project deliberately uses the **ZAP Baseline Scan** rather than aggressive active vulnerability testing because ZigWheels is a third-party live production website.

Security coverage includes:

- Homepage
- Upcoming Honda Bikes
- Used Cars Chennai

The scans generate:

- HTML reports
- JSON reports
- Markdown reports

Example ZAP observations included:

- vulnerable JavaScript library detection
- Content Security Policy configuration findings
- missing security headers
- cookie security configuration findings
- missing Subresource Integrity attributes
- cross-origin policy/header findings

ZAP findings are treated as **automated security observations**, not automatically as confirmed vulnerabilities.

Manual validation would be required before treating scanner output as a confirmed security defect.

Generated reports are excluded from Git.

For full instructions, see:

```text
security/zap/README.md
```

---

# Docker

Docker is used to provide a repeatable OWASP ZAP security-testing environment.

Official image:

```text
ghcr.io/zaproxy/zaproxy:stable
```

Verify Docker:

```bash
docker --version
docker info --format '{{.ServerVersion}}'
```

Pull ZAP:

```bash
docker pull ghcr.io/zaproxy/zaproxy:stable
```

The complete baseline-scan commands are documented in:

```text
security/zap/README.md
```

---

# Project Structure

```text
zigwheels-capstone-project/
│
├── accessibility/
│   ├── manual-checks/
│   └── reports/
│
├── artifacts/
│   ├── allure-report/
│   ├── allure-results/
│   ├── logs/
│   ├── screenshots/
│   └── traces/
│
├── performance/
│   └── jmeter/
│       ├── reports/
│       ├── test-data/
│       └── test-plans/
│
├── security/
│   └── zap/
│       ├── config/
│       ├── reports/
│       └── README.md
│
├── src/
│   └── ZigWheels.Framework.Core/
│
├── tests/
│   ├── ZigWheels.Playwright.BDD/
│   └── ZigWheels.Selenium.BDD/
│
├── .gitignore
├── README.md
└── ZigWheels.Capstone.project.sln
```

Generated report folders are present locally when tests execute but their runtime contents are excluded from Git.

---

# Running the Project

## Prerequisites

Install:

- .NET 8 SDK or compatible newer SDK
- Git
- Git Bash
- supported browsers
- Playwright browser binaries
- Allure CLI
- Apache JMeter
- Docker Desktop

---

## Restore Dependencies

From the repository root:

```bash
dotnet restore
```

---

## Build

```bash
dotnet build
```

---

# Selenium Tests

Run the complete Selenium BDD suite:

```bash
dotnet test tests/ZigWheels.Selenium.BDD/ZigWheels.Selenium.BDD.csproj \
  --logger "console;verbosity=normal"
```

Final verified result:

```text
Total: 2
Passed: 2
Failed: 0
Skipped: 0
```

---

# Playwright Tests

If Playwright browsers have not already been installed, build the project first and install the required browser binaries using the Playwright installation script generated by the package.

Then run:

```bash
dotnet test tests/ZigWheels.Playwright.BDD/ZigWheels.Playwright.BDD.csproj \
  --logger "console;verbosity=normal"
```

Final verified result:

```text
Total: 7
Passed: 7
Failed: 0
Skipped: 0
```

The Playwright suite supports parallel execution.

---

# Run Individual Playwright Areas

Navigation:

```bash
dotnet test tests/ZigWheels.Playwright.BDD/ZigWheels.Playwright.BDD.csproj \
  --filter "TestCategory=navigation"
```

Form validation:

```bash
dotnet test tests/ZigWheels.Playwright.BDD/ZigWheels.Playwright.BDD.csproj \
  --filter "TestCategory=formvalidation"
```

Frame handling:

```bash
dotnet test tests/ZigWheels.Playwright.BDD/ZigWheels.Playwright.BDD.csproj \
  --filter "TestCategory=frames"
```

---

# Final Verification

The completed project was verified using a clean regression run.

## Selenium

```text
2 passed
0 failed
0 skipped
```

## Playwright

```text
7 passed
0 failed
0 skipped
```

## Combined

```text
9 passed
0 failed
0 skipped
100% pass rate
```

A clean Allure results directory was used before the final execution so the final report contains only the completed-project regression results.

---

# Important Testing Notes

ZigWheels is a live third-party production website.

Because the website is outside the control of this project:

- page content can change
- locators can change
- live test data can change
- response times can vary
- external authentication behaviour can change
- advertisements and consent dialogs can affect page behaviour

The framework includes defensive handling for several dynamic behaviours, including cookie-consent dialogs and dynamically loaded video content.

Security testing is intentionally non-destructive.

No genuine personal credentials or sensitive user data are used by the automated tests.

---

# Capstone Objectives Demonstrated

This project demonstrates practical experience with:

- BDD automation
- Selenium
- Playwright
- Page Object Model
- browser-window handling
- iframe handling
- navigation testing
- form validation
- parallel execution
- test-data extraction
- accessibility testing
- performance testing
- security testing
- Docker
- automated reporting
- failure diagnostics
- Git feature-branch workflows