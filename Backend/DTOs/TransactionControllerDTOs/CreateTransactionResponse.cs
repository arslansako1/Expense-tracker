

public record CreateTransactionResponse(int Id, int AccountId, decimal Amount, string Type, string Description, DateTime CreatedAt, DateTime Date, int? CategoryId);
