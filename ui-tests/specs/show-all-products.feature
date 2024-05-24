Feature: As user I want that all products are displayed when you select "Alla" in the category selector.

  Scenario: Check all products are displayed.
    Given that I am on the product page
    When I choose the category "Alla"
    Then I should see the product "<product>"

    Examples:
      | category    | productName     | 
      | Alla        | Saturnus        |
      | Alla        | Jorden          | 
      | Alla        | Mars            | 
      | Alla        | Barnards Stjärna|
      | Alla        | Lalande Stjärna | 
      | Alla        | Vintergatan     | 
      | Alla        | Canis Major     |
      | Alla        | Sagittarius Elliptical  |
      | Alla        | Messier 60      |
      | Alla        | Arp 220         |
      | Alla        | Ton 618         |
      | Alla        | Halley          |
      | Alla        | Comet Encke     |
      | Alla        | Hyakutake       |



      



  


      


