using IocExample.Classes;
using Ninject;

namespace IocExample
{
    class Program
    {
        /*static void Main(string[] args)
        {
            var logger = new ConsoleLogger();
            var sqlConnectionFactory = new SqlConnectionFactory("SQL Connection", logger);
            var createUserHandler =
                new CreateUserHandler(
                    new UserService(new QueryExecutor(sqlConnectionFactory), new CommandExecutor(sqlConnectionFactory),
                        new CacheService(logger, new RestClient("API KEY"))), logger);

            createUserHandler.Handle();
        }

        /*static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new UserModule());
            var createUserHandler = kernel.Get<CreateUserHandler>();
            createUserHandler.Handle();
        }*/
        
        static void Main(string[] args)
        {
            var resolver = new DependencyResolver();
            resolver.Bind<CreateUserHandler, CreateUserHandler>();
            resolver.Bind<UserService, UserService>();
            resolver.Bind<ILogger, ConsoleLogger>();
            resolver.Bind<QueryExecutor, QueryExecutor>();
            resolver.Bind<CommandExecutor, CommandExecutor>();
            resolver.Bind<CacheService, CacheService>();
            
            resolver.Bind<IConnectionFactory, SqlConnectionFactory>(
                () => new SqlConnectionFactory("SQL Connection", resolver.Get<ILogger>()), isSingletonScope: true);

            resolver.Bind<RestClient, RestClient>(
                () => new RestClient("API Key"), isSingletonScope: true);

            var createUserHandler = resolver.Get<CreateUserHandler>();
            createUserHandler.Handle();
        }
    }
}
