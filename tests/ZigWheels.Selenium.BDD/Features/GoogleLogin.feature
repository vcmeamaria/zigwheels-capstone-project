@functional
@selenium
@window-handling
Feature: Google Login Error Handling

  As a ZigWheels user
  I want to receive clear feedback when Google authentication fails
  So that I understand that the authentication attempt was unsuccessful

  Scenario: Capture the error for an invalid Google account
    Given I am on the ZigWheels homepage
    When I open the ZigWheels login dialog
    And I choose Google login
    Then a Google authentication window should open
    When I choose to use another Google account
    And I submit an invalid Google account identifier
    Then a Google authentication error should be displayed
    And I should be able to return to the ZigWheels window