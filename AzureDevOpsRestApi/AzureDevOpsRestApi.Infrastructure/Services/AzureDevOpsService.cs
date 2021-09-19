using System;
using System.Threading.Tasks;
using AzureDevOpsRestApi.Infrastructure.Models;
using AzureDevOpsRestApi.Infrastructure.Providers;
using AzureDevOpsRestApi.Infrastructure.Resource.AzureDevOpsRestApiConfiguration;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace AzureDevOpsRestApi.Infrastructure.Services
{
    public class AzureDevOpsService
    {
        private readonly VssBasicCredential _credential;
        private readonly string _connectionUri;
        
        public AzureDevOpsService()
        {
            _credential = new VssBasicCredential(string.Empty, AzureDevOpsRestApiResource.AccessToken);
            _connectionUri = $"{AzureDevOpsRestApiResource.Uri}/{AzureDevOpsRestApiResource.Organization}";
        }
        
        public async Task<GitPush> PushAsync(ActionModel actionModel)
        {
            using var gitClient = new GitHttpClient(new Uri(_connectionUri), _credential);
            var repository = await gitClient.GetRepositoryAsync(
                AzureDevOpsRestApiResource.Project, 
                AzureDevOpsRestApiResource.Repository);
            actionModel.GitRef = await GitProvider.GetBranchesAsync(
                gitClient, 
                repository,
                null);
            var createBranch = GitProvider.CreateBranch(actionModel);
            var createCommit = GitProvider.CreateCommit(
                actionModel, 
                VersionControlChangeType.Add);
            var createGitPush = GitProvider.CreateGitPush(
                createBranch, 
                createCommit);
            var createPushAsync = await gitClient.CreatePushAsync(
                createGitPush, 
                repository.Id);
            
            return createPushAsync;
        }
        
        public async Task<GitItem> GetAsync(ActionModel actionModel)
        {
            using var gitClient = new GitHttpClient(new Uri(_connectionUri), _credential);
            var getRepositoryAsync = await gitClient.GetRepositoryAsync(
                AzureDevOpsRestApiResource.Project, 
                AzureDevOpsRestApiResource.Repository);
            
            var gitVersionDescriptor = new GitVersionDescriptor()
            {
                VersionType = GitVersionType.Branch,
                Version = actionModel.Branch,
                VersionOptions = GitVersionOptions.None
            };
            var getNameAsync = await GitProvider.GetNameAsync(
                actionModel, 
                getRepositoryAsync, 
                gitClient, 
                gitVersionDescriptor);

            return await gitClient.GetItemAsync(
                getRepositoryAsync.Id, 
                getNameAsync, 
                includeContent: true, 
                versionDescriptor: gitVersionDescriptor);
        }
    }
}