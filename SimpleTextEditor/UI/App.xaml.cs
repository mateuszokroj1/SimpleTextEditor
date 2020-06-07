using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SimpleTextEditor.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NamedPipeServerStream server;
        private Thread serverThread;
        private MainWindow mainWindow = null;
        private bool isClosing = false;
        private object serverThreadLocker = new object();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var process = Process.GetCurrentProcess();
            if(Process.GetProcessesByName(process.ProcessName).Where(row=> row.Id != process.Id).Count() > 0)
            { // is client - send filename to server and process exit
                try
                {
                    NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "SimpleTextEditor_PipeStream", PipeDirection.InOut);

                    pipeClient.Connect(4000);
                    if (!pipeClient.IsConnected || !pipeClient.CanWrite)
                    {
                        ErrorMessage.Show("Nie można otworzyć pliku z powodu problemów z połączeniem potoku.");
                        Shutdown(0);
                    }

                    /*if (pipeClient.TransmissionMode != PipeTransmissionMode.Message)
                    {
                        ErrorMessage.Show("Błędny tryb potoku po stronie serwera.");
                        pipeClient.Close();
                        Shutdown(0);
                    }*/

                    using (var writer = new StreamWriter(pipeClient))
                    {
                        using (var reader = new StreamReader(pipeClient))
                        {
                            Thread.Sleep(100);

                            /*if (!(reader.ReadLine()?.Equals("READY") ?? false))
                                ErrorMessage.Show("Pipe server is not connected properly");*/

                            foreach (var filename in e.Args)
                            {
                                if (!File.Exists(filename))
                                {
                                    WarningMessage.Show($"Nie znaleziono pliku: {filename}");
                                    continue;
                                }

                                writer.WriteLine(filename);
                            }
                            writer.WriteLine("END\0");
                        }
                    }

                    pipeClient.Flush();
                    pipeClient.Close();
                }
                catch(Exception exc)
                {
                    ErrorMessage.Show(exc);
                }

                Shutdown(0);
            }
            else // is server - load UI
            {
                try
                {
                    server = new NamedPipeServerStream("SimpleTextEditor_PipeStream", PipeDirection.InOut, 1, PipeTransmissionMode.Message);
                    serverThread = new Thread(new ThreadStart(RunPipeServer));
                    serverThread.Start();

                    mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                catch(Exception ex)
                {
                    ErrorMessage.Show(ex);
                    Shutdown(-1);
                }
            }
        }

        private void RunPipeServer()
        {/*
            while(!this.isClosing && server != null)
            {
                Thread.Sleep(500);
                server.WaitForConnection();
                GetMessageFromPipeClient();
            }

            server?.Close();*/
        }

        private void GetMessageFromPipeClient()
        {/*
            if (server == null || !server.IsConnected)
                throw new InvalidOperationException("Server is not connected or initialized.");
            while (!(server?.CanRead ?? false) && !isClosing);

            if (isClosing) return;

            lock (serverThreadLocker)
            {
                using (var writer = new StreamWriter(server))
                {
                    using (var reader = new StreamReader(server))
                    {
                        writer.WriteLine("READY");

                        string message = string.Empty;
                        do
                        {
                            message = reader.ReadLine();

                            if (!isClosing)
                                mainWindow.OpenFile(message);
                        } while (!(message?.Equals("BYE\0") ?? false) && !isClosing);
                    }
                }
                server.Disconnect();
            }*/
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorMessage.Show(e.Exception);
            Shutdown(-1);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                isClosing = true;
                lock(serverThreadLocker)
                {
                    server?.Disconnect();
                    server?.Close();
                }

                if ((serverThread?.ThreadState ?? System.Threading.ThreadState.Unstarted) == System.Threading.ThreadState.Running)
                    serverThread?.Abort();
            }
            catch(Exception) { return; }
        }
    }
}
