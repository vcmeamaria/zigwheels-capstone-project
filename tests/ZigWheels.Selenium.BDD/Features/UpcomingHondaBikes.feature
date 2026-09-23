@functional
@smoke
@selenium
Feature: Upcoming Honda Bikes

  As a prospective bike buyer
  I want to identify upcoming Honda bikes costing less than four lakh
  So that I can view suitable future models

  Scenario: Identify upcoming Honda bikes below four lakh
    Given I am on the ZigWheels homepage
    When I navigate to the upcoming Honda bikes page
    And I collect the upcoming Honda bikes priced below four lakh
    Then the filtered bike results should not be empty
    And every filtered bike should be manufactured by Honda
    And every filtered bike should cost less than four lakh
    And every filtered bike should display its name
    And every filtered bike should display its price
    And every filtered bike should display its expected launch date