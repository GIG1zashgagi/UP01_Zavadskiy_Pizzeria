using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms; // Важно для работы Chart
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace UP01_Zavadskiy_Pizzeria.Pages
{
    public partial class DiagrammPage : Page
    {
        private uppizzariaEntities _context = new uppizzariaEntities();
        private List<menu_categories> _categories;

        public DiagrammPage()
        {
            InitializeComponent();

            // 1. Настройка диаграммы
            ChartPayments.ChartAreas.Add(new ChartArea("Main"));
            var series = new Series("Платежи") { IsValueShownAsLabel = true };
            ChartPayments.Series.Add(series);

            // 2. Загрузка данных из БД
            _categories = _context.menu_categories.ToList();
            CmbUser.ItemsSource = _context.customers.ToList();

            // 3. Настройка ComboBox
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));

            // Значения по умолчанию
            CmbUser.SelectedIndex = 0;
            CmbDiagram.SelectedIndex = 0;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is customers currentCustomer &&
                CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                var series = ChartPayments.Series.FirstOrDefault();
                if (series == null) return;

                series.ChartType = currentType;
                series.Points.Clear();

                foreach (var cat in _categories)
                {
                    var sum = _context.order_items
                        .Where(oi => oi.orders.customer_id == currentCustomer.customer_id
                                  && oi.menu_items.category_id == cat.category_id)
                        .Sum(oi => (decimal?)oi.subtotal) ?? 0;

                    series.Points.AddXY(cat.name, sum);
                }
            }
        }

        private void BtnExportWord_Click(object sender, RoutedEventArgs e)
        {
            var allCustomers = _context.customers.ToList();
            var allCategories = _categories;

            var app = new Word.Application();
            Word.Document doc = app.Documents.Add();

            foreach (var customer in allCustomers)
            {
                Word.Paragraph p = doc.Paragraphs.Add();
                Word.Range r = p.Range;
                r.Text = $"{customer.first_name} {customer.last_name}";

                try { p.set_Style("Заголовок 1"); } catch { }

                r.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                r.InsertParagraphAfter();
                doc.Paragraphs.Add();

                Word.Paragraph tableP = doc.Paragraphs.Add();
                Word.Range tableR = tableP.Range;
                Word.Table table = doc.Tables.Add(tableR, allCategories.Count + 1, 2);
                table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

                table.Cell(1, 1).Range.Text = "Категория";
                table.Cell(1, 2).Range.Text = "Сумма расходов";

                for (int i = 0; i < allCategories.Count; i++)
                {
                    var cat = allCategories[i];
                    var sum = _context.order_items
                        .Where(oi => oi.orders.customer_id == customer.customer_id
                                  && oi.menu_items.category_id == cat.category_id)
                        .Sum(oi => (decimal?)oi.subtotal) ?? 0;

                    table.Cell(i + 2, 1).Range.Text = cat.name;
                    table.Cell(i + 2, 2).Range.Text = sum.ToString("N2") + " руб.";
                }

                doc.Paragraphs.Add();
                if (customer != allCustomers.Last())
                    doc.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
            }

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Payments.docx");
            doc.SaveAs2(path);
            doc.SaveAs2(path.Replace(".docx", ".pdf"), Word.WdExportFormat.wdExportFormatPDF);
            app.Visible = true;
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var allCustomers = _context.customers.ToList();
            var allCategories = _categories;

            var app = new Excel.Application();
            Excel.Workbook wb = app.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Sheets[1];

            int row = 1;
            foreach (var customer in allCustomers)
            {
                ws.Cells[row, 1] = $"{customer.first_name} {customer.last_name}";
                ws.Cells[row, 1].Font.Bold = true;
                row++;

                ws.Cells[row, 1] = "Категория";
                ws.Cells[row, 2] = "Сумма";
                row++;

                foreach (var cat in allCategories)
                {
                    var sum = _context.order_items
                        .Where(oi => oi.orders.customer_id == customer.customer_id
                                  && oi.menu_items.category_id == cat.category_id)
                        .Sum(oi => (decimal?)oi.subtotal) ?? 0;
                    ws.Cells[row, 1] = cat.name;
                    ws.Cells[row, 2] = sum;
                    row++;
                }
                row++;
            }

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Payments.xlsx");
            wb.SaveAs(path);
            app.Visible = true;
        }
    }
}