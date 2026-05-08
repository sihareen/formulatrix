using System;

namespace TaskA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("   TASK A: CODING COMPETENCY TEST        ");
            Console.WriteLine("==========================================");
            Console.WriteLine();
            Console.WriteLine("This program demonstrates the evolution of");
            Console.WriteLine("a configurable rule-based generator:");
            Console.WriteLine();
            Console.WriteLine("  Rule Set 1 = Task A.1 (Basic FooBar)");
            Console.WriteLine("  Rule Set 2 = Task A.2 (With Jazz)");
            Console.WriteLine("  Rule Set 3 = Task A.3 (Full Table)");
            Console.WriteLine();
            Console.WriteLine("All implemented using the configurable");
            Console.WriteLine("RuleGenerator API (Task A.4)");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            TaskA4.RunTask();
        }
    }
}
