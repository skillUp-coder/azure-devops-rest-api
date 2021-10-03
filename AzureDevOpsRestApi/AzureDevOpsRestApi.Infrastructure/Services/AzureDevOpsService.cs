using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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

        public async Task<GitPush> PushAsync(GitModel gitModel)
        {
            using var gitClient = new GitHttpClient(new Uri(_connectionUri), _credential);
            var repository = await gitClient.GetRepositoryAsync(
                AzureDevOpsRestApiResource.Project,
                AzureDevOpsRestApiResource.Repository);

            gitModel.GitRef = await GitProvider.GetBranchesAsync(gitClient, repository, null);
            var createBranch = GitProvider.CreateBranch(gitModel);
            var createCommit = GitProvider.CreateCommit(gitModel, VersionControlChangeType.Add);
            var createGitPush = GitProvider.CreateGitPush(createBranch, createCommit);
            var createPushAsync = await gitClient.CreatePushAsync(createGitPush, repository.Id);

            return createPushAsync;
        }

        public async Task<GitItem> GetAsync(GitModel gitModel)
        {
            using var gitClient = new GitHttpClient(new Uri(_connectionUri), _credential);
            var getRepositoryAsync = await gitClient.GetRepositoryAsync(
                AzureDevOpsRestApiResource.Project,
                AzureDevOpsRestApiResource.Repository);

            var getNameAsync = await GitProvider.GetNameAsync(
                gitModel,
                getRepositoryAsync,
                gitClient,
                GitProvider.GetGitVersionDescriptor(gitModel));

            return await gitClient.GetItemAsync(
                getRepositoryAsync.Id,
                getNameAsync,
                includeContent: true,
                versionDescriptor: GitProvider.GetGitVersionDescriptor(gitModel));
        }

        public async Task<HttpResponseMessage> RunPipeline(
            CancellationToken cancellationToken = default)
        {
            using var gitClient = new GitHttpClient(new Uri(_connectionUri), _credential);
            using var httpClient = new HttpClient();
            
            var getRepositoryAsync = await gitClient.GetRepositoryAsync(
                AzureDevOpsRestApiResource.Project,
                AzureDevOpsRestApiResource.Repository, 
                cancellationToken: cancellationToken);

            const string mediaType = "application/json";
            const string scheme = "Basic";
            
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{AzureDevOpsRestApiResource.AccessToken}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

            var bodyContent = new StringContent(GitProvider.RunPipeLineRequestBody(getRepositoryAsync.Id), Encoding.UTF8, mediaType);

            var response = await httpClient.PostAsync(AzureDevOpsRestApiResource.RunPipelineTemplate, bodyContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}