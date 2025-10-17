﻿using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Dreambuild.AutoCAD;

namespace CAM;

[Serializable]
public class AcadObject
{
    public long[] Handles { get; set; }
    [NonSerialized] private ObjectId[] _objectIds;

    private AcadObject(ObjectId[] objectIds)
    {
        _objectIds = objectIds;
        Handles = objectIds.ConvertAll(p => p.Handle.Value);
    }

    public static AcadObject Create(ObjectId id) => new([id]);
    public static AcadObject Create(IEnumerable<ObjectId> ids) => new(ids.ToArray());

    public ObjectId[] ObjectIds =>
        _objectIds ??= Handles
            .Select(p => Acad.Database.TryGetObjectId(new Handle(p), out var id) ? id : ObjectId.Null)
            .Where(p => p != ObjectId.Null)
            .ToArray();

    public ObjectId ObjectId => ObjectIds[0];
    public Curve GetCurve() => ObjectId.QOpenForRead<Curve>();
    public Curve[] GetCurves() => ObjectIds.QOpenForRead<Curve>();

    public string Description => ToString();
    public override string ToString() => ObjectIds.GetDesc();

    public void Select() => Acad.SelectObjectIds(ObjectIds);
}