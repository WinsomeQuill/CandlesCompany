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
            RestRequest request = new RestRequest("users/role", Method.Get)
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
    }
}
