

public record CreateTransferResponse(int Id, decimal Amount, string Description, int FromAccountId, int ToAccountId, DateTime Date, DateTime CreatedAt);
