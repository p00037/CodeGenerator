using System;
using System.Windows;

namespace CodeGenerator.Common
{
    public class TryCatch
    {
        public static void ShowMassage(Action action)
        {
            try
            {
                action();
            }
            catch (DoNothingException)
            {
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}
