using System;
using System.IO;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
		public float Gravity = -9.81f;
		public Color backgroundColor;
		public S2Sprite backgroundSprite;
    }

	[System.Serializable]
    public class Scene
	{
		public string name;
		public World world = new World();
		public List<Actor> actors = new List<Actor>();
	}

	public class SceneInstance
	{
		public Scene instanceOf;
		public SceneInstance(Scene s)
        {
			instanceOf = s;
        }
    }

    public class SceneManager
	{
		public static List<SceneInstance> ActiveLevels = new();
		public static SceneInstance CurrentScene;
		public static SceneInstance DestroyedObjectLevel;
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
		public static float SpriteScale = 64;
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

	public class S2Sprite
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
			
			spriteTexture = new Texture(path);
			
			this.sprite = new Sprite(spriteTexture);

			SetScale(Vector2.one);
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
			sprite.Scale = new Vector2f(
			scale.x / sprite.GetLocalBounds().Width,
			scale.y / sprite.GetLocalBounds().Height) * Constants.SpriteScale;
		}

		public void Draw()
        {
			Internal.context.MainWindow.Draw(sprite);
        }
	}

	public struct Color
	{
		public float
			r, g, b, a;

		#region Constructors
		public Color(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public Color(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1;
		}

		public Color(float r = 1)
		{
			this.r = r;
			this.g = r;
			this.b = r;
			this.a = 1;
		}

		public Color(Color c)
		{
			this.r = c.r;
			this.g = c.g;
			this.b = c.b;
			this.a = c.a;
		}
		#endregion

		#region Operators
		public static Color operator +(Color a, Color b)
		{
			Color c = new(a);
			a.r += b.r;
			a.g += b.g;
			a.b += b.b;
			a.a += b.a;
			return c;
		}

		public static Color operator -(Color a, Color b)
		{
			Color c = new(a);
			a.r -= b.r;
			a.g -= b.g;
			a.b -= b.b;
			a.a -= b.a;
			return c;
		}

		public static Color operator /(Color a, Color b)
		{
			Color c = new(a);
			a.r /= b.r;
			a.g /= b.g;
			a.b /= b.b;
			a.a /= b.a;
			return c;
		}


		public static Color operator /(Color a, float b)
		{
			Color c = new(a);
			a.r /= b;
			a.g /= b;
			a.b /= b;
			a.a /= b;
			return c;
		}

		public static Color operator *(Color a, float b)
		{
			Color c = new(a);
			a.r -= b;
			a.g -= b;
			a.b -= b;
			a.a -= b;
			return c;
		}
		#endregion
	}

	public struct Vector2
	{
		public float x, y;
		public static Vector2 zero = new Vector2(0, 0);
		public static Vector2 one = new Vector2(1, 1);

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
		public Vector2(float x = 0, float y = 0)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2(Vector2 d)
		{
			this.x = d.x;
			this.y = d.y;
		}

		public Vector2(Vector2f d)
		{
			this.x = d.X;
			this.y = d.Y;
		}

		public Vector2(Vec2 d)
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

	public class S2DSerializer
    {
		public void SerializeScene(Scene scene)
        {
			string result = "";
			result += JsonSerializer.Serialize(scene.world);

        }
    }

    [System.Serializable]
	public class Actor
	{
		public bool active;
		public string name = "New Actor";
		public Vector2 
			position, scale = Vector2.one;

		public float rotation = 0;
		public int instanceID;
		public SceneInstance? scene;
		public UpdateMode updateMode = UpdateMode.WhenLevelActive;

		public int layer;
		public List<string> Tags = new List<string>();

		public static Actor Create(Vector2 position, Vector2 scale, string name = "New Actor", float rotation = 0)
        {
			Actor actor = new();
			actor.active = true;
			actor.scale = scale;
			actor.position = position;
			actor.rotation = rotation;
			actor.SetScene(SceneManager.CurrentScene);
			return actor;
        }

		public T GetComponent<T>() where T : Component, new()
		{
			return Component.Get<T>(this);
        }

		public T AddComponent<T>() where T : Component, new()
		{
			return Component.Add<T>(this);
        }



		public void SetScene(SceneInstance destination)
        {
			scene = destination;
			
        }

		public void Destroy()
        {

        }

		public static bool operator true(Actor a)
        {
			return a != null;
        }

		public static bool operator false(Actor a)
        {
			return a == null;
        }

		public static bool operator !(Actor a)
        {
			return a == null;
        }
	}

	public class Component
    {
		public Actor actor;
		public int intanceID;
		public bool enabled = true;

		public static T Add<T>(Actor to) where T :  Component, new()
        {
			if (!to) 
			{
				Internal.Log("Cannot add a component to a null Actor");
				return null;
			}
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
				cmp = UpdateManager.ActiveComponents[i] as T;
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

		public static void TryGet<T>(Actor from, out T? v) where T :  Component
        {
            for (int i = 0; i < UpdateManager.ActiveComponents.Count; i++)
            {
				if (UpdateManager.ActiveComponents[i].GetType() == typeof(T))
                {
					if (UpdateManager.ActiveComponents[i].actor == from)
                    {
						v = (T?)UpdateManager.ActiveComponents[i];
						return;
					}
                }
            }

			v = null;
        }



		public void InitializeComponent(Component cmp)
        {
			UpdateManager.ActiveComponents.AddOnce(this);
        }

		~Component() {

        }

		#region Class Methods
		public static bool operator true(Component a)
		{
			return a != null;
		}

		public static bool operator false(Component a)
		{
			return a == null;
		}

		public static bool operator !(Component a)
		{
			return a == null;
		}

		public virtual void Start()
        {

        }

		public virtual void PreUpdate()
        {

        }

		public virtual void Update()
        {

        }

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
				switch(cmp.actor.updateMode)
                {
					case UpdateMode.Always:
						cmp.PreUpdate();
						cmp.Update();
						cmp.LateUpdate();
						break;

					case UpdateMode.WhenLevelActive:
						if(SceneManager.CurrentScene == cmp.actor.scene)
                        {
							cmp.PreUpdate();
							cmp.Update();
							cmp.LateUpdate();
						}
						break;
                }
            }
		}
	}
}
