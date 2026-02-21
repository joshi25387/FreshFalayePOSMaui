using FreshFalaye.Pos.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshFalaye.Pos.Shared.Services
{
    public class BillNumberService
    {

        private readonly IDbContextFactory<PosDbContext> _factory;

        public BillNumberService(IDbContextFactory<PosDbContext> factory)
        {
            _factory = factory;
        }

        //public async Task<string> GenerateNextBillNoAsync()
        //{
        //    var setting = await _db.LocalSettings.FirstAsync();

        //    setting.LastBillSequence += 1;

        //    //await _db.SaveChangesAsync();

        //    return $"{setting.BranchCode}-{setting.PosDeviceCode}-{setting.LastBillSequence:000000}";
        //}
        public async Task<string> PeekNextBillNoAsync()
        {
            await using var _db = await _factory.CreateDbContextAsync();
            var setting = await _db.LocalSettings.FirstAsync();

            return $"{setting.BranchCode}-{setting.PosDeviceCode}-{setting.LastBillSequence + 1:000000}";
        }        

    }
}
