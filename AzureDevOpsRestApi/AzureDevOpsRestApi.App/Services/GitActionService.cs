using System;
using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsRestApi.App.Enums;
using AzureDevOpsRestApi.Infrastructure.Models;
using AzureDevOpsRestApi.Infrastructure.Services;

namespace AzureDevOpsRestApi.App.Services
{
    public class GitActionService
    {
        private readonly AzureDevOpsService _azureDevOpsService;
        private readonly GitModel _gitModel;

        public GitActionService(
            AzureDevOpsService azureDevOpsService, 
            GitModel gitModel)
        {
            this._azureDevOpsService = azureDevOpsService;
            this._gitModel = gitModel;
        }

        public async Task ActionConsole()
        {
            Console.Write("Enter action: ");
            var chooseAction = Console.ReadLine();
            var actionTypeParse = Enum.Parse(typeof(GitActionTypes), chooseAction!, true);
            var isNotTypeRunPipeline = actionTypeParse.ToString() != GitActionTypes.RunPipeline.ToString();
            
            if (isNotTypeRunPipeline)
            {
                Console.Write("Enter commit: ");
                _gitModel.Commit = Console.ReadLine();

                Console.Write("Enter branch name:");
                _gitModel.Branch = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(_gitModel.Commit) && 
                string.IsNullOrEmpty(_gitModel.Branch) &&
                isNotTypeRunPipeline)
            {
                Console.WriteLine("Incorrect commit or branch name!");
            }
            else
            {
                switch (actionTypeParse)
                {
                    case GitActionTypes.Push:
                        await PushAsync();
                        break;
                    case GitActionTypes.Get:
                        await GetAsync();
                        break;
                    case GitActionTypes.RunPipeline:
                        await RunPipeline();
                        break;
                }
            }
        }

        private async Task GetAsync()
        {
            var getAsync = await _azureDevOpsService.GetAsync(_gitModel);
            
            Console.WriteLine($"Object ID: {getAsync.ObjectId}");
            Console.WriteLine($"Path: {getAsync.Path}");
            Console.WriteLine($"URL: {getAsync.Url}");
            Console.WriteLine($"Content: {getAsync.Content}");
        }

        private async Task PushAsync()
        {
            var pushAsync = await _azureDevOpsService.PushAsync(_gitModel);
            
            Console.WriteLine($"Repository: {pushAsync.Repository.Name}");
            Console.WriteLine($"Object ID: {pushAsync.RefUpdates.FirstOrDefault()?.NewObjectId}");
            Console.WriteLine($"Comment: {pushAsync.Commits.FirstOrDefault()?.Comment}");
        }

        private async Task RunPipeline()
        {
            var runPipeline = await _azureDevOpsService.RunPipeline();

            Console.WriteLine($"StatusCode: {runPipeline.StatusCode}");
        }
    }
}