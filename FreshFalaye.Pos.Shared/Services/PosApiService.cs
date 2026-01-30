using FreshFalaye.Pos.Shared.Models;
using FreshFalaye.Pos.Shared.Models.Dtos;
using System.Net.Http;
using System.Net.Http.Json;

namespace FreshFalaye.Pos.Shared.Services
{
    public class PosApiService
    {
        private readonly IHttpClientFactory _factory;

        public PosApiService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private HttpClient CreateClient()
            => _factory.CreateClient("Api");
        

        public async Task<RegisterResult?> RegisterPos(string branchCode, string deviceCode)
        {
            var _http = CreateClient();
            var response = await _http.PostAsync(
                $"api/pos-devices/register?branchCode={branchCode}&deviceCode={deviceCode}",
                null);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<RegisterResult>();
        }

        public async Task<List<PosProductDto>> GetPosProductsAsync(Guid branchId)
        {
            var _http = CreateClient();
            return await _http.GetFromJsonAsync<List<PosProductDto>>(
                $"api/pos/products?branchId={branchId}"
            ) ?? new();
        }

        public async Task<StoreSettingDto> GetBranchSettingsAsync(Guid branchId)
        {
            var _http = CreateClient();
            return await _http.GetFromJsonAsync<StoreSettingDto>(
                $"api/settings/branch?branchId={branchId}"
            ) ?? new();
        }
       

        public async Task<List<LocalExpenseMaster>> GetSaleExpensesAsync()
        {
            var _http = CreateClient();
            return await _http.GetFromJsonAsync<List<LocalExpenseMaster>>(
                "api/expense-master/sale"
            ) ?? new();
        }

        public async Task<List<LocalProductGroup>> GetProductGroupsAsync()
        {
            var _http = CreateClient();
            return await _http.GetFromJsonAsync<List<LocalProductGroup>>(
                "api/product-groups"
            ) ?? new();
        }

        public async Task<List<PosStockDto>> GetBranchStockAsync(Guid branchId)
        {
            var _http = CreateClient();
            var result = await _http.GetFromJsonAsync<List<PosStockDto>>(
                $"api/stock?branchId={branchId}"
            );

            return result ?? new();
        }

        // 🔹 Upload single sale
        public async Task<bool> UploadSaleAsync(SaleUploadDto dto)
        {
            var _http = CreateClient();
            try
            {
                var url = $"api/sales/upload";

                var response = await _http.PostAsJsonAsync(url, dto);

                if (response.IsSuccessStatusCode)
                    return true;

                // Optional: log response.Content here
                return false;
            }
            catch (HttpRequestException)
            {
                // ❌ Network issue (offline)
                return false;
            }
            catch (TaskCanceledException)
            {
                // ❌ Timeout
                return false;
            }
        }

        public async Task<SaleDownloadDto?> GetSaleBySyncIdAsync(Guid syncId)
        {
            var _http = CreateClient();
            return await _http.GetFromJsonAsync<SaleDownloadDto>(
                $"api/sales/by-sync-id/{syncId}"
            );
        }






        public class SaleExpenseDto
        {
            public int ExpenseId { get; set; }
            public string ExpenseName { get; set; } = null!;
            public string RateType { get; set; } = null!; // # or %
            public decimal Rate { get; set; }
            public string Bearer { get; set; } = null!;   // Self / Customer
        }

        public class RegisterResult
        {
            public Guid BranchId { get; set; }
            public Guid PosDeviceId { get; set; }
            public string PosDeviceCode { get; set; }
        }

        public class PosProductDto
        {
            public Guid ProductId { get; set; }
            public Guid SyncId { get; set; }
            public string ProductName { get; set; } = null!;
            public Guid ProductGroupId { get; set; }
            public decimal SalePrice { get; set; }
            public decimal GstPercent { get; set; }
            public string Unit { get; set; } = null!;
            public decimal Mrp { get; set; }
            public decimal Discount { get; set; }
            public bool DecimalAllowed { get; set; }
            public string? ImagePath { get; set; }
            public decimal AvailableQty { get; set; }
            public bool UseWeighingScale { get; set; }
        }

        public class StoreSettingDto
        {
            public string StoreName { get; set; }
            public string Address { get; set; }
            public string GstIn { get; set; }
            public string Phone { get; set; }
        }
    }
}
