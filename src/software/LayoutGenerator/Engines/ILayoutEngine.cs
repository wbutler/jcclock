using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace JCClock.LayoutGenerator.Engines
{
    interface ILayoutEngine
    {
        IEnumerable<Layout> AttemptLayout(int width, int height, IEnumerable<LayoutPhrase> phrases, ILogger logger);
    }
}
