@accessibility @playwright
Feature: ZigWheels Accessibility Testing

  As a QA engineer
  I want to scan important ZigWheels pages for accessibility issues
  So that accessibility findings can be identified and documented

  Scenario Outline: Scan key ZigWheels pages for accessibility issues
    Given I open the "<page>" page for accessibility testing
    When I run an axe accessibility scan
    Then the accessibility scan should complete successfully
    And the accessibility findings should be saved to a report

    Examples:
      | page                 |
      | Homepage             |
      | Upcoming Honda Bikes |
      | Used Cars Chennai    |