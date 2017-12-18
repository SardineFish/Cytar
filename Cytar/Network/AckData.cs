using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar.Network
{
    class AckData
    {
        public uint PackageSequence;
        public uint DataSequence;

        public AckData(uint packageSequence, uint dataSequence)
        {
            PackageSequence = packageSequence;
            DataSequence = dataSequence;
        }
    }
}
