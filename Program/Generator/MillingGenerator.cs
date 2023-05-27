namespace CAM.Program.Generator
{
    public class MillingGenerator
    {
        private readonly IPostProcessor _postProcessor;

        public MillingGenerator(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;
        }
    }
}
