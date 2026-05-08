using System;
using System.Collections.Generic;

public class FooBar
{
    public static string Generate(int n)
    {
        if (n < 1) return string.Empty;

        List<string> results = new List<string>();
        for (int i = 1; i <= n; i++)
        {
            if (i % 3 == 0 && i % 5 == 0)
                results.Add("foobar");
            else if (i % 3 == 0)
                results.Add("foo");
            else if (i % 5 == 0)
                results.Add("bar");
            else
                results.Add(i.ToString());
        }
        return string.Join(", ", results);
    }

    public static void RunTask()
    {
        Console.Write("Masukkan nilai n: ");
        int n = 15;
        if (!int.TryParse(Console.ReadLine(), out n) || n < 1)
        {
            Console.WriteLine("Input tidak valid, menggunakan n=15 sebagai default.");
            n = 15;
        }

        Console.WriteLine($">> {Generate(n)}");
    }
}
