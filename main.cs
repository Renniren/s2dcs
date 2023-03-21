using System;
using S2DCore;
using S2DComponents;

using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

class S2GameEntry
{

    static void Main(string[] args)
    {
        void RunSerializationTest()
	    {
            Scene serializetest = new();

            serializetest.name = "TestScene";
            serializetest.world = new();
            serializetest.world.backgroundColor = new S2DCore.Color(0.5f, 0, 0, 1);

            for (int i = 1; i <= 64; i++)
            {
                Actor a = Actor.Create(Vector2.Random(50.0f), Vector2.one, "newActor " + i.ToString(), 
                    S2Random.Range(-180f, 180f));

                a.scene = new(serializetest);

                SerializeTest test = a.AddComponent<SerializeTest>();
                SerializeTest test2 = a.AddComponent<SerializeTest>();
                a.AddComponent<InheritanceTestB>(); //here's the FUN part
                a.AddComponent<StructSerializationTestA>();

                test.value = S2Random.Range(0, 4);
                test.othervalue = S2Random.Range(0, 4);
                test.anothervalue = S2Random.Range(0, 4);

                test2.value = 4096;
                test2.othervalue = 403;
                test2.anothervalue = 143;

                test.FunnyActor = a;
                test2.FunnyActor = a;

                test.otherTestComponent = test2;
                serializetest.actors.AddOnce(a);
            }

            S2DSerializer.SerializeScene(serializetest);
            S2DSerializer.LoadScene(Internal.GetCWD() +
            Constants.ResourcesPath + "\\levels\\" + serializetest.name + ".s2");
        }
        
        Console.WriteLine(Internal.GetCWD());
        Console.WriteLine("Hello World!");

        string vector = "(403.1337, 6094492.30)";
        Vector2 r = Vector2.Parse(vector);

        Internal.CreateContext();
        Internal.context.MainWindow.Closed += (sender, e) =>
        {
            Internal.context.MainWindow.Close();
        };

        Actor actor = Actor.Create(Vector2.zero, Vector2.one);
        actor.name = "Test Subject";
        actor.AddComponent<InheritanceTestB>();

        SpriteRenderer sr = Component.Add<SpriteRenderer>(actor);
        //sr.sprite = new S2Sprite(Internal.GetCWD() + Constants.TexturesPath + "squareSmall.png", 2);

        sr.sprite = TextureManager.CreateSprite("squareSmall.png");

        S2GUI.Panel panel = new(50, 50);

        RunSerializationTest();
        while (Internal.context.MainWindow.IsOpen)
        { 
            Internal.context.MainWindow.SetView(Internal.calcView((Vector2f)Internal.context.MainWindow.Size, 0, 1));
            Internal.context.MainWindow.DispatchEvents();
             
            actor.position = Internal.context.MainWindow.DefaultView.Center;
            Internal.context.MainWindow.Clear();
            actor.scale = new Vector2(1, 1);
            actor.position = Internal.context.MainWindow.GetView().Center;

            UpdateManager.UpdateEngine();

            Internal.context.MainWindow.Display();
        }
    }
}