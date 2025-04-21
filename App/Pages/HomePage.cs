using Terminal.Gui;
using App.Routes;
using App.Utils;
using App.Services;
using App.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace App.Pages
{
    public class HomePage : Page
    {
        private TabView _tabView = new();
        private Window _win = new();
        private Label _statusLabel = new();
        private System.Timers.Timer _clearStatusTimer = new(3000);
        private readonly int _userId = GlobalStore.Instance.CurrentUser?.Id ?? -1;
        private View _cartContainer = new();
        private Label _totalLabel = new();
        private View _productContainer = new();
        private View _ordersContainer = new();
        private Button _clearCartButton = new();
        private Button _checkoutButton = new();

        public override Window Display(Router router)
        {
            _win = CreateWindow(router, "ShellMall - Shopping Made Easy");

            if (_userId == -1)
            {
                router.Navigate("/login");
                return _win;
            }

            _statusLabel = new Label("")
            {
                X = 2,
                Y = Pos.AnchorEnd(1),
                Width = Dim.Fill() - 4,
                ColorScheme = Colors.Dialog
            };

            _win.Add(_statusLabel);

            _tabView = new TabView()
            {
                X = 0,
                Y = 3,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 4
            };

            CreateProductsTab();
            CreateCartTab();
            CreateOrdersTab();

            _win.Add(_tabView);
            return _win;
        }

        private void CreateProductsTab()
        {
            var productsTab = new TabView.Tab("Products", new FrameView("Shop Products")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            var searchField = new TextField("")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(40)
            };

            var searchButton = new Button("Search")
            {
                X = Pos.Right(searchField) + 1,
                Y = 0
            };

            _productContainer = new View()
            {
                X = 0,
                Y = 2,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 4
            };

            System.Timers.Timer debounceTimer = new(300);

            void DebounceSearch(Action action, int milliseconds = 300)
            {
                debounceTimer?.Stop();
                debounceTimer?.Dispose();

                debounceTimer = new System.Timers.Timer(milliseconds)
                {
                    AutoReset = false,
                    Enabled = true
                };
                debounceTimer.Elapsed += (_, __) =>
                {
                    Application.MainLoop.Invoke(() => action());
                };
                debounceTimer.Start();
            }

            void PerformSearch()
            {
                var keyword = searchField.Text.ToString()?.Trim() ?? "";
                var results = ShopService.SearchProducts(keyword);
                RefreshProductList(results);
                SetStatus($"Found {results.Count} product(s).");
            }

            searchField.TextChanged += _ => DebounceSearch(PerformSearch);
            searchButton.Clicked += PerformSearch;

            RefreshProductList(ShopService.GetAllProducts());

            var frame = (FrameView)productsTab.View;
            frame.Add(searchField, searchButton, _productContainer);
            _tabView.AddTab(productsTab, true);
        }

        private void RefreshProductList(List<Product> products)
        {
            _productContainer.RemoveAll();
            int y = 0;

            foreach (var product in products)
            {
                var cartItems = ShopService.GetCartItems(_userId);
                var isInCart = cartItems.Any(c => c.ProductId == product.Id);

                var info = new Label($"{product.Id}: {product.Name} - ${product.Price:F2} - Stock: {product.Stock} - Category: {product.Category?.Name ?? "None"}")
                {
                    X = 0,
                    Y = y,
                    Width = Dim.Percent(70),
                };

                var actionButton = new Button(isInCart ? "Update Qty" : "Add")
                {
                    X = Pos.Right(info) + 1,
                    Y = y
                };

                var currentProduct = product;
                var currentIsInCart = isInCart;

                actionButton.Clicked += () =>
                {
                    PromptForQuantity(currentProduct, currentIsInCart);
                };
                if (product.Stock == 0)
                {
                    actionButton.Text = "Out of Stock";
                    actionButton.Enabled = false;
                }
                _productContainer.Add(info, actionButton);
                y += 2;
            }

            Application.Refresh();
        }

        private void CreateCartTab()
        {
            var cartTab = new TabView.Tab("Cart", new FrameView("Shopping Cart")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            _cartContainer = new View()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(80)
            };

            _totalLabel = new Label("Total: $0.00")
            {
                X = 0,
                Y = Pos.Bottom(_cartContainer) - 1,
                Width = Dim.Fill()
            };

            _clearCartButton = new Button("Clear All")
            {
                X = 0,
                Y = Pos.Bottom(_totalLabel) + 1
            };

            _checkoutButton = new Button("Checkout")
            {
                X = Pos.Right(_clearCartButton) + 2,
                Y = Pos.Bottom(_totalLabel) + 1
            };

            _clearCartButton.Clicked += () =>
            {
                if (MessageBox.Query("Clear Cart", "Are you sure you want to clear the entire cart?", "Yes", "No") == 0)
                {
                    try
                    {
                        ShopService.ClearCart(_userId);
                        RefreshCart();
                        RefreshProductList(ShopService.GetAllProducts());
                        SetStatus("Cart cleared");
                    }
                    catch (Exception ex)
                    {
                        SetStatus($"Error clearing cart: {ex.Message}");
                    }
                }
            };

            _checkoutButton.Clicked += () =>
            {
                if (ShopService.GetCartItems(_userId).Count == 0)
                {
                    SetStatus("Your cart is empty. Add some items before proceeding.");
                    return;
                }

                if (MessageBox.Query("Checkout", "Are you sure you want to proceed to checkout?", "Yes", "No") == 0)
                {
                    try
                    {
                        ShopService.PlaceOrder(_userId);
                        SetStatus("Checkout successful!");
                        RefreshCart();
                        RefreshProductList(ShopService.GetAllProducts());
                        RefreshOrdersList(_ordersContainer); // Refresh Orders tab
                    }
                    catch (Exception ex)
                    {
                        SetStatus($"Error during checkout: {ex.Message}");
                    }
                }
            };

            RefreshCart();
            var frame = (FrameView)cartTab.View;
            frame.Add(_cartContainer, _totalLabel, _clearCartButton, _checkoutButton);
            _tabView.AddTab(cartTab, false);
        }

        private void RefreshCart()
        {
            _cartContainer.RemoveAll();
            var cartItems = ShopService.GetCartItems(_userId);
            int y = 0;

            if (cartItems.Count == 0)
            {
                var emptyLabel = new Label("Your cart is empty. Add some products from the Products tab.")
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill()
                };
                _cartContainer.Add(emptyLabel);
            }
            else
            {
                foreach (var item in cartItems)
                {
                    var product = item.Product;
                    if (product == null) continue;

                    var itemLabel = new Label($"{product.Name} - ${product.Price:F2} each - Subtotal: ${product.Price * item.Quantity:F2}")
                    {
                        X = 0,
                        Y = y,
                        Width = Dim.Percent(60)
                    };

                    var qtyLabel = new Label($"Qty: {item.Quantity}")
                    {
                        X = Pos.Right(itemLabel) + 1,
                        Y = y,
                        Width = 10
                    };

                    var decButton = new Button(" - ")
                    {
                        X = Pos.Right(qtyLabel) + 1,
                        Y = y,
                        Width = 3
                    };

                    var incButton = new Button(" + ")
                    {
                        X = Pos.Right(decButton) + 1,
                        Y = y,
                        Width = 3
                    };

                    var delButton = new Button(" X ")
                    {
                        X = Pos.Right(incButton) + 1,
                        Y = y,
                        Width = 3
                    };

                    var productId = product.Id;
                    var currentProduct = product;

                    decButton.Clicked += () =>
                    {
                        try
                        {
                            if (item.Quantity > 1)
                            {
                                ShopService.UpdateQuantity(_userId, productId, item.Quantity - 1);
                                RefreshCart();
                                RefreshProductList(ShopService.GetAllProducts());
                                SetStatus($"Decreased quantity of {currentProduct.Name}");
                            }
                            else
                            {
                                if (MessageBox.Query("Remove Item", $"Remove {currentProduct.Name} from cart?", "Yes", "No") == 0)
                                {
                                    ShopService.RemoveFromCart(_userId, productId);
                                    RefreshCart();
                                    RefreshProductList(ShopService.GetAllProducts());
                                    SetStatus($"Removed {currentProduct.Name} from cart");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SetStatus($"Error: {ex.Message}");
                        }
                    };

                    incButton.Clicked += () =>
                    {
                        try
                        {
                            if (item.Quantity < currentProduct.Stock)
                            {
                                ShopService.UpdateQuantity(_userId, productId, item.Quantity + 1);
                                RefreshCart();
                                RefreshProductList(ShopService.GetAllProducts());
                                SetStatus($"Increased quantity of {currentProduct.Name}");
                            }
                            else
                            {
                                SetStatus($"Cannot add more {currentProduct.Name}. Stock limit reached.");
                            }
                        }
                        catch (Exception ex)
                        {
                            SetStatus($"Error: {ex.Message}");
                        }
                    };

                    delButton.Clicked += () =>
                    {
                        try
                        {
                            if (MessageBox.Query("Remove Item", $"Remove {currentProduct.Name} from cart?", "Yes", "No") == 0)
                            {
                                ShopService.RemoveFromCart(_userId, productId);
                                RefreshCart();
                                RefreshProductList(ShopService.GetAllProducts());
                                SetStatus($"Removed {currentProduct.Name} from cart");
                            }
                        }
                        catch (Exception ex)
                        {
                            SetStatus($"Error: {ex.Message}");
                        }
                    };

                    _cartContainer.Add(itemLabel, qtyLabel, decButton, incButton, delButton);
                    y += 2;
                }
            }

            decimal total = ShopService.GetCartItems(_userId)
                .Where(i => i.Product != null)
                .Sum(i => i.Product != null ? i.Quantity * i.Product.Price : 0);
            _totalLabel.Text = $"Total: ${total:F2}";
            Application.Refresh();
        }

        private void CreateOrdersTab()
        {
            var ordersTab = new TabView.Tab("Orders", new FrameView("Your Orders")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            _ordersContainer = new View()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            RefreshOrdersList(_ordersContainer);

            var frame = (FrameView)ordersTab.View;
            frame.Add(_ordersContainer);
            _tabView.AddTab(ordersTab, false);
        }

        private void RefreshOrdersList(View ordersContainer)
        {
            ordersContainer.RemoveAll();
            var orders = ShopService.GetUserOrders(_userId);
            int y = 0;

            if (orders.Count == 0)
            {
                var emptyLabel = new Label("You have no orders. Place an order from the Cart tab.")
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill()
                };
                ordersContainer.Add(emptyLabel);
            }
            else
            {
                foreach (var order in orders)
                {
                    decimal total = order.Items.Sum(i => i.Quantity * i.UnitPrice);
                    var orderLabel = new Label($"Order #{order.Id} - {order.CreatedAt:yyyy-MM-dd HH:mm} - Total: ${total:F2}")
                    {
                        X = 0,
                        Y = y,
                        Width = Dim.Percent(80)
                    };

                    var detailsButton = new Button("Details")
                    {
                        X = Pos.Right(orderLabel) + 1,
                        Y = y
                    };

                    detailsButton.Clicked += () => ShowOrderDetails(order);

                    ordersContainer.Add(orderLabel, detailsButton);
                    y += 2;
                }
            }

            Application.Refresh();
        }

        private void ShowOrderDetails(Order order)
        {
            var dialog = new Dialog($"Order #{order.Id} Details", 60, 15)
            {
                ColorScheme = Colors.Dialog,
                Modal = true
            };

            int y = 0;
            var dateLabel = new Label($"Date: {order.CreatedAt:yyyy-MM-dd HH:mm}")
            {
                X = 0,
                Y = y,
                Width = Dim.Fill()
            };
            y += 2;

            var itemsLabel = new Label("Items:")
            {
                X = 0,
                Y = y,
                Width = Dim.Fill()
            };
            y += 1;

            foreach (var item in order.Items)
            {
                var product = item.Product;
                var itemText = product != null
                    ? $"{product.Name} - Qty: {item.Quantity} - ${item.UnitPrice:F2} each - Subtotal: ${item.Quantity * item.UnitPrice:F2}"
                    : $"Product ID {item.ProductId} - Qty: {item.Quantity} - ${item.UnitPrice:F2} each";
                var itemLabel = new Label(itemText)
                {
                    X = 0,
                    Y = y,
                    Width = Dim.Fill()
                };
                y += 1;
                dialog.Add(itemLabel);
            }

            decimal total = order.Items.Sum(i => i.Quantity * i.UnitPrice);
            var totalLabel = new Label($"Total: ${total:F2}")
            {
                X = 0,
                Y = y + 1,
                Width = Dim.Fill()
            };

            var closeButton = new Button("Close");
            closeButton.Clicked += () => Application.RequestStop();

            dialog.Add(dateLabel, itemsLabel, totalLabel);
            dialog.AddButton(closeButton);

            Application.Run(dialog);
        }

        private void SetStatus(string message)
        {
            _statusLabel.Text = message;
            _clearStatusTimer.Start();
        }

        private void PromptForQuantity(Product product, bool isInCart)
        {
            var quantity = isInCart ? ShopService.GetCartItems(_userId).FirstOrDefault(c => c.ProductId == product.Id)?.Quantity ?? 0 : 0;
            var quantityDialog = new Dialog("Quantity", 50, 7)
            {
                ColorScheme = Colors.Dialog,
                Modal = true
            };

            var qtyInput = new TextField(quantity.ToString())
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 2,
            };
            var updateButton = new Button(isInCart ? "Update" : "Add");
            var cancelButton = new Button("Cancel");
            quantityDialog.AddButton(updateButton);
            quantityDialog.AddButton(cancelButton);
            quantityDialog.Add(qtyInput);

            cancelButton.Clicked += () => Application.RequestStop();
            updateButton.Clicked += () =>
            {
                if (!int.TryParse(qtyInput.Text.ToString(), out int qty) || qty < 0)
                {
                    MessageBox.ErrorQuery("Invalid Quantity", "Please enter a valid number.", "OK");
                    return;
                }

                try
                {
                    if (qty > product.Stock)
                    {
                        MessageBox.ErrorQuery("Invalid Quantity", $"Quantity cannot exceed stock ({product.Stock}).", "OK");
                        return;
                    }

                    if (isInCart)
                    {
                        if (qty == 0)
                        {
                            if (MessageBox.Query("Remove Item", $"Remove {product.Name} from cart?", "Yes", "No") == 0)
                            {
                                ShopService.RemoveFromCart(_userId, product.Id);
                                SetStatus($"Removed {product.Name} from cart");
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            ShopService.UpdateQuantity(_userId, product.Id, qty);
                            SetStatus($"Updated {product.Name} quantity to {qty}");
                        }
                    }
                    else
                    {
                        if (qty < 1)
                        {
                            MessageBox.ErrorQuery("Invalid Quantity", "Please enter a quantity of 1 or more.", "OK");
                            return;
                        }

                        ShopService.AddToCart(_userId, product.Id, qty);
                        SetStatus($"Added {qty} {product.Name}(s) to the cart");
                    }

                    RefreshCart();
                    RefreshProductList(ShopService.GetAllProducts());
                    Application.RequestStop();
                }
                catch (Exception ex)
                {
                    SetStatus($"Error: {ex.Message}");
                }
            };

            Application.Run(quantityDialog);
        }
    }
}
