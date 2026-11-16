namespace Loom.Net.AspNetCore.Exceptions
{
    public abstract class LoomAspNetCoreException : Exception
    {
        protected LoomAspNetCoreException(string message) : base(message)
        {
            
        }
    }
}
