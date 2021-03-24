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

        public ResponseContent(string action, string userMessage)
        {
            this.version = ResponseContent.ApiVersion;
            this.action = action;
            this.userMessage = userMessage;
            if (action != "Continue")
            {
                this.status = "400";
            }
        }

        public string version { get; }
        public string action { get; set; }
        public string userMessage { get; set; }
        public string status { get; set; }
    }
}
