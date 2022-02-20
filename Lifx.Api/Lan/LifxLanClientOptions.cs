using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lifx.Api.Lan
{
    public class LifxLanClientOptions
    {
        public ILogger Logger { get; set; } = NullLogger.Instance;
    }
}