using System;
using WkHtmlToPdfDotNet;

namespace DocumentArchiver.Rendering
{
    public class WkHtmlToPdfRenderer
    {
        private readonly BasicConverter converter = new BasicConverter(new PdfTools());

        public byte[] ConvertHtmlToPdf(string html, string source)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 }
                },

                Objects =
                    {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = html,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            FooterSettings = { FontSize = 9,
                                               Left = $"Archived from {source} {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}",
                                               Right = "Page [page] of [toPage]",
                                               Line = false,
                                               Spacing = 2.812 }
                    }
                }
            };

            return converter.Convert(doc);
        }
    }
}