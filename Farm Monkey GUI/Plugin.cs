using ArcheBuddy.Bot.Classes;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace FarmMonkey_GUI
{
    public class Plugin : Core
    {
        Thread WindowThread;
        MainWindow myWindow;
        bool WindowOpen = true;

        public static string GetPluginAuthor() { return "Ariana"; }
        public static string GetPluginVersion() { return "0.0.1"; }
        public static string GetPluginDescription() { return "Graphical farming helper"; }

        private void RunWindow()
        {
            // Create new interface object
            myWindow = new MainWindow();

            // Set Core
            myWindow.SetHost(this);

            // Show the window
            myWindow.Show();

            // Sry I can't explain it but it's necessary
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

            // Event for when the window get closed
            myWindow.Closed += (s, e) =>
            {
                Dispatcher.CurrentDispatcher.ShutdownFinished += (ss, ee) =>
                {
                    // Will be executed after the thread was close. 
                    WindowOpen = false;
                };
                // Tells the thread to close itself 
                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            };

            Dispatcher.Run();
        }

        public void PluginRun()
        {
            ClearLogs();

            if (gameState != GameState.Ingame) { Log("Waiting to be ingame to fully load the plugin..."); while (gameState != GameState.Ingame) Thread.Sleep(50); }

            // Create a new Thread and execute RuNWindow in it.
            WindowThread = new Thread(RunWindow);
            // Thread has to be Single threaded!
            WindowThread.SetApartmentState(ApartmentState.STA);
            WindowThread.Start();

            Log("Interface is loading.");

            //Prevent the end of the plugin
            while (WindowOpen) Thread.Sleep(100);

        }

        public void PluginStop()
        {
            if (myWindow != null)
            {
                Log("Plugin has succesfully stopped");
                myWindow.Close();
            }
            Application.Current.Shutdown();
            WindowThread.Abort();
            WindowThread.Join();
        }
    }
}
