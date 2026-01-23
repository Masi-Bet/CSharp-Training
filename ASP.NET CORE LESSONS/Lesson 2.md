Update `Models/SportEvent.cs`:

```csharp
using System.ComponentModel.DataAnnotations;  

public class SportEvent 
{
     public int Id { get; set; }
     
    [Required]
	public string Title { get; set; } = String.Empty;
	
	[Required]     
	public string Sport { get; set; } = String.Empty; 
	     
	[Display(Name = "Start Time")]     
	public DateTime StartAt { get; set; }
	   
	public List<Market> Markets { get; set; } = new(); 
}
```

In `EventsController`:

```csharp
// GET: /Events/Create 
public IActionResult Create() 
{
     return View(); 
}
```

Create `Views/Events/Create.cshtml`:

```cshtml
@model SportEvent  

<h2>Create Event</h2>  
<form asp-action="Create" method="post">     
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
	
	<button type="submit">Create</button> 
</form>  

@section Scripts
{    
	<partial name="_ValidationScriptsPartial" /> 
}
```

Back in `EventsController`:

```csharp
[HttpPost] [ValidateAntiForgeryToken] 
public IActionResult Create(SportEvent input) 
{
    if (!ModelState.IsValid)
    {
        return View(input);     
    }      
    
	input.Id = InMemoryStore.Events.Max(e => e.Id) + 1;
	
	InMemoryStore.Events.Add(input);      
	return RedirectToAction(nameof(Index)); }
```


Ensure that you have `Data/InMemoryStore.cs`
```csharp
using Playbook.Models;

namespace Playbook.Data;

public static class InMemoryStore
{
    public static List<SportEvent> Events { get; } = new()
    {
        new SportEvent
        {
            Id = 1,
            Title = "City FC vs Town United",
            Sport = "Football",
            StartAt = DateTime.UtcNow.AddHours(6),
            Markets = new List<Market>
            {
                new Market
                {
                    Id = 1,
                    Name = "Match Winner",
                    Outcomes = new List<Outcome>
                    {
                        new Outcome { Id = 1, Name = "City FC", DecimalOdds = 1.85m },
                        new Outcome { Id = 2, Name = "Draw", DecimalOdds = 3.60m },
                        new Outcome { Id = 3, Name = "Town United", DecimalOdds = 4.20m },
                    }
                },
                new Market
                {
                    Id = 1,
                    Name = "Half Time",
                    Outcomes = new List<Outcome>
                    {
                        new Outcome { Id = 1, Name = "Home", DecimalOdds = 0.85m },
                        new Outcome { Id = 2, Name = "Draw", DecimalOdds = 3.60m },
                        new Outcome { Id = 3, Name = "Away", DecimalOdds = 4.20m },
                    }
                },
                new Market
                {
                    Id = 1,
                    Name = "Both teams to score",
                    Outcomes = new List<Outcome>
                    {
                        new Outcome { Id = 1, Name = "Yes", DecimalOdds = 1.2m },
                        new Outcome { Id = 2, Name = "No", DecimalOdds = 2.8m }
                    }
                },
            }
        }

    };
} 
```

### Docs:
- [Model Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0)
- [Model Binding](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-10.0)
- [Data Annotations](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-6)
- [Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-10.0#what-are-tag-helpers)

