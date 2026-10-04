

public record CreateTransactionRequest(int AccountId, decimal Amount, string Type, string Description, DateTime Date, int? CategoryId);