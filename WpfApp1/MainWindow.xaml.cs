
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace KalkulatorWPF
{
    public partial class MainWindow : Window
    {
        private double firstNumber = 0;
        private string currentOperator = "";
        private bool startNewNumber = true;
        private bool error = false;

        private readonly CultureInfo culture =
            CultureInfo.GetCultureInfo("pl-PL");

        public MainWindow()
        {
            InitializeComponent();
        }

        private double CurrentValue
        {
            get
            {
                if (double.TryParse(
                    Display.Text,
                    NumberStyles.Float,
                    culture,
                    out double result))
                    return result;

                return 0;
            }
        }

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            if (error)
                Clear_Click(sender, e);

            string digit = ((Button)sender).Content.ToString();

            if (startNewNumber || Display.Text == "0")
            {
                Display.Text = digit;
                startNewNumber = false;
            }
            else if (Display.Text.Length < 16)
            {
                Display.Text += digit;
            }
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (error)
                Clear_Click(sender, e);

            if (startNewNumber)
            {
                Display.Text = "0,";
                startNewNumber = false;
            }
            else if (!Display.Text.Contains(","))
            {
                Display.Text += ",";
            }
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (error)
                return;

            string op = ((Button)sender).Tag.ToString();

            if (!startNewNumber && currentOperator != "")
            {
                if (!Calculate())
                    return;
            }

            firstNumber = CurrentValue;
            currentOperator = op;
            HistoryText.Text =
                $"{Display.Text} {OperatorSymbol(op)}";
            startNewNumber = true;
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (error || currentOperator == "")
                return;

            string expression =
                $"{Format(firstNumber)} {OperatorSymbol(currentOperator)} " +
                $"{Display.Text} =";

            if (Calculate())
            {
                HistoryText.Text = expression;
                currentOperator = "";
                startNewNumber = true;
            }
        }

        private bool Calculate()
        {
            double secondNumber = CurrentValue;
            double result;

            switch (currentOperator)
            {
                case "+":
                    result = firstNumber + secondNumber;
                    break;

                case "-":
                    result = firstNumber - secondNumber;
                    break;

                case "*":
                    result = firstNumber * secondNumber;
                    break;

                case "/":
                    if (secondNumber == 0)
                    {
                        Display.Text = "Błąd";
                        HistoryText.Text = "Nie można dzielić przez zero";
                        currentOperator = "";
                        startNewNumber = true;
                        error = true;
                        return false;
                    }

                    result = firstNumber / secondNumber;
                    break;

                default:
                    return false;
            }

            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                Display.Text = "Błąd";
                currentOperator = "";
                startNewNumber = true;
                error = true;
                return false;
            }

            Display.Text = Format(result);
            firstNumber = result;
            startNewNumber = true;
            return true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
            HistoryText.Text = "";
            firstNumber = 0;
            currentOperator = "";
            startNewNumber = true;
            error = false;
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (error)
            {
                Clear_Click(sender, e);
                return;
            }

            if (startNewNumber)
                return;

            if (Display.Text.Length <= 1 ||
                (Display.Text.Length == 2 && Display.Text[0] == '-'))
            {
                Display.Text = "0";
            }
            else
            {
                Display.Text =
                    Display.Text.Substring(0, Display.Text.Length - 1);
            }
        }

        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            if (error)
                return;

            if (CurrentValue == 0)
                return;

            Display.Text = Format(-CurrentValue);
        }

        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            if (error)
                return;

            Display.Text = Format(CurrentValue / 100.0);
            startNewNumber = true;
        }

        private string Format(double value)
        {
            return value.ToString("G15", culture);
        }

        private string OperatorSymbol(string op)
        {
            switch (op)
            {
                case "*": return "×";
                case "/": return "÷";
                case "-": return "−";
                case "+": return "+";
                default: return op;
            }
        }
    }
}
