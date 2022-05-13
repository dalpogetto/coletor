using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectorQi.Models;
using CollectorQi.Resources.DataBaseHelper;

namespace CollectorQi.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>();
            var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "1 item", Description="1" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "2 item", Description="2" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "3 item", Description="3" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "4 item", Description="4" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "5 item", Description="5" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "6 item", Description="6" },
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }

        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}