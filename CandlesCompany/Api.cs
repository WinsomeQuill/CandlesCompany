using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CandlesCompany
{
    public static class Api
    {
        private static string _baseUrl { get; } = "http://localhost:44316/api/";
        private static string _version { get; } = "CandlesStoreClient 1.0";
        private static RestClient _client { get; } = new RestClient(_baseUrl);

        public async static Task<JObject> Login(string email, string pass)
        {
            RestRequest request = new RestRequest("users/login", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("email", email)
                .AddQueryParameter("pass", pass);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> Registration(string email, string pass,
            string first_name, string last_name, string middle_name = null)
        {
            RestRequest request = new RestRequest("users/reg", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("email", email)
                .AddQueryParameter("pass", pass)
                .AddQueryParameter("first_name", first_name)
                .AddQueryParameter("last_name", last_name)
                .AddQueryParameter("middle_name", middle_name);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetRole(string email)
        {
            RestRequest request = new RestRequest("users/role", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("email", email);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetRoles()
        {
            RestRequest request = new RestRequest("users/roles", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> ChangeRoleById(int id_user, string role_name)
        {
            RestRequest request = new RestRequest("users/setrole", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("email", id_user)
                .AddQueryParameter("pass", role_name);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetUsersCount()
        {
            RestRequest request = new RestRequest("users/userscount", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetUsersForPage(int page)
        {
            RestRequest request = new RestRequest("users/usersforpage", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("page", page);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> FindUsers(string value)
        {
            RestRequest request = new RestRequest("users/findusers", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("value", value);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetEmployeesForPage(int page)
        {
            RestRequest request = new RestRequest("users/employeesforpage", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("page", page);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> FindEmployees(string value)
        {
            RestRequest request = new RestRequest("users/findemployees", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("value", value);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetEmployeesCount()
        {
            RestRequest request = new RestRequest("users/employeescount", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetCandles()
        {
            RestRequest request = new RestRequest("candles/candles", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetTypeCandles()
        {
            RestRequest request = new RestRequest("candles/candlestype", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> UpdateItem(int id_candle, int id_type, string name, string description, int count, double price, byte[] image)
        {
            RestRequest request = new RestRequest("candles/updateitem", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_candle", id_candle)
                .AddQueryParameter("id_type", id_type)
                .AddQueryParameter("name", name)
                .AddQueryParameter("description", description)
                .AddQueryParameter("count", count)
                .AddQueryParameter("price", price)
                .AddBody("image", image.ToString());
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> AddItem(int id_type, string name, string description, int count, double price, byte[] image)
        {
            RestRequest request = new RestRequest("candles/additem", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_type", id_type)
                .AddQueryParameter("name", name)
                .AddQueryParameter("description", description)
                .AddQueryParameter("count", count)
                .AddQueryParameter("price", price)
                .AddBody("image", image.ToString());
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> RemoveItem(int id_candle)
        {
            RestRequest request = new RestRequest("candles/removeitem", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_candle", id_candle);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetCandlesBasket(int id_user)
        {
            RestRequest request = new RestRequest("users/getcandlesbasket", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> AddCandlesBasket(int id_user, int id_candle)
        {
            RestRequest request = new RestRequest("users/addcandlesbasket", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user)
                .AddQueryParameter("id_candle", id_candle);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> UpdateCandlesBasket(int id_user, int id_candle, int count)
        {
            RestRequest request = new RestRequest("users/updatecandlesbasket", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user)
                .AddQueryParameter("id_candle", id_candle)
                .AddQueryParameter("count", count);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> RemoveCandlesBasket(int id_user, int id_candle)
        {
            RestRequest request = new RestRequest("users/removecandlesbasket", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user)
                .AddQueryParameter("id_candle", id_candle);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetOrders(int id_user)
        {
            RestRequest request = new RestRequest("orders/getorders", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetOrdersForPage(int page)
        {
            RestRequest request = new RestRequest("orders/getordersforpage", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("page", page);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetOrdersCount()
        {
            RestRequest request = new RestRequest("orders/getorderscount", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> FindOrders(string name_candle)
        {
            RestRequest request = new RestRequest("orders/findorders", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("name_candle", name_candle);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> ChangeOrderStatus(int id_order, string name_status)
        {
            RestRequest request = new RestRequest("orders/changeorderstatus", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_order", id_order)
                .AddQueryParameter("name_status", name_status);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetStatusList()
        {
            RestRequest request = new RestRequest("orders/getstatuslist", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> AddOrder(int id_user, int id_candle, int count, double price, int id_address)
        {
            RestRequest request = new RestRequest("orders/addorder", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user)
                .AddQueryParameter("id_candle", id_candle)
                .AddQueryParameter("count", count)
                .AddQueryParameter("price", price)
                .AddQueryParameter("id_address", id_address);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> RemoveAvatarUser(int id_user)
        {
            RestRequest request = new RestRequest("users/removeavatar", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> SetAvatarUser(int id_user, byte[] image)
        {
            RestRequest request = new RestRequest("users/setavatar", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user)
                .AddBody("image", image.ToString());
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> AddAddresses(string name)
        {
            RestRequest request = new RestRequest("address/addaddress", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("name", name);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> GetAddresses()
        {
            RestRequest request = new RestRequest("address/getaddress", Method.Get)
                .AddHeader("User-Agent", _version);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> RemoveAddress(int id_address)
        {
            RestRequest request = new RestRequest("address/removeaddress", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_address", id_address);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }

        public async static Task<JObject> ChangePhone(int id_user, string number)
        {
            RestRequest request = new RestRequest("users/setphone", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("User-Agent", _version)
                .AddQueryParameter("id_user", id_user)
                .AddQueryParameter("number", number);
            RestResponse a = await _client.ExecuteAsync(request);
            JObject jsonObject = JObject.Parse(a.Content);
            return jsonObject;
        }
    }
}
