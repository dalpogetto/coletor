using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CollectorQi.Models;
using CollectorQi.Resources.DataBaseHelper;
using Newtonsoft.Json;

namespace CollectorQi.Services
{
    public class ParametrosService
    {
        private IEnumerable<Parametros> parametros;
        public async Task<IEnumerable<Parametros>> GetItemsAsync()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(new Uri("https://brspupapl01.ad.diebold.com:8543/api/integracao/coletores/v1/escl002api/ObterParametro"));
                var result = await response.Content.ReadAsStringAsync();

                parametros = JsonConvert.DeserializeObject<IEnumerable<Parametros>>(result) as IEnumerable<Parametros>;
               
            }
            catch (Exception e)
            {}

            return parametros;
        }



        //public async Task<Item> GetItemAsync(string id)
        //{
        //    return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        //}

        //public async Task<bool> AddItemAsync(Item item)
        //{
        //    items.Add(item);

        //    return await Task.FromResult(true);
        //}

        //public async Task<bool> UpdateItemAsync(Item item)
        //{
        //    var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
        //    items.Remove(oldItem);
        //    items.Add(item);

        //    return await Task.FromResult(true);
        //}

        //public async Task<bool> DeleteItemAsync(string id)
        //{
        //    var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
        //    items.Remove(oldItem);

        //    return await Task.FromResult(true);
        //}  
    }
}