namespace Models.Entities;

public enum DocumentStatus
{
    InProcess = 0,
    Rejected = 1,
    Completed = 2
}

public static class DocumentStatusExtensions
{
    public static DocumentStatus ToDocumentStatus(this Int32 documentStatus)
    {
        if (documentStatus == 0)
            return DocumentStatus.InProcess;
        if (documentStatus == 1)
            return DocumentStatus.Rejected;
        
        return DocumentStatus.Completed;
    }
}