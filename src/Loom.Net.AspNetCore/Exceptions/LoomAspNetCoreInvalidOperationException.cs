namespace Loom.Net.AspNetCore.Exceptions
{
    internal sealed class LoomAspNetCoreInvalidOperationException : LoomAspNetCoreException
    {
        public LoomAspNetCoreInvalidOperationException(string message) : base(message)
        {
        }
    }
}
