using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CAM.Core
{
    public class Progressor
    {
        private ProgressMeter _progressMeter;
        private MyMessageFilter _filter;
        int _max;
        int _current;
        Stopwatch _stopwatch;

        public Progressor(string caption, int max)
        {
            _progressMeter = new ProgressMeter();
            _progressMeter.Start($"{caption} [Esc - остановить]");
            _progressMeter.SetLimit(max);

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _max = max;
            _current = 0;

            _filter = new MyMessageFilter();
            Application.AddMessageFilter(_filter);
        }

        public bool Progress(bool throwException = true)
        {
            _progressMeter.MeterProgress();
            _current++;

            Application.DoEvents();

            if (_filter.bCanceled == true)
            {
                _stopwatch.Stop();
                var time = TimeSpan.FromSeconds((_stopwatch.ElapsedMilliseconds * _max / _current - _stopwatch.ElapsedMilliseconds) / 1000);
                if (MessageBox.Show($"До завершения осталось {time}. Остановить выполнение?", "Запрос", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Stop();   
                    if (throwException)
                        throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.UserBreak);
                    return false;
                }
                _filter.bCanceled = false;
                _stopwatch.Start();
            }
            return true;
        }

        public void Stop()
        {
            _progressMeter.Stop();
            Application.RemoveMessageFilter(_filter);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //            Наш класс для фильтрации сообщений
    //////////////////////////////////////////////////////////////////////////
    public class MyMessageFilter : IMessageFilter
    {

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        public bool bCanceled = false;

        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP)
            {
                // Проверяем не нажата ли клавиша ESC
                Keys kc = (Keys)(int)m.WParam & Keys.KeyCode;
                if (kc == Keys.Escape)
                {
                    bCanceled = true;
                    // Возвращаем true, чтобы показать, что мы обработали это сообщение
                    return true;
                }
            }
            // Возвращаем false, чтобы сообщить, что мы не обрабатывали это сообщение
            return false;
        }
    }
}
