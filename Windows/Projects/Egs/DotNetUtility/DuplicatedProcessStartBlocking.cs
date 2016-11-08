namespace DotNetUtility
{
    using System;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Threading;
    using System.Security.AccessControl;
    using System.Security.Principal;

    public static class DuplicatedProcessStartBlocking
    {
        static System.Threading.Mutex mutex = null;
        //static System.Diagnostics.Process currentProcess = null;

        /// <summary>
        /// If it return false, exit program.  (ex. call if (Application.Current != null) { Application.Current.Shutdown(); })
        /// </summary>
        public static bool TryGetMutexOnTheBeginningOfApplicationConstructor()
        {
			// Refer to: http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
            var entryAssemblyFullName = Assembly.GetEntryAssembly().FullName;

            mutex = new System.Threading.Mutex(false, entryAssemblyFullName);
            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            mutex.SetAccessControl(securitySettings);

            if (mutex.WaitOne(TimeSpan.Zero, false))
            {
                //currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                //currentProcess.Exited += (sender, e) => { ReleaseMutex(); };
                return true;
            }

            mutex.Close();
            mutex = null;
            return false;
        }

        public static void ReleaseMutex()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
                mutex = null;
            }
        }
    }
}