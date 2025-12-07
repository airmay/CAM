using System;
using System.Diagnostics;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;

namespace CAM.Autocad;

public static class Progressor
{
    private static ProgressMeter _progressMeter;
    private static Stopwatch _stopwatch;
    private static MyMessageFilter _filter;
    private static int _limit;
    private static int _current;

    public static void Start(string caption)
    {
        _progressMeter ??= new ProgressMeter();
        _progressMeter.Start($"{caption} [Esc - остановить]");

        _stopwatch ??= new Stopwatch();
        _stopwatch.Start();            

        _filter ??= new MyMessageFilter();
        Application.AddMessageFilter(_filter);

        _limit = 0;
        _current = 0;
    }

    public static void SetLimit(int limit)
    {
        _progressMeter.SetLimit(limit);
        _limit = limit;
        _current = 0;
    }

    public static bool Report(bool throwException = true)
    {
        _progressMeter.MeterProgress();
        _current++;

        Application.DoEvents();

        if (_filter.Canceled)
        {
            _filter.Canceled = false;
            _stopwatch.Stop();
            var rest = _limit != 0
                ? $"До завершения осталось {TimeSpan.FromSeconds((_stopwatch.ElapsedMilliseconds * _limit / _current - _stopwatch.ElapsedMilliseconds) / 1000)}. "
                : string.Empty;
            if (Acad.Confirm($"{rest}Остановить выполнение?"))
            {
                Stop();   
                if (throwException)
                    throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.UserBreak);
                return false;
            }
            _stopwatch.Start();
        }
        return true;
    }

    public static void Stop()
    {
        _progressMeter.Stop();
        _stopwatch.Reset();
        Application.RemoveMessageFilter(_filter);
    }
}

//////////////////////////////////////////////////////////////////////////
//            Наш класс для фильтрации сообщений
//////////////////////////////////////////////////////////////////////////
public class MyMessageFilter : IMessageFilter
{
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    public bool Canceled;

    public bool PreFilterMessage(ref System.Windows.Forms.Message m)
    {
        if (m.Msg is WM_KEYDOWN or WM_KEYUP)
        {
            // Проверяем не нажата ли клавиша ESC
            var kc = (Keys)(int)m.WParam & Keys.KeyCode;
            if (kc == Keys.Escape)
            {
                Canceled = true;
                // Возвращаем true, чтобы показать, что мы обработали это сообщение
                return true;
            }
        }
        // Возвращаем false, чтобы сообщить, что мы не обрабатывали это сообщение
        return false;
    }
}