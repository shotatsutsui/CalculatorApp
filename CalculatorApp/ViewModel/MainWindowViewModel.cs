using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using CalculatorApp.Model;
using System.Reflection.Metadata;

namespace CalculatorApp.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 電卓アプリのModelクラスを持つ。
        /// </summary>
        private CalculatorModel CalculatorModel { get; set; }

        // Viewに表示する計算結果/入力値
        private string _formulanText;
        public string FormulaText
        {
            get => _formulanText;
            set
            {
                if (_formulanText != value)
                {
                    _formulanText = value;
                    OnPropertyChanged();
                }
            }
        }

        // Viewに表示する計算結果/入力値
        private string _displayText;
        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
        }

        // コマンドのプロパティ定義
        public ICommand NumberButtonCommnad { get; private set; }
        public ICommand OperatorButtonCommnad { get; private set; }
        public ICommand EqualButtonCommnad { get; private set; }
        public ICommand ACButtonCommnad { get; private set; }

        public ICommand PlusMinusButtonCommnad { get; private set; }

        public ICommand DotButtonCommnad { get; private set; }



        public MainWindowViewModel()
        {
            // ModelとViewModelを紐づける
            CalculatorModel = new CalculatorModel();

            // プロパティの初期化
            _displayText = "0";
            _formulanText = "";

            // コマンドの初期化（実際の処理は各メソッドに記述）
            NumberButtonCommnad = new RelayCommand(NumberButton_Click);
            OperatorButtonCommnad = new RelayCommand(OperatorButton_Click);
            EqualButtonCommnad = new RelayCommand(EqualButton_Click);
            ACButtonCommnad = new RelayCommand(ACButton_Click);
            PlusMinusButtonCommnad = new RelayCommand(PlusMinusButton_Click);
            DotButtonCommnad = new RelayCommand(DotButton_Click);

        }

        /// <summary>
        /// 数字ボタンが押されたとき、ディスプレイに数字を表示する
        /// </summary>
        /// <param name="parameter"></param>
        private void NumberButton_Click(object? parameter)
        {
            if (parameter is not string digit)
            {
                return;
            }

            // 新しい計算の開始、または現在の表示が"0"の場合
            if (CalculatorModel.isNewNumber || DisplayText.ToString() == "0")
            {
                DisplayText = digit;
                CalculatorModel.isNewNumber = false;
            }
            else
            {
                DisplayText += digit;
            }
            return;
        }

        /// <summary>
        /// オペレーターボタンが押されたとき、Modelにオペレーターの更新を行う
        /// イコールボタンが押されたときの演算はこのオペレーターが用いられる
        /// </summary>
        /// <param name="parameter"></param>
        private void OperatorButton_Click(object? parameter)
        {
            if (parameter is not string op)
            {
                return;
            }

            if (CalculatorModel.currentOperator != null && !CalculatorModel.isNewNumber)
            {
                var result = CalculatorModel.Calculate(double.Parse(DisplayText.ToString()));
                FormulaText = $"{result} {op}";
            }
            else
            {
                CalculatorModel.lastNumber = double.Parse(DisplayText.ToString());
                FormulaText = $"{CalculatorModel.lastNumber} {op}";
            }

            CalculatorModel.currentOperator = op;
            CalculatorModel.isNewNumber = true;
        }

        /// <summary>
        /// イコールボタンが押されたとき、2つ以上の数字とオペレーターで演算を行う
        /// </summary>
        /// <param name="parameter"></param>
        private void EqualButton_Click(object? parameter)
        {
            if (CalculatorModel.currentOperator == null)
            {
                return; // 演算子がなければ何もしない
            }

            double currentNumber = double.Parse(DisplayText.ToString());
            string formula = $"{CalculatorModel.lastNumber} {CalculatorModel.currentOperator} {currentNumber} =";

            var result = CalculatorModel.Calculate(currentNumber);
            DisplayText = result.ToString();
            FormulaText = formula;

            CalculatorModel.currentOperator = null;
            CalculatorModel.isNewNumber = true;
        }

        /// <summary>
        /// ACボタンが押されたとき、入力を全てクリアする
        /// </summary>
        /// <param name="parameter"></param>
        private void ACButton_Click(object? parameter)
        {
            CalculatorModel.InitializeData();
            DisplayText = "0";
            FormulaText = "";
        }

        /// <summary>
        /// プラスマイナスボタンが押されたとき、数字に符号をつける
        /// </summary>
        /// <param name="parameter"></param>
        private void PlusMinusButton_Click(object? parameter)
        {
            if (double.TryParse(DisplayText.ToString(), out double value))
            {
                value *= -1;
                DisplayText = value.ToString();
            }
        }

        /// <summary>
        /// ドットボタンが押されたとき、数字にドットをつける
        /// </summary>
        /// <param name="parameter"></param>
        private void DotButton_Click(object? parameter)
        {
            if (!DisplayText.ToString().Contains("."))
            {
                DisplayText += ".";
            }
        }


        // INotifyPropertyChangedの実装
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// ボタンのクリックなどユーザー操作を処理するためのインターフェースICommand継承クラス
    /// これを実装することでView、ViewModelの役割分担できる
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// コマンドが実行されたときに実際に呼び出すデリゲート
        /// </summary>
        private readonly Action<object?> _execute;

        /// <summary>
        /// CanExecute の戻り値が変わった可能性をWPFに通知するためのイベント。
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<object?> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// コマンドが現在実行可能か返す
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter) => true;


        /// <summary>
        /// コマンドが実行されtが時に呼び出される処理
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
