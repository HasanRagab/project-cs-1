using System;
using System.Collections.Generic;
using App.Pages;
using Terminal.Gui;

namespace App.Routes;
public class Router
{
    private readonly Dictionary<string, Route> _routes = new();
    private readonly Toplevel top = Application.Top;
    private string _currentPath = "";
    private readonly Stack<string> _previousPaths = new();

    public void Register(string path, Func<Page> factory)
    {
        if (_routes.ContainsKey(path))
        {
            var msg = new Label($"Route {path} already registered.")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            top.Add(msg);
            Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(2), _ =>
            {
                top.Remove(msg);
                return false;
            });
            return;
        }

        _routes[path] = new Route
        {
            PageFactory = factory,
        };
    }

    public void Navigate(string path)
    {
        if (!_routes.TryGetValue(path, out var route))
        {
            var msg = new Label($"Route {path} not found.")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(msg);
            Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(2), _ =>
            {
                top.Remove(msg);
                return false;
            });

            return;
        }

        top.RemoveAll();

        _currentPath = path;
        if (_previousPaths.Count == 0 || _previousPaths.Peek() != path)
        {
            _previousPaths.Push(path);
        }

        var page = route.PageFactory();
        var win = page.Display(this);
        top.Add(win);
    }

    public void Start(string startPath)
    {
        if (_routes.Count == 0)
        {
            Console.WriteLine("No routes registered. Please register pages before starting.");
            return;
        }

        Navigate(startPath);
    }

    public void RerouteCurrent()
    {
        Navigate(_currentPath);
    }

    public void GoBack()
    {
        if (_previousPaths.Count > 1)
        {
            _previousPaths.Pop();
            var previousPath = _previousPaths.Pop();
            Navigate(previousPath);
        }
        else
        {
            Console.WriteLine("No previous page to go back to.");
        }
    }

    public void Exit()
    {
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }
}
