namespace EcommerceAPI.Shared.Helpers
{
    public class RequestResult<T>
    {
        public ActionResultType ActionResultType { get; set; }
        public bool Success => !string.IsNullOrWhiteSpace(ErrorText);
        public string ErrorText { get; set; }
        public T? Data { get; set; }
        public RequestResult(T? data, ActionResultType actionResultType = ActionResultType.OK, string error = "")
        {
            ErrorText = error ?? "";
            Data = data;
            ActionResultType = actionResultType;
        }
        public RequestResult(ActionResultType actionResultType, string error = "")
        {
            ErrorText = error ?? "";
            ActionResultType = actionResultType;
            Data = default;
        }
    }
    public class ListResponse<T>
    {
        public T[] Items { get; set; }
        public int TotalRecords { get; set; }
    }
    public enum ActionResultType
    {
        OK = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        StatusCode634 = 634
    }
}