using FreshFalaye.Pos.Shared.Models;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace FreshFalaye.Pos.Shared.Services
{
    public static class ReceiptEscPosBuilder
    {
        // 🔧 SAFE WIDTH for most 80mm ReceiptSeries printers
        const int WIDTH = 42;

        static readonly Encoding ENC = Encoding.GetEncoding(437);

        // ================= ESC/POS COMMANDS =================
        static readonly byte[] Init = { 0x1B, 0x40 };

        static readonly byte[] BoldOn = { 0x1B, 0x45, 0x01 };
        static readonly byte[] BoldOff = { 0x1B, 0x45, 0x00 };
       
        // Double WIDTH only (not height)
        static readonly byte[] DoubleWidthOn = { 0x1D, 0x21, 0x10 };
        static readonly byte[] DoubleWidthOff = { 0x1D, 0x21, 0x00 };

        static readonly byte[] UnderlineOn = { 0x1B, 0x2D, 0x01 };
        static readonly byte[] UnderlineOff = { 0x1B, 0x2D, 0x00 };

        static readonly byte[] AlignCenter = { 0x1B, 0x61, 0x01 };
        static readonly byte[] AlignLeft = { 0x1B, 0x61, 0x00 };

        static readonly byte[] LF = { 0x0A };

        static readonly byte[] Cut = { 0x1D, 0x56, 0x41 };

        // Feed n lines
        static byte[] Feed(int n) => new byte[] { 0x1B, 0x64, (byte)n };

        // Full cut
        static readonly byte[] FullCut = { 0x1D, 0x56, 0x41 };

        // ================= TEXT HELPERS =================
        static byte[] Text(string s) =>
            ENC.GetBytes(s + "\n");

        static string Line() =>
            new string('-', WIDTH);

        static string CenterText(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            if (s.Length >= WIDTH) return s;
            int pad = (WIDTH - s.Length) / 2;
            return new string(' ', pad) + s;
        }

        static string LeftRight(string left, string right)
        {
            left ??= "";
            right ??= "";

            int space = WIDTH - (left.Length + right.Length);
            if (space < 1) space = 1;

            return left + new string(' ', space) + right;
        }

        static string ItemRow(decimal mrp, decimal dis, decimal rate, decimal qty, decimal amt)
        {
            return
                "        "+
                mrp.ToString("0.00").PadLeft(6) +
                dis.ToString("0.##").PadLeft(6) +
                rate.ToString("0.00").PadLeft(8) +
                qty.ToString("0.##").PadLeft(6) +
                amt.ToString("0.00").PadLeft(10);
        }

        static string TwoCol(string leftLabel, string leftValue,
                     string rightLabel, string rightValue)
        {
            string left = $"{leftLabel} {leftValue}";
            string right = $"{rightLabel} {rightValue}";

            int space = WIDTH - (left.Length + right.Length);
            if (space < 1) space = 1;

            return left + new string(' ', space) + right;
        }

        // ================= RECEIPT =================
        public static byte[] Build(LocalStoreSettings store, LocalSale sale)
        {
            var b = new List<byte>();

            // ===== INIT =====
            b.AddRange(Init);

            // ===== HEADER =====
            b.AddRange(AlignCenter);
            b.AddRange(BoldOn);
            b.AddRange(UnderlineOn);
            b.AddRange(Text(store.ReceiptLine1));
            b.AddRange(Text(store.ReceiptLine2));
            b.AddRange(Text(store.ReceiptLine3));
            b.AddRange(Text(store.ReceiptLine4));
            b.AddRange(Text(store.ReceiptLine5));
            b.AddRange(Text(store.ReceiptLine6));
            b.AddRange(UnderlineOff);
            b.AddRange(BoldOff);
            b.AddRange(AlignLeft);

            b.AddRange(Text(Line()));

            // ===== BILL DETAILS =====


            b.AddRange(Text(
                    TwoCol(
                        "BILL NO:",
                        sale.BillNo.Replace(store.BillPrefix, ""),
                        "AMOUNT:",
                        sale.GrandTotal.ToString()
                    )
                ));

            b.AddRange(Text(
                    TwoCol(
                        "DATE:",
                        sale.SaleDate.ToString("dd/MM/yyyy"),
                        "TIME:",
                        sale.SaleDate.ToString("HH:mm")

                    )
                ));

            b.AddRange(Text(
                    TwoCol(
                        "COUNTER:",
                        store.POSCounterNo,
                        "SALESMAN:",
                        store.POSSalesman

                    )
                ));

            //b.AddRange(Text(LeftRight("BILL NO", sale.BillNo.Replace(store.BillPrefix, ""))));
            //b.AddRange(Text(LeftRight("DATE", sale.SaleDate.ToString("dd/MM/yyyy"))));
            //b.AddRange(Text(LeftRight("TIME", sale.SaleDate.ToString("HH:mm"))));
            //b.AddRange(Text(LeftRight("COUNTER", store.POSCounterNo)));
            //b.AddRange(Text(LeftRight("SALESMAN", store.POSSalesman)));

            b.AddRange(Text(Line()));

            // ===== CUSTOMER =====
            if (!string.IsNullOrWhiteSpace(sale.CustomerName))
                b.AddRange(Text($"NAME : {sale.CustomerName}"));

            if (!string.IsNullOrWhiteSpace(sale.CustomerMobile))
                b.AddRange(Text($"MOBILE : {sale.CustomerMobile}"));

            b.AddRange(Text(Line()));

            // ===== ITEMS HEADER =====
            b.AddRange(BoldOn);
            b.AddRange(Text("ITEM     MRP     DIS     RATE     QTY     AMT"));            
            b.AddRange(BoldOff);
            b.AddRange(Text(Line()));

            foreach (var i in sale.Items)
            {
                b.AddRange(BoldOn);
                b.AddRange(Text(i.ProductName));
                b.AddRange(BoldOff);

                b.AddRange(Text(ItemRow(
                    i.Mrp,
                    i.Discount,
                    i.Rate,
                    i.Qty,
                    i.Amount
                )));
            }

            b.AddRange(Text(Line()));

            // ===== TOTAL ITEMS / QTY =====
            b.AddRange(Text(
                LeftRight(
                    $"ITEMS : {sale.Items.Count}",
                    $"QTY : {sale.Items.Sum(i => i.Qty):0.##}"
                )));

            b.AddRange(Text(Line()));

            // ===== EXPENSES =====
            foreach (var e in sale.Expenses)
                b.AddRange(Text(LeftRight(e.ExpenseName, e.Amount.ToString("0.00"))));

            if (sale.Expenses.Any())
                b.AddRange(Text(Line()));

            // ===== NET TOTAL =====
            b.AddRange(BoldOn);
            b.AddRange(Text(LeftRight("NET AMOUNT", $"Rs {sale.GrandTotal:0.00}")));
            b.AddRange(BoldOff);

            b.AddRange(Text("                          INCL. OF ALL GST"));
            b.AddRange(Text(Line()));

            // ===== SAVINGS =====
            decimal mrp = sale.Items.Sum(i => i.Mrp * i.Qty);
            decimal saleTotal = sale.Items.Sum(i => i.Rate * i.Qty);

            b.AddRange(Text(LeftRight("TOTAL (MRP)", mrp.ToString("0.00"))));
            b.AddRange(Text(LeftRight("TOTAL (SALE)", saleTotal.ToString("0.00"))));
            b.AddRange(Text(Line()));
            b.AddRange(BoldOn);
            b.AddRange(Text(LeftRight("YOU SAVE", (mrp - saleTotal).ToString("0.00"))));
            b.AddRange(BoldOff);

            b.AddRange(Text(Line()));

            // ===== PAYMENT =====
            b.AddRange(Text($"PAYMENT : {sale.PaymentMode}"));
            b.AddRange(Text(Line()));

            // ===== FOOTER =====
            b.AddRange(AlignCenter);
            b.AddRange(Text("Thank you for shopping!"));
            b.AddRange(Text("Visit Again"));
            b.AddRange(AlignLeft);

            // Blank lines
            b.AddRange(Text(""));
            b.AddRange(Text(""));

            // Feed paper
            b.AddRange(Feed(4));

            // Flush buffer
            b.AddRange(new byte[] { 0x0A, 0x0A });

            // CUT (mode 0 is more reliable)
            b.AddRange(new byte[] { 0x1D, 0x56, 0x00 });

            // 🔑 VERY IMPORTANT: RESET PRINTER STATE
            b.AddRange(Init);



            return b.ToArray();
        }

        // ================= PREVIEW =================
        public static string Preview(LocalStoreSettings store, LocalSale sale)
        {
            return ENC.GetString(Build(store, sale));
        }
    }
}
