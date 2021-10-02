using AzureDevOpsRestApi.Infrastructure.Enums;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsRestApi.Infrastructure.Models
{
    public class GitModel
    {
        public string Commit { get; set; }
        
        public string Branch { get; set; }
        
        public string Content { get; set; }
        
        public ContentType ContentType { get; set; }
        
        public GitRef GitRef { get; set; }
    }
}