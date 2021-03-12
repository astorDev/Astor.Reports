namespace Astor.Reports.Domain
{
    public interface IRowsStoreFactory
    {
        IRowsStore GetRowsStore(string reportId);
    }
}