using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreService.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NetCoreService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public WorkController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [Route("DoJob")]
        [HttpPost]
        [EnlistToDistributedTransactionActionFilter]
        public void DoJob([FromBody]string preffix)
        {
            if (preffix == "error") throw new Exception("We wanted an error!");
            using (var con = new System.Data.SqlClient.SqlConnection(_configuration.GetValue<string>("DB")))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Insert Into LogTable(Name) values ('{preffix}--{Guid.NewGuid()}')";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
