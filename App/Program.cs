using Terminal.Gui;
using App.Routes;
using App.Pages;
using App.Data;

AppDbContextFactory.SeedDatabase();

Application.Init();

var router = new Router();

router.Register("/", () => new LandingPage());
router.Register("/home", () => new HomePage());
router.Register("/warehouse", () => new WarehousePage());
router.Register("/login", () => new LoginPage());
router.Register("/signup", () => new SignupPage());

router.Start("/");
Application.Run();

