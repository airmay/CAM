﻿using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class GeneralOperation
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public List<Operation> Operations { get; set; }

        public MachineType? MachineType { get; set; }
        public CAM.Tool Tool { get; set; }
        public double ZSafety { get; set; } = 20;

        [NonSerialized]
        private List<ProcessCommand> _processCommands = new List<ProcessCommand>();

        public List<ProcessCommand> ProcessCommands => _processCommands;

        public double OriginX { get; set; }

        public double OriginY { get; set; }

        public int GetFirstCommandIndex() => 0;

        [NonSerialized]
        public ObjectId[] OriginObject;
        [NonSerialized]
        public Dictionary<ObjectId, int> ToolpathObjectIds;
        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;
        [NonSerialized]
        public ObjectId? ExtraObjectsGroup;

        public Dictionary<ObjectId, int> GetToolpathObjectIds() => ToolpathObjectIds;

        public ObjectId? GetToolpathObjectsGroup() => ToolpathObjectsGroup;

        public ObjectId? GetExtraObjectsGroup() => ExtraObjectsGroup;

        public void Select() => ToolpathObjectsGroup?.SetGroupVisibility(true);

        public virtual void SerializeInit()
        {
            if (OriginX != 0 || OriginY != 0)
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));

            Operations.ForEach(p =>
            {
                AcadObject.LoadAcadProps(p);
                //p.TechProcessBase = this;
                p.SerializeInit();
            });
        }


    }
}