namespace Infrastructure.Data
{
    public class AuthenticationConstants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string IMSAccountantRole = 
            "IMSAccountant";
        public static readonly string IMSManagersRole = "IMSManager";
        
        public static readonly string IMSStockKeeperRole = "IMSStockKeeper";
        
        public static readonly string IMSEmployee = "IMSEmployee";
    }
}