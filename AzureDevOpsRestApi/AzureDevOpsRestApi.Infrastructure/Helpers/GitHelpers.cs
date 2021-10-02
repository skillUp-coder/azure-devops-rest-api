using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsRestApi.Infrastructure.Helpers
{
    public static class GitHelpers
    {
        private const int ZeroStartIndex = 0;
        private const string Reference = "refs/";
        
        public static GitRef GetObjectFromList(this IEnumerable<GitRef> enumerable)
        {
            return enumerable?.FirstOrDefault();
        }

        public static string WithoutReferencePrefix(this string stringReference)
        {
            return stringReference.Remove(ZeroStartIndex, Reference.Length);
        }
    }
}