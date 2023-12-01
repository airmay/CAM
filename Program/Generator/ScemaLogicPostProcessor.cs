using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Program.Generator
{
    public class ScemaLogicPostProcessor : PostProcessorBase
    {
        public override string[] StartMachine()
        {
            throw new NotImplementedException();
        }

        public override string[] StopMachine()
        {
            throw new NotImplementedException();
        }

        public override string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber)
        {
            throw new NotImplementedException();
        }

        public override string[] StartEngine(int frequency, bool hasTool)
        {
            throw new NotImplementedException();
        }

        public override string[] StopEngine()
        {
            throw new NotImplementedException();
        }

        public override string Pause(double duration)
        {
            throw new NotImplementedException();
        }

        public override string Cycle() => "28;;XYCZ;;;;;;";
    }
}
