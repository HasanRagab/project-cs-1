using App.Routes;
using App.Services;
using Terminal.Gui;
namespace App.Utils;

public class GlobalMenuBar
{
    public static MenuBar GetMenu(Router router)
    {

        var menuItems = new List<MenuBarItem>
            {
                new MenuBarItem("Back", "", () =>
                {
                    router.GoBack();
                })
            };

        if (GlobalStore.Instance?.CurrentUser?.IsAdmin == true)
        {
            menuItems.Add(new MenuBarItem("Warehouse", "", () =>
            {
                router.Navigate("/warehouse");
            }));
        }

        if (GlobalStore.Instance?.CurrentUser != null)
        {
            menuItems.Add(new MenuBarItem("Shop", "", () =>
            {
                router.Navigate("/home");
            }));
        }

        return new MenuBar([.. menuItems]);
    }
}