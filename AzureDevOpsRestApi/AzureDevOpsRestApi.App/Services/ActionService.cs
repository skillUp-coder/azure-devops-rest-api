using System;
using System.Threading.Tasks;
using AzureDevOpsRestApi.App.Enums;
using AzureDevOpsRestApi.Infrastructure.Models;
using AzureDevOpsRestApi.Infrastructure.Services;

namespace AzureDevOpsRestApi.App.Services
{
    public class ActionService
    {
        private readonly AzureDevOpsService _azureDevOpsService;
        private readonly ActionModel _actionModel;

        public ActionService(
            AzureDevOpsService azureDevOpsService, 
            ActionModel actionModel)
        {
            this._azureDevOpsService = azureDevOpsService;
            this._actionModel = actionModel;
        }

        public async Task ActionConsole()
        {
            Console.WriteLine("Choose actions:");
            foreach (var changeType in Enum.GetNames(typeof(GitActionTypes)))
            {
                Console.WriteLine($"{changeType}");
            }
            
            var chooseAction = Console.ReadLine();
            
            Console.Write("Enter commit: ");
            _actionModel.Commit = Console.ReadLine();

            Console.Write("Enter branch name:");
            _actionModel.Branch = Console.ReadLine();

            if (string.IsNullOrEmpty(_actionModel.Commit) && 
                string.IsNullOrEmpty(_actionModel.Branch))
            {
                Console.WriteLine("Incorrect commit or branch name!");
            }
            else
            {
                var chooseActionParse = Enum.Parse(typeof(GitActionTypes), chooseAction!, true);
                switch (chooseActionParse)
                {
                    case GitActionTypes.Push:
                        await _azureDevOpsService.PushAsync(_actionModel);
                        break;
                    case GitActionTypes.Get:
                        var get = await _azureDevOpsService.GetAsync(_actionModel);
                        Console.WriteLine($"Content: {get.Content}");
                        break;
                }
            }
        }
    }
}