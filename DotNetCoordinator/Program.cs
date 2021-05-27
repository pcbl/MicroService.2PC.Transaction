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
                                //client.BaseAddress = new Uri(@"http://localhost:5000"); .Net core(not working... for now...)
                                //https://github.com/dotnet/runtime/issues/715
                                using (var request = new HttpRequestMessage(HttpMethod.Post, "work/dojob"))
                                {
                                    request.Content = new StringContent("\"client\"", System.Text.Encoding.UTF8,"application/json");
                                    //Uncomment to simulate an error!
                                    //request.Content = new StringContent("\"error\"", System.Text.Encoding.UTF8, "application/json");
                                    // forward transaction token  
                                    var token = request.AddTransactionPropagationToken();
                                    Console.WriteLine($"{token}::>>{i}...");
                                    var response = client.SendAsync(request).Result;
                                    response.EnsureSuccessStatusCode();
                                    Console.WriteLine($"{i}:OK!");
                                }
                            }
                            Console.WriteLine($"5 sec delay...");
                            System.Threading.Thread.Sleep(5000);
                        }
                    }
                    catch (Exception ex)
                    {
                        commit = false;
                        Console.WriteLine(ex.ToString());
                    }
                    if (commit)
                    {
                        Console.WriteLine("Press Enter to Commit! (You might use the transaciton IT to make request from another client such as postman)");
                        Console.ReadLine();
                        // Commit local and cross domain operations  
                        scope.Complete();
                        Console.WriteLine("C O M M I T E D");
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
