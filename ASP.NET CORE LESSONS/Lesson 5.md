*“If you can’t test it, you don’t really understand it.”*

---

## Learning Objectives
By the end of this lesson, learners will be able to:

- Explain **what unit testing is** (and what it is not)
- Understand **why repositories + DI enable testing**
- Create a test project using **xUnit**
- Use **Moq** to fake repositories
- Unit test **existing controllers** (Index, Details, Create, Edit, Delete)
- Recognise the difference between **unit tests and integration tests**

---

## Recap & Activation

> [!QUESTION]
> “What did we gain in the last lesson by introducing repositories and DI?”

Expected answers:
- Controllers no longer depend on storage
- Data access is abstracted
- Swappable implementations

> [!BRIDGE]
> Today we answer the question:
> **“So what?”**
>
> Repositories + DI exist *so that we can test our logic in isolation*.

---

## PART 1 What Is Unit Testing?

> [!NOTE]
> A **unit test** verifies one small unit of logic *in isolation*.

Unit tests **do not**:
- Start the web server
- Talk to a real database
- Test Razor views
- Test ASP.NET Core internals

Unit tests **do**:
- Test decision-making
- Test branching (happy/sad paths)
- Test how code behaves when dependencies succeed or fail

---

## PART 2 Create the Test Project (10 minutes)

From the **solution root**:

```bash
dotnet new xunit -n Playbook.Tests
dotnet sln add Playbook.Tests
dotnet add Playbook.Tests reference Playbook.Web
dotnet add Playbook.Tests package Moq
```

Run once: `dotnet test`

Confirm:
- Tests run
- No failures

Teaching note:

“This is our safety net. We’ll grow it as the app grows.”

### PART 3 What Are We Testing?
We will test existing code only:

- EventsController
- Logic paths inside its actions
- Behaviour when repositories return data vs null

We will mock:
- IEventRepository

We will not test:
- InMemoryEventRepository
- Razor views
- HTTP routing

### PART 4  First Controller Test: Index()

Existing Controller Code
```csharp
public IActionResult Index()
{
    return View(_repo.GetAll());
}
```

Test: Index returns View with events
Create Playbook.Tests/EventsControllerTests.cs:

```csharp
using Microsoft.AspNetCore.Mvc;
using Moq;
using Playbook.Web.Controllers;
using Playbook.Web.Models;
using Playbook.Web.Repositories;
using Xunit;

public class EventsControllerTests
{
    [Fact]
    public void Index_ReturnsView_WithEvents()
    {
        // Arrange
        var fakeEvents = new List<SportEvent>
        {
            new() { Id = 1, Title = "Match A" },
            new() { Id = 2, Title = "Match B" }
        };

        var mockRepo = new Mock<IEventRepository>();
        mockRepo.Setup(r => r.GetAll()).Returns(fakeEvents);

        var controller = new EventsController(mockRepo.Object);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<SportEvent>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }
}
```

Teaching points:
- No server
- No DB
- Just method call → result
- Arrange → Act → Assert

### PART 5  Testing Details(id): Sad Path
Existing Code

```csharp
public IActionResult Details(int id)
{
    var ev = _repo.GetById(id);
    if (ev == null) return NotFound();
    return View(ev);
}
```

Test: Returns NotFound when missing

```csharp
[Fact]
public void Details_ReturnsNotFound_WhenEventMissing()
{
    // Arrange
    var mockRepo = new Mock<IEventRepository>();
    mockRepo.Setup(r => r.GetById(99)).Returns((SportEvent?)null);

    var controller = new EventsController(mockRepo.Object);

    // Act
    var result = controller.Details(99);

    // Assert
    Assert.IsType<NotFoundResult>(result);
}
```

Teaching moment:

“We just proved our controller handles missing data gracefully.”

### PART 6  Testing Details(id): Happy Path

```csharp
[Fact]
public void Details_ReturnsView_WhenEventExists()
{
    // Arrange
    var ev = new SportEvent { Id = 1, Title = "Derby Match" };

    var mockRepo = new Mock<IEventRepository>();
    mockRepo.Setup(r => r.GetById(1)).Returns(ev);

    var controller = new EventsController(mockRepo.Object);

    // Act
    var result = controller.Details(1);

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsType<SportEvent>(viewResult.Model);
    Assert.Equal("Derby Match", model.Title);
}
```

### PART 7 Testing Create (POST)
Existing Create Action

```csharp
[HttpPost]
public IActionResult Create(SportEvent input)
{
    if (!ModelState.IsValid)
        return View(input);

    _repo.Add(input);
    return RedirectToAction(nameof(Index));
}
```

Test: Invalid `ModelState` returns `View`

```csharp
[Fact]
public void Create_ReturnsView_WhenModelInvalid()
{
    // Arrange
    var mockRepo = new Mock<IEventRepository>();
    var controller = new EventsController(mockRepo.Object);

    controller.ModelState.AddModelError("Title", "Required");

    var input = new SportEvent { Sport = "Football" };

    // Act
    var result = controller.Create(input);

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsType<SportEvent>(viewResult.Model);
    Assert.Equal(input, model);

    mockRepo.Verify(r => r.Add(It.IsAny<SportEvent>()), Times.Never);
}
```

Teaching point:

“We didn’t just check the result, we verified that data was not saved.”

Test: Valid Model redirects and calls `Add()`

```csharp
[Fact]
public void Create_RedirectsAndAdds_WhenModelValid()
{
    // Arrange
    var mockRepo = new Mock<IEventRepository>();
    var controller = new EventsController(mockRepo.Object);

    var input = new SportEvent
    {
        Title = "Final Match",
        Sport = "Football",
        StartAt = DateTime.UtcNow.AddHours(5)
    };

    // Act
    var result = controller.Create(input);

    // Assert
    var redirect = Assert.IsType<RedirectToActionResult>(result);
    Assert.Equal("Index", redirect.ActionName);

    mockRepo.Verify(r => r.Add(It.Is<SportEvent>(e => e.Title == "Final Match")), Times.Once);
}
```

### PART 8  Why This Works
Ask explicitly:

[!QUESTION]
“Why was this easy to test?”

Answer:
- Controller depends on IEventRepository
- Repository is mocked
- No static state
- No hidden dependencies

Tie back to Lesson 4:
- “This is why repositories + DI exist.”

### PART 9 What We Are NOT Testing (On Purpose)

We are not testing:

- InMemoryEventRepository
- Razor markup
- Route matching
- Model binding itself

Explain:
- “Those belong to integration tests, not unit tests.”

### Reflection Prompts
- What would testing look like without repositories?
- How does mocking give us control over scenarios?
- What bugs would unit tests catch early?
- Why do we test behaviour, not implementation?




---
