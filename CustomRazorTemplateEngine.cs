using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.Language;
using WebMarkupMin.Core;

namespace RazorMinification
{
    public class CustomRazorTemplateEngine : RazorTemplateEngine
    {
        private HtmlMinifier _htmlMinifier = new HtmlMinifier();

        public CustomRazorTemplateEngine(RazorEngine engine, RazorProject project) : base(engine, project)
        {
            Options.ImportsFileName = "_ViewImports.cshtml";
        }

        public override RazorCodeDocument CreateCodeDocument(RazorProjectItem projectItem)
        {
            if (projectItem == null)
            {
                throw new ArgumentNullException($"{nameof(projectItem)} is null!");
            }

            if (!projectItem.Exists)
            {
                throw new InvalidOperationException($"{nameof(projectItem)} doesn't exist!");
            }

            Console.WriteLine();
            Console.WriteLine($"File: {projectItem.FileName}");

            using (var inputStream = projectItem.Read())
            {
                using (var reader = new StreamReader(inputStream))
                {
                    var text = reader.ReadToEnd();

                    var markupStart = text.IndexOf("<!DOCTYPE");
                    var directives = text.Substring(0, markupStart);
                    var markup = text.Substring(markupStart);

                    text = directives + Minify(markup);

                    var byteArray = Encoding.UTF8.GetBytes(text);
                    var minifiedInputStream = new MemoryStream(byteArray);

                    var source = RazorSourceDocument.ReadFrom(minifiedInputStream, projectItem.PhysicalPath);
                    var imports = GetImports(projectItem);

                    return RazorCodeDocument.Create(source, imports);
                }
            }
        }

        private string Minify(string markup)
        {
            MarkupMinificationResult result = _htmlMinifier.Minify(markup, string.Empty, Encoding.UTF8, true);
            
            if (result.Errors.Count == 0)
            {
                MinificationStatistics statistics = result.Statistics;
                if (statistics != null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Original size: {statistics.OriginalSize:N0} Bytes | Minified size: {statistics.MinifiedSize:N0} Bytes | Saved: {statistics.SavedInPercent:N2}%");
                }
                //Console.WriteLine($"{Environment.NewLine}Minified content:{Environment.NewLine}{Environment.NewLine}{result.MinifiedContent}");

                return result.MinifiedContent;
            }
            else
            {
                IList<MinificationErrorInfo> errors = result.Errors;

                Console.WriteLine();
                Console.WriteLine($"Found {errors.Count:N0} error(s):");

                foreach (var error in errors)
                {
                    Console.WriteLine($" - Line {error.LineNumber}, Column {error.ColumnNumber}: {error.Message}");
                }

                return markup;
            }
        }
    }
}