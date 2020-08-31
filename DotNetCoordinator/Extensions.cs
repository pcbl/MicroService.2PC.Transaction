using System;
using System.Net.Http;
using System.Transactions;

namespace DotNetCoordinator
{
    public static class Extensions
    {
        public static void AddTransactionPropagationToken(this HttpRequestMessage request)
        {
            if (Transaction.Current != null)
            {
                var token = TransactionInterop.GetTransmitterPropagationToken(Transaction.Current);
                request.Headers.Add("TransactionToken", Convert.ToBase64String(token));
            }
        }
    }
}
