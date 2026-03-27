using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorApp.Model
{
    public class CalculatorModel
    {
        public double lastNumber;
        public string? currentOperator;
        public bool isNewNumber;

        /// <summary>
        /// 内部データの初期化
        /// </summary>
        public void InitializeData()
        {
            lastNumber = 0;
            currentOperator = null;
            isNewNumber = true;
        }

        public double Calculate(double currentNumber)
        {
            var result = 0.0;
            switch (currentOperator)
            {
                case "+":
                    result = lastNumber + currentNumber;
                    break;
                case "-":
                    result = lastNumber - currentNumber;
                    break;
                case "×":
                    result = lastNumber * currentNumber;
                    break;
                case "÷":
                    if (currentNumber != 0)
                    {
                        result = lastNumber / currentNumber;
                    }
                    else
                    {
                        result = double.NaN; // ゼロ除算
                    }
                    break;
                case "%":
                    result = lastNumber % currentNumber;
                    break;
                default:
                    result = currentNumber;
                    break;
            }
            lastNumber = result;
            return result;
        }

    }
}
