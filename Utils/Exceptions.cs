using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CAM
{
    public static class Exceptions
    {
        private static readonly List<Type> fatalExceptions = new List<Type> {
            typeof (OutOfMemoryException),
            typeof (StackOverflowException),
        };
        public static string FullMessage(this Exception ex)
        {
            var builder = new StringBuilder();
            while (ex != null)
            {
                builder.AppendFormat("{0}{1}", ex, Environment.NewLine);
                ex = ex.InnerException;
            }
            return builder.ToString();
        }

        public static void TryFilterCatch(Action tryAction, Func<Exception, bool> isRecoverPossible, Action handlerAction)
        {
            try
            {
                tryAction();
            }
            catch (Exception ex)
            {
                handlerAction();
                if (!isRecoverPossible(ex)) throw;
            }
        }

        public static void TryFilterCatch(Action tryAction, Func<Exception, bool> isRecoverPossible, Action<Exception> handlerAction)
        {
            try
            {
                tryAction();
            }
            catch (Exception ex)
            {
                handlerAction(ex);
                if (!isRecoverPossible(ex))
                    throw;
            }
        }

        public static void TryExecute(Action tryAction) => TryFilterCatch(tryAction, e => e.NotFatal(), HandleException);

        public static void HandleException(Exception exception)
        {
            Acad.Alert($"Ошибка: {exception.Message}");
            File.WriteAllText($@"\\US-CATALINA3\public\Программы станок\CodeRepository\Logs\error_{DateTime.Now.ToString("yyyy-MM-dd_hh-m-ss")}.log", $"{Acad.ActiveDocument.Name}\n\n{exception}");
        }

        public static bool NotFatal(this Exception ex)
        {
            return fatalExceptions.All(curFatal => ex.GetType() != curFatal);
        }

        public static bool IsFatal(this Exception ex)
        {
            return !NotFatal(ex);
        }
    }
}
