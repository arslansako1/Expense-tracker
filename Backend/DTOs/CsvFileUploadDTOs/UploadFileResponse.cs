

public record UploadFileResponse(int TotalRows, int SuccessCount, int ErrorCount, List<TransactionPreviewResponse> ValidTransactions, List<ImportErrorsResponse> Errors);


