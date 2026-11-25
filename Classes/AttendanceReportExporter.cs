using ClosedXML.Excel;
using System.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AMS.Classes
{
    internal static class AttendanceReportExporter
    {
        public static void ExportExcel(DataTable report, string path)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Attendance");
            sheet.Cell(1, 1).InsertTable(report, "AttendanceReport", true);
            sheet.Row(1).Style.Font.Bold = true;
            sheet.Row(1).Style.Font.FontColor = XLColor.White;
            sheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#234363");
            sheet.SheetView.FreezeRows(1);
            sheet.Columns().AdjustToContents();
            workbook.SaveAs(path);
        }

        public static void ExportPdf(DataTable report, string path)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(24);
                    page.DefaultTextStyle(style => style.FontSize(8));
                    page.Header().Column(column =>
                    {
                        column.Item().Text("Attendance report").FontSize(18).SemiBold().FontColor("#234363");
                        column.Item().Text($"Generated {DateTime.Now:dd MMM yyyy, HH:mm}").FontColor("#647485");
                    });
                    page.Content().PaddingTop(16).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (DataColumn column in report.Columns)
                            {
                                columns.RelativeColumn(column.ColumnName == "Student" || column.ColumnName == "Course"
                                    ? 1.5F
                                    : 1F);
                            }
                        });
                        table.Header(header =>
                        {
                            foreach (DataColumn column in report.Columns)
                            {
                                header.Cell()
                                    .Element(CellStyle)
                                    .Background("#234363")
                                    .Text(column.ColumnName)
                                    .FontColor(Colors.White)
                                    .SemiBold();
                            }
                        });
                        foreach (DataRow row in report.Rows)
                        {
                            foreach (DataColumn column in report.Columns)
                            {
                                table.Cell().Element(CellStyle).Text(Convert.ToString(row[column]) ?? string.Empty);
                            }
                        }
                    });
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            }).GeneratePdf(path);
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor("#E5EBF1")
                .PaddingVertical(4)
                .PaddingHorizontal(3);
        }
    }
}
