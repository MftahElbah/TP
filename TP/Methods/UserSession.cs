using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Methods
{
    // to help use userid and other thing in operation - offline
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static string Name { get; set; }
        public static string Password { get; set; }
        public static int UserType { get; set; }
        public static bool SessionYesNo {  get; set; }

        public static bool internet { get; set; }
    }
}
