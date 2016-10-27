namespace WindowsFormsApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Egs;

    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Egs.DotNetUtility.DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
            {
                MessageBox.Show("Only 1 instance can run at the same time.");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            Egs.DotNetUtility.DuplicatedProcessStartBlocking.ReleaseMutex();
        }
    }
}
