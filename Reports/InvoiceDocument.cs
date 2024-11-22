using CMCS.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace CMCS.Reports
{
    public class InvoiceDocument : IDocument
    {
        private readonly InvoiceModel _data;

        public InvoiceDocument(InvoiceModel data)
        {
            _data = data;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                // Company Details and Invoice Title
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_data.InstitutionName)
                            .FontSize(20)
                            .FontColor("#003366")
                            .Bold();
                        c.Item().Text(_data.InstitutionAddress)
                            .FontSize(10)
                            .FontColor("#333333");
                        c.Item().Text(_data.InstitutionCity)
                            .FontSize(10)
                            .FontColor("#333333");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().AlignRight().Text("INVOICE")
                            .FontSize(24)
                            .FontColor("#003366")
                            .Bold();
                        c.Item().AlignRight().Text($"Invoice #: {_data.InvoiceNumber}")
                            .FontSize(10);
                        c.Item().AlignRight().Text($"Date: {_data.InvoiceDate:yyyy/MM/dd}")
                            .FontSize(10);
                    });
                });

                // Lecturer Details and Bank Information
                column.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Bill To:").Bold();
                        c.Item().Text($"{_data.Lecturer.FirstName} {_data.Lecturer.LastName}");
                        c.Item().Text(_data.Lecturer.Address);
                        c.Item().Text(_data.Lecturer.PhoneNumber);
                        c.Item().Text(_data.Lecturer.UserEmail);
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Bank Details:").Bold();
                        c.Item().Text($"Bank: {_data.Lecturer.BankName}");
                        c.Item().Text($"Branch Code: {_data.Lecturer.BranchCode}");
                        c.Item().Text($"Account: {_data.Lecturer.BankAccountNumber}");
                    });
                });

                column.Item().PaddingTop(20).BorderBottom(1).BorderColor("#003366")
                    .Text($"Period: {_data.StartDate:yyyy/MM/dd} - {_data.EndDate:yyyy/MM/dd}")
                    .FontSize(10);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);
                column.Item().Element(ComposeTable);
                column.Item().Element(ComposeSummary);
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);    // Date
                    columns.RelativeColumn(1.5f); // Hours
                    columns.RelativeColumn(1.5f); // Rate
                    columns.RelativeColumn(2);    // Amount
                });

                // Add Header
                table.Header(header =>
                {
                    foreach (var text in new[] { "Date", "Hours", "Rate", "Amount" })
                    {
                        header.Cell().Background("#003366").Padding(10).Column(c =>
                        {
                            c.Item().Text(text)
                                .FontColor(Colors.White)
                                .Bold();
                        });
                    }
                });

                // Add Rows
                foreach (var claim in _data.Claims)
                {
                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(10)
                        .Text(claim.SubmissionDate.ToString("yyyy/MM/dd"));

                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(10)
                        .AlignRight().Text($"{claim.HoursWorked:N1}");

                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(10)
                        .AlignRight().Text($"R {claim.HourlyRate:N2}");

                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(10)
                        .AlignRight().Text($"R {claim.ClaimAmount:N2}");
                }
            });
        }

        private void ComposeSummary(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignRight().Text($"Total Amount: R {_data.TotalAmount:N2}")
                    .FontSize(12)
                    .Bold();

                column.Item().PaddingTop(10).Text(_data.PaymentTerms)
                    .FontSize(9)
                    .FontColor("#666666");
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        }
    }
}