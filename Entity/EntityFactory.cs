using DatabaseLibrary.Interfaces;
using MySql.Data.MySqlClient;
using static DatabaseLibrary.Helper.Helper;

namespace DatabaseLibrary.Entity
{
    public abstract class EntityFactory 
    {

        public static async Task<IEnumerable<VARIANT>> CreateInstanceAsync<VARIANT, VARIANT_ENUM>(IVariantBuilder<VARIANT_ENUM> builder) 
            where VARIANT : class
            where VARIANT_ENUM : Enum
        {
            try
            {
                var (conn, trans) = await Database.UseTransactionAsync();
                await using (conn)
                await using (trans)
                    try
                    {
                        builder.SetTransaction(trans);
                        return await builder.BuildAsync<VARIANT>();
                    }
                    catch (Exception e)
                    {
                        HandleError(e);
                        return Enumerable.Empty<VARIANT>();
                    }
            }
            catch (Exception e)
            {
                HandleError(e);
                return Enumerable.Empty<VARIANT>();
            }
        }

        public static async Task<IEnumerable<VARIANT>> CreateInstanceAsync<VARIANT, VARIAN_ENUM>(MySqlTransaction trans, IVariantBuilder<VARIAN_ENUM> builder) 
            where VARIANT : class 
            where VARIAN_ENUM : Enum
        {
            try
            {
                builder.SetTransaction(trans);
                return await builder.BuildAsync<VARIANT>();
            }
            catch (Exception e)
            {
                HandleError(e);
                return Enumerable.Empty<VARIANT>();
            }
        }

        public static async Task<IEnumerable<OUTPUT>> CreateInstanceAsync<OUTPUT>(IBuilder<OUTPUT> builder) where OUTPUT : class
        {
            try
            {
                var (conn, trans) = await Database.UseTransactionAsync();
                await using (conn)
                await using (trans)
                    try
                    {
                        builder.SetTransaction(trans);
                        return await builder.BuildAsync();
                    }
                    catch (Exception e)
                    {
                        HandleError(e);
                        return Enumerable.Empty<OUTPUT>();
                    }
            }
            catch (Exception e)
            {
                HandleError(e);
                return Enumerable.Empty<OUTPUT>();
            }
        }

        public static async Task<IEnumerable<OUTPUT>> CreateInstanceAsync<OUTPUT>(MySqlTransaction trans, IBuilder<OUTPUT> builder) where OUTPUT : class
        {
            try
            {
                builder.SetTransaction(trans);
                return await builder.BuildAsync();
            }
            catch (Exception e)
            {
                HandleError(e);
                return Enumerable.Empty<OUTPUT>();
            }
        }

    }
}
