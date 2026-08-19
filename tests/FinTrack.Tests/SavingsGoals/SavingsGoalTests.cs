// Savings goal tests for /api/savings-goals
// Activate tests (remove Skip) once savings goal endpoints are implemented (#19).

using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.SavingsGoals;

public class SavingsGoalTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // POST /api/savings-goals
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: POST /api/savings-goals (#19)")]
    public async Task CreateGoal_ValidData_Returns201WithAllFields()
    {
        // Assert body: id, name, targetAmount, currentAmount, progressPercent, targetDate, isAchieved
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals (#19)")]
    public async Task CreateGoal_Name100Chars_Returns201()
    {
        // Boundary: exactly at the limit must succeed
        var name = new string('a', 100);
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals (#19)")]
    public async Task CreateGoal_Name101Chars_Returns400()
    {
        var name = new string('a', 101);
        throw new NotImplementedException();
    }

    [Theory(Skip = "Pending: POST /api/savings-goals (#19)")]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateGoal_TargetAmountNotPositive_Returns400(decimal targetAmount)
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals (#19)")]
    public async Task CreateGoal_TargetDateYesterday_Returns400()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals (#19)")]
    public async Task CreateGoal_TargetDateToday_Returns400()
    {
        // "in the future" means strictly after today
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals (#19)")]
    public async Task CreateGoal_TargetDateTomorrow_Returns201()
    {
        // Boundary: tomorrow must pass
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // GET /api/savings-goals
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: GET /api/savings-goals (#19)")]
    public async Task GetGoals_Returns200WithArray()
    {
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // POST /api/savings-goals/{id}/contribute
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: POST /api/savings-goals/{id}/contribute (#19)")]
    public async Task Contribute_ValidAmount_Returns200WithUpdatedCurrentAmount()
    {
        // Create goal with targetAmount=1000, currentAmount=0
        // Contribute 250
        // Assert: currentAmount=250, progressPercent=25.0
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals/{id}/contribute (#19)")]
    public async Task Contribute_ProgressPercentCalculatedCorrectly()
    {
        // targetAmount=1000, contribute 350
        // Assert: progressPercent == 35.0
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals/{id}/contribute (#19)")]
    public async Task Contribute_ReachingTarget_SetsIsAchievedTrue()
    {
        // targetAmount=500, contribute 500
        // Assert: isAchieved=true
        throw new NotImplementedException();
    }

    [Theory(Skip = "Pending: POST /api/savings-goals/{id}/contribute (#19)")]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task Contribute_AmountNotPositive_Returns400(decimal amount)
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals/{id}/contribute (#19)")]
    public async Task Contribute_GoalNotFound_Returns404()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/savings-goals/{id}/contribute (#19)")]
    public async Task Contribute_BelongsToAnotherUser_Returns404()
    {
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // PUT /api/savings-goals/{id}
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: PUT /api/savings-goals/{id} (#19)")]
    public async Task UpdateGoal_ValidData_Returns200()
    {
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // DELETE /api/savings-goals/{id}
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: DELETE /api/savings-goals/{id} (#19)")]
    public async Task DeleteGoal_Exists_Returns204()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: DELETE /api/savings-goals/{id} (#19)")]
    public async Task DeleteGoal_BelongsToAnotherUser_Returns404()
    {
        throw new NotImplementedException();
    }
}
