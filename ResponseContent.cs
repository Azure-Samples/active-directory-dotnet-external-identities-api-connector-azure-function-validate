namespace Sample.ExternalIdentities
{
     public class ResponseContent
    {
        public const string ApiVersion = "0.0.1";

        public ResponseContent()
        {
            this.version = ResponseContent.ApiVersion;
            this.action = "Continue";
        }

        public ResponseContent(string action, string userMessage, string status='')
        {
            this.version = ResponseContent.ApiVersion;
            this.action = action;
            this.userMessage = userMessage;
            if(status=='400'){
                this.status = '400'
            }
        }

        public string version { get; }
        public string action { get; set; }
        public string userMessage { get; set; }
    }
}
