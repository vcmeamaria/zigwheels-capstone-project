# ZigWheels Manual Accessibility Checklist

This checklist complements the automated axe-core accessibility scans used in the ZigWheels Capstone Project.

Automated tools can identify many accessibility issues, but manual testing is also required to evaluate areas such as keyboard usability, visible focus, content structure, zoom behaviour and user experience.

## Pages in Scope

Manual accessibility checks should be completed against:

- ZigWheels Homepage
- Upcoming Honda Bikes
- Used Cars in Chennai

---

# Test Environment

| Item | Details |
|---|---|
| Website | https://www.zigwheels.com/ |
| Browser | Chromium / Chrome |
| Operating System | Windows |
| Automated Scanner | axe-core via Playwright |
| Standard | WCAG 2.x |
| Test Type | Manual Accessibility Review |

---

# Manual Test Checklist

## 1. Keyboard Navigation

### Objective

Verify that important interactive elements can be reached and operated without using a mouse.

### Checks

- [ ] Pressing `Tab` moves focus through interactive elements.
- [ ] Focus order follows a logical sequence.
- [ ] Links can be activated using `Enter`.
- [ ] Buttons can be activated using `Enter` or `Space`.
- [ ] Dropdowns and selectable controls can be operated with the keyboard.
- [ ] No keyboard traps are encountered.
- [ ] Modal dialogs can be entered and exited using the keyboard.
- [ ] The user can return to the main page after closing a modal.

### Result

**Status:** Not Tested

**Notes:**

---

## 2. Visible Focus Indicator

### Objective

Verify that users can visually determine which element currently has keyboard focus.

### Checks

- [ ] Focused links have a visible focus indicator.
- [ ] Focused buttons have a visible focus indicator.
- [ ] Search fields clearly show when they have focus.
- [ ] Dropdown controls clearly show when they have focus.
- [ ] Focus indication has sufficient visibility against the background.

### Result

**Status:** Not Tested

**Notes:**

---

## 3. Accessible Names and Labels

### Objective

Verify that interactive controls have understandable names or labels.

### Checks

- [ ] Search controls have meaningful accessible names.
- [ ] Form fields have visible or programmatically associated labels.
- [ ] Dropdowns have accessible names.
- [ ] Icon-only buttons have accessible names.
- [ ] Login/account controls have understandable accessible names.
- [ ] Links have meaningful text rather than ambiguous labels.

### Result

**Status:** Not Tested

**Notes:**

---

## 4. Images and Alternative Text

### Objective

Verify that meaningful images expose appropriate alternative text.

### Checks

- [ ] Informative images have meaningful `alt` text.
- [ ] Decorative images do not introduce unnecessary screen-reader content.
- [ ] Vehicle images have useful alternative descriptions where appropriate.
- [ ] Logos have an accessible name.
- [ ] Images used as links provide an understandable accessible purpose.

### Result

**Status:** Not Tested

**Notes:**

---

## 5. Heading Structure

### Objective

Check whether headings communicate the page structure logically.

### Checks

- [ ] The page has an identifiable primary heading.
- [ ] Heading levels follow a logical hierarchy.
- [ ] Headings describe the content that follows.
- [ ] Heading levels are not used purely for visual styling.
- [ ] Sections such as bike listings or used-car listings have meaningful headings.

### Result

**Status:** Not Tested

**Notes:**

---

## 6. Colour and Contrast

### Objective

Check whether important text and controls remain understandable for users with reduced vision or colour perception.

### Checks

- [ ] Text is readable against its background.
- [ ] Important controls have sufficient visual contrast.
- [ ] Links are identifiable and do not rely solely on colour where possible.
- [ ] Error states are not communicated using colour alone.
- [ ] Focus indicators remain visible against surrounding colours.

### Result

**Status:** Not Tested

**Notes:**

---

## 7. Browser Zoom

### Objective

Verify that content remains usable when magnified.

### Procedure

Test the page at:

- 100%
- 150%
- 200%

### Checks

- [ ] Text remains readable at 200% zoom.
- [ ] Important content is not clipped.
- [ ] Controls remain usable.
- [ ] Content does not significantly overlap.
- [ ] Navigation remains accessible.
- [ ] Horizontal scrolling does not make core functionality unusable.

### Result

**Status:** Not Tested

**Notes:**

---

## 8. Forms and Error Identification

### Objective

Check whether forms communicate their purpose and errors accessibly.

### Checks

- [ ] Required information is clearly indicated.
- [ ] Inputs have understandable labels.
- [ ] Validation errors are understandable.
- [ ] Error information is not communicated by colour alone.
- [ ] Error messages identify the affected field where applicable.
- [ ] Users can recover from validation errors using the keyboard.

### Result

**Status:** Not Tested

**Notes:**

---

## 9. Links and Interactive Elements

### Objective

Verify that interactive elements clearly communicate their purpose.

### Checks

- [ ] Link text describes its destination or purpose.
- [ ] Buttons describe the action they perform.
- [ ] Interactive icons have accessible names.
- [ ] Elements that behave like buttons are exposed appropriately.
- [ ] Disabled elements are distinguishable from active controls.

### Result

**Status:** Not Tested

**Notes:**

---

## 10. Page Structure and Reading Order

### Objective

Evaluate whether content is presented in a meaningful order.

### Checks

- [ ] Content follows a logical visual order.
- [ ] Keyboard focus broadly follows the visual layout.
- [ ] Main content can be identified.
- [ ] Navigation areas are understandable.
- [ ] Repeated components behave consistently.
- [ ] Pop-ups or dialogs do not unexpectedly disrupt navigation.

### Result

**Status:** Not Tested

**Notes:**

---

# Page Results

## ZigWheels Homepage

| Area | Status | Notes |
|---|---|---|
| Keyboard Navigation | Not Tested | |
| Visible Focus | Not Tested | |
| Labels / Accessible Names | Not Tested | |
| Images / Alt Text | Not Tested | |
| Heading Structure | Not Tested | |
| Colour / Contrast | Not Tested | |
| Zoom | Not Tested | |
| Forms / Errors | Not Tested | |
| Links / Controls | Not Tested | |
| Reading Order | Not Tested | |

---

## Upcoming Honda Bikes

| Area | Status | Notes |
|---|---|---|
| Keyboard Navigation | Not Tested | |
| Visible Focus | Not Tested | |
| Labels / Accessible Names | Not Tested | |
| Images / Alt Text | Not Tested | |
| Heading Structure | Not Tested | |
| Colour / Contrast | Not Tested | |
| Zoom | Not Tested | |
| Forms / Errors | Not Tested | |
| Links / Controls | Not Tested | |
| Reading Order | Not Tested | |

---

## Used Cars Chennai

| Area | Status | Notes |
|---|---|---|
| Keyboard Navigation | Not Tested | |
| Visible Focus | Not Tested | |
| Labels / Accessible Names | Not Tested | |
| Images / Alt Text | Not Tested | |
| Heading Structure | Not Tested | |
| Colour / Contrast | Not Tested | |
| Zoom | Not Tested | |
| Forms / Errors | Not Tested | |
| Links / Controls | Not Tested | |
| Reading Order | Not Tested | |

---

# Automated Accessibility Evidence

Automated axe-core scans are generated separately under:

```text
accessibility/reports/