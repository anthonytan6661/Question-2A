using RestSharp;
using TaskAPI.Models;

namespace TaskAPI.Services
{
    public interface IRegisterTokenService
    {
        RestResponse GetToken(UserTask user);
    }
}