namespace A2Algo.Inventory.Contracts.Responses
{
    public record BaseResponse
    (
         int StatusCode,
         string Message,
         object? Errors,
         object? Data
    );
}
