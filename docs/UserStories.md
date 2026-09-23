\# User Stories



\## ZigWheels QA Capstone Project



This document defines the initial Agile user stories for the ZigWheels QA Capstone Project.



The stories may be refined during implementation if the current ZigWheels website behaviour differs from the original project brief.



\---



\## US01 - View Upcoming Honda Bikes



\### User Story



As a prospective bike buyer,  

I want to view upcoming Honda bikes costing less than ₹4 lakh,  

so that I can identify suitable future models.



\### Acceptance Criteria



\- The manufacturer is Honda.

\- The bike price is below ₹400,000.

\- The bike name is displayed.

\- The price is displayed.

\- The expected launch date in India is displayed.

\- Matching bike information can be stored as structured test data.

\- The results can be displayed or logged by the automation.



\### Planned Coverage



\- Functional testing

\- BDD

\- Selenium

\- Playwright

\- Data extraction

\- Assertions

\- Allure reporting



\---



\## US02 - View Popular Used Cars in Chennai



\### User Story



As a used-car shopper,  

I want to view popular used-car models available for Chennai,  

so that I can understand the models presented for that location.



\### Acceptance Criteria



\- The user can navigate to the Used Cars area.

\- Chennai can be selected or accessed as the required location.

\- Popular used-car models are identified.

\- The extracted model names are stored in a collection.

\- The extracted model names can be displayed or logged.

\- The flow can return to the relevant ZigWheels page after completion.



\### Planned Coverage



\- Functional testing

\- BDD

\- Playwright

\- Selenium where useful

\- Collections

\- Navigation

\- Allure reporting



\---



\## US03 - Handle Invalid Google Login



\### User Story



As a user,  

I want to receive a clear error or warning when authentication is unsuccessful,  

so that I understand that login has failed.



\### Acceptance Criteria



\- The ZigWheels login area can be opened.

\- Google login can be triggered where currently supported by the website.

\- Any new browser window or tab is handled correctly.

\- Only non-production test details are used.

\- No credentials are hardcoded in source code.

\- The resulting error or warning message is captured where the external authentication flow permits it.

\- The automation can return control to ZigWheels.



\### Planned Coverage



\- Functional testing

\- BDD

\- Selenium

\- Browser window/tab handling

\- Warning/error capture

\- Secure test configuration

\- Allure reporting



\---



\## US04 - Reliable Site Navigation



\### User Story



As a ZigWheels user,  

I want navigation between relevant site sections to behave consistently,  

so that I can move through the required journeys successfully.



\### Acceptance Criteria



\- Relevant navigation controls can be identified.

\- Required menu items can be extracted into collections where applicable.

\- Navigation to the required project areas works.

\- The automation can navigate back to the homepage.

\- Frames are handled only where a genuine and relevant frame exists.

\- Tests do not invent artificial frame handling simply to satisfy a requirement.



\### Planned Coverage



\- Functional testing

\- BDD

\- Selenium

\- Playwright

\- Navigation

\- Collections

\- Window/frame handling where applicable



\---



\## US05 - Accessible Core Journeys



\### User Story



As a keyboard or assistive-technology user,  

I want important ZigWheels functionality to remain accessible without relying solely on a mouse,  

so that I can navigate and interact with important content.



\### Acceptance Criteria



\- Important navigation can be assessed using keyboard-only interaction.

\- Important interactive controls expose meaningful accessible names where applicable.

\- Keyboard focus behaviour is assessed.

\- Basic automated accessibility scanning is performed.

\- Accessibility violations are captured as test evidence.

\- Automated checks are not presented as proof of complete screen-reader compatibility.

\- A small manual accessibility checklist is maintained for checks requiring human verification.



\### Planned Coverage



\- Playwright

\- Accessibility automation

\- Keyboard-only testing

\- Automated accessibility scanning

\- Manual accessibility checks

\- Reporting



\---



\## US06 - Measure Critical Journey Performance



\### User Story



As a QA engineer,  

I want to measure the performance of important ZigWheels journeys,  

so that slow responses and request failures can be identified.



\### Acceptance Criteria



\- JMeter is used for performance testing.

\- Test traffic remains intentionally small and responsible.

\- Response time is measured.

\- Latency is captured where appropriate.

\- Throughput is measured.

\- Error rate is measured.

\- Percentile response times are reviewed where useful.

\- A JMeter HTML report can be generated.



\### Planned Coverage



\- Apache JMeter

\- Performance metrics

\- Responsible low-volume execution

\- HTML reporting



\---



\## US07 - Perform Basic Security Checks



\### User Story



As a QA engineer,  

I want to perform basic non-invasive security checks,  

so that observable security configuration issues can be identified responsibly.



\### Acceptance Criteria



\- HTTPS usage is reviewed.

\- HTTP-to-HTTPS behaviour is reviewed where applicable.

\- Observable security response headers are reviewed.

\- Observable cookie security flags are reviewed.

\- OWASP ZAP passive or baseline scanning is used where appropriate.

\- Findings can be exported into a report.

\- No destructive or aggressive security testing is performed against the public website.



\### Planned Coverage



\- OWASP ZAP

\- Passive security testing

\- Security configuration review

\- Reporting

