using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public interface IToolLocation
    {
        bool IsDefined { get; }
        IToolLocation Origin { get; }
        Matrix3d GetTransformMatrixFrom(IToolLocation location);
    }
}