using System;
using System.Collections.Generic;
using System.Text;

public class TableDrivenGenerator
{
    // Rule table: divisor -> word
    private static readonly Dictionary<int, string> Rules = new Dictionary<int, string>
    {
        { 3, "foo" },
        { 4, "baz" },
        { 5, "bar" },
        { 7, "jazz" },
        { 9, "huzz" }
    };

    public static string Generate(int n)
    {
        if (n < 1) return string.Empty;

        List<string> results = new List<string>();
        for (int i = 1; i <= n; i++)
        {
            StringBuilder sb = new StringBuilder();

            // Check each rule in order
            foreach (var rule in Rules)
            {
                if (i % rule.Key == 0)
                {
                    sb.Append(rule.Value);
                }
            }

            string result = sb.ToString();
            results.Add(string.IsNullOrEmpty(result) ? i.ToString() : result);
        }
        return string.Join(", ", results);
    }

    public static void RunTask()
    {
        Console.Write("Masukkan nilai n: ");
        int n = 50;
        if (!int.TryParse(Console.ReadLine(), out n) || n < 1)
        {
            Console.WriteLine("Input tidak valid, menggunakan n=50 sebagai default.");
            n = 50;
        }

        Console.WriteLine($">> {Generate(n)}");
    }
}
