namespace InventoryManagementSystem.PublicApi.UtilEndpoints
{
    public class DuplicateCheckerRequest : BaseRequest
    {
        public string SearchQuery { get; set; }
    }
    
    public class DuplicateCheckerResponse : BaseResponse
    {
        public bool HasMatch { get; set; }
        public string MatchId { get; set; }
    }
}