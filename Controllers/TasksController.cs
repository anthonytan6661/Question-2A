using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Net;
using TaskAPI.Models;
using TaskAPI.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IRegisterTokenService _registerTokenService;
        public TasksController(IRegisterTokenService registerTokenService)
        {
            _registerTokenService = registerTokenService;
        }


        [HttpPost]
        public IActionResult Post([FromBody] UserTask usertask)
        {
            var mytoken = _registerTokenService.GetToken(usertask);

            if (mytoken.StatusCode== HttpStatusCode.OK)
            {
                SendMessageToRabbit(mytoken.Content);
                return Ok(mytoken.Content);
            } else
            {
                return Unauthorized(mytoken.Content);
            }
        }

        private void SendMessageToRabbit(string registerToken)
        {
            var factory = new ConnectionFactory()
            {
                //HostName = "localhost" , 
                //Port = 32001
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };

            Console.WriteLine(factory.HostName + ":" + factory.Port);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "TaskQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = registerToken;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "TaskQueue",
                                     basicProperties: null,
                                     body: body);
            }
        }

    }
}
