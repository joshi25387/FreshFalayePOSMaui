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
        //private readonly PosDbContext _db;
        private readonly IDbContextFactory<PosDbContext> _factory;
        private readonly WindowsThermalPrinter _thermalPrinter;

        //public MauiReceiptPrinter(
        //    PosDbContext db,
        //    WindowsThermalPrinter thermalPrinter)
        //{
        //    _db = db;
        //    _thermalPrinter = thermalPrinter;
        //}
        public MauiReceiptPrinter(
            IDbContextFactory<PosDbContext> factory,
            WindowsThermalPrinter thermalPrinter)
        {
            _factory = factory;
            _thermalPrinter = thermalPrinter;
        }

        public Task PrintAsync(long saleId)
        {
            _ = Task.Run(async () =>
            {
                try 
	            {	        
		            await using var db = _factory.CreateDbContext(); // if using factory

                    var store = await db.LocalStoreSettings.FirstAsync();

                    var sale = await db.LocalSales
                        .Include(s => s.Items)
                        .Include(s => s.Expenses)
                        .FirstAsync(s => s.LocalSaleId == saleId);

                    var data = ReceiptEscPosBuilder.Build(store, sale);

                    _thermalPrinter.Print(data, "Rugtek Printer");
	            }
	            catch (global::System.Exception)
	            {
		        
	            }
            });


            return Task.CompletedTask;
            
            //var store = await _db.LocalStoreSettings.FirstAsync();

            //var sale = await _db.LocalSales
            //    .Include(s => s.Items)
            //    .Include(s => s.Expenses)
            //    .FirstAsync(s => s.LocalSaleId == saleId);

            //var data = ReceiptEscPosBuilder.Build(store, sale);

            //_thermalPrinter.Print(
            //    data,
            //    "Rugtek Printer"
            //);
        }
    }
}
#endif
