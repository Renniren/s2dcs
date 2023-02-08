using System;
using S2DInternal;

class S2GameEntry
{
    static void Main(string[] args)
    {
        Console.WriteLine(Internal.GetCWD());
        Console.WriteLine("Hello World!");
        Internal.CreateContext();

        Internal.context.MainWindow.Closed += (sender, e) =>

        {
            Internal.context.MainWindow.Close();
        };

        while (Internal.context.MainWindow.IsOpen)
        {
            Internal.context.MainWindow.DispatchEvents();
            Internal.context.MainWindow.Clear();
            Internal.context.MainWindow.Display();
        }
    }
}