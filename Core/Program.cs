using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAM.Core;

public class Program(
    List<Command> commands,
    IProcessing processing,
    Dictionary<short, IOperation> operations,
    Dictionary<short, int> operationNumbers,
    Dictionary<ObjectId, int> objectIds,
    Dictionary<short, ObjectId> operationToolpath)
{
    public List<Command> Commands { get; } = commands;
    public IProcessing Processing { get; } = processing;

    public IOperation GetOperation(short number) => operations[number];

    public bool TryGetCommandIndexByObjectId(ObjectId objectId, out int index) => objectIds.TryGetValue(objectId, out index);

    public bool TryGetCommandIndexByOperationNumber(short operationNumber, out int index) => operationNumbers.TryGetValue(operationNumber, out index);

    public void SetToolpathVisibility(bool value) => operationToolpath?.ForAll(p => p.Value.SetGroupVisibility(value));

    public void ShowOperationToolpath(short operationNumber) => operationToolpath?.ForAll(p => p.Value.SetGroupVisibility(p.Key == operationNumber));

    public void Export()
    {
        var extension = Processing.Machine switch
        {
            Machine.Donatoni => "pgm",
            _ => "txt"
        };

        var fileName = Acad.SaveFileDialog("program", extension, "Экспорт программы в файл");
        if (fileName == null)
            return;

        var contents = Commands.Select(p => $"N{p.Number} {p.Text}").ToArray();
        try
        {
            File.WriteAllLines(fileName, contents);
            Acad.Write($"Создан файл {fileName}");
        }
        catch (Exception ex)
        {
            Acad.Alert($"Ошибка при записи файла {fileName}", ex);
        }
    }

    /*
private void CreateImitationProgramm(string[] contents, string fileName)
{
List<string> result = new List<string>(contents.Length * 2);
foreach (var item in contents)
{
    if (item.StartsWith("M03"))
        continue;

    var line = item.Replace("G01", "G00");
    var vi = line.IndexOf('V');
    if (vi > 0)
        line = line.Substring(0, vi) + "V0";

    if (line == "G00 U0 V0")
        continue;

    result.Add(line);

    if (line.StartsWith("G00"))
        result.Add("M00");
}
var parts = fileName.Split('.');
fileName = parts[0] + "_i." + parts[1];
File.WriteAllLines(fileName, result);
Acad.Write($"Создан файл с имитацией {fileName}");
}
*/
}