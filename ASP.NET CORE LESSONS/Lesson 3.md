

---

---

## Recap & Activation

> [!QUESTION]
> “Right now, if an event’s start time is wrong… what can the user do?”

Answer:  
Nothing, they’d have to restart the app and re-enter everything.

> [!BRIDGE]
> Today we complete CRUD:
> **Create → Read → Update → Delete**
>
> After this lesson, your app becomes *editable*, and the cost of our current design becomes visible.

---

## 1 Edit (GET): Show Existing Data

Exactly like Courses, editing starts by **showing the current state**.

### In `EventsController`

```csharp
// GET: /Events/Edit/1
public IActionResult Edit(int id)
{
    var ev = InMemoryStore.Events.FirstOrDefault(e => e.Id == id);
    if (ev == null) return NotFound();
    return View(ev);
}
```

Teaching points:
- Same pattern as Details
- GET never mutates state
- If it doesn’t exist → NotFound()

### 2 Edit View (Form Pre-Filled)

Create Views/Events/Edit.cshtml:

```cshtml
@model SportEvent

<h2>Edit Event</h2>

<form asp-action="Edit" method="post">
    <input type="hidden" asp-for="Id" />

    <div>
        <label asp-for="Title"></label>
        <input asp-for="Title" />
        <span asp-validation-for="Title"></span>
    </div>

    <div>
        <label asp-for="Sport"></label>
        <input asp-for="Sport" />
        <span asp-validation-for="Sport"></span>
    </div>

    <div>
        <label asp-for="StartAt"></label>
        <input asp-for="StartAt" type="datetime-local" />
    </div>

    <button type="submit">Save</button>
    <a asp-action="Index">Cancel</a>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

Teaching moment:

“Notice: we didn’t write any code to ‘fill in’ the form.
Razor + model binding does it for us.”

###  3  Edit (POST): Mutating State Safely

Back to EventsController:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(SportEvent input)
{
    if (!ModelState.IsValid)
    {
        return View(input);
    }

    var existing = InMemoryStore.Events.FirstOrDefault(e => e.Id == input.Id);
    if (existing == null) return NotFound();

    existing.Title = input.Title;
    existing.Sport = input.Sport;
    existing.StartAt = input.StartAt;

    return RedirectToAction(nameof(Index));
}
```

Teaching points:

- POST mutates state
- We find the original object and update it
- We never trust the client blindly
- Post/Redirect/Get again

Pause here and ask:

“Why didn’t we just replace the object in the list?”

Let students answer, this leads naturally to persistence & tracking later.

### 4, Delete (GET): Confirmation Page

Deletion always gets a confirmation page.

```csharp
// GET: /Events/Delete/1
public IActionResult Delete(int id)
{
    var ev = InMemoryStore.Events.FirstOrDefault(e => e.Id == id);
    if (ev == null) return NotFound();
    return View(ev);
}
```

### 5 Delete View (Confirmation)

Create Views/Events/Delete.cshtml:

```cshtml
@model SportEvent

<h2>Delete Event</h2>

<p>
    Are you sure you want to delete
    <strong>@Model.Title</strong>?
</p>

<form asp-action="DeleteConfirmed" method="post">
    <input type="hidden" asp-for="Id" />
    <button type="submit">Yes, Delete</button>
    <a asp-action="Index">Cancel</a>
</form>
```

Teaching moment:

“GET asks. POST commits.”

# 6 Delete (POST): Actual Removal

Back to controller:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(int id)
{
    var ev = InMemoryStore.Events.FirstOrDefault(e => e.Id == id);
    if (ev == null) return NotFound();

    InMemoryStore.Events.Remove(ev);

    return RedirectToAction(nameof(Index));
}
```

Teaching points:

- Separate action name avoids accidental deletes
- Deletion mutates shared state
- This is intentionally naïve

### 7  Wire Actions into Index View

Update Views/Events/Index.cshtml:

```cshtml
@foreach (var e in Model)
{
    <li>
        <a asp-action="Details" asp-route-id="@e.Id">@e.Title</a>
        |
        <a asp-action="Edit" asp-route-id="@e.Id">Edit</a>
        |
        <a asp-action="Delete" asp-route-id="@e.Id">Delete</a>
    </li>
}
```

Run the app:

1. Create event
2. Edit it
3. Delete it
4. Refresh

Everything works.

### 8. The Intentional Discomfort

Stop coding. Ask:

>[!QUESTION]
“What if two controllers start editing events?”
“What if tests want to supply fake data?”
“What if we move to a database?”

>[!Answer]
>“Our controllers are now tightly coupled to data storage.
We fixed functionality… but we broke architecture.”
