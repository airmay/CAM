using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Commands
{
    public static class CommandNames
    {
        public static string CreateTechOperationCommand { get; } = "CreateTechOperationCommand";

        public static string CreateTechProcessCommand { get; } = "CreateTechProcessCommand";

        public static string MoveDownTechOperationCommand { get; } = "MoveDownTechOperationCommand";

        public static string MoveUpTechOperationCommand { get; } = "MoveUpTechOperationCommand";

        public static string RemoveTechOperationCommand { get; } = "RemoveTechOperationCommand";

        public static string SelectTechOperationCommand { get; } = "SelectTechOperationCommand";

        public static string SetTechOperationCommand { get; } = "SetTechOperationCommand";
    }
}
