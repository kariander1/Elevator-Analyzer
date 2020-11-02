using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevators
{
    class Request
    {
        Floor destFloor;
        Floor SourceFloor;
        bool ascending;
        public Request(Floor src, Floor dst)
        {
            this.destFloor = dst;
            this.SourceFloor = src;
            ascending = (dst.GetIndex() - src.GetIndex()) > 0;
        }
        public bool IsAscending()
        {
            return ascending;
        }
        public Floor GetDestination()
        {
            return this.destFloor;
        }
        public Floor GetSource()
        {
            return this.SourceFloor;
        }
        public override string ToString()
        {
            return "From F " + this.SourceFloor.GetIndex() + " To F " + this.destFloor.GetIndex();
        }
    }
}
