using FreshFalaye.Pos.Shared.Data;
using FreshFalaye.Pos.Shared.Models;
using FreshFalaye.Pos.Shared.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FreshFalaye.Pos.Shared.Services
{
    public class PosSaleService
    {
        private readonly IDbContextFactory<PosDbContext> _factory;
        //private readonly PosDbContext _db;
        private readonly BillNumberService _billNoService;
        private readonly PosApiService _posApiService;

        //public PosSaleService(
        //    PosDbContext db,
        //    BillNumberService billNoService,
        //    PosApiService posApiService)
        //{
        //    _db = db;
        //    _billNoService = billNoService;
        //    _posApiService = posApiService;
        //}

        public PosSaleService(
            IDbContextFactory<PosDbContext> factory,
            BillNumberService billNoService,
            PosApiService posApiService)
        {
            _factory = factory;
            _billNoService = billNoService;
            _posApiService = posApiService;
        }

        //public async Task<long> SaveSaleAsync(
        //    List<CartItem> cart,
        //    List<LocalSaleExpense> expenses,
        //    string paymentMode,
        //    string? customerMobile,
        //    string? customerName)
        //{
        //    if (!cart.Any())
        //        throw new Exception("Cart is empty");

        //    // 1️⃣ Validate stock
        //    foreach (var item in cart)
        //    {
        //        var stock = await _db.LocalStocks
        //            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

        //        if (stock == null || stock.Qty < item.Qty)
        //            throw new Exception($"Insufficient stock for {item.ProductName}");
        //    }

        //    // 2️⃣ Generate Bill No
        //    var billNo = await _billNoService.GenerateNextBillNoAsync();

        //    // 3️⃣ Calculate totals
        //    var productSubTotal = cart.Sum(x => x.Amount);
        //    var gstTotal = cart.Sum(x => x.GstAmount);



        //    decimal _expenseTotal = 0;
        //    foreach (var _exp in expenses)
        //    {
        //        int sign = _exp.AddDeduct == "Deduct" ? -1 : 1;
        //        _expenseTotal += sign * _exp.Amount;
        //    }


        //    var grandTotal = productSubTotal + gstTotal + _expenseTotal;

        //    // 4️⃣ Save sale header
        //    var sale = new LocalSale
        //    {
        //        BillNo = billNo,
        //        SaleDate = DateTime.Now,
        //        PaymentMode = paymentMode,
        //        CustomerMobile = customerMobile,
        //        CustomerName = customerName,
        //        ProductSubTotal = productSubTotal,
        //        GstTotal = gstTotal,
        //        ExpenseTotal = _expenseTotal,
        //        GrandTotal = grandTotal
        //    };

        //    _db.LocalSales.Add(sale);
        //    await _db.SaveChangesAsync();

        //    // 5️⃣ Save sale items & deduct stock
        //    foreach (var item in cart)
        //    {
        //        _db.LocalSaleItems.Add(new LocalSaleItem
        //        {
        //            LocalSaleId = sale.LocalSaleId,
        //            ProductId = item.ProductId,
        //            ProductName = item.ProductName,
        //            UnitCode = item.UnitCode,
        //            Qty = item.Qty,
        //            Mrp = item.Mrp,
        //            Discount = item.Discount,
        //            Rate = item.Rate,
        //            GstPercent = item.GstPercent,
        //            Amount = item.Amount,
        //            GstAmount = item.GstAmount,
        //            LineTotal = item.LineTotal
        //        });

        //        var stock = await _db.LocalStocks
        //            .FirstAsync(x => x.ProductId == item.ProductId);

        //        stock.Qty -= item.Qty;
        //        stock.LastUpdatedAt = DateTime.Now;
        //    }

        //    // 6️⃣ Save expenses
        //    foreach (var exp in expenses)
        //    {
        //        exp.LocalSaleId = sale.LocalSaleId;
        //        _db.LocalSaleExpenses.Add(exp);
        //    }

        //    await _db.SaveChangesAsync();

        //    return sale.LocalSaleId;
        //}



        public async Task<long> SaveSaleAsync(
    List<CartItem> cart,
    List<LocalSaleExpense> expenses,
    string paymentMode,
    string? customerMobile,
    string? customerName)
        {
            if (!cart.Any())
                throw new Exception("Cart is empty");


            await using var _db = await _factory.CreateDbContextAsync();

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Load all required stock in ONE query
                var productIds = cart.Select(c => c.ProductId).ToList();

                var stocks = await _db.LocalStocks
                    .Where(s => productIds.Contains(s.ProductId))
                    .ToDictionaryAsync(s => s.ProductId);

                // 2️⃣ Validate stock
                foreach (var item in cart)
                {

                    decimal _qtyToCheck = item.PrimaryUnitSelected ? Math.Round(item.Qty * item.UnitConversionRatio, 2) : item.Qty;

                    if (!stocks.TryGetValue(item.ProductId, out var stock) ||
                        stock.Qty < _qtyToCheck)
                    {
                        throw new Exception($"Insufficient stock for {item.ProductName}");
                    }
                }

                var setting = await _db.LocalSettings
                                .FromSqlRaw("SELECT * FROM LocalSettings WITH (UPDLOCK, ROWLOCK)")
                                .FirstAsync();

                setting.LastBillSequence += 1;

                var billNo = $"{setting.BranchCode}-{setting.PosDeviceCode}-{setting.LastBillSequence:000000}";


                // 3️⃣ Generate Bill No
                //var billNo = await _billNoService.GenerateNextBillNoAsync();

                // 4️⃣ Calculate totals
                var productSubTotal = cart.Sum(x => x.Amount);
                var gstTotal = cart.Sum(x => x.GstAmount);

                decimal expenseTotal = 0;
                foreach (var exp in expenses)
                {
                    int sign = exp.AddDeduct == "Deduct" ? -1 : 1;
                    expenseTotal += sign * exp.Amount;
                }

                var grandTotal = productSubTotal + gstTotal + expenseTotal;

                // 5️⃣ Create sale header
                var sale = new LocalSale
                {
                    BillNo = billNo,
                    SaleDate = DateTime.Now,
                    PaymentMode = paymentMode,
                    CustomerMobile = customerMobile,
                    CustomerName = customerName,
                    ProductSubTotal = productSubTotal,
                    GstTotal = gstTotal,
                    ExpenseTotal = expenseTotal,
                    GrandTotal = grandTotal
                };

                

                // 6️⃣ Add items + deduct stock (using loaded dictionary)
                foreach (var item in cart)
                {
                    sale.Items.Add(new LocalSaleItem
                    {                       
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitCode = item.UnitCode,
                        UnitId = item.UnitId,
                        Qty = item.Qty,
                        Mrp = item.Mrp,
                        Discount = item.Discount,
                        Rate = item.Rate,
                        GstPercent = item.GstPercent,
                        Amount = item.Amount,
                        GstAmount = item.GstAmount,
                        LineTotal = item.LineTotal
                    });

                    var stock = stocks[item.ProductId];

                    decimal _qtyToCheck = item.PrimaryUnitSelected ? Math.Round(item.Qty * item.UnitConversionRatio, 2) : item.Qty;

                    stock.Qty -= _qtyToCheck;
                    stock.LastUpdatedAt = DateTime.Now;
                }

                // 7️⃣ Add expenses
                foreach (var exp in expenses)
                {
                    sale.Expenses.Add(exp);
                }

                _db.LocalSales.Add(sale);

                // 8️⃣ Single save
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return sale.LocalSaleId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task UploadPendingSalesAsync(Guid branchId)
        {

            await using var _db = await _factory.CreateDbContextAsync();


            var pendingSales = await _db.LocalSales
                .Where(s => !s.IsUploaded)
                .Include(s => s.Items)
                .Include(s => s.Expenses)
                .ToListAsync();

            foreach (var sale in pendingSales)
            {
                var dto = new SaleUploadDto
                {
                    BranchId = branchId,
                    BillNo = sale.BillNo,
                    SaleDate = sale.SaleDate,
                    SubTotal = sale.ProductSubTotal,
                    GstTotal = sale.GstTotal,
                    GrandTotal = sale.GrandTotal,
                    PaymentMode = sale.PaymentMode,
                    Items = sale.Items.Select(i => new SaleItemUploadDto
                    {
                        ProductId = i.ProductId,
                        Qty = i.Qty,
                        Rate = i.Rate,
                        TotalAmount = i.LineTotal
                    }).ToList(),
                    Expenses = sale.Expenses.Select(e => new SaleExpenseUploadDto
                    {
                        ExpenseName = e.ExpenseName,
                        Amount = e.Amount,
                        AddDeduct = e.AddDeduct,
                        Bearer = e.Bearer,
                        ExpenseId = e.ExpenseId,
                        Rate = e.Rate,
                        RateType = e.RateType
                    }).ToList()
                };

                var success = await _posApiService.UploadSaleAsync(dto);

                if (success)
                {
                    sale.IsUploaded = true;
                    sale.UploadedAt = DateTime.Now;
                    await _db.SaveChangesAsync();
                }
            }
        }

    }
}
