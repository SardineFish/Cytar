using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public interface IAPIContext
    {
        object CallAPI(string name, params object[] param);

        APIInfo GetAPI(string name);
    }
}
