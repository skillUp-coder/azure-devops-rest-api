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
            
            var actionService = new GitActionService(azureDevOpsClient, GetGitModel());

            await actionService.ActionConsole();

            Console.ReadLine();
        }

        private static GitModel GetGitModel()
        {
            return new GitModel
            { 
                Content = "hello!",
                ContentType = ContentType.Txt
                    
            };
        }
    }
}