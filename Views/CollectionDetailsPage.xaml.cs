using System;
using System.Collections.Generic;
using System.Linq;
using CollectionManagementSystem.Models;
using CollectionManagementSystem.Services;

namespace CollectionManagementSystem.Views
{
    public partial class CollectionDetailsPage : ContentPage
    {
        private Collection _collection;

        public CollectionDetailsPage(Collection collection)
        {
            InitializeComponent();
            _collection = collection;
            BindingContext = _collection;
            RefreshTable();
        }

        private void RefreshTable()
        {
            TableHeader.Children.Clear();
            TableData.Children.Clear();

            
            var headers = new List<string> { "Nazwa", "Cena", "Status", "Ocena (1-10)", "Komentarz" };
            foreach (var col in _collection.CustomColumns) headers.Add(col.Name);
            headers.Add("Akcje"); 

            foreach (var h in headers)
            {
                TableHeader.Children.Add(new Label { Text = h, FontAttributes = FontAttributes.Bold, WidthRequest = 100, TextColor = Color.FromArgb("#2B303A") });
            }

            
            var sortedItems = _collection.Items
                .OrderBy(i => i.Status?.ToLower() == "sprzedany" ? 1 : 0)
                .ToList();

            foreach (var item in sortedItems)
            {
                var rowLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 10 };
                bool isSold = item.Status?.ToLower() == "sprzedany";
                Color textColor = isSold ? Colors.Gray : Color.FromArgb("#2B303A");

                rowLayout.Children.Add(new Label { Text = item.Name, WidthRequest = 100, TextColor = textColor });
                rowLayout.Children.Add(new Label { Text = item.Price.ToString("0.00"), WidthRequest = 100, TextColor = textColor });
                rowLayout.Children.Add(new Label { Text = item.Status, WidthRequest = 100, TextColor = textColor });
                rowLayout.Children.Add(new Label { Text = item.Rating.ToString(), WidthRequest = 100, TextColor = textColor });
                rowLayout.Children.Add(new Label { Text = item.Comment, WidthRequest = 100, TextColor = textColor, LineBreakMode = LineBreakMode.TailTruncation });

                foreach (var col in _collection.CustomColumns)
                {
                    string val = item.CustomValues.ContainsKey(col.Id) ? item.CustomValues[col.Id] : "-";
                    rowLayout.Children.Add(new Label { Text = val, WidthRequest = 100, TextColor = textColor });
                }

                var btnEdit = new Button { Text = "Edytuj", HeightRequest = 35, FontSize = 12, Padding = 2, BackgroundColor = Color.FromArgb("#F2A03D") };
                btnEdit.Clicked += (s, e) => OnEditItem(item);
                var btnDel = new Button { Text = "Usun", HeightRequest = 35, FontSize = 12, Padding = 2, BackgroundColor = Color.FromArgb("#D64933") };
                btnDel.Clicked += (s, e) => OnDeleteItem(item);

                rowLayout.Children.Add(new HorizontalStackLayout { Children = { btnEdit, btnDel }, Spacing = 5 });

                TableData.Children.Add(rowLayout);
            }
        }

        private async void OnEditItem(CollectionItem item)
        {
            await Navigation.PushAsync(new EditItemPage(_collection, item));
        }

        private async void OnDeleteItem(CollectionItem item)
        {
            var confirm = await DisplayAlert("Usun", "Czy na pewno chcesz usunac element?", "Tak", "Nie");
            if (confirm)
            {
                _collection.Items.Remove(item);
                SaveData();
                RefreshTable();
            }
        }

        private async void OnAddItemClicked(object sender, EventArgs e)
        {
            var newItem = new CollectionItem { Name = "Nowy" };
            await Navigation.PushAsync(new EditItemPage(_collection, newItem));
        }

        private async void OnAddColumnClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Nowa kolumna", "Podaj nazwe kolumny:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string type = await DisplayActionSheet("Wybierz typ", "Anuluj", null, "Text", "Number", "List");
            if (type == "Anuluj" || string.IsNullOrEmpty(type)) return;

            var col = new CustomColumn { Name = name, Type = type };

            if (type == "List")
            {
                string opts = await DisplayPromptAsync("Opcje listy", "Podaj opcje po przecinku (np. Jeden, Dwa, Trzy)");
                if (!string.IsNullOrWhiteSpace(opts))
                {
                    col.Options = opts.Split(',').Select(x => x.Trim()).ToList();
                }
            }

            _collection.CustomColumns.Add(col);
            SaveData();
            RefreshTable();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshTable();
        }

        private void SaveData()
        {
            var allCollections = DataManager.LoadData();
            var current = allCollections.FirstOrDefault(c => c.Id == _collection.Id);
            if (current != null)
            {
                allCollections[allCollections.IndexOf(current)] = _collection;
            }
            else
            {
                allCollections.Add(_collection);
            }
            DataManager.SaveData(allCollections);
        }
    }
}
