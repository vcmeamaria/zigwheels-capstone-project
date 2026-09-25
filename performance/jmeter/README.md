# ZigWheels JMeter Performance Testing

This folder contains the Apache JMeter performance tests for the ZigWheels Capstone Project.

The tests provide lightweight performance coverage for the main public user flows already automated within the Selenium and Playwright BDD frameworks.

## Test Scenarios

Three ZigWheels flows are covered:

| Scenario | JMeter Test Plan |
|---|---|
| ZigWheels Homepage | `test-plans/zigwheels-homepage.jmx` |
| Upcoming Honda Bikes | `test-plans/zigwheels-upcoming-honda-bikes.jmx` |
| Used Cars in Chennai | `test-plans/zigwheels-used-cars-chennai.jmx` |

## Load Configuration

Each test currently uses the following controlled load:

| Setting | Value |
|---|---:|
| Virtual Users | 5 |
| Ramp-up Period | 5 seconds |
| Loop Count | 1 |
| Total Requests | 5 |

The load is intentionally small because the tests run against the public ZigWheels website.

## Performance Acceptance Criteria

Each HTTP request includes two assertions:

- HTTP response code must equal `200`
- Response duration must not exceed `2000 ms`

A successful test therefore requires:

```text
HTTP Status = 200
Response Time <= 2000 ms
Error Rate = 0%
```

## Baseline Results

Baseline results recorded on 25 September 2026:

| Scenario | Samples | Average | Minimum | Maximum | Error Rate |
|---|---:|---:|---:|---:|---:|
| Homepage | 5 | 424 ms | 232 ms | 955 ms | 0.00% |
| Upcoming Honda Bikes | 5 | 557 ms | 247 ms | 1007 ms | 0.00% |
| Used Cars Chennai | 5 | 528 ms | 263 ms | 954 ms | 0.00% |

All three scenarios passed the defined HTTP and duration assertions.

> Performance measurements may vary depending on network conditions, server load and the execution environment.

## Running Tests in JMeter GUI

Open Apache JMeter and load one of the `.jmx` files from:

```text
performance/jmeter/test-plans/
```

The GUI contains:

```text
Thread Group
├── HTTP Request Defaults
├── HTTP Cookie Manager
├── HTTP Request
│   ├── Response Code Assertion
│   └── Duration Assertion
├── View Results Tree
└── Summary Report
```

The `View Results Tree` listener is intended for test development and debugging.

For performance execution and report generation, non-GUI mode should be used.

## Running from Git Bash

JMeter 5.6.3 is used for this project.

If JMeter is not configured in the system PATH, define its executable for the current Git Bash session.

Example:

```bash
JMETER="/c/path/to/apache-jmeter-5.6.3/bin/jmeter.bat"
```

Verify the installation:

```bash
"$JMETER" --version
```

## Homepage Performance Test

```bash
"$JMETER" \
-n \
-t performance/jmeter/test-plans/zigwheels-homepage.jmx \
-l performance/jmeter/reports/homepage-results.jtl
```

## Upcoming Honda Bikes Performance Test

```bash
"$JMETER" \
-n \
-t performance/jmeter/test-plans/zigwheels-upcoming-honda-bikes.jmx \
-l performance/jmeter/reports/upcoming-honda-bikes-results.jtl
```

## Used Cars Chennai Performance Test

```bash
"$JMETER" \
-n \
-t performance/jmeter/test-plans/zigwheels-used-cars-chennai.jmx \
-l performance/jmeter/reports/used-cars-chennai-results.jtl
```

## Generate an HTML Dashboard

JMeter can run the test and automatically generate an HTML performance dashboard.

Example:

```bash
"$JMETER" \
-n \
-t performance/jmeter/test-plans/zigwheels-homepage.jmx \
-l performance/jmeter/reports/homepage-results.jtl \
-e \
-o performance/jmeter/reports/homepage
```

Open the generated report:

```text
performance/jmeter/reports/homepage/index.html
```

The dashboard provides metrics including:

- response times
- throughput
- error rate
- latency
- APDEX
- request statistics

## Generated Reports

Runtime reports are stored under:

```text
performance/jmeter/reports/
```

Generated JMeter reports and `.jtl` result files are excluded from Git through `.gitignore`.

The `.gitkeep` file allows the reports directory itself to remain part of the repository.

## Test Data

Reusable JMeter test data can be stored under:

```text
performance/jmeter/test-data/
```

The directory is currently retained using `.gitkeep`.

## Notes

These performance tests are designed as controlled Capstone demonstrations rather than high-volume load or stress tests against the public ZigWheels production environment.

Higher-volume performance testing should only be performed against an authorised environment with appropriate permission and capacity planning.