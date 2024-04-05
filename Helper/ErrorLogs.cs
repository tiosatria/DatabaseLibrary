
namespace DatabaseLibrary.Helper
{
    public class ErrorLogs
    {

        private const string _tName = "sys_error_logs";

        private QueryBuilder QPersistent => new QueryBuilder(QueryBuilder.QueryTypeEnums.INSERT, _tName)
            .AddInsertValue(
                ("module", "@Module"),
                ("msg", "@Message"),
                ("trigger", "@Trigger"))
            .AddParams(this);

        #region Prop
        private static List<ErrorLogs> Collections { get; set; } = new();
        public int? ID { get; set; }
        public DateTime Time { get; set; }
        public string? Module { get; init; }
        public string? Message { get; init; }
        public string? Trigger { get; init; }
        #endregion

        #region Ctor
        public ErrorLogs() { }

        public ErrorLogs(string module, string? message, string? trigger)
        {
            Module = module;
            Message = message;
            Trigger = trigger;
        }

        #endregion

        #region Method

        public void AddToCollection()
        {
            Collections.Add(this);
        }

        #endregion

        public static async Task<bool> MakePersistent()
        {
            var (conn, trans) = await Database.UseTransactionAsync();
            await using (conn)
            await using (trans)
                try
                {
                    var persistenceTasks = Collections.Select(e => e.QPersistent.SetTransaction(trans).ExecuteDMLAsync()).ToList();
                    var persisted = persistenceTasks.Count(e=>e.Result > 0) == persistenceTasks.Count;
                    await Task.WhenAll(persistenceTasks);
                    if (!persisted) return persisted;
                    await trans.CommitAsync();
                    Collections.Clear();
                    return persisted;
                }
                catch (Exception e)
                {
                    Helper.HandleError(e);
                    await trans.RollbackAsync();
                    return false;
                }
        } 

    }
}
