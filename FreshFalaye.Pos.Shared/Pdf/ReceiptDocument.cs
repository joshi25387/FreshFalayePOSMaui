using FreshFalaye.Pos.Shared.Helpers;
using FreshFalaye.Pos.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshFalaye.Pos.Shared.Pdf
{
    public class ReceiptDocument : IDocument
    {
        private readonly LocalStoreSettings store;
        private readonly LocalSale sale;

        public ReceiptDocument(LocalStoreSettings store, LocalSale sale)
        {
            this.store = store;
            this.sale = sale;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(226, PageSizes.A4.Height); // 80mm
                page.Margin(5);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Content().Column(col =>
                {
                    ComposeHeader(col);
                    ComposeBillInfo(col);
                    ComposeItems(col);
                    ComposeTotals(col);
                    ComposeFooter(col);
                });
            });
        }
        void ComposeHeader(ColumnDescriptor col)
        {
            col.Item().AlignCenter().Text(store.ReceiptLine1).Bold().FontSize(11);
            col.Item().AlignCenter().Text(store.ReceiptLine2).Bold();
            col.Item().AlignCenter().Text(store.ReceiptLine3).Bold();

            col.Item().PaddingVertical(5).LineHorizontal(1);
        }

        void ComposeBillInfo(ColumnDescriptor col)
        {
            col.Item().Row(r =>
            {
                r.RelativeItem().Text($"BILL NO: {sale.BillNo.Replace(store.BillPrefix, "")}");
                r.ConstantItem(60).AlignRight()
                    .Text($"₹ {sale.GrandTotal:0.00}");
            });

            col.Item().Text($"DATE: {sale.SaleDate:dd/MM/yyyy}  TIME: {sale.SaleDate:HH:mm}");
            col.Item().Text($"COUNTER: {store.POSCounterNo}");
            col.Item().Text($"SALESMAN: {store.POSSalesman}");

            col.Item().Text($"NAME: {sale.CustomerName}").Bold();
            col.Item().Text($"MOBILE: {sale.CustomerMobile}").Bold();

            col.Item().PaddingVertical(5).LineHorizontal(1);
        }

        void ComposeItems(ColumnDescriptor col)
        {
            foreach (var i in sale.Items)
            {
                col.Item().Text(i.ProductName).Bold();

                col.Item().Row(r =>
                {
                    r.RelativeItem()
                        .Text($"{i.Qty} x {i.Rate:0.00}");

                    r.ConstantItem(60)
                        .AlignRight()
                        .Text(i.Amount.ToString("0.00"));
                });
            }

            col.Item().PaddingVertical(5).LineHorizontal(1);
        }
        void ComposeTotals(ColumnDescriptor col)
        {
            if (sale.Expenses.Any())
            {
                foreach (var e in sale.Expenses)
                {
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text(e.ExpenseName);
                        r.ConstantItem(60).AlignRight()
                            .Text(e.Amount.ToString("0.00"));
                    });
                }

                col.Item().LineHorizontal(1);
            }

            col.Item().Row(r =>
            {
                r.RelativeItem().Text("NET AMOUNT").Bold();
                r.ConstantItem(60).AlignRight()
                    .Text(sale.GrandTotal.ToString("0.00")).Bold();
            });

            col.Item().Text(
                AmountToWords.Convert(sale.GrandTotal))
                .Bold();

            col.Item().PaddingVertical(5).LineHorizontal(1);
        }
        void ComposeFooter(ColumnDescriptor col)
        {
            col.Item().AlignCenter().Text("Thank you for shopping!");
            col.Item().AlignCenter().Text("Visit again 🙂");
        }


    }

}