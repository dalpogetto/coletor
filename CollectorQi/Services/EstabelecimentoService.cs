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
    public class EstabelecimentoService
    {
        //private IEnumerable<Parametros> parametros;

        //public async Task<IEnumerable<Parametros>> GetItemsAsync(Parametros parametros)
        //{
        //    try
        //    {
                
        //        string json = JsonConvert.SerializeObject(parametros);
                
        //        var client = new RestClient("");

        //        var request = new RestRequest();
                
        //        request.Method = Method.POST;
        //        request.AddHeader("Accept", "application/json");
        //        request.Parameters.Clear();
        //        request.AddParameter("application/json", json,
        //        ParameterType.RequestBody);
        //        //
        //        var response = client.Execute(request);
        //        var content = response.Content;


        //    }
        //    catch (Exception e)
        //    {}

        //    return parametros;
        //}


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