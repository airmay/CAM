﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    public abstract class TechOperation : ITechOperation
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Параметры технологического процесса обработки
        /// </summary>
        public TechProcessParams TechProcessParams { get; }

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public ProcessingArea ProcessingArea { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Действия для выполнения операции
        /// </summary>
        public List<ProcessAction> ProcessActions { get; } = new List<ProcessAction>();

        protected TechOperation(TechProcessParams techProcessParams, ProcessingArea processingArea)
        {
            TechProcessParams = techProcessParams;
            ProcessingArea = processingArea;
        }

        public abstract void BuildProcessing();
    }
}
