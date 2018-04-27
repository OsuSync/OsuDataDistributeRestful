using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful.Server
{
    class RouteAttribute : Attribute
    {
        public string Route { get; private set; }

        public RouteAttribute(string route)
        {
            Route = route;
        }
    }
}
