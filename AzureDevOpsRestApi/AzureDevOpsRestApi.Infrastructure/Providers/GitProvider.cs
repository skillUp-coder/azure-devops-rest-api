using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsRestApi.Infrastructure.Constants;
using AzureDevOpsRestApi.Infrastructure.Enums;
using AzureDevOpsRestApi.Infrastructure.Helpers;
using AzureDevOpsRestApi.Infrastructure.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;

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
            ActionModel actionModel,
            GitRepository repository,
            GitHttpClient gitClient,
            GitVersionDescriptor versionDescriptor)
        {
            var componentType = Enum.GetName(typeof(ContentType), actionModel.ContentType);
            var scopePath = $"/{actionModel.Commit}.{componentType}";
            var references = await gitClient.GetItemsAsync(
                repository.Id,
                scopePath,
                VersionControlRecursionType.OneLevel,
                versionDescriptor: versionDescriptor);

            return references.FirstOrDefault()?.Path!;
        }

        public static GitCommitRef CreateCommit(
            ActionModel actionModel,
            VersionControlChangeType versionControlChangeType)
        {
            var itemType = Enum.GetName(actionModel.ContentType);

            var itemPath = $"/{actionModel.Commit}.{itemType}";

            var gitChanges = new List<GitChange>
            {
                new GitChange
                {
                    ChangeType = versionControlChangeType,
                    Item = new GitItem
                    {
                        Path = itemPath
                    },
                    NewContent = new ItemContent()
                    {
                        Content = actionModel.Content,
                        ContentType = ItemContentType.RawText
                    }
                }
            };

            return new GitCommitRef
            {
                Comment = actionModel.Commit,
                Changes = gitChanges
            };
        }

        public static GitRefUpdate CreateBranch(ActionModel action)
        {
            return new GitRefUpdate
            {
                Name = $"{ApplicationConstants.Branches.BranchReference}{action.Branch}",
                OldObjectId = action.GitRef.ObjectId
            };
        }
    }
}