Feature: As a user I want all products should be presented with their correct prices regardless of it they are shown filtered on a specific category or not

    Scenario Outline: When I choose the category <"category"> the product <"poductName"> should be shown with the price <"price">
    
    Given that I am on the product page
    When I choose the category <"category">
    Then the product <"productName"> should be shown with the price <"price">

    Examples: 
    | category    | productName       | price
    | Alla        | Basic Tomatsås    | 10
    | Alla        | Mjöliga makaroner | 10
    | Prisvänligt | Basic Tomatsås    | 15
    | Prisvänligt | Mjöliga makaroner | 15