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

using Math = System.Math;


namespace S2DInternal
{
	public static class Internal
	{
		public static S2DContext? context;

		public static void s2log(object v)
		{
			Console.WriteLine(v);
		}

		public static string GetCWD()
		{
			return System.IO.Directory.GetCurrentDirectory();
		}

		public static bool CreateContext()
		{
			if (context == null)
			{
				context = new();
				context.MainWindow = new(new VideoMode(640, 480),
					"game", Styles.Default);
				return true;
			}

			return false;
		}

		public struct S2Sprite
		{
			public string path;
			public Sprite sprite;
			public int id;

			public S2Sprite(string path, Sprite sprite, int id)
			{
				this.path = path;
				this.sprite = sprite;
				this.id = id;
			}
		}

		public class TextureManager
		{
			public S2Sprite CreateSprite(string path)
			{

				return new S2Sprite("", null, Constants.NullTextureID);
			}
		}
	}

	public enum UpdateMode { WhenLevelActive, Always };
	public enum DrawMode { WhenLevelActive, DrawAlways, DontDraw };
	public enum LoadLevelType { Override, Background };

	public class Level
	{

	}

	public class LevelManager
	{

	}

	public static class Constants
	{
		public static int NullTextureID = -1;
		public static string ResourcesPath = "\\resources\\";
		public static string TexturesPath = "\\resources\\textures\\";
	}
	public class S2DContext
	{
		public RenderWindow? MainWindow;
		public string WindowName = "game";
	}

	public struct Vector2
	{
		float x, y;
		static Vector2 zero = new Vector2(0, 0);

		static float sqrt(float num)
		{
			return (float)Math.Sqrt((double)num);
		}

		static float pow(float num, float num2)
		{
			return (float)Math.Pow((double)num, (double)num2);
		}

		#region Class Methods
	static float Distance(Vector2 a, Vector2 b)
		{
			return sqrt(pow(a.x - b.x, 2) + pow(a.y - b.y, 2));
		}

		float Distance(Vector2 b)
		{
			return sqrt(pow(this.x - b.x, 2) + pow(this.y - b.y, 2));
		}

		void Zero()
		{
			x = 0;
			y = 0;
		}
		float magnitude()
		{
			return sqrt(pow(x, 2) + pow(y, 2));
		}
		#endregion

		#region Casts

		#endregion

		#region Constructors
		Vector2(float x = 0, float y = 0)
		{
			this.x = 0;
			this.y = 0;
		}

		Vector2(Vector2 d)
		{
			this.x = d.x;
			this.y = d.y;
		}

		Vector2(Vector2f d)
		{
			this.x = d.X;
			this.y = d.Y;
		}

		Vector2(Vec2 d)
		{
			this.x = d.X;
			this.y = d.Y;
		}
		#endregion

		#region Operators
		public static Vector2 operator * (Vector2 f, float n)
		{
			Vector2 r = new Vector2(f.x, f.y);
			r.x *= n;
			r.y *= n;
			return r;
		}

		public static Vector2 operator / (Vector2 f, float n)
		{
			Vector2 r = new Vector2(f.x, f.y);
			r.x /= n;
			r.y /= n;
			return r;
		}

		public static Vector2 operator +(Vector2 n, Vector2 t)
		{
			Vector2 r = new Vector2(n.x, n.y);
			r.x += t.x;
			r.y += t.y;
			return r;
		}

		public static Vector2 operator -(Vector2 n, Vector2 t)
		{
			Vector2 r = new Vector2(n.x, n.y);
			r.x -= t.x;
			r.y -= t.y;
			return r;
		}

		public static Vector2 operator *(Vector2 n, Vector2 t)
		{
			Vector2 r = new Vector2(n.x, n.y);
			r.x *= t.x;
			r.y *= t.y;
			return r;
		}
		#endregion
	}

	[System.Serializable]
	public class Actor
	{
		public bool active;
		public string name = "New Actor";
		public Vector2 
			position, scale;

		public float rotation = 0;
		public int instanceID;
		public Level? level;

		public UpdateMode updateMode = UpdateMode.WhenLevelActive;
		public DrawMode drawMode = DrawMode.WhenLevelActive;


	}

	public class Component
    {
		public Actor? actor;
		public UpdateMode updateMode = UpdateMode.WhenLevelActive;
	}
}
