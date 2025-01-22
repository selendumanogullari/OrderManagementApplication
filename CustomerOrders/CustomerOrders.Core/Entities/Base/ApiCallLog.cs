namespace CustomerOrders.Core.Entities.Base
{
    public class ApiCallLog
    {
        public int Id { get; set; }  
        public string RequestId { get; set; }  
        public string Endpoint { get; set; }  
        public string HttpMethod { get; set; } 
        public string ClientIp { get; set; }  
        public string UserAgent { get; set; } 
        public string Username { get; set; } 
        public string UserId { get; set; }  
        public string AuthToken { get; set; }  
        public DateTime RequestDate { get; set; } 
        public TimeSpan Duration { get; set; } 
        public bool IsSuccess { get; set; } 
        public string ResponseMessage { get; set; }  
        public string ErrorDetails { get; set; } 
    }
}
