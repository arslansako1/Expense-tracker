

public record ConfirmImportRequest( int AccountId, int? CategoryId, List<TransactionPreviewResponse> Transactions);