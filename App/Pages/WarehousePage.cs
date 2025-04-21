using Terminal.Gui;
using App.Routes;
using App.Utils;
using App.Services;
using App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace App.Pages
{
    public class WarehousePage : Page
    {
        private TabView _tabView = new();
        private Window _win = new();
        private Label _statusLabel = new();

        public override Window Display(Router router)
        {
            _win = CreateWindow(router, "Warehouse Management");

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
            CreateCategoriesTab();
            CreateRestockTab();
            CreateUserManagementTab();
            CreateReportsTab();

            _win.Add(_tabView);

            
            var homeButton = new Button("Back to Home")
            {
                X = Pos.Center(),
                Y = Pos.AnchorEnd(1)
            };
            homeButton.Clicked += () => router.Navigate("/home");
            _win.Add(homeButton);

            return _win;
        }

        private void CreateProductsTab()
        {
            var productTab = new TabView.Tab("Products", new FrameView("Product Management")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            var productList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(80)
            };

            void RefreshProductList()
            {
                var products = ShopService.GetAllProducts();
                productList.SetSource(products.Select(p => $"{p.Id}: {p.Name} - ${p.Price} (Stock: {p.Stock})").ToList());
            }

            RefreshProductList();

            var addButton = new Button("Add New Product")
            {
                X = 0,
                Y = Pos.Bottom(productList) + 1
            };
            addButton.Clicked += () =>
            {
                var addDialog = new Dialog("Add New Product", 60, 15);

                var nameLabel = new Label("Name:")
                {
                    X = 1,
                    Y = 1
                };
                var nameField = new TextField("")
                {
                    X = 10,
                    Y = 1,
                    Width = 40
                };

                var priceLabel = new Label("Price:")
                {
                    X = 1,
                    Y = 3
                };
                var priceField = new TextField("0")
                {
                    X = 10,
                    Y = 3,
                    Width = 20
                };

                var stockLabel = new Label("Stock:")
                {
                    X = 1,
                    Y = 5
                };
                var stockField = new TextField("0")
                {
                    X = 10,
                    Y = 5,
                    Width = 20
                };

                var categoryLabel = new Label("Category:")
                {
                    X = 1,
                    Y = 7
                };
                var categories = ShopService.GetAllCategories();
                var categoryDropdown = new ComboBox()
                {
                    X = 10,
                    Y = 7,
                    Width = 30
                };
                categoryDropdown.SetSource(categories);

                var cancelButton = new Button("Cancel")
                {
                    X = 10,
                    Y = 10
                };
                cancelButton.Clicked += () => Application.RequestStop();

                var saveButton = new Button("Save")
                {
                    X = 30,
                    Y = 10
                };
                saveButton.Clicked += () =>
                {
                    try
                    {
                        string name = nameField.Text?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            MessageBox.ErrorQuery("Error", "Name cannot be empty", "OK");
                            return;
                        }

                        if (!int.TryParse(priceField.Text.ToString(), out int price) || price <= 0)
                        {
                            MessageBox.ErrorQuery("Error", "Price must be a positive number", "OK");
                            return;
                        }

                        if (!int.TryParse(stockField.Text.ToString(), out int stock) || stock < 0)
                        {
                            MessageBox.ErrorQuery("Error", "Stock must be a non-negative number", "OK");
                            return;
                        }

                        var index = categoryDropdown.SelectedItem;
                        var categoryName = categories[index];
                        var category = ShopService.GetCategoryId(categoryName);
                        if (category == null)
                        {
                            MessageBox.ErrorQuery("Error", $"{categoryName} category not found", "OK");
                            return;

                        }

                        WarehouseService.AddProduct(new(name, price, stock, category.Id));
                        SetStatus($"Product '{name}' added successfully");
                        Application.RequestStop();
                        RefreshProductList();
                    }
                    catch (DbUpdateException ex)
                    {
                        var innerMessage = ex.InnerException?.Message ?? ex.Message;
                        throw new Exception($"Failed to add product: {innerMessage}", ex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Error", $"Failed to add product: {ex.Message}", "OK");
                    }
                };

                
                addDialog.Add(nameLabel, nameField);
                addDialog.Add(priceLabel, priceField);
                addDialog.Add(stockLabel, stockField);
                addDialog.Add(categoryLabel, categoryDropdown);
                addDialog.Add(cancelButton, saveButton);

                Application.Run(addDialog);
            };

            
            var editButton = new Button("Edit Selected Product")
            {
                X = Pos.Right(addButton) + 2,
                Y = Pos.Bottom(productList) + 1
            };
            editButton.Clicked += () =>
            {
                if (productList.SelectedItem < 0)
                {
                    MessageBox.ErrorQuery("Error", "No product selected", "OK");
                    return;
                }

                try
                {
                    var products = ShopService.GetAllProducts();
                    var selectedProduct = products[productList.SelectedItem];

                    var editDialog = new Dialog($"Edit Product: {selectedProduct.Name}", 60, 15);

                    var nameLabel = new Label("Name:")
                    {
                        X = 1,
                        Y = 1
                    };
                    var nameField = new TextField(selectedProduct.Name)
                    {
                        X = 10,
                        Y = 1,
                        Width = 40
                    };

                    var priceLabel = new Label("Price:")
                    {
                        X = 1,
                        Y = 3
                    };
                    var priceField = new TextField(selectedProduct.Price.ToString())
                    {
                        X = 10,
                        Y = 3,
                        Width = 20
                    };

                    var stockLabel = new Label("Stock:")
                    {
                        X = 1,
                        Y = 5
                    };
                    var stockField = new TextField(selectedProduct.Stock.ToString())
                    {
                        X = 10,
                        Y = 5,
                        Width = 20
                    };

                    var categoryLabel = new Label("Category:")
                    {
                        X = 1,
                        Y = 7
                    };
                    var categories = ShopService.GetAllCategories();
                    var categoryDropdown = new ComboBox()
                    {
                        X = 10,
                        Y = 7,
                        Width = 30
                    };
                    categoryDropdown.SetSource(categories);
                    
                    int categoryIndex = categories.FindIndex(c => c == selectedProduct.Category?.Name);
                    if (categoryIndex >= 0)
                    {
                        categoryDropdown.SelectedItem = categoryIndex;
                    }

                    var cancelButton = new Button("Cancel")
                    {
                        X = 10,
                        Y = 10
                    };
                    cancelButton.Clicked += () => Application.RequestStop();

                    var saveButton = new Button("Save")
                    {
                        X = 30,
                        Y = 10
                    };
                    saveButton.Clicked += () =>
                    {
                        try
                        {
                            string name = nameField.Text?.ToString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                MessageBox.ErrorQuery("Error", "Name cannot be empty", "OK");
                                return;
                            }

                            if (!int.TryParse(priceField.Text.ToString(), out int price) || price <= 0)
                            {
                                MessageBox.ErrorQuery("Error", "Price must be a positive number", "OK");
                                return;
                            }

                            if (!int.TryParse(stockField.Text.ToString(), out int stock) || stock < 0)
                            {
                                MessageBox.ErrorQuery("Error", "Stock must be a non-negative number", "OK");
                                return;
                            }

                            var index = categoryDropdown.SelectedItem;
                            var categoryName = categories[index];
                            var category = ShopService.GetCategoryId(categoryName);
                            if (category == null)
                            {
                                MessageBox.ErrorQuery("Error", $"{categoryName} category not found", "OK");
                                return;

                            }



                            WarehouseService.UpdateProduct(selectedProduct.Id,
                                new(name, price, stock, category.Id)
                            );
                            SetStatus($"Product '{name}' updated successfully");
                            Application.RequestStop();
                            RefreshProductList();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.ErrorQuery("Error", $"Failed to update product: {ex.Message}", "OK");
                        }
                    };

                    
                    editDialog.Add(nameLabel, nameField);
                    editDialog.Add(priceLabel, priceField);
                    editDialog.Add(stockLabel, stockField);
                    editDialog.Add(categoryLabel, categoryDropdown);
                    editDialog.Add(cancelButton, saveButton);

                    Application.Run(editDialog);
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to load product data: {ex.Message}", "OK");
                }
            };

            
            var deleteButton = new Button("Delete Selected Product")
            {
                X = Pos.Right(editButton) + 2,
                Y = Pos.Bottom(productList) + 1
            };
            deleteButton.Clicked += () =>
            {
                if (productList.SelectedItem < 0)
                {
                    MessageBox.ErrorQuery("Error", "No product selected", "OK");
                    return;
                }

                var products = ShopService.GetAllProducts();
                var selectedProduct = products[productList.SelectedItem];

                var result = MessageBox.Query(
                    "Confirm Delete",
                    $"Are you sure you want to delete '{selectedProduct.Name}'?",
                    "Yes", "No");

                if (result == 0)
                {
                    try
                    {
                        WarehouseService.DeleteProduct(selectedProduct.Id);
                        SetStatus($"Product '{selectedProduct.Name}' deleted successfully");
                        RefreshProductList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Error", $"Failed to delete product: {ex.Message}", "OK");
                    }
                }
            };

            var productDetailsLabel = new Label("Select a product to view details")
            {
                X = Pos.Right(productList) + 2,
                Y = 0
            };

            productList.SelectedItemChanged += (args) =>
            {
                try
                {
                    if (productList.SelectedItem >= 0)
                    {
                        var products = ShopService.GetAllProducts();
                        var product = products[productList.SelectedItem];

                        decimal totalSales = WarehouseService.GetProductSales(product.Id);

                        productDetailsLabel.Text =
                            $"PRODUCT DETAILS\n\n" +
                            $"ID: {product.Id}\n" +
                            $"Name: {product.Name}\n" +
                            $"Price: ${product.Price}\n" +
                            $"Stock: {product.Stock}\n" +
                            $"Category: {product.Category?.Name ?? "Unknown"}\n" +
                            $"Total Sales: ${totalSales}";
                    }
                    else
                    {
                        productDetailsLabel.Text = "Select a product to view details";
                    }
                }
                catch (Exception ex)
                {
                    productDetailsLabel.Text = $"Error loading details: {ex.Message}";
                }
            };

            var refreshButton = new Button("Refresh List")
            {
                X = 0,
                Y = Pos.Bottom(addButton) + 1
            };
            refreshButton.Clicked += () =>
            {
                RefreshProductList();
                SetStatus("Product list refreshed");
            };

            var frame = (FrameView)productTab.View;
            frame.Add(productList);
            frame.Add(productDetailsLabel);
            frame.Add(addButton);
            frame.Add(editButton);
            frame.Add(deleteButton);
            frame.Add(refreshButton);

            _tabView.AddTab(productTab, true);
        }

        private void CreateCategoriesTab()
        {
            var categoryTab = new TabView.Tab("Categories", new FrameView("Category Management")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            var categoryList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(80)
            };

            void RefreshCategoryList()
            {
                var categories = ShopService.GetAllCategories();
                var categoryIds = ShopService.GetAllCategoryIds();

                if (categories.Count == categoryIds.Count)
                {
                    categoryList.SetSource(
                        categories.Select((cat, index) => $"{categoryIds[index]}: {cat}").ToList()
                    );
                }
                else
                {
                    
                    categoryList.SetSource(categories);
                }
            }

            RefreshCategoryList();

            
            var addButton = new Button("Add New Category")
            {
                X = 0,
                Y = Pos.Bottom(categoryList) + 1
            };
            addButton.Clicked += () =>
            {
                var addDialog = new Dialog("Add New Category", 50, 10);

                var nameLabel = new Label("Name:")
                {
                    X = 1,
                    Y = 1
                };
                var nameField = new TextField("")
                {
                    X = 10,
                    Y = 1,
                    Width = 30
                };

                var cancelButton = new Button("Cancel")
                {
                    X = 10,
                    Y = 5
                };
                cancelButton.Clicked += () => Application.RequestStop();

                var saveButton = new Button("Save")
                {
                    X = 30,
                    Y = 5
                };
                saveButton.Clicked += () =>
                {
                    try
                    {
                        string name = nameField.Text?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            MessageBox.ErrorQuery("Error", "Category name cannot be empty", "OK");
                            return;
                        }

                        WarehouseService.AddCategory(name);
                        SetStatus($"Category '{name}' added successfully");
                        Application.RequestStop();
                        RefreshCategoryList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Error", $"Failed to add category: {ex.Message}", "OK");
                    }
                };

                
                addDialog.Add(nameLabel, nameField);
                addDialog.Add(cancelButton, saveButton);

                Application.Run(addDialog);
            };

            
            var editButton = new Button("Edit Selected Category")
            {
                X = Pos.Right(addButton) + 2,
                Y = Pos.Bottom(categoryList) + 1
            };
            editButton.Clicked += () =>
            {
                if (categoryList.SelectedItem < 0)
                {
                    MessageBox.ErrorQuery("Error", "No category selected", "OK");
                    return;
                }

                try
                {
                    var categories = ShopService.GetAllCategories();
                    var categoryIds = ShopService.GetAllCategoryIds();
                    string selectedCategoryName = categoryList.SelectedItem < categories.Count ?
                        categories[categoryList.SelectedItem] : "";
                    int categoryId = categoryList.SelectedItem < categoryIds.Count ?
                        categoryIds[categoryList.SelectedItem] : 0;

                    var editDialog = new Dialog($"Edit Category: {selectedCategoryName}", 50, 10);

                    var nameLabel = new Label("Name:")
                    {
                        X = 1,
                        Y = 1
                    };
                    var nameField = new TextField(selectedCategoryName)
                    {
                        X = 10,
                        Y = 1,
                        Width = 30
                    };

                    var cancelButton = new Button("Cancel")
                    {
                        X = 10,
                        Y = 5
                    };
                    cancelButton.Clicked += () => Application.RequestStop();

                    var saveButton = new Button("Save")
                    {
                        X = 30,
                        Y = 5
                    };
                    saveButton.Clicked += () =>
                    {
                        try
                        {
                            string name = nameField.Text.ToString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                MessageBox.ErrorQuery("Error", "Category name cannot be empty", "OK");
                                return;
                            }

                            WarehouseService.UpdateCategory(categoryId, name);
                            SetStatus($"Category updated to '{name}' successfully");
                            Application.RequestStop();
                            RefreshCategoryList();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.ErrorQuery("Error", $"Failed to update category: {ex.Message}", "OK");
                        }
                    };

                    
                    editDialog.Add(nameLabel, nameField);
                    editDialog.Add(cancelButton, saveButton);

                    Application.Run(editDialog);
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to load category data: {ex.Message}", "OK");
                }
            };

            
            var deleteButton = new Button("Delete Selected Category")
            {
                X = Pos.Right(editButton) + 2,
                Y = Pos.Bottom(categoryList) + 1
            };
            deleteButton.Clicked += () =>
            {
                if (categoryList.SelectedItem < 0)
                {
                    MessageBox.ErrorQuery("Error", "No category selected", "OK");
                    return;
                }

                var categories = ShopService.GetAllCategories();
                var categoryIds = ShopService.GetAllCategoryIds();

                if (categoryList.SelectedItem >= categories.Count || categoryList.SelectedItem >= categoryIds.Count)
                {
                    MessageBox.ErrorQuery("Error", "Invalid category selection", "OK");
                    return;
                }

                string selectedCategoryName = categories[categoryList.SelectedItem];
                int categoryId = categoryIds[categoryList.SelectedItem];

                var result = MessageBox.Query(
                    "Confirm Delete",
                    $"Are you sure you want to delete '{selectedCategoryName}'?\n\nWARNING: This will affect all products in this category!",
                    "Yes", "No");

                if (result == 0) 
                {
                    try
                    {
                        WarehouseService.DeleteCategory(categoryId);
                        SetStatus($"Category '{selectedCategoryName}' deleted successfully");
                        RefreshCategoryList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Error", $"Failed to delete category: {ex.Message}", "OK");
                    }
                }
            };

            
            var categoryDetailsLabel = new Label("Select a category to view details")
            {
                X = Pos.Right(categoryList) + 2,
                Y = 0
            };

            
            categoryList.SelectedItemChanged += (args) =>
            {
                try
                {
                    if (categoryList.SelectedItem >= 0)
                    {
                        var categories = ShopService.GetAllCategories();
                        var categoryIds = ShopService.GetAllCategoryIds();

                        if (categoryList.SelectedItem < categories.Count && categoryList.SelectedItem < categoryIds.Count)
                        {
                            string categoryName = categories[categoryList.SelectedItem];
                            int categoryId = categoryIds[categoryList.SelectedItem];

                            int productCount = ShopService.GetAllProducts()
                                .Count(p => p.Category?.Name == categoryName);

                            decimal totalSales = WarehouseService.GetCategorySales(categoryId);

                            categoryDetailsLabel.Text =
                                $"CATEGORY DETAILS\n\n" +
                                $"ID: {categoryId}\n" +
                                $"Name: {categoryName}\n" +
                                $"Products: {productCount}\n" +
                                $"Total Sales: ${totalSales}";
                        }
                        else
                        {
                            categoryDetailsLabel.Text = "Error: Category data mismatch";
                        }
                    }
                    else
                    {
                        categoryDetailsLabel.Text = "Select a category to view details";
                    }
                }
                catch (Exception ex)
                {
                    categoryDetailsLabel.Text = $"Error loading details: {ex.Message}";
                }
            };

            
            var refreshButton = new Button("Refresh List")
            {
                X = 0,
                Y = Pos.Bottom(addButton) + 1
            };
            refreshButton.Clicked += () =>
            {
                RefreshCategoryList();
                SetStatus("Category list refreshed");
            };

            
            var frame = (FrameView)categoryTab.View;
            frame.Add(categoryList);
            frame.Add(categoryDetailsLabel);
            frame.Add(addButton);
            frame.Add(editButton);
            frame.Add(deleteButton);
            frame.Add(refreshButton);

            _tabView.AddTab(categoryTab, false);
        }

        private void CreateRestockTab()
        {
            var restockTab = new TabView.Tab("Restock", new FrameView("Restock Products")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            
            var productList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(70)
            };

            void RefreshProductList()
            {
                var products = ShopService.GetAllProducts();
                productList.SetSource(
                    products.Select(p => $"{p.Id}: {p.Name} - Current Stock: {p.Stock}").ToList()
                );
            }

            RefreshProductList();

            
            var quantityLabel = new Label("Quantity to add:")
            {
                X = 0,
                Y = Pos.Bottom(productList) + 2
            };
            var quantityField = new TextField("10")
            {
                X = Pos.Right(quantityLabel) + 2,
                Y = Pos.Bottom(productList) + 2,
                Width = 10
            };

            
            var restockButton = new Button("Restock Selected Product")
            {
                X = Pos.Right(quantityField) + 2,
                Y = Pos.Bottom(productList) + 2
            };
            restockButton.Clicked += () =>
            {
                if (productList.SelectedItem < 0)
                {
                    MessageBox.ErrorQuery("Error", "No product selected", "OK");
                    return;
                }

                try
                {
                    if (!int.TryParse(quantityField.Text.ToString(), out int quantity))
                    {
                        MessageBox.ErrorQuery("Error", "Quantity must be a number", "Ok");
                        quantityField.SetFocus();
                        return;
                    }

                    var products = ShopService.GetAllProducts();
                    var selectedProduct = products[productList.SelectedItem];

                    WarehouseService.RestockProduct(selectedProduct.Id, quantity);
                    SetStatus($"Added {quantity} units to '{selectedProduct.Name}' stock");
                    RefreshProductList();
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to restock product: {ex.Message}", "OK");
                }
            };

            
            var lowStockButton = new Button("Show Low Stock (<10)")
            {
                X = 0,
                Y = Pos.Bottom(quantityLabel) + 2
            };
            lowStockButton.Clicked += () =>
            {
                var products = ShopService.GetAllProducts();
                var lowStockProducts = products.Where(p => p.Stock < 10).ToList();

                if (lowStockProducts.Count == 0)
                {
                    MessageBox.Query("Low Stock", "No products with low stock found.", "OK");
                    return;
                }

                productList.SetSource(
                    lowStockProducts.Select(p => $"{p.Id}: {p.Name} - Current Stock: {p.Stock}").ToList()
                );
                SetStatus("Showing products with stock less than 10");
            };

            
            var showAllButton = new Button("Show All Products")
            {
                X = Pos.Right(lowStockButton) + 2,
                Y = Pos.Bottom(quantityLabel) + 2
            };
            showAllButton.Clicked += () =>
            {
                RefreshProductList();
                SetStatus("Showing all products");
            };

            
            var frame = (FrameView)restockTab.View;
            frame.Add(productList);
            frame.Add(quantityLabel, quantityField);
            frame.Add(restockButton);
            frame.Add(lowStockButton, showAllButton);

            _tabView.AddTab(restockTab, false);
        }

        private void CreateUserManagementTab()
        {
            var userTab = new TabView.Tab("Users", new FrameView("User Management")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            
            var userList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(80)
            };

            void RefreshUserList()
            {
                var users = WarehouseService.GetAllUsers();
                userList.SetSource(
                    users.Select(u => $"{u.Id}: {u.Email} - {(u.IsAdmin ? "Admin" : "Customer")}").ToList()
                );
            }

            RefreshUserList();

            
            var toggleAdminButton = new Button("Toggle Admin Rights")
            {
                X = 0,
                Y = Pos.Bottom(userList) + 1
            };
            toggleAdminButton.Clicked += () =>
            {
                if (userList.SelectedItem < 0)
                {
                    MessageBox.ErrorQuery("Error", "No user selected", "OK");
                    return;
                }

                try
                {
                    var users = WarehouseService.GetAllUsers();
                    var selectedUser = users[userList.SelectedItem];

                    
                    if (selectedUser.Id == GlobalStore.Instance?.CurrentUser?.Id)
                    {
                        MessageBox.ErrorQuery("Error", "You cannot change your own admin status", "OK");
                        return;
                    }

                    if (selectedUser.IsAdmin)
                    {
                        WarehouseService.RemoveAdminRights(selectedUser.Id);
                        SetStatus($"Removed admin rights from {selectedUser.Email}");
                    }
                    else
                    {
                        WarehouseService.SetUserAsAdmin(selectedUser.Id);
                        SetStatus($"Granted admin rights to {selectedUser.Email}");
                    }

                    RefreshUserList();
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to update user rights: {ex.Message}", "OK");
                }
            };

            
            var refreshButton = new Button("Refresh List")
            {
                X = Pos.Right(toggleAdminButton) + 2,
                Y = Pos.Bottom(userList) + 1
            };
            refreshButton.Clicked += () =>
            {
                RefreshUserList();
                SetStatus("User list refreshed");
            };

            
            var frame = (FrameView)userTab.View;
            frame.Add(userList);
            frame.Add(toggleAdminButton);
            frame.Add(refreshButton);

            _tabView.AddTab(userTab, false);
        }

        private void CreateReportsTab()
        {
            var reportsTab = new TabView.Tab("Reports", new FrameView("Sales Reports")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });

            
            var reportList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(80)
            };
            reportList.SetSource(new List<string>
    {
        "1. Total Sales",
        "2. Sales by Product",
        "3. Sales by Category"
    });
            reportList.SelectedItem = 0;

            
            var reportDetailsLabel = new Label("Select a report to view details")
            {
                X = 0,
                Y = Pos.Bottom(reportList) + 1
            };

            
            void UpdateReportDetails()
            {
                if (reportList.SelectedItem >= 0)
                {
                    reportDetailsLabel.Text = "Loading report...";
                    Application.Refresh(); 

                    if (reportList.SelectedItem == 0)
                    {
                        reportDetailsLabel.Text = "Total Sales: $" + WarehouseService.GetTotalSales();
                    }
                    else if (reportList.SelectedItem == 1)
                    {
                        var products = ShopService.GetAllProducts();
                        var salesByProduct = products.Select(p => $"{p.Name}: ${WarehouseService.GetProductSales(p.Id)}").ToList();
                        reportDetailsLabel.Text = string.Join("\n", salesByProduct);
                    }
                    else if (reportList.SelectedItem == 2)
                    {
                        var categories = ShopService.GetAllCategoriesData();
                        var salesByCategory = categories.Select(c => $"{c.Name}: ${WarehouseService.GetCategorySales(c.Id)}").ToList();
                        reportDetailsLabel.Text = string.Join("\n", salesByCategory);
                    }
                }
                else
                {
                    reportDetailsLabel.Text = "Select a report to view details";
                }
            }

            
            reportList.SelectedItemChanged += (args) =>
            {
                UpdateReportDetails();
            };

            
            var refreshButton = new Button("Refresh Report")
            {
                X = 0,
                Y = Pos.Bottom(reportDetailsLabel) + 1
            };
            refreshButton.Clicked += () =>
            {
                UpdateReportDetails();
                SetStatus("Report refreshed");
            };

            
            var frame = (FrameView)reportsTab.View;
            frame.Add(reportList);
            frame.Add(reportDetailsLabel);
            frame.Add(refreshButton);
            _tabView.AddTab(reportsTab, false);
        }
        private void SetStatus(string message)
        {
            _statusLabel.Text = message;
            Application.Refresh();
        }
    }
}



