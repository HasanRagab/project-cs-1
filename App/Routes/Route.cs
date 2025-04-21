using App.Pages;

namespace App.Routes;

public class Route
{
    public Func<Page> PageFactory { get; set; } = null!;
}