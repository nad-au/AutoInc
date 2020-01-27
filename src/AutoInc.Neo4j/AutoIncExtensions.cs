using Neo4j.Driver;

namespace AutoInc
{
    public static class AutoIncExtensions
    {
        public static void InitialiseUniqueIds(this IDriver driver)
        {
            using (var session = driver.Session(builder => builder.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.InitialiseUniqueIds();
            }
        }

        public static void InitialiseUniqueIds(this ISession session)
        {
            using (var tx = session.BeginTransaction())
            {
                tx.Initialise();
                tx.Commit();
            }
        }

        public static void InitialiseUniqueIds(this ITransaction transaction)
        {
            transaction.Initialise();
        }

        public static void UpdateUniqueId(this IDriver driver, string scope, long value)
        {
            using (var session = driver.Session(builder => builder.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.UpdateUniqueId(scope, value);
            }
        }

        public static void UpdateUniqueId(this ISession session, string scope, long value)
        {
            using (var tx = session.BeginTransaction())
            {
                tx.UpdateUniqueId(scope, value);
                tx.Commit();
            }
        }

        public static void UpdateUniqueId(this ITransaction transaction, string scope, long value)
        {
            transaction.Update(scope, value);
        }

        public static long NextUniqueId(this IDriver driver, string scope)
        {
            using (var session = driver.Session(builder => builder.WithDefaultAccessMode(AccessMode.Write)))
            {
                return session.NextUniqueId(scope);
            }
        }

        public static long NextUniqueId(this ISession session, string scope)
        {
            return session.WriteTransaction(
                transaction => transaction.NextUniqueId(scope));
        }

        public static long NextUniqueId(this ITransaction transaction, string scope)
        {
            return transaction.NextId(scope);
        }
    }
}
