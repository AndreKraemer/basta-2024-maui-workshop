﻿using DontLetMeExpire.Models;
using DontLetMeExpire.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontLetMeExpire.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        private IItemService _itemService;
        private IStorageLocationService _storageLocationService;

        public ItemsViewModel(IItemService itemService, IStorageLocationService storageLocationService)
        {
            _itemService = itemService;
            _storageLocationService = storageLocationService;
        }

        public ObservableCollection<Item> Items { get; set; } = new();

        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }


        public async Task InitializeAsync(string displayMode, string locationId)
        {
            IEnumerable<Item> items; // = await _itemService.GetAsync();

            switch (displayMode)
            {
                case "Stock":
                    items = await _itemService.GetAsync();
                    Title = "Mein Vorrat";
                    break;
                case "ExpiringSoon":
                    items = await _itemService.GetExpiresSoonAsync();
                    Title = "Läuft bald ab";
                    break;
                case "ExpiresToday":
                    items = await _itemService.GetExpiresTodayAsync();
                    Title = "Läuft heute ab";
                    break;
                case "Expired":
                    Title = "Abgelaufen";
                    items = await _itemService.GetExpiredAsync();
                    break;
                case "Location":
                    items = await _itemService.GetByLocationAsync(locationId);
                    var location = await _storageLocationService.GetByIdAsync(locationId);
                    Title = location?.Name ?? "Ort";
                    break;
                default:
                    Title = "Mein Vorrat";
                    items = await _itemService.GetAsync();
                    break;
            }

            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
    }
}
