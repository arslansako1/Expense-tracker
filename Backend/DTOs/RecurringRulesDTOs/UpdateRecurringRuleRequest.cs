

public record UpdateRecurringRuleRequest(int AccountId, decimal Amount, int CategoryId, string Frequency, DateTime NextRunDate);
