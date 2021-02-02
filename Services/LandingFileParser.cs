using Landing.API.Models;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class LandingFileParser
    {
        private readonly Dictionary<string, Func<ProjectInfo, MarkdownBlock, ValueTask>> handlers;


        private readonly string repoFullName;
        private readonly string defaultBranch;
        public LandingFileParser(string repoName, string defaultBranch)
        {
            this.repoFullName = repoName;
            this.defaultBranch = defaultBranch;
            handlers = new Dictionary<string, Func<ProjectInfo, MarkdownBlock, ValueTask>>
            {
                { " Title", async (i, b) => i.Title = await ParseTextParagraph(b) },
                { " Description", HandleDescription },
                { " Images", HandleImagesAsync },
                { " Videos", async (i, b) => i.Videos = await ParseVideos(b) },
                { " Tech",async  (i, b) => i.Tech = await ParseStringListOrParagraph(b) },
                { " Tags", async (i, b) => i.Tags = await ParseStringListOrParagraph(b) },
                { " Developers", async (i, b) => i.Developers = await ParseStringListOrParagraph(b) },
                { " Site", async (i, b) => i.Site = await ParseTextParagraph(b) },
                { " SourceCode", HandleSourceCode },
            };
        }


        public async ValueTask<ProjectInfo> ParseAsync(string markdown)
        {
            MarkdownDocument md = new MarkdownDocument();
            md.Parse(markdown);
            var info = new ProjectInfo();
            for (int i = 0; i < md.Blocks.Count; i++)
            {
                var block = md.Blocks[i];
                if (block is HeaderBlock header)
                {
                    var hContent = header.ReadAllInline();
                    var nextBlock = md.Blocks[++i];
                    if (nextBlock is HorizontalRuleBlock)
                    {
                        continue;
                    }
                    if (handlers.ContainsKey(hContent))
                    {
                        try
                        {
                            await handlers[hContent](info, nextBlock);
                        }
                        catch (ParsingException ex)
                        {
                            throw new ParsingException($"Can't handle {hContent} item", ex);
                        }
                    }
                }
            }
            return info;
        }

        private static ValueTask HandleDescription(ProjectInfo info, MarkdownBlock block)
        {
            if (block is ParagraphBlock paragraph)
            {
                info.Description = paragraph.ReadAllInline().Trim();
            }
            if (block is CodeBlock code)
            {
                info.Description = code.Text;
            }
            return default;
        }
        private static ValueTask HandleSourceCode(ProjectInfo info, MarkdownBlock block)
        {
            if (block is TableBlock table)
            {
                info.SourceCode = table.Rows.Skip(1)
                    .Select(r => (name: r.Cells[0].ReadAllInline(), link: r.Cells[1].ReadAllInline()))
                    .Select(p => new SourceCodeLink { Name = p.name, Link = p.link })
                    .ToArray();
            }
            return default;
        }

        private static ValueTask<string> ParseTextParagraph(MarkdownBlock block)
        {
            if (block is ParagraphBlock paragraph)
            {
                return new ValueTask<string>(paragraph.ReadAllInline().Trim());
            }
            throw new ParsingException($"block is {block.Type} but must be {MarkdownBlockType.Paragraph}");
        }

        private ValueTask HandleImagesAsync(ProjectInfo info, MarkdownBlock block)
        {
            if (block is ListBlock list)
            {
                info.Images = list.Items.Select(i =>
                    ((i.Blocks.Single() as ParagraphBlock)
                    .Inlines.Single() as ImageInline)
                    .RenderUrl).ToArray();
            }
            for (int i = 0; i < info.Images.Length; i++)
            {
                if (!info.Images[i].StartsWith("http"))
                {
                    var rawContent = $"https://github.com/{repoFullName}/raw/{defaultBranch}/{info.Images[i].TrimStart('/')}";
                    info.Images[i] = rawContent;
                }
            }
            return default;
        }

        private static ValueTask<string[]> ParseStringList(MarkdownBlock block)
        {
            if (block is ListBlock list)
            {
                return new ValueTask<string[]>(
                    list.Items.Select(i =>
                    (i.Blocks.Single() as ParagraphBlock).ReadAllInline())
                    .ToArray());
            }
            throw new ParsingException($"block is {block.Type} but must be {MarkdownBlockType.List}");
        }

        private static async ValueTask<string[]> ParseStringListOrParagraph(MarkdownBlock block)
        {
            try
            {
                return await ParseStringList(block);
            }
            catch (ParsingException)
            {
                return new string[] { await ParseTextParagraph(block) };
            }
        }
        private static readonly Regex youtubeRegex = new Regex("https://youtu.be/(?<id>[^/]+)");
        private static async ValueTask<string[]> ParseVideos(MarkdownBlock block)
        {
            var videos = await ParseStringListOrParagraph(block);
            for (int i = 0; i < videos.Length; i++)
            {
                var match = youtubeRegex.Match(videos[i]);
                if (match.Success)
                {
                    videos[i] = $"https://www.youtube.com/embed/{match.Groups["id"].Value}";
                }
            }
            return videos;
        }
    }
}
