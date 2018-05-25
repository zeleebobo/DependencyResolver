using IocExample.Classes;
using Ninject.Modules;

namespace IocExample
{
    public class UserModule : NinjectModule
    {
        public override void Load()
        {
            Bind<CreateUserHandler>().To<CreateUserHandler>();
            Bind<UserService>().To<UserService>();
            Bind<ILogger>().To<ConsoleLogger>()
                .InSingletonScope();
            Bind<QueryExecutor>().To<QueryExecutor>();
            Bind<CommandExecutor>().To<CommandExecutor>();
            Bind<CacheService>().To<CacheService>();
            Bind<IConnectionFactory>()
                .ToConstructor(k => new SqlConnectionFactory("SQL Connection", k.Inject<ILogger>()))
                .InSingletonScope();
            Bind<RestClient>().ToConstructor(k => new RestClient("API Key"));

        }
    }
}