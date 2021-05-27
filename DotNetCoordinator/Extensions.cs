using System;
using System.Net.Http;
using System.Transactions;

namespace DotNetCoordinator
{
    public static class Extensions
    {
        public static string AddTransactionPropagationToken(this HttpRequestMessage request)
        {
            string token=null;
            if (Transaction.Current != null)
            {
                var tokenBytes = TransactionInterop.GetTransmitterPropagationToken(Transaction.Current);
                token = Convert.ToBase64String(tokenBytes);
                request.Headers.Add("TransactionToken", token);
            }
            return token;
        }
    }
}
