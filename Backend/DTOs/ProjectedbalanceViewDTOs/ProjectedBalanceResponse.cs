

public record ProjectedBalanceResponse
(
    decimal CurrentBalance,
    List<ProjectionItem> Projections
);