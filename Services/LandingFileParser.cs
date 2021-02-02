﻿using Landing.API.Models;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class LandingFileParser
    {
        private readonly Dictionary<string, Action<ProjectInfo, MarkdownBlock>> handlers;


        private readonly string repoFullName;
        private readonly string defaultBranch;

        public LandingFileParser(string repoName, string defaultBranch)
        {
            this.repoFullName = repoName;
            this.defaultBranch = defaultBranch;
            handlers = new Dictionary<string, Action<ProjectInfo, MarkdownBlock>>
            {
                { " Title", (i, b) => i.Title = ParseTextParagraph(b) },
                { " Description", HandleDescription },
                { " Images", HandleImages },
                { " Videos", (i, b) => i.Videos = ParseVideos(b) },
                { " Tech", (i, b) => i.Tech = ParseStringListOrParagraph(b) },
                { " Tags", (i, b) => i.Tags = ParseStringListOrParagraph(b) },
                { " Developers", (i, b) => i.Developers = ParseStringListOrParagraph(b) },
                { " Site", (i, b) => i.Site = ParseTextParagraph(b) },
                { " SourceCode", HandleSourceCode },
            };
        }


        public ProjectInfo Parse(string markdown)
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

                            handlers[hContent](info, nextBlock);
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

        private static void HandleDescription(ProjectInfo info, MarkdownBlock block)
        {
            if (block is ParagraphBlock paragraph)
            {
                info.Description = paragraph.ReadAllInline().Trim();
            }
            if (block is CodeBlock code)
            {
                info.Description = code.Text;
            }
        }
        private static void HandleSourceCode(ProjectInfo info, MarkdownBlock block)
        {
            if (block is TableBlock table)
            {
                info.SourceCode = table.Rows.Skip(1)
                    .Select(r => (name: r.Cells[0].ReadAllInline(), link: r.Cells[1].ReadAllInline()))
                    .Select(p => new SourceCodeLink { Name = p.name, Link = p.link })
                    .ToArray();
            }
        }

        private static string ParseTextParagraph(MarkdownBlock block)
        {
            if (block is ParagraphBlock paragraph)
            {
                return paragraph.ReadAllInline().Trim();
            }
            throw new ParsingException($"block is {block.Type} but must be {MarkdownBlockType.Paragraph}");
        }

        private void HandleImages(ProjectInfo info, MarkdownBlock block)
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
                    info.Images[i] = $"https://raw.githubusercontent.com/{repoFullName}/{defaultBranch}/{info.Images[i].TrimStart('/')}";
                }
            }
        }

        private static string[] ParseStringList(MarkdownBlock block)
        {
            if (block is ListBlock list)
            {
                return list.Items.Select(i =>
                    (i.Blocks.Single() as ParagraphBlock).ReadAllInline()).ToArray();
            }
            throw new ParsingException($"block is {block.Type} but must be {MarkdownBlockType.List}");
        }

        private static string[] ParseStringListOrParagraph(MarkdownBlock block)
        {
            try
            {
                return ParseStringList(block);
            }
            catch (ParsingException)
            {
                return new string[] { ParseTextParagraph(block) };
            }
        }
        private static readonly Regex youtubeRegex = new Regex("https://youtu.be/(?<id>[^/]+)");
        private static string[] ParseVideos(MarkdownBlock block)
        {
            var videos = ParseStringListOrParagraph(block);
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
