using System;
using S2DCore;
using S2DComponents;

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

        
        Actor actor = Actor.Create();
        actor.name = "Cool Name";
        Component.Add<ComponentTest>(actor);
        Component.Get<ComponentTest>(actor).test();

        while (Internal.context.MainWindow.IsOpen)
        {
            Internal.context.MainWindow.DispatchEvents();
            Internal.context.MainWindow.Clear();

            UpdateManager.UpdateEngine();


            Internal.context.MainWindow.Display();
        }
    }
}

class S2EngineEntry
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

        
        Actor actor = Actor.Create();
        actor.name = "Cool Name";
        Component.Add<ComponentTest>(actor);
        Component.Get<ComponentTest>(actor).test();

        while (Internal.context.MainWindow.IsOpen)
        {
            Internal.context.MainWindow.DispatchEvents();
            Internal.context.MainWindow.Clear();

            UpdateManager.UpdateEngine();


            Internal.context.MainWindow.Display();
        }
    }
}

