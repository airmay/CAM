﻿using System;
using System.Collections.Generic;

namespace CAM.Domain
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    public class TechProcess
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        public TechProcessParams TechProcessParams { get; set; }

        /// <summary>
        /// Список технологических операций процесса
        /// </summary>
        public List<SawingTechOperation> TechOperations { get; } = new List<SawingTechOperation>();

        /// <summary>
        /// Команды установки
        /// </summary>
        public List<TechProcessCommand> SetupCommands { get; } = new List<TechProcessCommand>();

        /// <summary>
        /// Команды завершения
        /// </summary>
        public List<TechProcessCommand> TeardownCommands { get; } = new List<TechProcessCommand>();

        public TechProcess(string name, TechProcessParams techProcessParams)
        {
            Name = name;
            TechProcessParams = techProcessParams;
        }

        /// <summary>
        /// Создает программу обработки по техпроцессу
        /// </summary>
        /// <returns></returns>
        public void BuildProcessing()
        {
            SetupCommands.Clear();
            SetupCommands.Add(new TechProcessCommand("G98", "Установка"));
            SetupCommands.Add(new TechProcessCommand("G97 M2 1", "Установка"));
            SetupCommands.Add(new TechProcessCommand("17 XYCZ", "Установка"));
            SetupCommands.Add(new TechProcessCommand("28 XYCZ", "Установка"));
            SetupCommands.Add(new TechProcessCommand($"G97 M6 {TechProcessParams.Tool.Number}", "Установка"));
        }
    }
}