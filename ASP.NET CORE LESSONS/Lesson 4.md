# Lesson 4: Repositories & Dependency Injection
*“We fixed functionality. Now we fix architecture.”*

---

Recap & Activation

> [!QUESTION]
> “In Lesson 3, where did we edit and delete events from?”

Answer:
Directly from `InMemoryStore` inside the controller.

Follow-ups:
- “What if two controllers touch that list?”
- “What if we want to test the controller?”
- “What if the data comes from a database next week?”

> [!BRIDGE]
> Today we introduce **repositories** and **dependency injection**, not because they’re fashionable, but because our current design *demands* them.

---

## 1 Identifying the Smell (10 minutes)

Show the current controller snippet:

```csharp
var ev = InMemoryStore.Events.FirstOrDefault(e => e.Id == id);
```

Explain the issues explicitly:
- Controller knows where data lives
- Tight coupling to storage
- Hard to test
- Hard to change

Why that’s a problem:

- **Tight coupling**: If you later decide to move from an in-memory store to a database (SQL, NoSQL, etc.), you’ll have to rewrite the controller logic. The controller is tied to the storage implementation instead of just asking for “an event by id.”
- **Hard to test**: To unit test the controller, you now need to set up `InMemoryStore.Events` with test data. You can’t easily swap in a fake or mock repository because the controller is hard-coded to use the real store.
- **Breaks separation of concerns**: The controller’s job should be handling HTTP requests and responses, not knowing how data is persisted.
- **Hard to change**: Any change in the storage layer (e.g., renaming `Events`, changing how IDs are handled) forces changes in the controller, even though the controller shouldn’t care about those details.

>[!NOTE]
When changing one thing forces you to touch many files, you have a design smell.

## 2.  What Is a Repository?

Definition:

A repository is an abstraction that represents a collection of domain objects and hides how they are stored or retrieved.

Repositories answer:
What data we want

Not:
How it’s stored

Analogy:
Controller = waiter
Repository = kitchen pass
Storage = kitchen

The waiter never walks into the kitchen.

##  3. Create the Repository Interface (15 minutes)
Create folder: Repositories/

Repositories/IEventRepository.cs
using Playbook.Web.Models;

```csharp
public interface IEventRepository
{
    IEnumerable<SportEvent> GetAll();
    SportEvent? GetById(int id);
    void Add(SportEvent ev);
    void Update(SportEvent ev);
    void Remove(int id);
}
```

Teaching points:
- Interface defines capabilities
- No implementation details
- Mirrors CRUD operations already implemented

## 4. Implement the In-Memory Repository (15 minutes)
Repositories/InMemoryEventRepository.cs

```csharp
using Playbook.Web.Data;
using Playbook.Web.Models;

public class InMemoryEventRepository : IEventRepository
{
    public IEnumerable<SportEvent> GetAll()
    {
        return InMemoryStore.Events;
    }

    public SportEvent? GetById(int id)
    {
        return InMemoryStore.Events.FirstOrDefault(e => e.Id == id);
    }

    public void Add(SportEvent ev)
    {
        ev.Id = InMemoryStore.Events.Max(e => e.Id) + 1;
        InMemoryStore.Events.Add(ev);
    }

    public void Update(SportEvent ev)
    {
        var existing = GetById(ev.Id);
        if (existing == null) return;

        existing.Title = ev.Title;
        existing.Sport = ev.Sport;
        existing.StartAt = ev.StartAt;
    }

    public void Remove(int id)
    {
        var ev = GetById(id);
        if (ev != null)
            InMemoryStore.Events.Remove(ev);
    }
}
```


We moved all mutation logic out of the controller.

## 5. Register Repository with DI (10 minutes)

Open Program.cs and add:

```csharp
builder.Services.AddScoped<IEventRepository, InMemoryEventRepository>();
```

Explain:
AddScoped → one instance per HTTP request
ASP.NET Core now knows how to build this dependency

## 6. Refactor Controller (20 minutes)

Before
InMemoryStore.Events.Add(input);

After: EventsController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using Playbook.Web.Models;
using Playbook.Web.Repositories;

public class EventsController : Controller
{
    private readonly IEventRepository _repo;

    public EventsController(IEventRepository repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        return View(_repo.GetAll());
    }

    public IActionResult Details(int id)
    {
        var ev = _repo.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SportEvent input)
    {
        if (!ModelState.IsValid)
            return View(input);

        _repo.Add(input);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var ev = _repo.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(SportEvent input)
    {
        if (!ModelState.IsValid)
            return View(input);

        _repo.Update(input);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var ev = _repo.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _repo.Remove(id);
        return RedirectToAction(nameof(Index));
    }
}
```
