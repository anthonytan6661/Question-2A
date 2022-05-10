using RestSharp;
using TaskAPI.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp.Serializers;

namespace TaskAPI.Services
{
    public class RegisterTokenService : IRegisterTokenService
    {
        public RegisterTokenService() { }

        public RestResponse GetToken(UserTask user)
        {

            var url = "https://reqres.in";

            var client = new RestClient(url);
            var request = new RestRequest("/api/register", Method.Post);
          
            request.AddJsonBody(user);

            //request.AddJsonBody(JsonSerializer.Serialize(user));
            var response = client.ExecuteAsync(request);

            return response.Result;


        }

    }
}
