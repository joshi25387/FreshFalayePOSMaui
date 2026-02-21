using FreshFalaye.Pos.Shared.Data;
using FreshFalaye.Pos.Shared.Models;
using FreshFalaye.Pos.Shared.Pdf;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace FreshFalaye.Pos.Shared.Services
{
    public class ReceiptPdfService
    {
        private readonly IDbContextFactory<PosDbContext> _factoryDb;
        //private readonly PosDbContext _db;

        //public ReceiptPdfService(PosDbContext db)
        //{
        //    _db = db;
        //}
        public ReceiptPdfService(IDbContextFactory<PosDbContext> factoryDb)
        {
            _factoryDb = factoryDb;
        }

        public async Task<byte[]> GenerateAsync(long saleId)
        {
            await using var _db = await _factoryDb.CreateDbContextAsync();
            var store = await _db.LocalStoreSettings.FirstAsync();

            var sale = await _db.LocalSales
                .Include(s => s.Items)
                .Include(s => s.Expenses)
                .FirstAsync(s => s.LocalSaleId == saleId);

            var document = new ReceiptDocument(store, sale);

            return document.GeneratePdf();
        }
    }
}
