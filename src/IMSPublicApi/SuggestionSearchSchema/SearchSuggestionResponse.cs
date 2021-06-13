using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.SuggestionSearchSchema
{
    public class SearchSuggestionResponse : BaseResponse
    {
        public List<string> Suggestions { get; set; } = new List<string>();
    }
}