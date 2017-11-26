using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class APIContextChildren: List<APIContext>
    {
        public APIContext CurrentContext { get; private set; }
        public new void Add(APIContext context)
        {
            context.Parent = CurrentContext;
            if (!Contains(context))
                base.Add(context);
        }

        public APIContextChildren(APIContext context)
        {
            CurrentContext = context;
        }
    }
}
