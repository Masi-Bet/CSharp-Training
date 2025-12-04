# LINQ Challenge – Mini Sales Analytics

## Overview

You are building a small internal **sales analytics** tool using C# and LINQ.

You are given four in-memory collections:

- `Product` – what is sold  
- `Customer` – who buys  
- `Order` – each purchase  
- `OrderItem` – line items per order  

Your job: **use LINQ** to answer the business questions in the tasks below.

---

## Setup

Create a new C# console application and add the following model types and sample data:

```csharp
public record Product(int Id, string Name, string Category, decimal Price);
public record Customer(int Id, string Name, string City);
public record Order(int Id, int CustomerId, DateTime Date);
public record OrderItem(int OrderId, int ProductId, int Quantity);

List<Product> products =
[
    new(1, "Laptop Pro 15",        "Computers",   25000m),
    new(2, "Laptop Air 13",        "Computers",   18000m),
    new(3, "Wireless Mouse",       "Accessories",   350m),
    new(4, "Mechanical Keyboard",  "Accessories",  1200m),
    new(5, "4K Monitor",           "Displays",     7000m),
    new(6, "USB-C Hub",            "Accessories",   600m)
];

List<Customer> customers =
[
    new(1, "Acme Corp",        "Johannesburg"),
    new(2, "Global Dynamics",  "Cape Town"),
    new(3, "Innotech",         "Johannesburg"),
    new(4, "Blue Ocean Ltd",   "Durban")
];

List<Order> orders =
[
    new(1, 1, new DateTime(2025, 01, 10)),
    new(2, 1, new DateTime(2025, 01, 15)),
    new(3, 2, new DateTime(2025, 02, 01)),
    new(4, 3, new DateTime(2025, 02, 10)),
    new(5, 4, new DateTime(2025, 02, 20)),
    new(6, 2, new DateTime(2025, 03, 05))
];

List<OrderItem> orderItems =
[
    new(1, 1,  5),   // 5 Laptop Pro for Acme
    new(1, 3, 10),
    new(2, 2,  3),
    new(2, 6,  5),
    new(3, 5,  2),
    new(3, 3, 15),
    new(4, 1,  1),
    new(4, 4,  3),
    new(5, 2,  2),
    new(5, 5,  1),
    new(6, 6, 10)
];
```

---

## General Rules

For all tasks:

1. Use **LINQ** for the core logic (`Where`, `Select`, `GroupBy`, `Join`, `OrderBy`, etc.).
2. Do **not** use `foreach` to implement the logic. You may use `foreach` only to print results.
3. Use **at least two** queries written in **query syntax** (`from ... in ... where ... select ...`) and **at least two** written in **method syntax** (`.Where(...).Select(...)`).

---

## Task 1 – Order Totals

For each order, calculate the **total order value**.

- Use `orders`, `orderItems`, and `products`.
- Total per order is: `sum of (Quantity × Product.Price)` for that order.
- Include the **customer name** in the output.

**Requirements:**

- Produce a sequence where each element contains:
  - `OrderId`
  - `CustomerName`
  - `OrderDate`
  - `OrderTotal`
- Sort the sequence by `OrderDate` ascending.

**Example output format (you can format it your own way):**

```text
Order 1 – Customer: Acme Corp – Date: 2025-01-10 – Total: Rxxxxx
Order 2 – Customer: Acme Corp – Date: 2025-01-15 – Total: Rxxxxx
...
```

Use either method syntax or query syntax for this task.

---

## Task 2 – Top Customers by Spend

Find the **top 3 customers** by total spend across all their orders.

**Requirements:**

- For each customer, calculate:
  - `TotalSpent` – sum of all their order totals.
  - `OrdersCount` – how many orders they have.
- Return the **top 3** customers ordered by `TotalSpent` descending.
- If there are fewer than 3 customers, just return all of them.

**Output example:**

```text
1. Acme Corp – Total: Rxxxxx – Orders: 2
2. Global Dynamics – Total: Rxxxxx – Orders: 2
3. Innotech – Total: Rxxxxx – Orders: 1
```

For this task, **use method syntax**.

---

## Task 3 – Product Performance

For each product, calculate how well it’s selling.

**Requirements:**

For **each product**:

- Compute the **total quantity sold** across all orders.
- Compute the **total revenue** = `Price × total quantity sold`.

Then:

- Return only products where total quantity sold is **greater than 0**.
- Order the results by **total revenue** descending.

**Output example:**

```text
Laptop Pro 15 – Qty: 6 – Revenue: R150000
Laptop Air 13 – Qty: 5 – Revenue: R90000
Wireless Mouse – Qty: 25 – Revenue: R8750
...
```

You may use method syntax or query syntax.

---

## Task 4 – City-Level Summary (Query Syntax Only)

Management wants a simple summary **per city**.

For each **city**:

- Number of **customers** in that city.
- Total **revenue** from those customers (sum of all their orders).

**Requirements:**

- Use **query syntax** (`from`, `join`, `group`, `select`) for this task.
- Start from `customers` and join to the other collections as needed.
- Group by `City`.

**Example output:**

```text
Johannesburg – Customers: 2 – Revenue: Rxxxxxx
Cape Town     – Customers: 1 – Revenue: Rxxxxxx
Durban        – Customers: 1 – Revenue: Rxxxxxx
```

---

## Task 5 – Customer Report DTO

Create a class to hold a structured report per customer:

```csharp
public class CustomerReport
{
    public string CustomerName { get; set; } = "";
    public string City { get; set; } = "";
    public decimal TotalSpent { get; set; }
    public int OrdersCount { get; set; }
    public decimal AverageOrderValue { get; set; }
}
```

**Your task:**

Build a single LINQ query that returns an `IEnumerable<CustomerReport>`.

For each customer:

- `CustomerName` – from `Customer.Name`.
- `City` – from `Customer.City`.
- `TotalSpent` – sum of all their order totals.
- `OrdersCount` – number of orders placed.
- `AverageOrderValue` – `TotalSpent / OrdersCount` (if `OrdersCount` is 0, set this to 0).

Then:

- Order the resulting sequence by `TotalSpent` descending.
- Print one line per customer using `CustomerReport`.

Try to use **method syntax** here, combining `GroupBy`, `Select`, and any aggregations you need.

---

## Task 6 (Stretch) – Filterable “Dashboard”

Add simple filtering on top of your `CustomerReport` query.

1. Ask the user to input a **minimum revenue** (e.g. `20000`).
2. Filter the `CustomerReport` sequence to only include customers with `TotalSpent >= minimum`.
3. (Optional) Ask the user for a **city name**:
   - If the user enters a non-empty city name, filter further to only include that city.
   - If they press Enter without typing a city, ignore the city filter.

Reuse your existing `CustomerReport` query and apply `.Where(...)` on top of it instead of rewriting everything.

---

## What You Should Hand In

For this challenge, you should produce:

1. A `Program.cs` (or equivalent) that:
   - Defines the models and sample data.
   - Contains your LINQ queries for Tasks 1–5 (and Task 6 if you attempt it).
2. Console output demonstrating your queries working.

Focus on:

- Correctness of the results.
- Clear, readable LINQ queries.
- Using both query syntax and method syntax where requested.
