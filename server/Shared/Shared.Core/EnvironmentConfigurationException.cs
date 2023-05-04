namespace Shared.Core;

public sealed class EnvironmentConfigurationException : Exception
{
    public EnvironmentConfigurationException(string message): base(message)
    {
    }
}
