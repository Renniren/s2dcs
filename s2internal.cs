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


namespace S2DCore
{
	public static class Internal
	{
		public static S2DContext? context;

		public static void Log(object v)
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

			if (!Directory.Exists(GetCWD() + Constants.ResourcesPath))
            {

            }

			return false;
		}
	}

	public static class Extensions
    {
		public static void AddOnce<T>(this List<T> to, T what)
        {
			if (!to.Contains(what)) to.Add(what);
        }

		public static void RemoveOnce<T>(this List<T> to, T what)
        {
			if (to.Contains(what)) to.Remove(what);
        }
    }

    public enum UpdateMode { WhenLevelActive, Always };
	public enum DrawMode { WhenLevelActive, DrawAlways, DontDraw };
	public enum LoadLevelType { Override, Background };

    public class World
    {
		public string name = "world";
		
    }

    public class Scene
	{

	}

	public class SceneInstance
	{
		public Scene instanceOf;
    }

    public class SceneManager
	{
		public List<SceneInstance> ActiveLevels = new();
		public SceneInstance CurrentScene;
    }

	public static class S2Random
    {
		public static int Range(int lower, int upper)
        {
			return new Random().Next(lower, upper);
        }

		public static float Range(float lower, float upper)
        {
			System.Random random = new System.Random();
			double val = (random.NextDouble() * (upper - lower) + lower);
			return (float)val;
		}
    }

    public static class Constants
	{
		public static int NullTextureID = -1;
		public static string ResourcesPath = "\\resources\\";
		public static string TexturesPath = ResourcesPath + "\\textures\\";
		public static string SoundsPath = ResourcesPath + "\\sounds\\";
	}

	public class S2DContext
	{
		public RenderWindow? MainWindow;
		public string WindowName = "game";
	}

	public struct S2Sprite
	{
		public string path;
		public int id;

		public Vector2 offset;
		public Texture spriteTexture;
		public Sprite sprite;

		public S2Sprite(string path, int id)
		{
			this.path = path;
			this.id = id;
			this.sprite = new Sprite();
			spriteTexture = new Texture(path);
			sprite.Texture = spriteTexture;
			offset = Vector2.zero;
		}

		public void SetPosition(Vector2 pos)
        {
			sprite.Position = pos + offset;
        }

		public void SetRotation(float r)
        {
			sprite.Rotation = r;
		}

		public void SetScale(Vector2 scale)
        {
			sprite.Scale = scale;
		}

		public void Draw()
        {
			Internal.context.MainWindow.Draw(sprite);
        }
	}

	public struct Vector2
	{
		float x, y;
		public static Vector2 zero = new Vector2(0, 0);

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
		public static implicit operator Vector2(Vec2 v)
        {
			return new Vector2(v.X, v.Y);
        }

		public static implicit operator Vector2(Vector2f v)
        {
			return new Vector2(v.X, v.Y);
        }


		public static implicit operator Vec2(Vector2 v)
        {
			return new Vec2(v.x, v.y);
        }

		public static implicit operator Vector2f(Vector2 v)
        {
			return new Vector2f(v.x, v.y);
        }
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
		public SceneInstance? level;

		public UpdateMode updateMode = UpdateMode.WhenLevelActive;
		public DrawMode drawMode = DrawMode.WhenLevelActive;

		public int layer;
		public List<string> Tags;

		public static Actor Create()
        {
			Actor actor = new();
			actor.active = true;
			return actor;
        }

		public void Destroy()
        {

        }
	}

	public abstract class Component
    {
		public Actor actor;
		public int intanceID;
		public bool enabled = true;

		public static T Add<T>(Actor to) where T :  Component, new()
        {
			T cmp = new();
			cmp.actor = to;
			cmp.enabled = true;
			return cmp;
        }

		public static T? Get<T>(Actor from) where T :  Component, new()
        {
			T cmp = null;
            for (int i = 0; i < UpdateManager.ActiveComponents.Count; i++)
            {
				cmp = (T?)UpdateManager.ActiveComponents[i];
				if (cmp.GetType() == typeof(T))
                {
					if (cmp.actor == from)
                    {
						return cmp;
                    }
                }
            }

			return null;
        }

		public void InitializeComponent(Component cmp)
        {
			UpdateManager.ActiveComponents.AddOnce(this);
        }

		~Component() {

        }

		#region Class Methods
		public virtual void Start()
        {

        }

		public virtual void PreUpdate()
        {

        }

		public abstract void Update();

		public virtual void PhysUpdate()
        {

        }

		public virtual void LateUpdate()
        {

        }
        #endregion
    }



    public static class UpdateManager
    {
		public static List<Component> ActiveComponents = new();
		public static List<Actor> ActiveActors = new();


		public static void UpdateEngine()
		{
			Component cmp = null;
            for (int i = 0; i < ActiveComponents.Count; i++)
            {
				cmp = ActiveComponents[i];
				if (cmp == null) continue;
				if (!cmp.enabled) continue;
				if (!cmp.actor.active) continue;

				cmp.PreUpdate();
			}

			for (int i = 0; i < ActiveComponents.Count; i++)
            {
				cmp = ActiveComponents[i];
				if (cmp == null) continue;
				if (!cmp.enabled) continue;
				if (!cmp.actor.active) continue;

				cmp.Update();
            }

			for (int i = 0; i < ActiveComponents.Count; i++)
            {
				cmp = ActiveComponents[i];
				if (cmp == null) continue;
				if (!cmp.enabled) continue;
				if (!cmp.actor.active) continue;

				cmp.LateUpdate();
            }
		}
	}
}
