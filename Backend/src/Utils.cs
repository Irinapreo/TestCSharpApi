namespace WebApp;
public static class Utils
{
    private static readonly Arr mockUsers = JSON.Parse(
        File.ReadAllText(FilePath("json", "mock-users.json"))
    );

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
        var read = File.ReadAllText(FilePath("json", "mock-users.json"));
        Arr mockUsers = JSON.Parse(read);
        Arr successFullyWrittenUsers = Arr();
        
        foreach (var user in mockUsers)
        {
            user.password = "12345678";
            var result = SQLQueryOne(
                @"INSERT INTO users(firstName,lastName,email,password)
                VALUES($firstName, $lastName, $email, $password)
            ", user);
            if (!result.HasKey("error"))
            {
                user.Delete("password");
                successFullyWrittenUsers.Push(user);
            }
        }
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