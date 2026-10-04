

public record CreateRecurringRuleRequest(int AccountId, decimal Amount, int CategoryId, string Frequency, DateTime NextRunDate);