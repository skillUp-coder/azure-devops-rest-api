using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsRestApi.Infrastructure.Enums;
using AzureDevOpsRestApi.Infrastructure.Helpers;
using AzureDevOpsRestApi.Infrastructure.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using GitConstants = AzureDevOpsRestApi.Infrastructure.Constants.GitConstants;

namespace AzureDevOpsRestApi.Infrastructure.Providers
{
    public static class GitProvider
    {
        public static GitPush CreateGitPush(GitRefUpdate gitRefUpdate, GitCommitRef gitCommitRef)
        {
            return new GitPush
            {
                RefUpdates = new List<GitRefUpdate>
                {
                    gitRefUpdate
                },
                Commits = new List<GitCommitRef>
                {
                    gitCommitRef
                }
            };
        }

        public static async Task<GitRef> GetBranchesAsync(
            GitHttpClient gitClient,
            GitRepository repository,
            string nameBranch)
        {
            var gitRef = (string.IsNullOrEmpty(nameBranch))
                ? await gitClient.GetRefsAsync(
                    repository.Id,
                    filter: repository.DefaultBranch.WithoutReferencePrefix())
                : gitClient.GetRefsAsync(
                    repository.Id).Result.Where(gitRef => gitRef.Name == $"refs/heads/{nameBranch}")!;

            return gitRef.GetObjectFromList();
        }

        public static async Task<string> GetNameAsync(
            GitModel gitModel,
            GitRepository repository,
            GitHttpClient gitClient,
            GitVersionDescriptor versionDescriptor)
        {
            var componentType = Enum.GetName(typeof(ContentType), gitModel.ContentType);
            var scopePath = $"/{gitModel.Commit}.{componentType}";
            
            var getItemsAsync = await gitClient.GetItemsAsync
                (repository.Id, scopePath, VersionControlRecursionType.OneLevel, versionDescriptor: versionDescriptor);

            return getItemsAsync.FirstOrDefault()?.Path!;
        }

        public static GitCommitRef CreateCommit(
            GitModel gitModel,
            VersionControlChangeType versionControlChangeType)
        {
            var itemType = Enum.GetName(gitModel.ContentType)?.ToLower();

            var itemPath = $"/{gitModel.Commit}.{itemType}";

            var gitChanges = new List<GitChange>
            {
                new GitChange
                {
                    ChangeType = versionControlChangeType,
                    Item = new GitItem
                    {
                        Path = itemPath
                    },
                    NewContent = new ItemContent
                    {
                        Content = gitModel.Content,
                        ContentType = ItemContentType.RawText
                    }
                }
            };

            return new GitCommitRef
            {
                Comment = gitModel.Commit,
                Changes = gitChanges
            };
        }

        public static GitVersionDescriptor GetGitVersionDescriptor(GitModel gitModel)
        {
            return new GitVersionDescriptor()
            {
                VersionType = GitVersionType.Branch,
                Version = gitModel.Branch,
                VersionOptions = GitVersionOptions.None
            };
        }

        public static GitRefUpdate CreateBranch(GitModel git)
        {
            return new GitRefUpdate
            {
                Name = $"{GitConstants.Branches.BranchReference}{git.Branch}",
                OldObjectId = git.GitRef.ObjectId
            };
        }
    }
}