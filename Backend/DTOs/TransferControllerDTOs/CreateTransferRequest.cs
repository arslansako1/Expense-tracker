

public record CreateTransferRequest(decimal Amount, string Description, int FromAccountId, int ToAccountId, DateTime Date);