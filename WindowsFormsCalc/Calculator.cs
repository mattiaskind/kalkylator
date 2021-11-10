using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCalc
{
    // En klass för att genomföra uträkningae
    class Calculator
    {
        // Metoden sköter alla uträkningar genom att anropa metoder motsvarande den matematiska operatorn
        public static double DoOperation(double number1, double number2, string op)
        {
            double result = 0;
            switch (op)
            {
                case "+":
                    result = Add(number1, number2);
                    break;
                case "-":
                    result = Subtract(number1, number2);
                    break;
                case "/":
                    // Hindrar division med noll
                    if (number2 != 0)
                    {
                        result = Divide(number1, number2);
                    }
                    break;
                case "x":
                    result = Multiply(number1, number2);
                    break;
            }
            return Math.Round(result, 12);
        }
        // En metod för respektive uträkning
        private static double Add(double number1, double number2)
        {
            return number1 + number2;
        }
        private static double Subtract(double number1, double number2)
        {
            return number1 - number2;
        }
        private static double Divide(double number1, double number2)
        {
            return number1 / number2;
        }
        private static double Multiply(double number1, double number2)
        {
            return number1 * number2;
        }
    }
}
