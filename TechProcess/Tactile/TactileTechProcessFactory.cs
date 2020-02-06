using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Tactile
{
    /// <summary>
    /// Фабрика для создания техпроцесса "Тактилка"
    /// </summary>
    //[Serializable]
    //[TechProcess(TechProcessType.Tactile)]
    //public class TactileTechProcessFactory : ITechProcessFactory
    //{
    //    //public ProcessingType ProcessingType => ProcessingType.Tactile;

    //    private readonly TactileTechProcessParams _tactileTechProcessParams;

    //    private int _techProcessCount;

    //    /// <summary>
    //    /// Конструктор фабрики
    //    /// </summary>
    //    public TactileTechProcessFactory(Settings settings)
    //    {
            
    //        _tactileTechProcessParams = settings.TactileTechProcessParams;
    //    }

    //    /// <summary>
    //    /// Создает техпроцесс "Тактилка"
    //    /// </summary>
    //    public TactileTechProcess Create()
    //    {
    //        return new TactileTechProcess($"Тактилка {++_techProcessCount}", _tactileTechProcessParams.Clone());
    //    }
    //}
}
