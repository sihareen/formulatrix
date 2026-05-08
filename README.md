# Task A: CODING COMPETENCY TEST
### 1. The Baseline (Task A.1)
I started with a basic implementation using standard if-else logic to handle the "foo", "bar", and "foobar" rules. While this worked perfectly for a small set of rules, it was "hardcoded"—meaning any change to the rules would require modifying the core logic and recompiling the code. This is fine for a prototype but not for a production system.

### 2. Scaling the Logic (Task A.2)
When the "jazz" rule (divisible by 7) was introduced, the simple if-else structure became problematic. To avoid a combinatorial explosion of conditions (like if 3 AND 5 AND 7), I shifted to a "concatenation strategy." By checking each rule independently and appending the result to a string, the system could naturally handle any combination of rules (e.g., "foobarjazz") without needing explicit conditions for every possible pair.

### 3. Decoupling Data from Logic (Task A.3)
As the rule set grew (adding "baz" and "huzz"), I realized the rules themselves were "data," while the loop was "logic." I refactored the implementation to be table-driven, storing the rules in a Dictionary. This was a critical turning point: the core engine became generic. Adding a new rule no longer required changing the code—only updating the table.

### 4. The Final Architecture: A Configurable API (Task A.4)
The final step was to transform this logic into a professional, reusable class. I implemented a RuleGenerator class that exposes a clean API: `AddRule(int divisor, string word)`.

This design follows the Open-Closed Principle: the class is open for extension (anyone can add any rule at runtime) but closed for modification (the core generation logic never needs to change). To make the final delivery more impressive, I wrapped this API in an interactive console application. Now, the user can specify the range (n) and choose between different rule sets (Basic, Jazz, or Full Table) dynamically.

### HOW TO RUN
-----------

I have designed the project to be "plug-and-play." There is no need to edit the source code to test different scenarios.
1. Execute the program:
   dotnet run
2. Follow the interactive prompts:
   - Enter the value for 'n' (e.g., 100)
   - Select a rule set (1, 2, or 3) to see the evolution of the rules in action.
The program will then output the formatted sequence immediately.

# Task B: Repository Manager - Class Library
## Project Overview
This is a C# Class Library implementation of the Formulatrix Repository Manager. It provides functionality to store, retrieve, and manage JSON and XML content using unique identifiers. The implementation is designed as a reusable class library (compiled to TaskB.dll) that can be integrated into other applications.
## How to Build and Run
### Build the Class Library
```bash
cd TaskB
dotnet build
```
Output: `TaskB/bin/Debug/net8.0/TaskB.dll`

### Run Tests
```bash
cd TaskB.Tests
dotnet run
```
Expected: All tests pass (11/11)
## Implementation Files

| File | Description |
|:---|:---|
| `RepositoryManager.cs` | Main API class with all public methods |
| `InMemoryStorage.cs` | Thread-safe in-memory storage using ConcurrentDictionary |
| `ContentValidator.cs` | Basic JSON/XML structure validation |
| `ItemType.cs` | Enum for content types (JSON=1, XML=2) |
| `RepositoryItem.cs` | Data model for stored items |

---

# Task C: SOFTWARE COMPREHENSION TEST
### Improvement 1: Copy Frame Data on Receive

