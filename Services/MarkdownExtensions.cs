using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Landing.API.Services
{
    public static class MarkdownExtensions
    {
        public static string ReadAllInline(this HeaderBlock block)
        {
            return string.Join("", block.Inlines.Select(i => i.ToString()));
        }
        public static string ReadAllInline(this ParagraphBlock block)
        {
            return string.Join("", block.Inlines.Select(i => i.ToString()));
        }
        public static string ReadAllInline(this TableBlock.TableCell cell)
        {
            return string.Join("", cell.Inlines.Select(i => i.ToString()));
        }
    }
}
