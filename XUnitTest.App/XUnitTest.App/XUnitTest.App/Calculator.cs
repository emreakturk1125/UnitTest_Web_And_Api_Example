using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTest.App
{
    public class Calculator
    {
        private ICalculatorService _calculatorService;
        public Calculator(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public int Add(int a, int b)
        {
            return _calculatorService.Add(a, b);
        }

        public int Multiply(int a, int b)
        {
            return _calculatorService.Multiply(a, b);
        }
    }
}
