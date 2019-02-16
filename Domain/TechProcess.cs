using System;
using System.Collections.Generic;

namespace CAM.Domain
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    public class TechProcess
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Параметры технологического процесса
        /// </summary>
        public TechProcessParams TechProcessParams { get; set; }

        /// <summary>
        /// Список технологических операций процесса
        /// </summary>
        public List<TechOperation> TechOperations { get; } = new List<TechOperation>();

        public TechProcess(string name, TechProcessParams techProcessParams)
        {
            Name = name;
            TechProcessParams = techProcessParams;
        }

        /// <summary>
        /// Создает обработку по техпроцессу
        /// </summary>
        /// <returns></returns>
        public void BuildProcessing()
        {

            //SetupCommands.Clear();
            //SetupCommands.Add(new TechProcessCommand("G98", "Установка"));
            //SetupCommands.Add(new TechProcessCommand("G97 M2 1", "Установка"));
            //SetupCommands.Add(new TechProcessCommand("17 XYCZ", "Установка"));
            //SetupCommands.Add(new TechProcessCommand("28 XYCZ", "Установка"));
            //SetupCommands.Add(new TechProcessCommand($"G97 M6 {TechProcessParams.Tool.Number}", "Установка"));
        }

        internal void BuildProcessing(ScemaLogicProgramBuilder actionGenerator)
        {
            TechOperations.ForEach(p => p.BuildProcessing(actionGenerator));
        }
    }
}