using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Methods
{
    public class DegreeTableView
    {
        public string StdName { get; set; }
        public float Degree { get; set; }
        public float MidDegree { get; set; }

        public float Total
        {
            get { return Degree + MidDegree; }
        }
    }
}
