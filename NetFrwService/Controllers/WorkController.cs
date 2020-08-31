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
        public void DoJob()
        {
            using(var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
            {
                con.Open();
                using(var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Insert Into LogTable(Name) values ('{Guid.NewGuid().ToString()}')";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
