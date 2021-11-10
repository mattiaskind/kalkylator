using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCalc
{
    // En klass med egenskaper som representerar en uträkning
    class MathOperation
    {
        public double Number1 { get; set; } = 0;
        public double Number2 { get; set; } = 0;
        public string MathOperator { get; set; } = "";
        public double Result { get; set; } = 0;

    }
}
