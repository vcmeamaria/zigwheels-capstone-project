using NUnit.Framework;

// Allows separate Reqnroll feature fixtures to execute concurrently.
//
// Each Playwright BDD scenario creates its own isolated browser context
// through PlaywrightFixture, preventing scenarios from sharing page state.
[assembly: Parallelizable(ParallelScope.Fixtures)]

// Limit the test assembly to a maximum of four concurrent NUnit workers.
[assembly: LevelOfParallelism(4)]