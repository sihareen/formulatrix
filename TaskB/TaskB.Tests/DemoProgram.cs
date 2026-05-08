using System;
using System.Collections.Generic;

namespace TaskB
{
    /// <summary>
    /// Demo program to verify the RepositoryManager functionality.
    /// </summary>
    public static class DemoProgram
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Task B: Repository Manager Demo ===\n");
            Console.WriteLine("This demo verifies all public API methods of the RepositoryManager class.\n");

            int testsRun = 0;
            int testsPassed = 0;
            var failures = new List<string>();

            // Test 1: Initialize
            testsRun++;
            if (TestInitialize()) testsPassed++;
            else failures.Add("TestInitialize");

            // Test 2: Register JSON
            testsRun++;
            if (TestRegisterJson()) testsPassed++;
            else failures.Add("TestRegisterJson");

            // Test 3: Register XML
            testsRun++;
            if (TestRegisterXml()) testsPassed++;
            else failures.Add("TestRegisterXml");

            // Test 4: Retrieve
            testsRun++;
            if (TestRetrieve()) testsPassed++;
            else failures.Add("TestRetrieve");

            // Test 5: GetType
            testsRun++;
            if (TestGetType()) testsPassed++;
            else failures.Add("TestGetType");

            // Test 6: Deregister
            testsRun++;
            if (TestDeregister()) testsPassed++;
            else failures.Add("TestDeregister");

            // Test 7: Duplicate Item (Negative)
            testsRun++;
            if (TestDuplicateItem()) testsPassed++;
            else failures.Add("TestDuplicateItem");

            // Test 8: Invalid ItemType (Negative)
            testsRun++;
            if (TestInvalidItemType()) testsPassed++;
            else failures.Add("TestInvalidItemType");

            // Test 9: Null ItemName (Negative)
            testsRun++;
            if (TestNullItemName()) testsPassed++;
            else failures.Add("TestNullItemName");

            // Test 10: Non-Existent Item (Negative)
            testsRun++;
            if (TestNonExistentItem()) testsPassed++;
            else failures.Add("TestNonExistentItem");

            // Test 11: Not Initialized (Negative)
            testsRun++;
            if (TestNotInitialized()) testsPassed++;
            else failures.Add("TestNotInitialized");

            Console.WriteLine("\n=== Results ===");
            Console.WriteLine($"Tests Run: {testsRun}");
            Console.WriteLine($"Passed: {testsPassed}");
            Console.WriteLine($"Failed: {testsRun - testsPassed}");

