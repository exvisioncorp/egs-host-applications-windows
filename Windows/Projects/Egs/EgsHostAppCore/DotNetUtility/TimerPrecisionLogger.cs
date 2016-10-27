namespace Egs.DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class TimerPrecisionLogger
    {
        public bool IsToUseTimerPrecisionMonitor { get; set; }
        Stopwatch elapsed;
        List<long> beforeUpdateElapsedMillisecondList;
        List<long> afterUpdateElapsedMillisecondList;
        List<string> descriptionList;

        public TimerPrecisionLogger()
        {
            IsToUseTimerPrecisionMonitor = false;
            elapsed = new Stopwatch();
            beforeUpdateElapsedMillisecondList = new List<long>() { Capacity = 100000 };
            afterUpdateElapsedMillisecondList = new List<long>() { Capacity = 100000 };
            descriptionList = new List<string>() { Capacity = 100000 };
            elapsed.Start();
        }

        public void CallBeforeUpdating(string description)
        {
            if (IsToUseTimerPrecisionMonitor == false) { return; }
            beforeUpdateElapsedMillisecondList.Add(elapsed.ElapsedMilliseconds);
            descriptionList.Add(description);
        }

        public void CallAfterUpdated()
        {
            if (IsToUseTimerPrecisionMonitor == false) { return; }
            afterUpdateElapsedMillisecondList.Add(elapsed.ElapsedMilliseconds);
        }

        public void ExportLog()
        {
            if (IsToUseTimerPrecisionMonitor == false) { return; }

            var resultSavingPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "timerPrecision.csv");
            using (var sw = new System.IO.StreamWriter(resultSavingPath))
            {
                sw.WriteLine("i, Before, After, DeltaT, After-Before, Description");
                for (int i = 0; i < beforeUpdateElapsedMillisecondList.Count; i++)
                {
                    sw.Write(i + ", " + beforeUpdateElapsedMillisecondList[i] + ", " + afterUpdateElapsedMillisecondList[i]);
                    sw.Write(", " + ((i == 0) ? 0 : beforeUpdateElapsedMillisecondList[i] - beforeUpdateElapsedMillisecondList[i - 1]));
                    sw.Write(", " + (afterUpdateElapsedMillisecondList[i] - beforeUpdateElapsedMillisecondList[i]));
                    sw.Write(", " + descriptionList[i]);
                    sw.WriteLine();
                }
            }
        }
    }
}
