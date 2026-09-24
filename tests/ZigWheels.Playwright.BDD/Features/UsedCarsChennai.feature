@functional
@playwright
@smoke
Feature: Popular Used Cars in Chennai

  As a used-car shopper
  I want to view the popular used-car models in Chennai
  So that I can understand which models are commonly available

  Scenario: Extract popular used-car models in Chennai
    Given I am on the ZigWheels used cars page for Chennai
    When I collect the popular used-car models
    Then the popular used-car model collection should not be empty
    And each collected model should have a name
    And the collected models should be displayed in the test output