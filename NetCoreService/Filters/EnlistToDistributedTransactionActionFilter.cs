using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace NetCoreService.Filters
{
    public class EnlistToDistributedTransactionActionFilter : ActionFilterAttribute
    {
        private const string TransactionId = "TransactionToken";

        /// <summary>
        /// Retrieve a transaction propagation token, create a transaction scope and promote the current transaction to a distributed transaction.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.HttpContext.Request.Headers.ContainsKey(TransactionId))
            {
                var values = actionContext.HttpContext.Request.Headers[TransactionId];
                if (values.Any())
                {
                    //27-05-2021 -> NOT YET Supported on .net 5
                    //https://github.com/dotnet/runtime/issues/715
                    //Otherwise
                    //System.PlatformNotSupportedException: 'This platform does not support distributed transactions.'
                    byte[] transactionToken = Convert.FromBase64String(values.FirstOrDefault());
                    var transaction = TransactionInterop.GetTransactionFromTransmitterPropagationToken(transactionToken);
                    var transactionScope = new TransactionScope(transaction);
                    actionContext.HttpContext.Items.Add(TransactionId, transactionScope);
                }
            }
        }

        /// <summary>
        /// Rollback or commit transaction.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.HttpContext.Items.ContainsKey(TransactionId))
            {
                var transactionScope = actionExecutedContext.HttpContext.Items[TransactionId] as TransactionScope;
                if (transactionScope != null)
                {
                    if (actionExecutedContext.Exception != null)
                    {
                        Transaction.Current.Rollback();
                    }
                    else
                    {
                        transactionScope.Complete();
                    }

                    transactionScope.Dispose();
                    actionExecutedContext.HttpContext.Items[TransactionId] = null;
                    actionExecutedContext.HttpContext.Items.Remove(TransactionId);
                }
            }
        }
    }
}