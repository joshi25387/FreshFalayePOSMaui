using FreshFalaye.Pos.Shared.Data;
using FreshFalaye.Pos.Shared.Helpers;
using FreshFalaye.Pos.Shared.Models;
using FreshFalaye.Pos.Shared.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FreshFalaye.Pos.Shared.Services
{
    public class PosSyncService
    {
        private readonly PosDbContext _db;
        private readonly PosApiService _api;
        private readonly PosImageDownloader _imageDownloader;

        public PosSyncService(
            PosDbContext db,
            PosApiService api,
            PosImageDownloader imageDownloader)
        {
            _db = db;
            _api = api;
            _imageDownloader = imageDownloader;
        }

        public async Task SyncProductsAsync(Guid branchId)
        {
            try
            {
                // calls API
                // saves LocalProducts & LocalStock

                // 1️⃣ Call API and get products + stock
                var productsFromApi = await _api.GetPosProductsAsync(branchId);

                if (productsFromApi == null || productsFromApi.Count == 0)
                    return;
                

                // 2️⃣ Clear old local cache (simple & safe for first version)
                _db.LocalProducts.RemoveRange(_db.LocalProducts);
                //_db.LocalStocks.RemoveRange(_db.LocalStocks);

                // 3️⃣ Save new data
                foreach (var p in productsFromApi)
                {
                    // 🔹 Download image (if any)
                    string? localImagePath = null;

                    if (!string.IsNullOrWhiteSpace(p.ImagePath))
                    {
                        string _imagename = p.ProductName + Path.GetExtension(p.ImagePath);
                        localImagePath =
                            await _imageDownloader.DownloadImageAsync(p.ImagePath, _imagename);
                    }


                    _db.LocalProducts.Add(new LocalProduct
                    {
                        SyncId = p.SyncId,
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ProductGroupId = p.ProductGroupId,                        
                        UnitCode = p.Unit,
                        DecimalAllowed = p.DecimalAllowed,
                        Mrp = p.Mrp,
                        Discount = p.Discount,
                        SalePrice = p.SalePrice,
                        GstPercent = p.GstPercent,
                        ImagePath = localImagePath,
                        IsActive = true,
                        UseWeighingScale = p.UseWeighingScale
                    });

                    //_db.LocalStocks.Add(new LocalStock
                    //{
                    //    ProductId = p.ProductId,
                    //    Qty = p.AvailableQty,
                    //    LastUpdatedAt = DateTime.UtcNow
                    //});
                }

                // 4️⃣ Commit to local DB
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        public async Task SyncProductGroupsAsync()
        {
            try
            {
                var groups = await _api.GetProductGroupsAsync();

                

                foreach (var g in groups)
                {
                    var local = await _db.LocalProductGroups
                        .FirstOrDefaultAsync(x => x.SyncId == g.SyncId);

                    if (local == null)
                    {
                        _db.LocalProductGroups.Add(g);
                    }
                    else
                    {
                        local.GroupName = g.GroupName;
                        local.IsActive = g.IsActive;
                    }
                }

                // deactivate missing ones
                var apiSyncIds = groups.Select(x => x.SyncId).ToHashSet();

                var locals = await _db.LocalProductGroups.ToListAsync();
                foreach (var l in locals)
                {
                    if (!apiSyncIds.Contains(l.SyncId))
                        l.IsActive = false;
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        public async Task SyncStockAsync(Guid branchId)
        {
            try
            {
                var stocks = await _api.GetBranchStockAsync(branchId);
                if (!stocks.Any())
                    return;


                _db.LocalStocks.RemoveRange(_db.LocalStocks);

                // 🔑 Load once, track once
                //var localStocks = await _db.LocalStocks
                //    .ToDictionaryAsync(x => x.ProductId);

                foreach (var s in stocks)
                {
                    var newStock = new LocalStock
                    {
                        ProductId = s.ProductId,
                        Qty = s.Qty,
                        LastUpdatedAt = s.LastUpdatedAt
                    };

                    _db.LocalStocks.Add(newStock);

                    //if (localStocks.TryGetValue(s.ProductId, out var local))
                    //{
                    //    // ✏️ Update
                    //    local.Qty = s.Qty;
                    //    local.LastUpdatedAt = s.LastUpdatedAt;
                    //}
                    //else
                    //{
                    //    // ➕ Insert
                    //    var newStock = new LocalStock
                    //    {
                    //        ProductId = s.ProductId,
                    //        Qty = s.Qty,
                    //        LastUpdatedAt = s.LastUpdatedAt
                    //    };

                    //    _db.LocalStocks.Add(newStock);
                    //    localStocks[s.ProductId] = newStock; // 🔑 prevent duplicates
                    //}
                }               
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task SyncExpenses()
        {
            try
            {
                var apiExpenses = await _api.GetSaleExpensesAsync();

                // 1️⃣ Clear local table
                _db.LocalExpenseMaster.RemoveRange(
                    await _db.LocalExpenseMaster.ToListAsync()
                );

                // 2️⃣ Insert fresh data
                await _db.LocalExpenseMaster.AddRangeAsync(apiExpenses);

                // 3️⃣ Save once
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SyncStoreSettingsAsync(Guid branchId)
        {
            var dto = await _api.GetBranchSettingsAsync(branchId);
            if (dto == null) return;

            var local = await _db.LocalStoreSettings.FirstOrDefaultAsync();
            if (local == null)
            {
                local = new LocalStoreSettings();
                _db.LocalStoreSettings.Add(local);
            }

            local.StoreName = dto.StoreName;
            local.Address = dto.Address;
            local.GstIn = dto.GstIn;
            local.Phone = dto.Phone;

            await _db.SaveChangesAsync();
        }

        public async Task SyncSalesAsync(Guid branchId)
        {
            // 1️⃣ Get all unsynced local sales
            var unsyncedSales = await _db.LocalSales
                .Where(s => !s.IsUploaded)
                .Include(s => s.Items)
                .Include(s => s.Expenses)
                .OrderBy(s => s.LocalSaleId)
                .ToListAsync();

            if (!unsyncedSales.Any())
                return;


            bool anyUpdated = false;
            // 2️⃣ Upload one-by-one (safe & retryable)
            foreach (var sale in unsyncedSales)
            {
                var dto = new SaleUploadDto
                {
                    SyncId = sale.SyncId,
                    BranchId = branchId,
                    BillNo = sale.BillNo,
                    SaleDate = sale.SaleDate,
                    CustomerMobile = sale.CustomerMobile,
                    CustomerName = sale.CustomerName,
                    SubTotal = sale.ProductSubTotal,
                    GstTotal = sale.GstTotal,
                    GrandTotal = sale.GrandTotal,

                    PaymentMode = sale.PaymentMode,

                    Items = sale.Items.Select(i => new SaleItemUploadDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName, 
                        UnitCode= i.UnitCode,
                        Qty = i.Qty,
                        Mrp = i.Mrp,
                        Discount = i.Discount,
                        Rate = i.Rate,
                        TotalAmount = i.LineTotal
                    }).ToList(),

                    Expenses = sale.Expenses.Select(e => new SaleExpenseUploadDto
                    {
                        ExpenseId= e.ExpenseId,
                        ExpenseName = e.ExpenseName,
                        Amount = e.Amount,
                        RateType= e.RateType,
                        Rate = e.Rate,
                        AddDeduct = e.AddDeduct,
                        Bearer = e.Bearer
                    }).ToList()
                };

                // ⬆️ Upload to backend
                var _saleUploaded = await _api.UploadSaleAsync(dto);
                if (_saleUploaded)
                {
                    // ✅ Mark as uploaded ONLY after success
                    sale.IsUploaded = true;
                    sale.UploadedAt = DateTime.Now;
                    sale.IsSynced = true;
                    anyUpdated = true;                    
                }                
            }

            if (anyUpdated)
            {
                await _db.SaveChangesAsync();
            }
        }

        public async Task SyncSingleSaleAsync(Guid syncId, Guid branchId)
        {
            var sale = await _db.LocalSales
                .Include(s => s.Items)
                .Include(s => s.Expenses)
                .FirstOrDefaultAsync(s => s.SyncId == syncId);

            if (sale == null || sale.IsUploaded)
                return;

            var dto = new SaleUploadDto
            {
                SyncId = sale.SyncId,
                BranchId = branchId,
                BillNo = sale.BillNo,
                SaleDate = sale.SaleDate,
                CustomerName = sale.CustomerName,
                CustomerMobile = sale.CustomerMobile,
                SubTotal = sale.ProductSubTotal,
                GstTotal = sale.GstTotal,
                GrandTotal = sale.GrandTotal,
                PaymentMode = sale.PaymentMode,

                Items = sale.Items.Select(i => new SaleItemUploadDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitCode = i.UnitCode,
                    Qty = i.Qty,
                    Mrp = i.Mrp,
                    Discount = i.Discount,
                    Rate = i.Rate,
                    TotalAmount = i.LineTotal
                }).ToList(),

                Expenses = sale.Expenses.Select(e => new SaleExpenseUploadDto
                {
                    ExpenseId = e.ExpenseId,
                    ExpenseName = e.ExpenseName,
                    Amount = e.Amount,
                    RateType = e.RateType,
                    Rate = e.Rate,
                    AddDeduct = e.AddDeduct,
                    Bearer = e.Bearer
                }).ToList()
            };

            bool _isUploaded = await _api.UploadSaleAsync(dto);

            sale.IsUploaded = _isUploaded;
            sale.UploadedAt = _isUploaded ? DateTime.Now : null;
            //sale.IsSynced = _isUploaded;

            await _db.SaveChangesAsync();
        }

        public async Task DownloadSaleAsync(Guid syncId)
        {
            try
            {
                var dto = await _api.GetSaleBySyncIdAsync(syncId);
                if (dto == null) return;


                var _localSaleRecord = await _db.LocalSales.Where(s => s.SyncId == dto.SyncId).FirstOrDefaultAsync();
                if (_localSaleRecord != null)
                {
                    var _localSaleItems = await _db.LocalSaleItems.Where(s => s.LocalSaleId == _localSaleRecord.LocalSaleId).ToListAsync();
                    var _localSaleExpenses = await _db.LocalSaleExpenses.Where(s => s.LocalSaleId == _localSaleRecord.LocalSaleId).ToListAsync();
                    _db.LocalSaleItems.RemoveRange(_localSaleItems);
                    _db.LocalSaleExpenses.RemoveRange(_localSaleExpenses);
                    _db.LocalSales.Remove(_localSaleRecord);
                    await _db.SaveChangesAsync();
                }


                var sale = new LocalSale
                {
                    SyncId = dto.SyncId,
                    BillNo = dto.BillNo,
                    SaleDate = dto.SaleDate,
                    CustomerMobile = dto.CustomerMobile,
                    CustomerName = dto.CustomerName,
                    ProductSubTotal = dto.SubTotal,
                    GstTotal = dto.GstTotal,
                    GrandTotal = dto.GrandTotal,
                    PaymentMode = dto.PaymentMode,
                    IsUploaded = true,
                    IsSynced = true,
                    UploadedAt = DateTime.UtcNow
                };

                foreach (var i in dto.Items)
                {
                    sale.Items.Add(new LocalSaleItem
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        UnitCode = i.UnitCode,
                        Qty = i.Qty,
                        Mrp = i.Mrp,
                        Discount= i.Discount,
                        Rate = i.Rate,
                        LineTotal = i.LineTotal
                    });
                }

                foreach (var e in dto.Expenses)
                {
                    sale.Expenses.Add(new LocalSaleExpense
                    {
                        ExpenseId = e.ExpenseId,
                        ExpenseName = e.ExpenseName,
                        Amount = e.Amount
                    });
                }

                _db.LocalSales.Add(sale);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }


    }
}
