using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLibrary
{
    public struct Cookie
    {
        public string DeviceId { set; get; }
        public string Password { set; get; }
        public Time LoginTime { set; get; }
    }
}