**Problem Addressed:** Buffer aliasing (#1), Frame disposal misuse (#6)

**Solution:**
Instead of wrapping the native pointer, **copy the frame data** into managed memory:

```csharp
public void FrameReceived(IntPtr pFrame, int pixelWidth, int pixelHeight)
{
    int size = pixelWidth * pixelHeight;
    byte[] frameCopy = new byte[size];
    Marshal.Copy(pFrame, frameCopy, 0, size);
    
    Frame frame = new Frame(frameCopy);
    // Now safe to enqueue and process later
}
```

**Benefit:** Decouples from native library lifecycle, eliminates use-after-free.

---

### Improvement 2: Use Thread-Safe Collection

**Problem Addressed:** Race conditions (#2)

**Solution:**
Replace `Queue<Frame>` with `ConcurrentQueue<Frame>`:

```csharp
private ConcurrentQueue<Frame> _receivedFrames = new ConcurrentQueue<Frame>();

// Producer (thread-safe):
_receivedFrames.Enqueue(frame);

// Consumer (thread-safe):
if(_receivedFrames.TryDequeue(out Frame frame))
{
    // Process frame
}
```

**Benefit:** Eliminates race conditions, no manual locking needed.

---

### Improvement 3: Implement Bounded Queue or Latest-Frame Strategy

**Problem Addressed:** Unbounded growth (#3)

**Solution A (Bounded Queue):**
```csharp
private const int MaxQueueSize = 10;

if(_receivedFrames.Count < MaxQueueSize)
    _receivedFrames.Enqueue(frame);
else
    ; // Drop frame or log warning
```

**Solution B (Latest-Frame Strategy):**
```csharp
private Frame _latestFrame;
private readonly object _lock = new object();

public void FrameReceived(...)
{
    byte[] frameCopy = CopyFrame(pFrame, pixelWidth, pixelHeight);
    lock(_lock)
    {
        _latestFrame?.Dispose();
        _latestFrame = new Frame(frameCopy);
    }
}
```

**Benefit:** Prevents memory exhaustion, ensures timely processing.

---

### Improvement 4: Use Dedicated Worker Thread Instead of Timer

**Problem Addressed:** Timer overlap (#4), reentrancy

**Solution:**
```csharp
private BlockingCollection<Frame> _frameQueue = new BlockingCollection<Frame>(boundedCapacity: 10);
private CancellationTokenSource _cts = new CancellationTokenSource();

public void StartStreaming()
{
    Task.Run(() => ProcessFrames(_cts.Token));
}

private void ProcessFrames(CancellationToken ct)
{
    foreach(var frame in _frameQueue.GetConsumingEnumerable(ct))
    {
        ProcessFrame(frame);
        frame.Dispose();
    }
}
```

**Benefit:** No overlap, deterministic execution, clean cancellation.

---

### Improvement 5: Use Long Accumulator and Double for Average

**Problem Addressed:** Integer overflow (#5), precision loss (#10)

**Solution:**
```csharp
long sum = 0;
for(int i = 0; i < raw.Length; i++)
    sum += raw[i];
double result = (double)sum / raw.Length;
_reporter.Report(result);
```

**Benefit:** Handles any resolution, preserves precision.

---

### Improvement 6: Implement Proper Disposal

**Problem Addressed:** Resource leaks (#7, #9)

**Solution:**
```csharp
public class FrameCalculateAndStream : IDisposable
{
    public void Dispose()
    {
        _cts?.Cancel();
        _timer?.Dispose();
        _frameGrabber.OnFrameUpdated -= HandleFrameUpdated;
        
        while(_frameQueue.TryTake(out var frame))
            frame.Dispose();
    }
}
```

**Benefit:** Clean shutdown, no resource leaks.

---

### Improvement 7: Add Null-Conditional Event Invocation

**Problem Addressed:** Null event crash (#8)

**Solution:**
```csharp
OnFrameUpdated?.Invoke(bufferedFrame);
```

**Benefit:** Prevents crash when no subscribers.

---

### Summary of Improvements

| Problem | Severity | Solution |
|---------|----------|----------|
| Buffer aliasing | Critical | Copy frame data |
| Race conditions | Critical | Use ConcurrentQueue |
| Unbounded growth | High | Bounded queue or latest-frame |
| Timer overlap | Medium | Dedicated worker thread |
| Integer overflow | Medium | Use long accumulator |
| Frame disposal | High | Don't dispose native buffers |
| Event leak | Medium | Unsubscribe in Dispose |
| Null event | Low | Use ?. operator |
| No cleanup | High | Implement IDisposable |
| Precision loss | Low | Use double for average |

---
