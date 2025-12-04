# Asynchronous Methods in C# – Exercise Sheet

## Overview

In this exercise, you will practice the basics of **asynchronous programming** in C# using:

- `Task` and `Task<T>`
- The `async` and `await` keywords
- Composing multiple async methods
- Handling errors in async code

You will work in a simple **console application** and simulate I/O work using `Task.Delay`.

---

## Setup

Create a new **C# console application**.

In `Program.cs`, start with this basic skeleton:

```csharp
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Async Exercises Starting...");

        // You will call your exercise methods from here
        // Example:
        // await RunExercise1Async();

        Console.WriteLine("Done.");
    }
}
```

You will add methods and logic for each task below.  
**Important rule:** Do **not** use `.Result` or `.Wait()` anywhere. Use `await` instead.

---

## Task 1 – Your First Async Method

### Goal

Create a simple async method that simulates a slow operation, such as fetching data from an API.

### Instructions

1. Create a method with this signature:

   ```csharp
   static async Task<string> GetGreetingAsync()
   ```

2. Inside `GetGreetingAsync`:

   - Use `await Task.Delay(1000);` to simulate a 1-second delay.
   - After the delay, return the string: `"Hello from async world!"`.

3. In `Main`, call this method and print the result:

   ```csharp
   var greeting = await GetGreetingAsync();
   Console.WriteLine(greeting);
   ```

### What you should see

After about 1 second, the console should print:

```text
Hello from async world!
```

---

## Task 2 – Composing Async Methods

### Goal

Call multiple async methods in sequence and understand that `await` resumes after each one completes.

### Instructions

1. Add two new methods:

   ```csharp
   static async Task<string> GetUserNameAsync()
   {
       // simulate a slow operation
   }

   static async Task<string> GetUserRoleAsync()
   {
       // simulate a slow operation
   }
   ```

2. Inside each method:

   - Use `await Task.Delay(1000);` to simulate work.
   - `GetUserNameAsync` should return: `"Alice"`.
   - `GetUserRoleAsync` should return: `"Administrator"`.

3. Create another method:

   ```csharp
   static async Task<string> BuildUserDescriptionAsync()
   ```

   Inside this method:

   - Call `GetUserNameAsync()` and `GetUserRoleAsync()` using `await`.
   - Build and return a string like: `"User Alice has role Administrator"`.

4. In `Main`, call:

   ```csharp
   var description = await BuildUserDescriptionAsync();
   Console.WriteLine(description);
   ```

### What you should see

After about 2 seconds total, you should see:

```text
User Alice has role Administrator
```

---

## Task 3 – Async “All the Way” (Service Style)

### Goal

Propagate async from a lower-level method up through a higher-level method and then to `Main`.

### Instructions

Imagine you have a service that fetches some configuration from a remote system.

1. Add this method:

   ```csharp
   static async Task<string> FetchConfigAsync()
   {
       Console.WriteLine("Fetching config...");
       await Task.Delay(1500); // simulate remote call
       return "Config: RetryCount=3;Timeout=5000;";
   }
   ```

2. Add a higher-level method:

   ```csharp
   static async Task PrintConfigAsync()
   {
       // Call FetchConfigAsync, then print the result
   }
   ```

   Inside `PrintConfigAsync`:

   - Call `FetchConfigAsync()` using `await`.
   - Print the returned config string to the console.

3. In `Main`, call:

   ```csharp
   await PrintConfigAsync();
   ```

### Checkpoint

You should now see in the console:

```text
Fetching config...
Config: RetryCount=3;Timeout=5000;
```

Notice that:

- `FetchConfigAsync` is async.
- `PrintConfigAsync` is async because it awaits `FetchConfigAsync`.
- `Main` is async because it awaits `PrintConfigAsync`.

This is the **“async all the way”** pattern.

---

## Task 4 – Running Multiple Async Operations with Task.WhenAll

### Goal

Start multiple independent async operations and wait for all of them to finish.

### Instructions

You will simulate three independent operations, such as loading different parts of a dashboard.

1. Add three methods:

   ```csharp
   static async Task<string> LoadProfileAsync()
   {
       await Task.Delay(1000);
       return "Profile loaded";
   }

   static async Task<string> LoadNotificationsAsync()
   {
       await Task.Delay(1500);
       return "Notifications loaded";
   }

   static async Task<string> LoadMessagesAsync()
   {
       await Task.Delay(2000);
       return "Messages loaded";
   }
   ```

