using Landing.API.Models;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public class LandingFileParser
    {
        private static readonly Dictionary<string, Action<ProjectInfo, MarkdownBlock>> handlers = new Dictionary<string, Action<ProjectInfo, MarkdownBlock>>
        {
            { " Title", (i, b) => i.Title = ParseTextParagraph(b) },
            { " Description", HandleDescription },
            { " Images", HandleImages },
            { " Videos", (i, b) => i.Videos = ParseStringList(b) },
            { " Tech", (i, b) => i.Tech= ParseStringList(b) },
            { " Tags", (i, b) => i.Tags= ParseStringList(b) },
            { " Developers", (i, b) => i.Developers= ParseStringList(b) },
            { " Site", (i, b) => i.Site= ParseTextParagraph(b) },
            { " SourceCode", HandleSourceCode },
        };



        public static ProjectInfo Parse(string markdown)
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
                        handlers[hContent](info, nextBlock);
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
            throw new Exception($"block is {block.Type} but must be {MarkdownBlockType.Paragraph}");
        }

        private static void HandleImages(ProjectInfo info, MarkdownBlock block)
        {
            if (block is ListBlock list)
            {
                info.Images = list.Items.Select(i =>
                    ((i.Blocks.Single() as ParagraphBlock)
                    .Inlines.Single() as ImageInline)
                    .RenderUrl).ToArray();
            }
        }

        private static string[] ParseStringList(MarkdownBlock block)
        {
            if (block is ListBlock list)
            {
                return list.Items.Select(i =>
                    (i.Blocks.Single() as ParagraphBlock).ReadAllInline()).ToArray();
            }
            throw new Exception($"block is {block.Type} but must be {MarkdownBlockType.List}");
        }
    }
}
