using System;
using System.Linq;
using CollectionManagementSystem.Models;
using CollectionManagementSystem.Services;
using Microsoft.Maui.Controls;

namespace CollectionManagementSystem.Views
{
    public partial class EditItemPage : ContentPage
    {
        private Collection _collection;
        public CollectionItem Item { get; set; }

        public EditItemPage(Collection collection, CollectionItem item)
        {
            InitializeComponent();
            _collection = collection;
            Item = item;
            BindingContext = this;
            BuildDynamicColumns();
        }

        private void BuildDynamicColumns()
        {
            foreach (var col in _collection.CustomColumns)
            {
                var label = new Label { Text = col.Name, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#2B303A") };
                DynamicColumnsLayout.Children.Add(label);

                
                if (!Item.CustomValues.TryGetValue(col.Id, out string val))
                    val = string.Empty;

                if (col.Type == "Text")
                {
                    var entry = new Entry { Text = val, BackgroundColor = Colors.White, TextColor = Colors.Black };
                    entry.TextChanged += (s, e) => Item.CustomValues[col.Id] = e.NewTextValue;
                    DynamicColumnsLayout.Children.Add(entry);
                }
                else if (col.Type == "Number")
                {
                    var entry = new Entry { Text = val, Keyboard = Keyboard.Numeric, BackgroundColor = Colors.White, TextColor = Colors.Black };
                    entry.TextChanged += (s, e) => Item.CustomValues[col.Id] = e.NewTextValue;
                    DynamicColumnsLayout.Children.Add(entry);
                }
                else if (col.Type == "List")
                {
                    var picker = new Picker { ItemsSource = col.Options, SelectedItem = val, BackgroundColor = Colors.White, TextColor = Colors.Black };
                    picker.SelectedIndexChanged += (s, e) => {
                        if (picker.SelectedItem != null)
                            Item.CustomValues[col.Id] = picker.SelectedItem.ToString();
                    };
                    DynamicColumnsLayout.Children.Add(picker);
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            
            var allCollections = DataManager.LoadData();
            var targetCol = allCollections.FirstOrDefault(x => x.Id == _collection.Id);
            if (targetCol != null)
            {
                bool exists = targetCol.Items.Any(x => x.Id != Item.Id && string.Equals(x.Name, Item.Name, StringComparison.OrdinalIgnoreCase));
                if (exists)
                {
                    bool proceed = await DisplayAlert("Uwaga", "Element o takiej nazwie ju¿ istnieje. Czy na pewno chcesz dodaæ/zapisaæ ten element?", "Tak", "Nie");
                    if (!proceed) return;
                }

                var targetItem = targetCol.Items.FirstOrDefault(x => x.Id == Item.Id);
                if (targetItem != null)
                {
                    targetCol.Items[targetCol.Items.IndexOf(targetItem)] = Item;
                    var localItem = _collection.Items.FirstOrDefault(x => x.Id == Item.Id);
                    if (localItem != null)
                    {
                        _collection.Items[_collection.Items.IndexOf(localItem)] = Item;
                    }
                }
                else
                {
                    targetCol.Items.Add(Item);
                    _collection.Items.Add(Item);
                }

                DataManager.SaveData(allCollections);
            }

            await Navigation.PopAsync();
        }
    }
}
