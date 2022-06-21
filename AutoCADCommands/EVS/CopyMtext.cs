using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System.Linq;
using System.Windows;
using Dreambuild.AutoCAD;
using System;
using System.Diagnostics.Eventing.Reader;
//[assembly: CommandClass(typeof(AutoCADCommands.EVS.CopyMtext))]
namespace AutoCADCommands.EVS
{
    public class CopyMtext
    {
        [CommandMethod("COPYMTEXTEVS", CommandFlags.UsePickSet)]
        public static void COPYMTEXTEVS2()
        {

            //var ids = Interaction.GetSelection("\nSelect MText", "MTEXT, TEXT");
            var ids = Interaction.GetSelection("\nSelect MText or TEXT\n", "*TEXT,MTEXT");
            using (Entity ent = (Entity)ids.FirstOrDefault().Open(OpenMode.ForRead))

            {
                Interaction.Write($"ТИП ВЫБРАННОГО ПРИМИТИВА -  {ent.GetType()}");



                if (ent.GetType() == typeof(MText))
                {
                    var mts = ids.QOpenForRead<MText>().FirstOrDefault(mt =>
                    {

                        Clipboard.Clear();
                        Clipboard.SetText(mt.Text);
                        Interaction.Write($"В буфере: {mt.Text}");
                        return true;
                    });
                }
                if (ent.GetType() == typeof(DBText))
                {
                    var dts = ids.QOpenForRead<DBText>().FirstOrDefault(dt =>
                          {

                              Clipboard.Clear();
                              Clipboard.SetText(dt.TextString);
                              Interaction.Write($"В буфере: {dt.TextString}");
                              return true;

                              //var mt = NoDraw.MText(dt.TextString, dt.Height, dt.Position, dt.Rotation, false);
                              //mt.Layer = dt.Layer;
                              //return mt;
                          });
                }

            }
        }

        [CommandMethod("PASTEMTEXTEVS", CommandFlags.UsePickSet)]
        public static void PASTEMTEXTEVS()
        {


            var ids = Interaction.GetSelection("\nSelect MText or TEXT\n", "*TEXT,MTEXT");
            foreach (var idOne in ids)
            {
                Entity ent = (Entity)idOne.Open(OpenMode.ForWrite);
                using (ent)
                {
                    Interaction.Write($"ТИП ВЫБРАННОГО ПРИМИТИВА -  {ent.GetType().Name} \n");
                    if (ent.GetType() == typeof(MText))
                    {
                        idOne.QOpenForWrite<MText>((mt =>
                                {
                                    mt.Contents = Clipboard.GetText();
                                }
                                ));
                        //var mts = ids.QOpenForRead<MText>().Select(mt =>
                        //{
                        //    var clip = Clipboard.GetText();
                        //    var mtt = NoDraw.MText(clip, mt.TextHeight, mt.Location, mt.Attachment, mt.Rotation, false/*, mt.Width*/  );
                        //    Interaction.Write($"ВСТАВКА: {mt.Text}");
                        //    return mtt;
                        //}).ToArray();
                        //ids.QForEach(mt => mt.Erase());
                        //mts.AddToCurrentSpace();
                    }

                    if (ent.GetType() == typeof(DBText))
                    {
                        idOne.QOpenForWrite<DBText>((dt =>
                                {
                                    dt.TextString = Clipboard.GetText();
                                }
                            ));
                        //var dts = ids.QOpenForRead<DBText>().Select(dt =>
                        //{
                        //    var clip = Clipboard.GetText();
                        //    var dtt = NoDraw.Text(clip, dt.Height, dt.Position, dt.Rotation, false);
                        //    Interaction.Write($"ВСТАВКА: {dt.TextString}");
                        //    return dtt;
                        //}).ToArray();

                        //ids.QForEach(dt => dt.Erase());
                        //dts.AddToCurrentSpace();

                    }

                }
            }
        }
    }
}








