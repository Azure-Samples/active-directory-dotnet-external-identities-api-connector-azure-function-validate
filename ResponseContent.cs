namespace Sample.ExternalIdentities
{
     public class ResponseContent
    {
        public ResponseContent()
        {
            version = "1.0.1";
            this.action = "Allow";
        }

        public ResponseContent(string action, string code, string userMessage)
        {
            version = "1.0.1";
            this.action = action;
            this.code = code;
            this.userMessage = userMessage;
        }

        public string version { get; set; }
        public string action { get; set; }
        public string code { get; set; }
        public string userMessage { get; set; }
    }
}
