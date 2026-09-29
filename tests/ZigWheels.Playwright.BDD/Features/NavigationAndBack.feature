@functional @playwright @navigation
Feature: ZigWheels Menu Navigation and Back Navigation

  As a user
  I want to navigate through the ZigWheels menu
  So that I can access a vehicle collection and return to the homepage

  Scenario: Navigate to the Upcoming Bikes collection and return to the homepage
    Given I am on the ZigWheels homepage for navigation testing
    When I open the New Bikes menu
    And I select the Upcoming Bikes collection
    Then the Upcoming Bikes collection page should be displayed
    When I navigate back to the previous page
    Then I should return to the ZigWheels homepage