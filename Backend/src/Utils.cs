namespace WebApp;
public static class Utils
{
    // Read all mock users from file
    private static readonly Arr mockUsers = JSON.Parse(
        File.ReadAllText(FilePath("json", "mock-users.json"))
    );

    // Read all bad words from file and sort from longest to shortest
    // if we didn't sort we would often get "---ing" instead of "---" etc.
    // (Comment out the sort - run the unit tests and see for yourself...)
    private static readonly Arr badWords = ((Arr)JSON.Parse(
        File.ReadAllText(FilePath("json", "bad-words.json"))
    )).Sort((a, b) => ((string)b).Length - ((string)a).Length);

    public static bool IsPasswordGoodEnough(string password)
    {
        return password.Length >= 8
            && password.Any(Char.IsDigit)
            && password.Any(Char.IsLower)
            && password.Any(Char.IsUpper)
            && password.Any(x => !Char.IsLetterOrDigit(x));
    }

    public static bool IsPasswordGoodEnoughRegexVersion(string password)
    {
        // See: https://dev.to/rasaf_ibrahim/write-regex-password-validation-like-a-pro-5175
        var pattern = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
        return Regex.IsMatch(password, pattern);
    }

    public static string RemoveBadWords(string comment, string replaceWith = "---")
    {
        comment = " " + comment;
        replaceWith = " " + replaceWith + "$1";
        badWords.ForEach(bad =>
        {
            var pattern = @$" {bad}([\,\.\!\?\:\; ])";
            comment = Regex.Replace(
                comment, pattern, replaceWith, RegexOptions.IgnoreCase);
        });
        return comment[1..];
    }

    public static Arr CreateMockUsers()
    {
        // Read all mock users from the JSON file
        var read = File.ReadAllText(FilePath("json", "mock-users.json"));
        //parse file
        Arr mockUsers = JSON.Parse(read);
        //new arr 
        Arr successFullyWrittenUsers = Arr();
        
        foreach (var user in mockUsers)
        {
            //give each user a password
            user.password = "12345678";
            //insert each user from json file to db
            var result = SQLQueryOne(
                @"INSERT INTO users(firstName,lastName,email,password)
                VALUES($firstName, $lastName, $email, $password)
            ", user);
            // If we get an error from the DB then we haven't added
            // the mock users, if not we have to add to the successful list
            if (!result.HasKey("error"))
            {
                // The specification says return the user list without password
                user.Delete("password");
                successFullyWrittenUsers.Push(user);
            }
        }
        //return users that have been added
        return successFullyWrittenUsers;
    }

    public static Arr RemoveMockUsers()
    {
        //stores users that are removed from database
        Arr removedUsers = Arr();
        foreach (var user in mockUsers)
        {
            // Remove user from database
            var result = SQLQueryOne(
                "DELETE FROM users WHERE email = $email",
                Obj(new { email = user.email })
            );
            // If no error, add user to the removedUsers array
            if (!result.HasKey("error"))
            {
                // Add user without password to removedUsers array
                var userWithoutPassword = Obj(user);
                userWithoutPassword.Delete("password");
                removedUsers.Push(userWithoutPassword);
            }
        }
        return removedUsers;
    }

    public static Obj CountDomainsFromUserEmails()
    {
        // Retrieve all users' emails from the database
        Arr userEmails = SQLQuery("SELECT email FROM users");

        // Initialize an empty object to store domain counts
        Obj domainCounts = Obj();

        // Iterate through each user's email
        foreach (var userEmail in userEmails)
        {
            // Extract domain from email
            string email = userEmail.email;
            string domain = email.Substring(email.IndexOf('@') + 1);

            // Increment count for the domain or set it to 1 if it's the first occurrence
            if (domainCounts.HasKey(domain))
            {
                domainCounts[domain] = (int)domainCounts[domain] + 1;
            }
            else
            {
                domainCounts[domain] = 1;
            }
        }

        // Return the object containing domain counts
        return domainCounts;
    }

}