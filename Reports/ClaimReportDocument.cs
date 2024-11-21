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

        public ClaimReportDocument(List<ClaimReportData> claims, DateTime startDate, DateTime endDate)
        {
            _claims = claims;
            _startDate = startDate;
            _endDate = endDate;
            _summary = CalculateSummary(claims);
        }

        private ClaimSummary CalculateSummary(List<ClaimReportData> claims)
        {
            var summary = new ClaimSummary
            {
                TotalClaims = claims.Count,
                TotalAmount = claims.Sum(c => c.ClaimAmount)
            };

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

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Background(Colors.Grey.Lighten5)
                .Padding(20)
                .Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("Claims Report")
                            .FontSize(24)
                            .FontColor("#003366") // Your primary color
                            .Bold();

                        column.Item().Text($"Period: {_startDate:yyyy/MM/dd} - {_endDate:yyyy/MM/dd}")
                            .FontSize(14)
                            .FontColor("#87CEEB"); // Your secondary color
                    });

                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                            .FontSize(12)
                            .FontColor("#333333")
                            .AlignRight();
                    });
                });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);

                column.Item().Element(ComposeSummary);
                column.Item().Element(ComposeTable);
            });
        }

        private void ComposeSummary(IContainer container)
        {
            container.Background("#F0F0F0") // Your background color
                .Padding(20)
                .Column(column =>
                {
                    column.Item().Text("Summary")
                        .FontSize(16)
                        .FontColor("#003366")
                        .Bold();

                    column.Spacing(10);

                    column.Item().Text($"Total Claims: {_summary.TotalClaims}")
                        .FontColor("#333333");
                    column.Item().Text($"Total Amount: R {_summary.TotalAmount:N2}")
                        .FontColor("#333333");

                    column.Spacing(5);

                    column.Item().Text("Claims by Status:")
                        .FontColor("#003366")
                        .Bold();

                    foreach (var status in _summary.ClaimsByStatus)
                    {
                        column.Item().Text($"{status.Key}: {status.Value} claims (R {_summary.AmountByStatus[status.Key]:N2})")
                            .FontColor("#333333");
                    }
                });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // Header
                table.Header(header =>
                {
                    foreach (var text in new[] { "Lecturer", "Submission Date", "Hours", "Rate", "Amount", "Status" })
                    {
                        header.Cell().Background("#003366").Padding(10).Column(c =>
                        {
                            c.Item().Text(text)
                                .FontColor(Colors.White)
                                .Bold();
                        });
                    }
                });

                // Content
                foreach (var claim in _claims)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                        .Text(claim.LecturerName).FontColor("#333333");

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                        .Text(claim.SubmissionDate.ToString("yyyy/MM/dd")).FontColor("#333333");

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                        .Text($"{claim.HoursWorked:N1}").FontColor("#333333");

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                        .Text($"R {claim.HourlyRate:N2}").FontColor("#333333");

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                        .Text($"R {claim.ClaimAmount:N2}").FontColor("#333333");

                    var statusColor = claim.Status == "Approved" ? "#28a745" : "#dc3545";
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                        .Text(claim.Status).FontColor(statusColor).Bold();
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Background(Colors.Grey.Lighten5)
                .Padding(10)
                .Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span("Page ").FontColor("#333333");
                        x.CurrentPageNumber().FontColor("#003366");
                        x.Span(" / ").FontColor("#333333");
                        x.TotalPages().FontColor("#003366");
                    });

                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Generated: ").FontColor("#333333");
                        x.Span(DateTime.Now.ToString("g")).FontColor("#003366");
                    });
                });
        }
    }
}