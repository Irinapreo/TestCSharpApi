namespace WebApp;
public class UtilsTest(Xlog Console)
{
    private static readonly Arr mockUsers = JSON.Parse(
        File.ReadAllText(FilePath("json", "mock-users.json"))
    );

    [Theory]
    [InlineData("abC9#fgh", true)]  
    [InlineData("stU5/xyz", true)] 
    [InlineData("abC9#fg", false)]  
    [InlineData("abCd#fgh", false)] 
    [InlineData("abc9#fgh", false)] 
    [InlineData("abC9efgh", false)] 
    public void TestIsPasswordGoodEnough(string password, bool expected)
    {
        Assert.Equal(expected, Utils.IsPasswordGoodEnough(password));
    }

    [Theory]
    [InlineData(
        "---",
        "Hello, I am going through hell. Hell is a real fucking place " +
            "outside your goddamn comfy tortoiseshell!",
        "Hello, I am going through ---. --- is a real --- place " +
            "outside your --- comfy tortoiseshell!"
    )]
    [InlineData(
        "---",
        "Rhinos have a horny knob? (or what should I call it) on " +
            "their heads. And doorknobs are damn round.",
        "Rhinos have a --- ---? (or what should I call it) on " +
            "their heads. And doorknobs are --- round."
    )]
    public void TestRemoveBadWords(string replaceWith, string original, string expected)
    {
        Assert.Equal(expected, Utils.RemoveBadWords(original, replaceWith));
    }

    [Fact]
    public void TestCreateMockUsers()
    {
        var read = File.ReadAllText(FilePath("json", "mock-users.json"));
        Arr mockUsers = JSON.Parse(read);
        Arr usersInDb = SQLQuery("SELECT email FROM users");
        Arr emailsInDb = usersInDb.Map(user => user.email);
        Arr mockUsersNotInDb = mockUsers.Filter(
            mockUser => !emailsInDb.Contains(mockUser.email)
        );
        var result = Utils.CreateMockUsers();
        Console.WriteLine($"The test expected that {mockUsersNotInDb.Length} users should be added.");
        Console.WriteLine($"And {result.Length} users were added.");
        Console.WriteLine("The test also asserts that the users added " +
            "are equivalent (the same) to the expected users!");
        Assert.Equivalent(mockUsersNotInDb, result);
        Console.WriteLine("The test passed!");
    }

    [Fact]
    public void TestRemoveMockUsers()
    {
        // Get all users from the database
        Arr usersInDb = SQLQuery("SELECT email FROM users");
        Arr emailsInDb = usersInDb.Map(user => user.email);

        // Remove all mock users from the database
        Arr removedUsers = Utils.RemoveMockUsers();

        // Only keep the mock users that were actually removed from the database
        Arr mockUsersRemoved = mockUsers.Filter(
            mockUser => emailsInDb.Contains(mockUser.email)
        );

        // Assert that the RemoveMockUsers method returned the correct removed users
        Console.WriteLine($"The test expected that {mockUsersRemoved.Length} users should be removed.");
        Console.WriteLine($"And {removedUsers.Length} users were removed.");
        Console.WriteLine("The test also asserts that the users removed " +
            "are equivalent (the same) to the expected users!");
        Assert.Equivalent(mockUsersRemoved, removedUsers);
        Console.WriteLine("The test passed!");
    }

    [Fact]
    public void TestCountDomainsFromUserEmails()
    {
        // Call the method to count domains from user emails
        Obj domainCounts = Utils.CountDomainsFromUserEmails();

        // Output the domain counts for inspection
        Console.WriteLine("Domain Counts:");
        // Iterating through the keys and values to print them
        foreach (var key in domainCounts.GetKeys())
        {
            Console.WriteLine($"{key}: {domainCounts[key]}");
        }

        // Add your assertions here to validate the domain count logic
    }
}