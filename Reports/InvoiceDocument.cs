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

        // This method is used to get the metadata of the document and return the default metadata of the document.
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

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the header of the document. The header of the document will contain the contractor details, invoice details, bill to section, and bank details section.
        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                // Contractor Details (Left side)
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_data.CompanyName)
                            .FontSize(20)
                            .FontColor("#003366")
                            .Bold();
                        c.Item().Text("Independent Contractor")
                            .FontSize(12)
                            .FontColor("#666666");
                        c.Item().Text(_data.CompanyAddress)
                            .FontSize(10);
                        c.Item().Text($"Tel: {_data.CompanyContact}")
                            .FontSize(10);
                        c.Item().Text($"Email: {_data.CompanyEmail}")
                            .FontSize(10);
                    });

                    // Invoice Details (Right side)
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().AlignRight().Text("INVOICE")
                            .FontSize(24)
                            .FontColor("#003366")
                            .Bold();
                        c.Item().AlignRight().Text($"Invoice #: {_data.InvoiceNumber}")
                            .FontSize(10);
                        c.Item().AlignRight().Text($"Date: {_data.InvoiceDate:dd/MM/yyyy}")
                            .FontSize(10);
                    });
                });

                // Bill To Section
                column.Item().PaddingTop(20).Column(c =>
                {
                    c.Item().Text("Bill To:").Bold();
                    c.Item().Text(_data.BillTo.Name);
                    c.Item().Text(_data.BillTo.Address);
                    c.Item().Text($"{_data.BillTo.City}, {_data.BillTo.PostalCode}");
                    c.Item().Text(_data.BillTo.Country);
                });

                // Bank Details Section
                column.Item().PaddingTop(20).Column(c =>
                {
                    c.Item().Text("Bank Details:").Bold();
                    c.Item().Text($"Bank: {_data.BankDetails.BankName}");
                    c.Item().Text($"Account Number: {_data.BankDetails.AccountNumber}");
                    c.Item().Text($"Branch Code: {_data.BankDetails.BranchCode}");
                });

                column.Item().PaddingTop(10)
                    .BorderBottom(1)
                    .BorderColor("#003366")
                    .Text($"Period: {_data.StartDate:dd/MM/yyyy} - {_data.EndDate:dd/MM/yyyy}")
                    .FontSize(10);
            });
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the content of the document. The content of the document will contain the table with the claims data and the summary section with the total amount and payment terms.
        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);
                column.Item().Element(ComposeTable);
                column.Item().Element(ComposeSummary);
            });
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the table with the claims data. The table will contain the columns for Date, Hours, Rate, and Amount. The table will also contain the header row and the data rows.
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

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the summary section of the document. The summary section will contain the total amount and payment terms.
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

        // <-------------------------------------------------------------------------------------->

        // This method is used to compose the footer of the document. The footer of the document will contain the page number and total pages.
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