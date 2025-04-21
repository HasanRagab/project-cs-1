using Terminal.Gui;

namespace App.Utils;

public static class UIHelper
{
    public static Label CreateLabel(string text, int x, int y, bool center = false)
    {
        return new Label(text)
        {
            X = center ? Pos.Center() : x,
            Y = y
        };
    }

    public static Button CreateButton(string text, int y, Action onClick, bool center = true)
    {
        var button = new Button(text)
        {
            X = center ? Pos.Center() : 0,
            Y = y
        };
        button.Clicked += onClick;
        return button;
    }

    public static TextField CreateInput(int x, int y, int width, string initialText = "")
    {
        return new TextField(initialText)
        {
            X = x,
            Y = y,
            Width = width
        };
    }

    public static Label CreateAsciiArtLabel(string asciiText, int y)
    {
        return new Label(asciiText)
        {
            X = Pos.Center(),
            Y = y
        };
    }


    public static (Label, TextField) CreateLabeledInput(string labelText, int y, int labelX = 2, int inputX = 20, int inputWidth = 40)
    {
        var label = new Label(labelText)
        {
            X = labelX,
            Y = y
        };

        var input = new TextField("")
        {
            X = inputX,
            Y = y,
            Width = inputWidth
        };

        return (label, input);
    }
}
