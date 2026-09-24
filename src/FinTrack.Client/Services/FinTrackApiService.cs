using System.Net.Http.Json;
using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Common;
using FinTrack.Shared.DTOs.Dashboard;
using FinTrack.Shared.DTOs.Report;
using FinTrack.Shared.DTOs.Savings;
using FinTrack.Shared.DTOs.Transaction;

namespace FinTrack.Client.Services;

public class FinTrackApiService : IFinTrackApiService
{
    private readonly HttpClient _http;
    public event Action? OnDataChanged;

    public FinTrackApiService(HttpClient http)
    {
        _http = http;
    }

    public void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    #region Dashboard
    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string? month = null)
    {
        var url = string.IsNullOrEmpty(month) ? "api/dashboard" : $"api/dashboard?month={Uri.EscapeDataString(month)}";
        var response = await _http.GetFromJsonAsync<DashboardSummaryDto>(url);
        return response ?? new DashboardSummaryDto { Month = month ?? DateTime.Today.ToString("yyyy-MM") };
    }
    #endregion

    #region Transactions
    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(string? search = null, string? type = null, Guid? categoryId = null, int page = 1, int pageSize = 50)
    {
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
        return await _http.GetFromJsonAsync<TransactionDto>($"api/transactions/{id}");
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/transactions", request);
        await EnsureSuccessOrThrowAsync(response);
        var created = await response.Content.ReadFromJsonAsync<TransactionDto>();
        NotifyDataChanged();
        return created!;
    }

    public async Task<TransactionDto> UpdateTransactionAsync(Guid id, UpdateTransactionRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/transactions/{id}", request);
        await EnsureSuccessOrThrowAsync(response);
        var updated = await response.Content.ReadFromJsonAsync<TransactionDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<bool> DeleteTransactionAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/transactions/{id}");
        await EnsureSuccessOrThrowAsync(response);
        NotifyDataChanged();
        return response.IsSuccessStatusCode;
    }
    #endregion

    #region Categories
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var response = await _http.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        return response ?? new List<CategoryDto>();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/categories", request);
        await EnsureSuccessOrThrowAsync(response);
        var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
        NotifyDataChanged();
        return created!;
    }
    #endregion

    #region Budgets
    public async Task<List<BudgetDto>> GetBudgetsAsync(string? month = null)
    {
        var url = string.IsNullOrEmpty(month) ? "api/budgets" : $"api/budgets?month={Uri.EscapeDataString(month)}";
        var response = await _http.GetFromJsonAsync<List<BudgetDto>>(url);
        return response ?? new List<BudgetDto>();
    }

    public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/budgets", request);
        await EnsureSuccessOrThrowAsync(response);
        var created = await response.Content.ReadFromJsonAsync<BudgetDto>();
        NotifyDataChanged();
        return created!;
    }

    public async Task<BudgetDto> UpdateBudgetAsync(Guid id, UpdateBudgetRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/budgets/{id}", request);
        await EnsureSuccessOrThrowAsync(response);
        var updated = await response.Content.ReadFromJsonAsync<BudgetDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<bool> DeleteBudgetAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/budgets/{id}");
        await EnsureSuccessOrThrowAsync(response);
        NotifyDataChanged();
        return response.IsSuccessStatusCode;
    }
    #endregion

    #region Savings Goals
    public async Task<List<SavingsGoalDto>> GetSavingsGoalsAsync()
    {
        var response = await _http.GetFromJsonAsync<List<SavingsGoalDto>>("api/savings-goals");
        return response ?? new List<SavingsGoalDto>();
    }

    public async Task<SavingsGoalDto> CreateSavingsGoalAsync(CreateSavingsGoalRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/savings-goals", request);
        await EnsureSuccessOrThrowAsync(response);
        var created = await response.Content.ReadFromJsonAsync<SavingsGoalDto>();
        NotifyDataChanged();
        return created!;
    }

    public async Task<SavingsGoalDto> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/savings-goals/{id}", request);
        await EnsureSuccessOrThrowAsync(response);
        var updated = await response.Content.ReadFromJsonAsync<SavingsGoalDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<SavingsGoalDto> ContributeToSavingsGoalAsync(Guid id, ContributeRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/savings-goals/{id}/contribute", request);
        await EnsureSuccessOrThrowAsync(response);
        var updated = await response.Content.ReadFromJsonAsync<SavingsGoalDto>();
        NotifyDataChanged();
        return updated!;
    }

    public async Task<bool> DeleteSavingsGoalAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/savings-goals/{id}");
        await EnsureSuccessOrThrowAsync(response);
        NotifyDataChanged();
        return response.IsSuccessStatusCode;
    }
    #endregion

    #region Reports
    public async Task<SpendingByCategoryReportDto> GetSpendingByCategoryReportAsync(DateTime from, DateTime to)
    {
        var fromStr = from.ToString("yyyy-MM-dd");
        var toStr = to.ToString("yyyy-MM-dd");
        var url = $"api/reports/spending-by-category?from={fromStr}&to={toStr}";
        try
        {
            var response = await _http.GetFromJsonAsync<SpendingByCategoryReportDto>(url);
            return response ?? new SpendingByCategoryReportDto { From = from, To = to };
        }
        catch
        {
            return new SpendingByCategoryReportDto { From = from, To = to };
        }
    }

    public async Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(DateTime from, DateTime to, string granularity = "monthly")
    {
        var fromStr = from.ToString("yyyy-MM-dd");
        var toStr = to.ToString("yyyy-MM-dd");
        var url = $"api/reports/income-vs-expense?from={fromStr}&to={toStr}&granularity={Uri.EscapeDataString(granularity)}";
        try
        {
            var response = await _http.GetFromJsonAsync<IncomeVsExpenseReportDto>(url);
            return response ?? new IncomeVsExpenseReportDto { Granularity = granularity };
        }
        catch
        {
            return new IncomeVsExpenseReportDto { Granularity = granularity };
        }
    }
    #endregion

    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        string? errorMessage = null;
        try
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (errorResponse != null && !string.IsNullOrWhiteSpace(errorResponse.Message))
            {
                if (errorResponse.Errors != null && errorResponse.Errors.Count > 0)
                {
                    var details = string.Join("; ", errorResponse.Errors.SelectMany(e => e.Value));
                    errorMessage = $"{errorResponse.Message} ({details})";
                }
                else
                {
                    errorMessage = errorResponse.Message;
                }
            }
        }
        catch
        {
            // Ignore JSON parse failure and fallback below
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    errorMessage = content;
                }
            }
            catch
            {
                // Ignore read failure
            }
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            errorMessage = $"Request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
        }

        throw new InvalidOperationException(errorMessage);
    }
}
