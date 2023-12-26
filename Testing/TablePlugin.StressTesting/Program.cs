namespace TablePlugin.StressTesting
{
    using System.Diagnostics;
    using System.IO;
    using System;
    using Microsoft.VisualBasic.Devices;
    using TablePlugin.Connector;
    using TablePlugin.Model;
    using System.Collections.Generic;

    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new TableBuilder();
            var stopWatch = new Stopwatch();
            var parameters = new Parameters();
            parameters.ParamsDictionary = new Dictionary<ParameterType.ParamType, Parameter>()
            {
                { ParameterType.ParamType.TableLength, new Parameter(1200, 600, 1200) },
                { ParameterType.ParamType.TableWidth, new Parameter(1200, 600, 1200) },
                { ParameterType.ParamType.TableHeight, new Parameter(500, 400, 500) },
                { ParameterType.ParamType.ShelfLength, new Parameter(1100, 300, 1100) },
                { ParameterType.ParamType.ShelfWidth, new Parameter(1100, 300, 1100) },
                { ParameterType.ParamType.ShelfHeight, new Parameter(40, 10, 40) },
                { ParameterType.ParamType.SupportSize, new Parameter(50, 30, 50) },
                { ParameterType.ParamType.ShelfFloorDistance, new Parameter(360, 30, 360) },
                { ParameterType.ParamType.BracingSize, new Parameter(45, 20, 45) },
                { ParameterType.ParamType.WheelSize, new Parameter(70, 0, 70) },
            };
            var streamWriter = new StreamWriter($"log.txt", true);
            Process currentProcess = Process.GetCurrentProcess();
            var count = 0;

            while (count < 70)
            { 
                const double gigabyteInByte = 0.000000000931322574615478515625; 
                stopWatch.Start();
                builder.Build(parameters); 
                var computerInfo = new ComputerInfo(); 
                var usedMemory = (computerInfo.TotalPhysicalMemory
                    - computerInfo.AvailablePhysicalMemory)
                    * gigabyteInByte; 
                stopWatch.Stop();
                stopWatch.Reset(); 
                streamWriter.WriteLine(
                    $"{++count}\t{stopWatch.Elapsed:hh\\:mm\\:ss}\t{usedMemory}"); 
                streamWriter.Flush(); 
            }

            streamWriter.Close();
            streamWriter.Dispose();
            Console.Write($"End {new ComputerInfo().TotalPhysicalMemory}");
        }
    }
}
