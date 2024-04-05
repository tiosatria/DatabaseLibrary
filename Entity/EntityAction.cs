using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace DatabaseLibrary.Entity
{
    public abstract class EntityAction<IActionTypeEnum, T>
        where IActionTypeEnum : Enum
        where T : EntityAction<IActionTypeEnum, T>
    {

        protected EntityAction(params string[] allowed)
        {
            ActionData = new();
            AllowedMessage = allowed;
            ActionPayload = new();
        }

        protected void Initialize(T instance)
        {
            Instance = instance;
            DelegateApplyTask = instance.ActionMapper;
        }

        #region Prop

        protected T Instance { get; set; }
        public delegate Task<bool> DelActionMapper(IActionTypeEnum action);
        public DelActionMapper DelegateApplyTask { get; protected set; }
        private MySqlTransaction? _transaction { get; set; }
        protected IActionTypeEnum[]? _actions { get; set; }
        public string Message { get; protected set; } = "unknown_error";
        protected string[]? AllowedMessage { get; set; }
        public Dictionary<string, object> ActionData;
        protected readonly Dictionary<string, object?> ActionPayload;

        protected MySqlTransaction TransCTX =>
            _transaction ?? throw new InvalidOperationException("please_supply_transaction");

        private List<Task<bool>> getActionTask() => _actions?.Select(DelegateApplyTask.Invoke).ToList() ?? new();

        public abstract Task<bool> ActionMapper(IActionTypeEnum action);

        #endregion

        #region Setter

        public EntityAction<IActionTypeEnum, T> AddPayload(IActionTypeEnum act, object obj)
        {
            ActionPayload.Add(act.ToString(), obj);
            return this;
        }

        protected EntityAction<IActionTypeEnum, T> SetTransaction(MySqlTransaction transaction)
        {
            _transaction = transaction;
            return this;
        }

        public EntityAction<IActionTypeEnum, T> SetAction(params IActionTypeEnum[] actions)
        {
            this._actions = actions;
            return this;
        }

        public EntityAction<IActionTypeEnum, T> AddAction(params IActionTypeEnum[] actions)
        {
            if (actions.Length == 0) return this;
            var currentLength = _actions?.Length ?? 0;
            var newActions = new IActionTypeEnum[currentLength + actions.Length];
            if (_actions != null)
                _actions.CopyTo(newActions, 0);
            actions.CopyTo(newActions, currentLength);
            _actions = newActions;
            return this;
        }

        protected X? GetPayloadVal<X>(string action) 
        {
            return (X?)ActionPayload.GetValueOrDefault(action);
        }

        #endregion

        #region Method

        protected void HandleError(Exception e)
        {
            if (Debugger.IsAttached || (AllowedMessage != null && AllowedMessage.Contains(e.Message)))
            {
                Message = e.Message;
            }
            else
            {
                Helper.Helper.HandleError(e);
            }
        }

        public async Task<bool> ExecuteAsync()
        {
            if (_actions is null || _actions?.Length <= 0)
                throw new InvalidOperationException("no_operation_performed");
            if (_transaction != null)
            {
                var tasks = getActionTask();
                foreach (var task in tasks)
                {
                    await task;
                }
                return tasks.Count(e => e.Result) == tasks.Count;
            }
            var t = new Transaction();
            try
            {
                await t.BeginTransactionAsync();
                _transaction = t.TransCTX;
                var tasks = getActionTask();
                foreach (var task in tasks)
                {
                    await task;
                }
                var succ = tasks.Count(e => e.Result) == tasks.Count;
                return await t.FinishTransactionAsync(succ);
            }
            catch (Exception e)
            {
                HandleError(e);
                return await t.FinishTransactionAsync();
            }
        }

        #endregion
        
    }
}
