using Terminal.Gui;
using App.Routes;
using App.Utils;
using Figgle;

namespace App.Pages;

public class LandingPage : Page
{
    public override Window Display(Router router)
    {
        var win = CreateWindow(router, "Login");
        var asciiArt = FiggleFonts.Standard.Render("Welcome to ShellMall!");
        win.Add(UIHelper.CreateAsciiArtLabel(asciiArt, 1));

        win.Add(UIHelper.CreateButton("Login", 12, () => router.Navigate("/login")));
        win.Add(UIHelper.CreateButton("Signup", 14, () => router.Navigate("/signup")));
        win.Add(UIHelper.CreateButton("Exit", 16, () => Application.RequestStop()));

        return win;
    }
}
