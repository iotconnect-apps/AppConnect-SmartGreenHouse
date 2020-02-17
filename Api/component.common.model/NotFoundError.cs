namespace component.common.model
{
    public class NotFoundError
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; } = "Record not found";
    }
}
