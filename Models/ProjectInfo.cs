using System;
using System.Collections.Generic;
using System.Linq;

namespace Landing.API.Models
{

    public class ProjectInfo
    {
        public string CommitSha { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Images { get; set; }
        public string[] Videos { get; set; }
        public string Date { get; set; }
        public string[] Tags { get; set; }
        public string[] Tech { get; set; }
        public string[] Developers { get; set; }
        public string Site { get; set; }
        public SourceCodeLink[] SourceCode { get; set; }

        //TODO: Save date in DateTimeOffset instead string
        [Obsolete("Save date in DateTimeOffset instead string")]
        public void SetDate(DateTimeOffset date)
        {
            Date = date.ToString("dd/MM/yyyy");
        }

        public bool EqualsWithoutCommitAndDate(ProjectInfo info)
        {
            return Title == info.Title &&
                   Description == info.Description &&
                   Site == info.Site &&
                   CompareSequences(Images, info.Images) &&
                   CompareSequences(Videos, info.Videos) &&
                   CompareSequences(Tags, info.Tags) &&
                   CompareSequences(Tech, info.Tech) &&
                   CompareSequences(Developers, info.Developers) &&
                   CompareSequences(SourceCode, info.SourceCode);
        }
        private bool CompareSequences<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null && second == null)
            {
                return true;
            }
            // One is null
            if (first == null ^ second == null)
            {
                return false;
            }
            return first.SequenceEqual(second);
        }
    }
    public class SourceCodeLink
    {
        public string Name { get; set; }
        public string Link { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SourceCodeLink link &&
                   Name == link.Name &&
                   Link == link.Link;
        }

        public override int GetHashCode()
        {
            int hashCode = -1372958849;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Link);
            return hashCode;
        }
    }

}
