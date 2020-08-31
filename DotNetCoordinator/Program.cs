using System;
using System.Net.Http;
using System.Transactions;

namespace DotNetCoordinator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Press a Key to start the Transaction");
                Console.ReadLine();
                using (var scope = new TransactionScope())
                {
                    bool commit = true;
                    try
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            // database operation done in an external app domain  
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(@"http://localhost:50714");
                                using (var request = new HttpRequestMessage(HttpMethod.Post, "work/dojob"))
                                {
                                    // forward transaction token  
                                    request.AddTransactionPropagationToken();
                                    Console.WriteLine($"{i}...");
                                    var response = client.SendAsync(request).Result;
                                    response.EnsureSuccessStatusCode();
                                    Console.WriteLine($"{i}:OK!");
                                }
                            }
                            Console.WriteLine($"10 sec delay...");
                            System.Threading.Thread.Sleep(10000);
                        }
                    }
                    catch (Exception ex)
                    {
                        commit = false;
                        Console.WriteLine(ex.ToString());
                    }
                    if (commit)
                    {
                        Console.WriteLine("Commiting!");

                        // Commit local and cross domain operations  
                        scope.Complete();
                    }
                    else
                    {
                        Console.WriteLine("Rolling Back!!!!!");
                    }
                }
            }
        }
    }
}
