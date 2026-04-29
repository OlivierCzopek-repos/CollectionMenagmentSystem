using System;
using System.Collections.ObjectModel;
using System.Linq;
using CollectionManagementSystem.Models;
using CollectionManagementSystem.Services;
using CollectionManagementSystem.Views;

namespace CollectionManagementSystem
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Collection> Collections { get; set; } = new ObservableCollection<Collection>();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Collections.Clear();
            var data = DataManager.LoadData();
            foreach (var col in data)
            {
                Collections.Add(col);
            }
        }

        private async void OnAddCollectionClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Nowa Kolekcja", "Podaj nazwe kolekcji:");
            if (!string.IsNullOrWhiteSpace(result))
            {
                var newCollection = new Collection { Name = result };
                Collections.Add(newCollection);
                DataManager.SaveData(Collections.ToList());
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Collection collection)
            {
                await Navigation.PushAsync(new CollectionDetailsPage(collection));
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}
