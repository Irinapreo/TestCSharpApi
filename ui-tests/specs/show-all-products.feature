Feature: As user I want that all products are displayed when you select "All" in the category selector.

  Scenario: Check all products are displayed.
    Given that I am on the product page
    When I choose the category "Alla"
    Then I should see the product "<product>"

    Examples:
      | category    | product           |
      | Prisvänligt | Basic tomatsås    |
      | Prisvänligt | Mjöliga makaroner |
      | Vardag      | Potatis           |
      | Vardag      | Gul lök           |
      | Lyx         | Champagne         |
      | Lyx         | Rysk kaviar       |