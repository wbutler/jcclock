using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCClock.LayoutGenerator.Engines
{
    interface ILayoutEngine
    {
        LayoutResult AttemptLayout(int width, int height, IEnumerable<LayoutPhrase> phrases);
    }
}
