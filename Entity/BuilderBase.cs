using MySql.Data.MySqlClient;

namespace DatabaseLibrary.Entity
{
    public abstract class EntityBuilder
    {
        public int limit { get; set; }
        private QueryBuilder.OrderQueryTypeEnum _ORDER_QUERY_TYPE;
        private string _col;

        public (QueryBuilder.OrderQueryTypeEnum order, string col) order
        {
            get
            {
                return (_ORDER_QUERY_TYPE, _col);
            }
            set
            {
                if (Enum.TryParse<QueryBuilder.OrderQueryTypeEnum>(value.col, true, out var parsedOrder))
                {
                    _ORDER_QUERY_TYPE = parsedOrder;
                    _col = value.col;
                }
                else
                {
                    _ORDER_QUERY_TYPE = QueryBuilder.OrderQueryTypeEnum.TRUE;
                    _col = "";
                }
            }
        }
        private MySqlTransaction? _transaction;

        protected MySqlTransaction TransCtx
            => _transaction ?? throw new InvalidOperationException("Transaction is not supplied.");



        public virtual EntityBuilder SetTransaction(MySqlTransaction trans)
        {
            _transaction = trans;
            return this;
        }

        public virtual EntityBuilder SetLimit(int limit)
        {
            this.limit = limit;
            return this;
        }

        public virtual EntityBuilder Order((QueryBuilder.OrderQueryTypeEnum order, string col) ordTuple)
        {
            _ORDER_QUERY_TYPE = ordTuple.order;
            _col = ordTuple.col;
            return this;
        }

    }
}