# ZigWheels OWASP ZAP Security Testing

This folder contains the OWASP ZAP security testing configuration and documentation for the ZigWheels Capstone Project.

Security testing is performed using the official OWASP ZAP Docker image and the ZAP Baseline Scan.

## Scope

The following public ZigWheels pages are included:

- ZigWheels Homepage
- Upcoming Honda Bikes
- Used Cars in Chennai

## Testing Approach

The project uses the OWASP ZAP Baseline Scan.

The baseline scan performs passive security analysis and does not intentionally launch active attacks against the target application.

This approach was selected because ZigWheels is an external production website outside the control of this project.

## Docker Image

The official stable ZAP Docker image is used:

```text
ghcr.io/zaproxy/zaproxy:stable
```

Verify Docker:

```bash
docker --version
docker info --format '{{.ServerVersion}}'
```

Pull the ZAP image:

```bash
docker pull ghcr.io/zaproxy/zaproxy:stable
```

## Homepage Scan

```bash
mkdir -p security/zap/reports/2026-09-28/homepage

MSYS_NO_PATHCONV=1 docker run --rm \
-v "$PWD/security/zap/reports/2026-09-28/homepage:/zap/wrk:rw" \
-t ghcr.io/zaproxy/zaproxy:stable \
zap-baseline.py \
-t https://www.zigwheels.com/ \
-m 1 \
-I \
-r homepage-report.html \
-J homepage-report.json \
-w homepage-report.md
```

## Upcoming Honda Bikes Scan

```bash
mkdir -p security/zap/reports/2026-09-28/upcoming-honda-bikes

MSYS_NO_PATHCONV=1 docker run --rm \
-v "$PWD/security/zap/reports/2026-09-28/upcoming-honda-bikes:/zap/wrk:rw" \
-t ghcr.io/zaproxy/zaproxy:stable \
zap-baseline.py \
-t https://www.zigwheels.com/upcoming-honda-bikes \
-m 1 \
-I \
-r upcoming-honda-bikes-report.html \
-J upcoming-honda-bikes-report.json \
-w upcoming-honda-bikes-report.md
```

## Used Cars Chennai Scan

```bash
mkdir -p security/zap/reports/2026-09-28/used-cars-chennai

MSYS_NO_PATHCONV=1 docker run --rm \
-v "$PWD/security/zap/reports/2026-09-28/used-cars-chennai:/zap/wrk:rw" \
-t ghcr.io/zaproxy/zaproxy:stable \
zap-baseline.py \
-t https://www.zigwheels.com/used-car/Chennai \
-m 1 \
-I \
-r used-cars-chennai-report.html \
-J used-cars-chennai-report.json \
-w used-cars-chennai-report.md
```

## Command Options

| Option | Purpose |
|---|---|
| `-n` | Not used directly because `zap-baseline.py` manages ZAP execution |
| `-t` | Target URL |
| `-m 1` | Spider duration of one minute |
| `-I` | Do not fail the process purely because warnings are identified |
| `-r` | Generate HTML report |
| `-J` | Generate JSON report |
| `-w` | Generate Markdown report |

`MSYS_NO_PATHCONV=1` prevents Git Bash on Windows from incorrectly converting the Docker container path.

## Generated Evidence

Each scan produces:

```text
HTML report
JSON report
Markdown report
ZAP configuration file
```

Reports are stored under:

```text
security/zap/reports/<date>/<scenario>/
```

Generated security reports are excluded from Git because they are runtime artefacts and can be regenerated when required.

## Baseline Results

### Homepage

```text
FAIL-NEW: 0
WARN-NEW: 17
PASS: 50
```

### Used Cars Chennai

```text
FAIL-NEW: 0
WARN-NEW: 21
PASS: 46
```

The exact results can change between executions because ZigWheels is a live production website.

## Example Security Findings

The ZAP reports identified potential findings across multiple risk levels.

Examples include:

### High

- Vulnerable JavaScript Library
  - Swiper 11.2.10 detected
  - ZAP associated the library with CVE-2026-27212
  - Recommended remediation: upgrade the affected dependency

### Medium

- Content Security Policy configuration issues
- CSP wildcard directives
- `script-src` using `unsafe-inline`
- `style-src` using `unsafe-inline`
- Content Security Policy header not set on some responses
- Source code disclosure findings
- Missing Subresource Integrity attributes

### Low

- Cookie without `HttpOnly`
- Cookie without `SameSite`
- Permissions Policy header not set
- Cross-Origin policy/header findings
- Unix timestamp disclosure

## Interpretation

ZAP findings are treated as automated security observations rather than automatically confirmed vulnerabilities.

Automated scanner results can contain false positives and should be manually validated before remediation decisions are made.

The purpose of this Capstone security testing is to demonstrate:

- passive security scanning
- security-header analysis
- cookie configuration analysis
- dependency-risk identification
- risk classification
- repeatable Docker-based security testing
- report generation

## Security Limitation

No aggressive active vulnerability scanning is performed against ZigWheels.

ZigWheels is a third-party live production website, so testing is deliberately restricted to passive/baseline security analysis.