using System;
using App.Routes;
using App.Utils;
using Terminal.Gui;

namespace App.Pages
{
    public abstract class Page
    {
        public abstract Window Display(Router router);

        protected Window CreateWindow(Router router, string title)
        {
            var win = new Window(title)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var menu = GlobalMenuBar.GetMenu(router);
            Application.Top.Add(menu);

            return win;
        }

        protected static bool ValidateInput(string input, TextField inputField, string fieldName)
        {
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.ErrorQuery("Error", $"{fieldName} cannot be empty.", "OK");
                inputField.SetFocus();
                return false;
            }
            return true;
        }
    }
}
