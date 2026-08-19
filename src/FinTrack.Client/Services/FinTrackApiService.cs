using System.Net.Http.Headers;
using System.Net.Http.Json;
using FinTrack.Shared.DTOs.Auth;
using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Dashboard;
using FinTrack.Shared.DTOs.Report;
using FinTrack.Shared.DTOs.Savings;
using FinTrack.Shared.DTOs.Transaction;

namespace FinTrack.Client.Services;

public class FinTrackApiService : IFinTrackApiService
{
    private readonly HttpClient _http;
    private readonly IAuthService _authService;
    public event Action? OnDataChanged;

    public FinTrackApiService(HttpClient http, IAuthService authService)
    {
        _http = http;
        _authService = authService;
    }

    public void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    private async Task EnsureAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    #region Dashboard
    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string? month = null)
    {
        await EnsureAuthHeaderAsync();
        var url = string.IsNullOrEmpty(month) ? "api/dashboard" : $"api/dashboard?month={Uri.EscapeDataString(month)}";
        var response = await _http.GetFromJsonAsync<DashboardSummaryDto>(url);
        return response ?? new DashboardSummaryDto { Month = month ?? DateTime.Today.ToString("yyyy-MM") };
    }
    #endregion

    #region Transactions
    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(string? search = null, string? type = null, Guid? categoryId = null, int page = 1, int pageSize = 50)
    {
        await EnsureAuthHeaderAsync();
        var query = $"api/transactions?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(type) && type != "All") query += $"&type={Uri.EscapeDataString(type)}";
        if (categoryId.HasValue && categoryId.Value != Guid.Empty) query += $"&categoryId={categoryId.Value}";

        var result = await _http.GetFromJsonAsync<PagedResult<TransactionDto>>(query);
        if (result != null && !string.IsNullOrWhiteSpace(search))
        {
            var filtered = result.Items
                .Where(t => (t.Description != null && t.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                            (t.CategoryName != null && t.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            result.Items = filtered;
            result.TotalCount = filtered.Count;
        }

        return result ?? new PagedResult<TransactionDto>();
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(Guid id)
    {
        await EnsureAuthHeaderAsync();
        return await _http.GetFromJsonAsync<TransactionDto>($"api/transactions/{id}");
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/transactions", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TransactionDto>();
        NotifyDataChanged();
        return created!;
    }

    public async Task<TransactionDto> UpdateTransactionAsync(Guid id, UpdateTransactionRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"api/transactions/{id}", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<TransactionDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<bool> DeleteTransactionAsync(Guid id)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.DeleteAsync($"api/transactions/{id}");
        NotifyDataChanged();
        return response.IsSuccessStatusCode;
    }
    #endregion

    #region Categories
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        return response ?? new List<CategoryDto>();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/categories", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
        NotifyDataChanged();
        return created!;
    }
    #endregion

    #region Budgets
    public async Task<List<BudgetDto>> GetBudgetsAsync(string? month = null)
    {
        await EnsureAuthHeaderAsync();
        var url = string.IsNullOrEmpty(month) ? "api/budgets" : $"api/budgets?month={Uri.EscapeDataString(month)}";
        var response = await _http.GetFromJsonAsync<List<BudgetDto>>(url);
        return response ?? new List<BudgetDto>();
    }

    public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/budgets", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<BudgetDto>();
        NotifyDataChanged();
        return created!;
    }

    public async Task<BudgetDto> UpdateBudgetAsync(Guid id, UpdateBudgetRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"api/budgets/{id}", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<BudgetDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<bool> DeleteBudgetAsync(Guid id)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.DeleteAsync($"api/budgets/{id}");
        NotifyDataChanged();
        return response.IsSuccessStatusCode;
    }
    #endregion

    #region Savings Goals
    public async Task<List<SavingsGoalDto>> GetSavingsGoalsAsync()
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.GetFromJsonAsync<List<SavingsGoalDto>>("api/savings-goals");
        return response ?? new List<SavingsGoalDto>();
    }

    public async Task<SavingsGoalDto> CreateSavingsGoalAsync(CreateSavingsGoalRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/savings-goals", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<SavingsGoalDto>();
        NotifyDataChanged();
        return created!;
    }

    public async Task<SavingsGoalDto> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"api/savings-goals/{id}", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<SavingsGoalDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<SavingsGoalDto> ContributeToSavingsGoalAsync(Guid id, ContributeRequest request)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/savings-goals/{id}/contribute", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<SavingsGoalDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<bool> DeleteSavingsGoalAsync(Guid id)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.DeleteAsync($"api/savings-goals/{id}");
        NotifyDataChanged();
        return response.IsSuccessStatusCode;
    }
    #endregion

    #region Reports
    public async Task<SpendingByCategoryReportDto> GetSpendingByCategoryReportAsync(DateTime from, DateTime to)
    {
        await EnsureAuthHeaderAsync();
        var fromStr = Uri.EscapeDataString(from.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var toStr = Uri.EscapeDataString(to.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var url = $"api/reports/spending-by-category?from={fromStr}&to={toStr}";
        var response = await _http.GetFromJsonAsync<SpendingByCategoryReportDto>(url);
        return response ?? new SpendingByCategoryReportDto { From = from, To = to };
    }

    public async Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(DateTime from, DateTime to, string granularity = "monthly")
    {
        await EnsureAuthHeaderAsync();
        var fromStr = Uri.EscapeDataString(from.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var toStr = Uri.EscapeDataString(to.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        var url = $"api/reports/income-vs-expense?from={fromStr}&to={toStr}&granularity={Uri.EscapeDataString(granularity)}";
        var response = await _http.GetFromJsonAsync<IncomeVsExpenseReportDto>(url);
        return response ?? new IncomeVsExpenseReportDto { Granularity = granularity };
    }
    #endregion
}
