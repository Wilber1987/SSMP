namespace RabbitMQService
{
    public class MessageDto
    {

        public MessagesConstants.Types TypeEnum { set; get; }

        private int type;


        public int Type
        {
            get { return (int)TypeEnum; }
            set { type = value; }
        }


        public string Message { get; set; }
        public List<string> Messages { get; set; }

        public string TransactionNumber { get; set; }

        public string ErrorCode { get; set; }

        public dynamic ResultObject { get; set; }

        public MessageDto()
        {
            //this.Messages = new List<string>();
            this.TypeEnum = MessagesConstants.Types.Information;
            this.Messages = new List<string>();
            Message = string.Empty;
        }

        public void AddError(string errorMessage)
        {
            this.TypeEnum = MessagesConstants.Types.Error;
            this.Messages.Add(errorMessage);
            this.Message = errorMessage;
        }

        public void AddWarning(string warningMessage)
        {
            if (this.TypeEnum == MessagesConstants.Types.Information) this.TypeEnum = MessagesConstants.Types.Warning;
            this.Messages.Add(warningMessage);
            this.Message = warningMessage;
        }

        public bool HasError()
        {
            return (this.TypeEnum == MessagesConstants.Types.Error);
        }

        public bool HasWarning()
        {
            return (this.TypeEnum == MessagesConstants.Types.Warning);
        }

        public bool HasErrorOrWarning()
        {
            return (this.TypeEnum == MessagesConstants.Types.Error || this.TypeEnum == MessagesConstants.Types.Warning);
        }

        public void SetToOk(string message)
        {
            TypeEnum = MessagesConstants.Types.Information;
            Message = message;
        }

        public void SetToOk(string message, string transactionNumber)
        {
            TypeEnum = MessagesConstants.Types.Information;
            Message = message;
            TransactionNumber = transactionNumber;
        }
    }


    public class MessagesConstants
    {
        public enum Types
        {
            None = 0,
            Information = 1,
            Warning = 2,
            OperationalWarnning = 3,
            Error = 4,
            OperationalError = 5,   
            ReturnError //Debe retornar obligatoriamente de la función en donde este.
        }
    }

}
