using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Box2DX;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

using S2DCore;

namespace S2DComponents
{
	public class TextureManager
	{
		public static S2Sprite CreateSprite(string path)
		{
			
			return new S2Sprite(Internal.GetCWD() +
			Constants.TexturesPath + path, S2Random.Range(int.MinValue, int.MaxValue));
		}
	}

	public enum testEnum { optionA, optionB }

	public class SerializeTest : Component
    {
		public int value = 0;
		public int othervalue = 0;
		public int anothervalue = 0;
		public SerializeTest otherTestComponent;
		public Actor FunnyActor;
		public testEnum myenum;
		public string testString;

		public SerializeTest()
        {
			InitializeComponent(this);
        }

        public override void Start()
        {
			testString = "fuckfuck";
			myenum = testEnum.optionB;
			Console.WriteLine(actor.name + ": " + value);
        }
    }

	public class InheritanceTestA : Component
    {
		public int someValue = 420;
		public InheritanceTestA()
        {
			InitializeComponent(this);
        }

        public override void Start()
        {
			Internal.Log("hello from Test A");
        }
    }

	public struct testStruct
    {
		public int mySuperFunnyValue;
		public float myOtherFunnyValue;
		public string myString;
    }

	public struct testStruct2
    {
		public int mySuperFunnyValue;
		public float myOtherFunnyValue;
		public string myString;
		public testStruct ImpendingDisaster;
    }



	public class testClass
	{
		public int shit = -1;
	}

	public class StructSerializationTestA : Component
    {
		public testStruct2 test;
		public testClass tc;
		public Vector2 vector;

		public StructSerializationTestA()
        {
			InitializeComponent(this);
			tc = new();
        }

        public override void Start()
        {
			test.mySuperFunnyValue = 69;
			test.myOtherFunnyValue = 13.37f;
			test.myString = "oh no...";
			vector = Vector2.Random(69);
			tc.shit = 143;
        }
    }

    public class InheritanceTestB : InheritanceTestA
    {
		public int someOtherValue = 69;
		public InheritanceTestB()
        {
			InitializeComponent(this);
        }

        public override void Start()
        {
			//if this goes according to plan it should be A then B (which it is)
			base.Start();
			Internal.Log("hello from Test B"); 
		}
    }



    public class ComponentTest : Component
	{
		public ComponentTest()
		{
			InitializeComponent(this);
		}

		public void test()
		{
			Internal.Log("\n" + actor.name);
		}

		public override void Update()
		{
			Internal.Log("\n" + actor.name);
		}
	}

	public class MeshRenderer : Component
    {

    }

    public class SpriteRenderer : Component
    {
		public float Depth = 1;
		public S2Sprite sprite;
		public DrawMode drawMode = DrawMode.WhenLevelActive;

		public SpriteRenderer()
        {
			InitializeComponent(this);
        }

        public override void Update()
        {
			Depth.Clamp(0.1f, float.MaxValue);
			if (sprite == null) return;

			sprite.SetPosition(actor.position * (1 / Depth));
			sprite.SetRotation(actor.rotation.z);
			sprite.SetScale(actor.scale);

			Renderer.Draw(sprite);
        }
    }
}