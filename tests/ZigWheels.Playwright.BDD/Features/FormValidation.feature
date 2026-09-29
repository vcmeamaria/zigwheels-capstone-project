@functional @playwright @formvalidation
Feature: ZigWheels Form Validation

  As a user
  I want required fields to be validated
  So that incomplete form submissions are prevented

  Scenario: Display a warning when the on-road price form is submitted without selecting a make
    Given I am on the ZigWheels on-road price form
    When I submit the on-road price form without selecting a vehicle make
    Then the vehicle make validation warning should be displayed