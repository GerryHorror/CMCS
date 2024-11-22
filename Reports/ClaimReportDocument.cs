using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using static CMCS.Models.ReportModel;

namespace CMCS.Reports
{
    public class ClaimReportDocument : IDocument
    {
        private readonly List<ClaimReportData> _claims;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private readonly ClaimSummary _summary;

        // This constructor is used to initialise the ClaimReportDocument with the claims, start date, and end date.
        public ClaimReportDocument(List<ClaimReportData> claims, DateTime startDate, DateTime endDate)
        {
            _claims = claims;
            _startDate = startDate;
            _endDate = endDate;
            _summary = CalculateSummary(claims);
        }

        // This method is used to calculate the summary of the claims and return the summary.
        private ClaimSummary CalculateSummary(List<ClaimReportData> claims)
        {
            var summary = new ClaimSummary
            {
                TotalClaims = claims.Count,
                TotalAmount = claims.Sum(c => c.ClaimAmount)
            };

            // Group claims by status and calculate total amount for each status
            foreach (var claim in claims)
            {
                if (summary.ClaimsByStatus.ContainsKey(claim.Status))
                    summary.ClaimsByStatus[claim.Status]++;
                else
                    summary.ClaimsByStatus[claim.Status] = 1;

                if (summary.AmountByStatus.ContainsKey(claim.Status))
                    summary.AmountByStatus[claim.Status] += claim.ClaimAmount;
                else
                    summary.AmountByStatus[claim.Status] = claim.ClaimAmount;
            }

            return summary;
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to get the metadata of the document and return the default metadata of the document.
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // This method is used to compose the document with the header, content, and footer.
        public void Compose(IDocumentContainer container)
        {
            // Compose the document with the header, content, and footer
            container.Page(page =>
            {
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(8));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the header of the document. The header of the document will contain the title and period of the report.
        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                // Title section
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Claims Report")
                        .FontFamily("Arial")
                        .FontSize(20)
                        .FontColor("#003366")
                        .Bold();

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontColor("#333333");
                });

                // Period section
                column.Item().PaddingTop(10).Text($"Period: {_startDate:yyyy/MM/dd} - {_endDate:yyyy/MM/dd}")
                    .FontSize(14)
                    .FontColor("#87CEEB");

                column.Item().BorderBottom(1).BorderColor("#003366").PaddingTop(5);
            });
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the content of the document. The content of the document will contain the summary and table of the claims.
        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(30);
                column.Item().Element(ComposeSummary);
                column.Item().Element(ComposeTable);
            });
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the summary section of the document. The summary section will contain the total claims, total amount, claims by status, and amount by status.
        private void ComposeSummary(IContainer container)
        {
            container.Background("#F8F9FA")
                .Border(1)
                .BorderColor("#DEE2E6")
                .Padding(30)
                .Column(column =>
                {
                    column.Spacing(15);
                    column.Item().Text("Summary")
                        .FontSize(20)
                        .FontColor("#003366")
                        .Bold();

                    column.Item().Column(c =>
                    {
                        c.Spacing(8);
                        c.Item().Text($"Total Claims: {_summary.TotalClaims}")
                            .FontSize(12);
                        c.Item().Text($"Total Amount: R {_summary.TotalAmount:N2}")
                            .FontSize(12);
                    });

                    column.Item().Text("Claims by Status:")
                        .FontSize(14)
                        .FontColor("#003366")
                        .Bold();

                    foreach (var status in _summary.ClaimsByStatus)
                    {
                        column.Item().Text($"{status.Key}: {status.Value} claims (R {_summary.AmountByStatus[status.Key]:N2})")
                            .FontSize(12);
                    }
                });
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the table with the claims data. The table will contain the columns for Lecturer, Date, Hours, Rate, Amount, and Status. The table will also contain the header row and the data rows.
        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);    // Lecturer
                    columns.RelativeColumn(2);    // Date
                    columns.RelativeColumn(1.5f);    // Hours
                    columns.RelativeColumn(1.5f); // Rate
                    columns.RelativeColumn(2);    // Amount
                    columns.RelativeColumn(1.5f); // Status
                });

                // Header
                table.Header(header =>
                {
                    foreach (var (text, width) in new[]
                    {
                    ("Lecturer", 2f),
                    ("Submission Date", 2f),
                    ("Hours", 1.5f),
                    ("Rate", 1.5f),
                    ("Amount", 2f),
                    ("Status", 1.5f)
                })
                    {
                        header.Cell().Background("#003366").Padding(15).AlignLeft().Column(c =>
                        {
                            c.Item().Text(text)
                                .FontColor(Colors.White)
                                .FontSize(10)
                                .Bold();
                        });
                    }
                });

                // Content
                foreach (var claim in _claims)
                {
                    var cells = new[]
                    {
                    claim.LecturerName,
                    claim.SubmissionDate.ToString("yyyy/MM/dd"),
                    $"{claim.HoursWorked:N1}",
                    $"R {claim.HourlyRate:N2}",
                    $"R {claim.ClaimAmount:N2}",
                    claim.Status
                };

                    foreach (var (text, index) in cells.Select((t, i) => (t, i)))
                    {
                        if (index == cells.Length - 1) // Status column
                        {
                            var statusColor = text == "Approved" ? "#28a745" : "#dc3545";
                            table.Cell().Border(1).BorderColor("#dee2e6").Padding(12)
                                .Text(text)
                                .FontColor(statusColor)
                                .Bold();
                        }
                        else
                        {
                            table.Cell().Border(1).BorderColor("#dee2e6").Padding(12)
                                .Text(text);
                        }
                    }
                }
            });
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the footer of the document. The footer of the document will contain the page number and the generated date.
        private void ComposeFooter(IContainer container)
        {
            container.BorderTop(1).BorderColor("#dee2e6").PaddingTop(10)
                .Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });

                    row.RelativeItem().AlignRight().Text($"Generated: {DateTime.Now:yyyy/MM/dd HH:mm}")
                        .FontColor("#666666");
                });
        }
    }
}