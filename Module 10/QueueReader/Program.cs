using Azure;
using Azure.Messaging.ServiceBus;

namespace QueueReader;

class Program
{
    static string EndPoint = "pszeurkous.servicebus.windows.net";
    static (string Name, string KeY) SasKeyReader = ("lezert", "");
    static string QueueName = "myqueue2";

    static async Task Main(string[] args)
    {
        //await ReadQueueAsync();
        await ReadQueueProcessorAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task ReadQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyReader.Name, SasKeyReader.KeY);
        var client = new ServiceBusClient(EndPoint, cred);
        var receiver = client.CreateReceiver(QueueName);
        
        int teller = 0;
        do
        {
            var msg = await receiver.ReceiveMessageAsync();
            Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken}");
            var data = msg.Body.ToString();
            if (teller++ % 10 == 0)
            {
               
                continue;
               // throw new Exception("Ooops");
            }
            Console.WriteLine(data);
            await receiver.CompleteMessageAsync(msg);
            //await receiver.AbandonMessageAsync(msg);
            //await receiver.RenewMessageLockAsync(msg);
            await Task.Delay(1000);
        }
        while (true);
    }
    private static async Task ReadQueueProcessorAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyReader.Name, SasKeyReader.KeY);
        var client = new ServiceBusClient(EndPoint, cred);
        //var receiver = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions { AutoCompleteMessages=false});
        var opt = new ServiceBusSessionProcessorOptions
        {
            AutoCompleteMessages = false
        };
        opt.SessionIds.Add("me0");

        var receiver = client.CreateSessionProcessor(QueueName, opt);
            
        
        int i = 0;
        receiver.ProcessMessageAsync += async evtArg => {
           // evtArg.Message.LockedUntil = DateTimeOffset.Now.AddSeconds(20);
            var msg = evtArg.Message;
            Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken} (Session: {msg.SessionId})");
            var data = msg.Body.ToString();
            if (++i % 5 == 0)
                return;
                //throw new Exception("Ooops");
            Console.WriteLine(data);
            await evtArg.CompleteMessageAsync(msg);
           
        };

        receiver.ProcessErrorAsync += evtArg => {
            Console.WriteLine("Ook Ooops");
            Console.WriteLine(evtArg.Exception.Message);
            return Task.CompletedTask;
        };

        await receiver.StartProcessingAsync();
        Console.WriteLine("Press Enter to quit processing");
        Console.ReadLine();
        await receiver.StopProcessingAsync();

    }
}
