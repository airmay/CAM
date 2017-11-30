using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    public static class ProcessActionNames
    {
        public const string Approach = "Подвод";
        public const string Departure = "Отвод";
        public const string Pass = "Проход";
        public const string InitialMove = "Первый подвод";
        public const string Fast = "Быстрая подача";
        public const string Descent = "Опускание";
        public const string Uplifting = "Поднятие";
        public const string Penetration = "Заглубление";
        public const string Cutting = "Рабочий ход";
    }
}
