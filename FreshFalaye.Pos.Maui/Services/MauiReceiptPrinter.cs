#if WINDOWS
using FreshFalaye.Pos.Shared.Abstractions;
using FreshFalaye.Pos.Shared.Data;
using FreshFalaye.Pos.Shared.Services;
using Microsoft.EntityFrameworkCore;
using FreshFalaye.Pos.Maui.Services; // 🔴 THIS IS REQUIRED
using System.Drawing.Printing;
using System.Text;

namespace FreshFalaye.Pos.Maui.Services
{
    public class MauiReceiptPrinter : IReceiptPrinter
    {
        private readonly PosDbContext _db;
        private readonly WindowsThermalPrinter _thermalPrinter;

        public MauiReceiptPrinter(
            PosDbContext db,
            WindowsThermalPrinter thermalPrinter)
        {
            _db = db;
            _thermalPrinter = thermalPrinter;
        }

        public async Task PrintAsync(long saleId)
        {
            var store = await _db.LocalStoreSettings.FirstAsync();

            var sale = await _db.LocalSales
                .Include(s => s.Items)
                .Include(s => s.Expenses)
                .FirstAsync(s => s.LocalSaleId == saleId);


                // 🔍 RECEIPT PREVIEW (TEMPORARY)
            //var preview = ReceiptEscPosBuilder.Preview(store, sale);

            //var path = Path.Combine(
            //    FileSystem.AppDataDirectory,
            //    $"receipt-{saleId}.txt"
            //);

            //File.WriteAllText(path, preview);

            //foreach (string p in PrinterSettings.InstalledPrinters)
            //{
            //    //Debug.WriteLine(p);
            //}
            

//            var test = Encoding.ASCII.GetBytes(
//    "RUGTEK TEST\n\n\n"
//);

//_thermalPrinter.Print(test, "Rugtek Printer");

            var data = ReceiptEscPosBuilder.Build(store, sale);

            _thermalPrinter.Print(
                data,
                "Rugtek Printer"
            );
        }
    }
}
#endif
