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

        public bool EqualsWithoutCommitAndDate(ProjectInfo info)
        {
            bool current = Title == info.Title;
            current = Description == info.Description;
            current = Images.SequenceEqual(info.Images);
            current = Videos.SequenceEqual(info.Videos);
            current = Tags.SequenceEqual(info.Tags);
            current = Tech.SequenceEqual(info.Tech);
            current = Developers.SequenceEqual(info.Developers);
            current = Site == info.Site;
            current = SourceCode.SequenceEqual(info.SourceCode);
            return current;
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
