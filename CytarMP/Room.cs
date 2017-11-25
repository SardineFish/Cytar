using System;
using System.Collections.Generic;
using System.Text;

namespace CytarMP
{
    public abstract class Room
    {
        public List<Room> SubRooms { get; set; }

        public List<Session> Sessions { get; set; }
    }
}
