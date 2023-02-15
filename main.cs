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

        
        Actor actor = Actor.Create(Vector2.zero, Vector2.one);

        actor.name = "Cool Name";
        SpriteRenderer sr = Component.Add<SpriteRenderer>(actor);
        Component.Add<SpriteRenderer>(null);
        sr.sprite = new S2Sprite(Internal.GetCWD() + 
            Constants.TexturesPath + "squareSmall.png", 2);

        
        Scene serializetest = new();

        serializetest.name = "TestScene";
        serializetest.world = new();
        serializetest.world.backgroundColor = new Color(0.5f, 0, 0, 1);

        for (int i = 1; i <= 64; i++)
        {
            Actor a = Actor.Create(Vector2.zero, Vector2.one, "new Actor " + i.ToString(), 0);
            SerializeTest test = a.AddComponent<SerializeTest>();
            SerializeTest test2 = a.AddComponent<SerializeTest>();
            
            test.value = S2Random.Range(0, 4);
            test.othervalue = S2Random.Range(0, 4);
            test.anothervalue = S2Random.Range(0, 4);

            test2.value = 4096;
            test2.othervalue = 403;
            test2.anothervalue = 143;

            test.otherTestComponent = test2;
            serializetest.actors.AddOnce(a);
        }

        S2DSerializer.SerializeScene(serializetest);
        S2DSerializer.LoadScene(Internal.GetCWD() +
                Constants.ResourcesPath + "\\levels\\" + serializetest.name + ".s2");

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