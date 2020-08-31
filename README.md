Even that Transactions are not recommended on MicroServices(that usually is a sign that your services were mistakenly splitted), there are some situations(usually legacy calls) that demans some transaction control...

# MicroService.2PC.Transaction
Sample using Transactions(via MSDTC) on Rest Calls to .Net Framework Service(Via System.Transactions)

Created based on this article:
https://www.c-sharpcorner.com/article/distributed-transactions-with-webapi-across-application-domains/

# Getting it to work
1. Create a Database with a table named LogTable with this structure:
```
CREATE TABLE [dbo].[LogTable](
                [ID] [int] IDENTITY(1,1) NOT NULL,
                [Name] [varchar](50) NOT NULL,
CONSTRAINT [PK_LogTable] PRIMARY KEY CLUSTERED 
(
                [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
```
2. If needed, just update the DB connectionstring on the NetFrwService\Web.Config

3. Start the solution with both Projects(Multiple) set as Startup Project. Press enter on the console application. It will run a loop of 2 calls to the service withina  transactionscope. Between the calls it is waiting 10 seconds... Between calls you can see the table is locked and that only everythign is fine, the Transaction will be commited.

#The important Bits
What really matters is:
- When the HTTP Client makes the request, we inject a header named "TransactionToken" with Transaction Propagation Token Serialized as Base64:
https://github.com/pcbl/MicroService.2PC.Transaction/blob/c04e9dca4f8042ab4439755beb0372602e2a6e36/DotNetCoordinator/Extensions.cs#L13-L14

- When the Rest Service receives the request, it checks for the "TransactionToken" header and creates a transaction Scope connected with the transaction started on the Caller. We do this over a Filter:
https://github.com/pcbl/MicroService.2PC.Transaction/blob/c04e9dca4f8042ab4439755beb0372602e2a6e36/NetFrwService/Filters/EnlistToDistributedTransactionActionFilter.cs#L17-L56

Note that the Controller Action that needs to be "transactionable" have an attribute(EnlistToDistributedTransactionActionFilter) to use the filter:
https://github.com/pcbl/MicroService.2PC.Transaction/blob/c04e9dca4f8042ab4439755beb0372602e2a6e36/NetFrwService/Controllers/WorkController.cs#L13

Obviously, both machines needs to have access to the MSDTC which is coordinating the transaction. Otherwise it will not work.

**ENJOY**

