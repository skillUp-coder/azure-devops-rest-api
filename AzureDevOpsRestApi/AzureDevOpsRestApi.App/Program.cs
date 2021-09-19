using System;
using System.Threading.Tasks;
using AzureDevOpsRestApi.App.Services;
using AzureDevOpsRestApi.Infrastructure.Enums;
using AzureDevOpsRestApi.Infrastructure.Models;
using AzureDevOpsRestApi.Infrastructure.Services;

namespace AzureDevOpsRestApi.App
{
    internal static class Program
    {
        private static async Task Main()
        {
            var azureDevOpsClient = new AzureDevOpsService();
            var action = new ActionModel
            { 
                Content = "hello!",
                ContentType = ContentType.Txt
                    
            };
            var actionService = new ActionService(azureDevOpsClient, action);

            await actionService.ActionConsole();

            Console.ReadLine();
        }
    }
}