2. Add a method:

   ```csharp
   static async Task LoadDashboardAsync()
   {
       // Start the three tasks
       // Wait for all of them
       // Print their results
   }
   ```

   Inside `LoadDashboardAsync`:

   - Start all three tasks **without** immediately awaiting them:

     ```csharp
     var profileTask = LoadProfileAsync();
     var notificationsTask = LoadNotificationsAsync();
     var messagesTask = LoadMessagesAsync();
     ```

   - Use:

     ```csharp
     var results = await Task.WhenAll(profileTask, notificationsTask, messagesTask);
     ```

   - Loop through `results` and print each line.

3. In `Main`, call:

   ```csharp
   await LoadDashboardAsync();
   ```

### What you should see

After about 2 seconds (not 1 + 1.5 + 2 = 4.5), you should see something like:

```text
Profile loaded
Notifications loaded
Messages loaded
```

All three operations ran **concurrently**, and you waited for all of them to complete.

---

## Task 5 – Error Handling in Async Methods

### Goal

Handle exceptions thrown by async methods using `try` / `catch`.

### Instructions

1. Add this method:

   ```csharp
   static async Task<string> GetPaymentStatusAsync(bool simulateError)
   {
       await Task.Delay(1000);

       if (simulateError)
       {
           throw new InvalidOperationException("Payment gateway is unavailable.");
       }

       return "Payment completed";
   }
   ```

2. Add another method:

   ```csharp
   static async Task CheckPaymentAsync()
   {
       // Call GetPaymentStatusAsync in a try/catch
   }
   ```

   Inside `CheckPaymentAsync`:

   - Use a `try` / `catch` block.
   - In the `try`, call:

     ```csharp
     var status = await GetPaymentStatusAsync(simulateError: true);
     Console.WriteLine(status);
     ```

   - In the `catch (Exception ex)`, print a friendly message, for example:

     ```csharp
     Console.WriteLine($"Could not get payment status: {ex.Message}");
     ```

3. In `Main`, call:

   ```csharp
   await CheckPaymentAsync();
   ```

### What you should see

After about 1 second, you should see:

```text
Could not get payment status: Payment gateway is unavailable.
```

The important part: the exception from the async method is caught at the `await` line in your `try` block, just like synchronous code.

---

## Task 6 (Stretch) – Basic Cancellation

### Goal

Introduce a simple cancellation scenario using `CancellationToken`.

### Instructions

> This is optional, but useful if you want a first contact with cancellation.

1. Add `using System.Threading;` at the top of your file.

2. Add this method:

   ```csharp
   static async Task LongRunningOperationAsync(CancellationToken cancellationToken)
   {
       Console.WriteLine("Long operation started...");

       for (int i = 1; i <= 10; i++)
       {
           cancellationToken.ThrowIfCancellationRequested();

           Console.WriteLine($"Step {i}...");
           await Task.Delay(500, cancellationToken);
       }

       Console.WriteLine("Long operation completed.");
   }
   ```

3. In `Main`, create a `CancellationTokenSource` and cancel it after a short delay:

   ```csharp
   var cts = new CancellationTokenSource();

   var longTask = LongRunningOperationAsync(cts.Token);

   // Simulate user cancelling after 2 seconds
   await Task.Delay(2000);
   cts.Cancel();

   try
   {
       await longTask;
   }
   catch (OperationCanceledException)
   {
       Console.WriteLine("Operation was cancelled.");
   }
   ```

### What you should see

- A few “Step X…” messages.
- Then: `Operation was cancelled.`

You’ve just wired up a basic cooperative cancellation pattern with async methods.

---

## What You Should Hand In

By the end of these tasks, you should have:

1. A `Program.cs` (or equivalent) that:
   - Contains the async methods for Tasks 1–5 (and Task 6 if attempted).
   - Calls each exercise method from `Main` in a sensible order.
2. Console output demonstrating that:
   - Async methods run correctly.
   - `await` is used instead of `.Result` or `.Wait()`.
   - Exceptions in async methods are properly handled with `try` / `catch`.

Focus on:

- Clear method names (`GetGreetingAsync`, `LoadDashboardAsync`, etc.).
- Correct use of `async` / `await`.
- No blocking calls on tasks.