            if (failures.Count > 0)
            {
                Console.WriteLine("\nFailed Tests:");
                foreach (var f in failures)
                {
                    Console.WriteLine($"  - {f}");
                }
            }
        }

        private static bool TestInitialize()
        {
            Console.WriteLine("Test: Initialize");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                Console.WriteLine("  PASS: Initialize succeeded");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: {ex.Message}");
                return false;
            }
        }

        private static bool TestRegisterJson()
        {
            Console.WriteLine("Test: Register JSON");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                repo.Register("user1", "{\"name\":\"John\",\"age\":30}", 1);
                Console.WriteLine("  PASS: JSON item registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: {ex.Message}");
                return false;
            }
        }

        private static bool TestRegisterXml()
        {
            Console.WriteLine("Test: Register XML");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                repo.Register("config1", "<root><setting>value</setting></root>", 2);
                Console.WriteLine("  PASS: XML item registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: {ex.Message}");
                return false;
            }
        }

        private static bool TestRetrieve()
        {
            Console.WriteLine("Test: Retrieve");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                repo.Register("user1", "{\"name\":\"John\"}", 1);
                string content = repo.Retrieve("user1");
                if (content == "{\"name\":\"John\"}")
                {
                    Console.WriteLine($"  PASS: Retrieved content matches: {content}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"  FAIL: Content mismatch. Expected: {{\"name\":\"John\"}}, Got: {content}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: {ex.Message}");
                return false;
            }
        }

        private static bool TestGetType()
        {
            Console.WriteLine("Test: GetType");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                repo.Register("user1", "{\"name\":\"John\"}", 1);
                repo.Register("config1", "<root/>", 2);
                int type1 = repo.GetType("user1");
                int type2 = repo.GetType("config1");
                if (type1 == 1 && type2 == 2)
                {
                    Console.WriteLine($"  PASS: Types retrieved correctly (JSON={type1}, XML={type2})");
                    return true;
                }
                else
                {
                    Console.WriteLine($"  FAIL: Types incorrect. Expected JSON=1, XML=2, Got JSON={type1}, XML={type2}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: {ex.Message}");
                return false;
            }
        }

        private static bool TestDeregister()
        {
            Console.WriteLine("Test: Deregister");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                repo.Register("user1", "{\"name\":\"John\"}", 1);
                repo.Deregister("user1");
                try
                {
                    repo.Retrieve("user1");
                    Console.WriteLine("  FAIL: Item should have been removed");
                    return false;
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine("  PASS: Item successfully deregistered and cannot be retrieved");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: {ex.Message}");
                return false;
            }
        }

        private static bool TestDuplicateItem()
        {
            Console.WriteLine("Test: Duplicate Item (Negative)");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                repo.Register("user1", "{\"name\":\"John\"}", 1);
                try
                {
                    repo.Register("user1", "{\"name\":\"Jane\"}", 1);
                    Console.WriteLine("  FAIL: Should have thrown exception for duplicate item");
                    return false;
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("already registered"))
                {
                    Console.WriteLine($"  PASS: Exception thrown as expected: {ex.Message}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: Unexpected exception: {ex.Message}");
                return false;
            }
        }

        private static bool TestInvalidItemType()
        {
            Console.WriteLine("Test: Invalid ItemType (Negative)");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                try
                {
                    repo.Register("user1", "{\"name\":\"John\"}", 99);
                    Console.WriteLine("  FAIL: Should have thrown exception for invalid itemType");
                    return false;
                }
                catch (ArgumentException ex) when (ex.Message.Contains("Invalid itemType"))
                {
                    Console.WriteLine($"  PASS: Exception thrown as expected: {ex.Message}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: Unexpected exception: {ex.Message}");
                return false;
            }
        }

        private static bool TestNullItemName()
        {
            Console.WriteLine("Test: Null ItemName (Negative)");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                try
                {
                    repo.Register(null, "{\"name\":\"John\"}", 1);
                    Console.WriteLine("  FAIL: Should have thrown exception for null itemName");
                    return false;
                }
                catch (ArgumentNullException ex) when (ex.Message.Contains("itemName"))
                {
                    Console.WriteLine($"  PASS: Exception thrown as expected: {ex.Message}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: Unexpected exception: {ex.Message}");
                return false;
            }
        }

        private static bool TestNonExistentItem()
        {
            Console.WriteLine("Test: Non-Existent Item (Negative)");
            try
            {
                var repo = new RepositoryManager();
                repo.Initialize();
                try
                {
                    repo.Retrieve("nonexistent");
                    Console.WriteLine("  FAIL: Should have thrown exception for non-existent item");
                    return false;
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine($"  PASS: Exception thrown as expected: {ex.Message}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: Unexpected exception: {ex.Message}");
                return false;
            }
        }

        private static bool TestNotInitialized()
        {
            Console.WriteLine("Test: Not Initialized (Negative)");
            try
            {
                var repo = new RepositoryManager();
                try
                {
                    repo.Register("user1", "{\"name\":\"John\"}", 1);
                    Console.WriteLine("  FAIL: Should have thrown exception for un-initialized repo");
                    return false;
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Initialize() must be called"))
                {
                    Console.WriteLine($"  PASS: Exception thrown as expected: {ex.Message}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL: Unexpected exception: {ex.Message}");
                return false;
            }
        }
    }
}
