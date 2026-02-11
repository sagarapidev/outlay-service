using DotNetEnv;

public static class EnvConfig
{
    public static void Load(IConfigurationBuilder config)
    {
        // Load .env file into environment variables (local only)
        Env.Load();

        // Merge environment variables into configuration
        config.AddEnvironmentVariables();
    }
}