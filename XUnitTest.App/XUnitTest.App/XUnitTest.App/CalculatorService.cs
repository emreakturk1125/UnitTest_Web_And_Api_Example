using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTest.App
{
    public class CalculatorService : ICalculatorService
    {
        public int Add(int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return 0;
            }
            return a + b;
        }

        public int Multiply(int a, int b)
        {
            return a + b;
        }
    }
}
