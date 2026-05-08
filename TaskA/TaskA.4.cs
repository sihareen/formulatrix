using System;
using System.Collections.Generic;
using System.Text;


public class RuleGenerator
{
    private readonly List<(int divisor, string word)> _rules = new List<(int, string)>();

    public void AddRule(int divisor, string word)
    {
        if (divisor <= 0) throw new ArgumentException("Divisor must be positive.", nameof(divisor));
        if (string.IsNullOrEmpty(word)) throw new ArgumentException("Word cannot be null or empty.", nameof(word));

        _rules.Add((divisor, word));
    }

       public string Generate(int n)
    {
        if (n < 1) return string.Empty;

        List<string> results = new List<string>();
        for (int i = 1; i <= n; i++)
        {
            StringBuilder sb = new StringBuilder();

            // Check each configured rule
            foreach (var rule in _rules)
            {
                if (i % rule.divisor == 0)
                {
                    sb.Append(rule.word);
                }
            }

            string result = sb.ToString();
            results.Add(string.IsNullOrEmpty(result) ? i.ToString() : result);
        }
        return string.Join(", ", results);
    }


    public void Run(int n)
    {
        Console.WriteLine($">> {Generate(n)}");
    }

    public string GetResult(int x)
    {
        if (x < 1) return string.Empty;

        StringBuilder sb = new StringBuilder();
        foreach (var rule in _rules)
        {
            if (x % rule.divisor == 0)
            {
                sb.Append(rule.word);
            }
        }

        string result = sb.ToString();
        return string.IsNullOrEmpty(result) ? x.ToString() : result;
    }

    public void ClearRules()
    {
        _rules.Clear();
    }
}


public static class RuleSets
{
    public static void ApplyRuleSet1(RuleGenerator gen)
    {
        // Rule 1: Basic (3→foo, 5→bar)
        gen.AddRule(3, "foo");
        gen.AddRule(5, "bar");
    }

    public static void ApplyRuleSet2(RuleGenerator gen)
    {
        // Rule 2: With Jazz (3→foo, 5→bar, 7→jazz)
        gen.AddRule(3, "foo");
        gen.AddRule(5, "bar");
        gen.AddRule(7, "jazz");
    }

    public static void ApplyRuleSet3(RuleGenerator gen)
    {
        // Rule 3: Full Table (3→foo, 4→baz, 5→bar, 7→jazz, 9→huzz)
        gen.AddRule(3, "foo");
        gen.AddRule(4, "baz");
        gen.AddRule(5, "bar");
        gen.AddRule(7, "jazz");
        gen.AddRule(9, "huzz");
    }

    public static string GetRuleSetName(int choice)
    {
        return choice switch
        {
            1 => "Basic (3→foo, 5→bar)",
            2 => "With Jazz (3→foo, 5→bar, 7→jazz)",
            3 => "Full Table (3→foo, 4→baz, 5→bar, 7→jazz, 9→huzz)",
            _ => "Unknown"
        };
    }
}

public static class TaskA4
{
    public static void RunTask()
    {
        Console.WriteLine("=== Task A.4: Configurable Rule Generator ===\n");

        // Get user input for n
        Console.Write("Masukkan nilai n: ");
        int n = 15; // default
        if (!int.TryParse(Console.ReadLine(), out n) || n < 1)
        {
            Console.WriteLine("Input tidak valid, menggunakan n=15 sebagai default.");
            n = 15;
        }

        // Get user choice for rule set
        Console.WriteLine("\nPilih rule set:");
        Console.WriteLine("1. Basic (3→foo, 5→bar)");
        Console.WriteLine("2. With Jazz (3→foo, 5→bar, 7→jazz)");
        Console.WriteLine("3. Full Table (3→foo, 4→baz, 5→bar, 7→jazz, 9→huzz)");

        Console.Write("\nPilihan (1/2/3): ");
        int choice = 1; // default
        if (!int.TryParse(Console.ReadLine(), out choice))
        {
            choice = 1;
        }

        // Validate choice
        if (choice < 1 || choice > 3)
        {
            Console.WriteLine("Pilihan tidak valid, menggunakan rule set 1 sebagai default.");
            choice = 1;
        }

        // Apply selected rule set
        var generator = new RuleGenerator();
        switch (choice)
        {
            case 1:
                RuleSets.ApplyRuleSet1(generator);
                break;
            case 2:
                RuleSets.ApplyRuleSet2(generator);
                break;
            case 3:
                RuleSets.ApplyRuleSet3(generator);
                break;
        }

        // Display selected rules
        Console.WriteLine($"\nMenggunakan n={n} dengan rule set {choice}: {RuleSets.GetRuleSetName(choice)}\n");

        // Generate and print
        generator.Run(n);
    }
}
