using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeQueue.Queue;

namespace TimeQueue.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly QueueCore core;

        public TestController(QueueCore _core)
        {
            core = _core;
        }


        public Task<string> Index()
        {
            return Task.FromResult("hello,virus");
        }


        [HttpPost]
        public Task<string> Push([FromBody] PushRequest request)
        {
            if (request == null)
            {
                return Task.FromResult("错误的请求参数");
            }


            if (request.datetime <= DateTime.Now)
            {
                return Task.FromResult("不能写入过去的时间");
            }

            core.Push(request.data, request.datetime);
            return Task.FromResult("success"); 
        }


        [HttpPost]
        public ActionResult Pop()
        {
            string s = core.Pop();
            if (s == null)
            {
                s = "";
            }
            return Content(s);
        }

    }


    public class PushRequest
    {
        public string data { get; set; }
        public DateTime datetime { get; set; }
    }
}
