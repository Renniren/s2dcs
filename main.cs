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
        SpriteRenderer sr = Component.Add<SpriteRenderer>(actor);

        sr.sprite = new S2Sprite(Internal.GetCWD() + 
            Constants.TexturesPath + "squareSmall.png", 2);

        while (Internal.context.MainWindow.IsOpen)
        {
            Internal.context.MainWindow.DispatchEvents();

            actor.position = Internal.context.MainWindow.DefaultView.Center;
            Internal.context.MainWindow.Clear();
            actor.scale = new Vector2(1, 1);

            UpdateManager.UpdateEngine();

            Internal.context.MainWindow.Display();

        }
    }
}