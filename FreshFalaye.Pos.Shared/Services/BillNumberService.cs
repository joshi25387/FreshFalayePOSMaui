using FreshFalaye.Pos.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshFalaye.Pos.Shared.Services
{
    public class BillNumberService
    {
        private readonly PosDbContext _db;

        public BillNumberService(PosDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateNextBillNoAsync()
        {
            var setting = await _db.LocalSettings.FirstAsync();

            setting.LastBillSequence += 1;

            await _db.SaveChangesAsync();

            return $"{setting.BranchCode}-{setting.PosDeviceCode}-{setting.LastBillSequence:000000}";
        }
        public async Task<string> PeekNextBillNoAsync()
        {
            var setting = await _db.LocalSettings.FirstAsync();

            return $"{setting.BranchCode}-{setting.PosDeviceCode}-{setting.LastBillSequence + 1:000000}";
        }        

    }
}
