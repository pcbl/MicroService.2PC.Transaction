using NetFrwService.Filters;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;

namespace NetFrwService.Controllers
{
    public class WorkController : ApiController
    {
        [ActionName("DoJob")]
        [HttpPost]
        [EnlistToDistributedTransactionActionFilter]
        public void DoJob([FromBody]string preffix)
        {
            if (preffix == "error") throw new Exception("We wanted an error!");
            using(var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
            {
                con.Open();
                using(var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Insert Into LogTable(Name) values ('{preffix}--{Guid.NewGuid()}')";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
