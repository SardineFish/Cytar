using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar.Unity
{
    public static class CytarForUnity
    {
        public static void Extent()
        {
            CytarDeserializeUnity.ExtentToCytar();
            CytarSerializeUnity.ExtentToCytar();
        }
    }
}
