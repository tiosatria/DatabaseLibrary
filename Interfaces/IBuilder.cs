using DatabaseLibrary.Entity;
using MySql.Data.MySqlClient;

namespace DatabaseLibrary.Interfaces
{
    public interface IBuilder<T> where T : class
    {
        public int limit { get; set; }
        public (QueryBuilder.OrderQueryTypeEnum order, string col) order { get; set; }

        public EntityBuilder SetTransaction(MySqlTransaction trans);
        public EntityBuilder SetLimit(int limit);
        public EntityBuilder Order((QueryBuilder.OrderQueryTypeEnum order, string col) ordTuple);
        public Task<IEnumerable<T>> BuildAsync();
    }

    public interface IVariantBuilder<VariantEnums> where VariantEnums : Enum
    {
        public int limit { get; set; }
        public (QueryBuilder.OrderQueryTypeEnum order, string col) order { get; set; }
        public EntityBuilder SetTransaction(MySqlTransaction trans);
        public EntityBuilder SetLimit(int limit);
        public EntityBuilder Order((QueryBuilder.OrderQueryTypeEnum order, string col) ordTuple);
        public VariantEnums variant { get; set; }
        public EntityBuilder SetVariant(VariantEnums variant);
        public Task<IEnumerable<VARIANT>> BuildAsync<VARIANT>();
    }

}
