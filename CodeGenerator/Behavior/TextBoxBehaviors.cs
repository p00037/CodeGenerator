using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeGenerator.Behavior
{
    public class TextBoxBehaviors : Behavior<TextBox>
    {
        public bool IsNumeric { get;set; }

        /// <summary>
        /// True なら入力を数字のみに制限します。
        /// </summary>
        public static readonly DependencyProperty IsNumericProperty =
                    DependencyProperty.RegisterAttached(
                        "IsNumeric", typeof(bool),
                        typeof(TextBoxBehaviors),
                        new UIPropertyMetadata(false, IsNumericChanged)
                    );

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumericProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetIsNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumericProperty, value);
        }

        private static void IsNumericChanged
            (DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            var textBox = sender as TextBox;
            if (textBox == null) return;

            // イベントを登録・削除 
            //textBox.KeyDown -= OnKeyDown;
            //textBox.TextChanged -= OnTextChanged;
            textBox.PreviewTextInput -= PreviewTextInput;
            var newValue = (bool)e.NewValue;
            if (newValue)
            {
                //textBox.KeyDown += OnKeyDown;
                //textBox.TextChanged += OnTextChanged;
                textBox.PreviewTextInput += PreviewTextInput;
            }
        }

        static void OnKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if ((Key.D0 <= e.Key && e.Key <= Key.D9) ||
                (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9) ||
                (Key.Delete == e.Key) || (Key.Back == e.Key) || (Key.Tab == e.Key))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private static void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            bool yes_parse = false;
            {
                // 既存のテキストボックス文字列に、
                // 今新規に一文字追加された時、その文字列が
                // 数値として意味があるかどうかをチェック
                {
                    float xx;
                    var tmp = textBox.Text + e.Text;
                    yes_parse = Single.TryParse(tmp, out xx);
                }
                // 最初の一文字の場合に限り、プラス文字とマイナス文字の
                // 入力は許可する。
                yes_parse |=
                    (textBox.Text.Length == 0) &&
                    (e.Text == "+") || (e.Text == "-");
            }
            // 更新したい場合は false, 更新したくない場合は true
            // を返すべし。（混乱しやすいので注意！）
            e.Handled = !yes_parse;
        }
    }
}
