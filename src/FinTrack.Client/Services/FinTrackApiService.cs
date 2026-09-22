using System.Net.Http.Json;
using FinTrack.Shared.DTOs.Auth;
using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Dashboard;
using FinTrack.Shared.DTOs.Report;
using FinTrack.Shared.DTOs.Savings;
using FinTrack.Shared.DTOs.Transaction;
using FinTrack.Shared.DTOs.Common;

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
        var url = string.IsNullOrEmpty(month) ? "/api/dashboard" : $"/api/dashboard?month={month}";
        var response = await _http.GetFromJsonAsync<DashboardSummaryDto>(url)
            ?? throw new InvalidOperationException("Failed to retrieve dashboard summary from the API.");
        return response;
    }
    #endregion

    #region Transactions
    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(string? search = null, string? type = null, Guid? categoryId = null, int page = 1, int pageSize = 50)
    {
        var query = $"/api/transactions?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(type) && type != "All") query += $"&type={type}";
        if (categoryId.HasValue) query += $"&categoryId={categoryId.Value}";

        var response = await _http.GetFromJsonAsync<PagedResult<TransactionDto>>(query)
            ?? throw new InvalidOperationException("Failed to retrieve transactions from the API.");
        return response;
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<TransactionDto>($"/api/transactions/{id}");
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/transactions", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TransactionDto>()
            ?? throw new InvalidOperationException("API returned an empty response when creating a transaction.");
        NotifyDataChanged();
        return created;
    }

    public async Task<TransactionDto> UpdateTransactionAsync(Guid id, UpdateTransactionRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/transactions/{id}", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<TransactionDto>()
            ?? throw new InvalidOperationException("API returned an empty response when updating a transaction.");
        NotifyDataChanged();
        return updated;
    }

    public async Task<bool> DeleteTransactionAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"/api/transactions/{id}");
        response.EnsureSuccessStatusCode();
        NotifyDataChanged();
        return true;
    }
    #endregion

    #region Categories
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var response = await _http.GetFromJsonAsync<List<CategoryDto>>("/api/categories")
            ?? throw new InvalidOperationException("Failed to retrieve categories from the API.");
        return response;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/categories", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<CategoryDto>()
            ?? throw new InvalidOperationException("API returned an empty response when creating a category.");
        NotifyDataChanged();
        return created;
    }
    #endregion

    #region Budgets
    public async Task<List<BudgetDto>> GetBudgetsAsync(string? month = null)
    {
        var url = string.IsNullOrEmpty(month) ? "/api/budgets" : $"/api/budgets?month={month}";
        var response = await _http.GetFromJsonAsync<List<BudgetDto>>(url)
            ?? throw new InvalidOperationException("Failed to retrieve budgets from the API.");
        return response;
    }

    public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/budgets", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<BudgetDto>()
            ?? throw new InvalidOperationException("API returned an empty response when creating a budget.");
        NotifyDataChanged();
        return created;
    }

    public async Task<BudgetDto> UpdateBudgetAsync(Guid id, UpdateBudgetRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/budgets/{id}", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<BudgetDto>()
            ?? throw new InvalidOperationException("API returned an empty response when updating a budget.");
        NotifyDataChanged();
        return updated;
    }

    public async Task<bool> DeleteBudgetAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"/api/budgets/{id}");
        response.EnsureSuccessStatusCode();
        NotifyDataChanged();
        return true;
    }
    #endregion

    #region Savings Goals
    public async Task<List<SavingsGoalDto>> GetSavingsGoalsAsync()
    {
        var response = await _http.GetFromJsonAsync<List<SavingsGoalDto>>("/api/savings-goals")
            ?? throw new InvalidOperationException("Failed to retrieve savings goals from the API.");
        return response;
    }

    public async Task<SavingsGoalDto> CreateSavingsGoalAsync(CreateSavingsGoalRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/savings-goals", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<SavingsGoalDto>()
            ?? throw new InvalidOperationException("API returned an empty response when creating a savings goal.");
        NotifyDataChanged();
        return created;
    }

    public async Task<SavingsGoalDto> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/savings-goals/{id}", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<SavingsGoalDto>()
            ?? throw new InvalidOperationException("API returned an empty response when updating a savings goal.");
        NotifyDataChanged();
        return updated;
    }

    public async Task<SavingsGoalDto> ContributeToSavingsGoalAsync(Guid id, ContributeRequest request)
    {
        var response = await _http.PostAsJsonAsync($"/api/savings-goals/{id}/contribute", request);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<SavingsGoalDto>()
            ?? throw new InvalidOperationException("API returned an empty response when contributing to a savings goal.");
        NotifyDataChanged();
        return updated;
    }

    public async Task<bool> DeleteSavingsGoalAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"/api/savings-goals/{id}");
        response.EnsureSuccessStatusCode();
        NotifyDataChanged();
        return true;
    }
    #endregion

    #region Reports
    public async Task<SpendingByCategoryReportDto> GetSpendingByCategoryReportAsync(DateTime from, DateTime to)
    {
        var url = $"/api/reports/spending-by-category?from={from:O}&to={to:O}";
        var response = await _http.GetFromJsonAsync<SpendingByCategoryReportDto>(url)
            ?? throw new InvalidOperationException("Failed to retrieve spending by category report from the API.");
        return response;
    }

    public async Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(DateTime from, DateTime to, string granularity = "monthly")
    {
        var url = $"/api/reports/income-vs-expense?from={from:O}&to={to:O}&granularity={granularity}";
        var response = await _http.GetFromJsonAsync<IncomeVsExpenseReportDto>(url)
            ?? throw new InvalidOperationException("Failed to retrieve income vs expense report from the API.");
        return response;
    }
    #endregion
}
