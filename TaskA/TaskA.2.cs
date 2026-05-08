using System;
using System.Collections.Generic;
using System.Text;

public class FooBarJazz
{
    public static string Generate(int n)
    {
        if (n < 1) return string.Empty;

        List<string> results = new List<string>();
        for (int i = 1; i <= n; i++)
        {
            StringBuilder sb = new StringBuilder();

            if (i % 3 == 0) sb.Append("foo");
            if (i % 5 == 0) sb.Append("bar");
            if (i % 7 == 0) sb.Append("jazz");

            string result = sb.ToString();
            results.Add(string.IsNullOrEmpty(result) ? i.ToString() : result);
        }
        return string.Join(", ", results);
    }

    public static void RunTask()
    {
        Console.Write("Masukkan nilai n: ");
        int n = 105;
        if (!int.TryParse(Console.ReadLine(), out n) || n < 1)
        {
            Console.WriteLine("Input tidak valid, menggunakan n=105 sebagai default.");
            n = 105;
        }

        Console.WriteLine($">> {Generate(n)}");
    }
}
