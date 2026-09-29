@functional @playwright @frames
Feature: ZigWheels Frame Handling

  As a user
  I want embedded content to be accessible
  So that video content displayed inside an iframe can be viewed

  Scenario: Access an embedded video within an iframe
    Given I open a ZigWheels video review page
    When I access the embedded video iframe
    Then the embedded video frame should be available
    And the ZigWheels page content should remain available outside the iframe