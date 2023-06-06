using System;
using System.IO;
using System.Reflection;
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
using SFML.Graphics.Glsl;


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

		public static bool InitializeEngine()
        {
			CreateContext();
			Time.Initialize();
			context.MainWindow.Closed += (sender, e) =>
			{
				Internal.context.MainWindow.Close();
			};

			return true;
        }

		public static View calcView(Vector2f windowSize, float minRatio, float maxRatio)
		{
			Vector2f viewSize = windowSize;

			// clip ratio
			float ratio = viewSize.X / viewSize.Y;
			if (ratio < minRatio) // too high
				viewSize.Y = viewSize.X / minRatio;
			else if (ratio > maxRatio) // too wide
				viewSize.X = viewSize.Y * maxRatio;

			View view = new(new FloatRect(new Vector2(), viewSize));

			FloatRect viewport = new((windowSize - viewSize) / 2.0f, viewSize);
			viewport.Left /= windowSize.X;
			viewport.Top /= windowSize.Y;
			viewport.Width /= windowSize.X;
			viewport.Height /= windowSize.Y;
			view.Viewport = (viewport);

			return view;
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
		public static void Clamp(this float f, float min, float max)
        {
			if (f > max) f = max; return;
			if (f < min) f = min; return;
        }

		public static void AddOnce<T>(this List<T> to, T what)
		{
			if (!to.Contains(what)) to.Add(what);
		}

		public static void RemoveOnce<T>(this List<T> to, T what)
		{
			if (to.Contains(what)) to.Remove(what);
		}
	}

	public static class Input
	{

	}

	public static class S2GUI
	{
		public static List<Element> ActiveElements = new();

		public static RenderWindow MainWindow
        {
			get
            {
				return Internal.context.MainWindow;
            }
        }
        public static Vector2 Center
        {
			get
            {
				return MainWindow.GetView().Center;
            }
        }
        public static Vector2 TopCenter
		{
			get
			{
				return 
					(Vector2f)new Vector2f(Center.x, MainWindow.GetView().Viewport.Top * 0.25f);
			}
		}

		public static Vector2 BottomCenter
		{
			get
			{
				return
					Internal.context.MainWindow.GetView().Center -
					new Vector2f(0, Internal.context.MainWindow.GetView().Viewport.Height / 2);
			}
		}

		public static Vector2 CenterLeft;
		public static Vector2 CenterRight;
		public static Vector2 TopLeft;
		public static Vector2 TopRight;
		public static Vector2 BottomRight;
		public static Vector2 BottomLeft;


		public class Element
		{
			public float name;
			public bool movable = true;
			public bool collapsable = false;
			public bool active = true;

			public Vector2 anchor = TopCenter;
			public Vector2 position;
			public Vector2 size;
			bool init;

			public void Initialize()
			{
				if (!init)
				{
					ActiveElements.AddOnce(this);
					init = true;
				}
			}

			public Element()
			{

			}

			public virtual void Update()
			{

			}
		}

		public class Panel : Element
		{
			RectangleShape shape;

			public Panel(int width, int height) 
			{
				shape = new();
				size = new(width, height);
				shape.Size = size;
				shape.FillColor = new(16, 32, 64, 128);

				Initialize();
			}

			public override void Update()
			{
				shape.Size = size;
				shape.Position = anchor + position;
				Internal.context.MainWindow.Draw(shape);
				base.Update();
			}
		}
	}

	public enum UpdateMode { WhenLevelActive, Always };
	public enum DrawMode { WhenLevelActive, DrawAlways, DontDraw };
	public enum LoadLevelType { Override, Background };

	public class Object
	{

	}

	public class World
	{
		public string name = "world";
		public float Gravity = -9.81f;
		public Color backgroundColor;
		public S2Sprite backgroundSprite;
	}

	//the Scene datatype describes a Scene: the Actors contained within it and their
	//components, its name, and other attributes it may have. Scenes themselves are 
	//not actually used on their own, and are instead loaded via a SceneInstance.
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

		public bool Loaded(Scene scene)
        {
			bool b = false;
            for (int i = 0; i < ActiveLevels.Count; i++)
            {
				if (ActiveLevels[i].instanceOf == scene)
                {
					b = true;
					break;
                }
            }

			return b;
        }
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

		public Vector2 position;
		public Vector2 align;
		public Vector2 offset;
		public Texture spriteTexture;
		public Sprite sprite;
		public Color color = Color.white;

		public S2Sprite(string path, int id)
		{
			this.path = path;
			this.id = id;
			
			spriteTexture = new Texture(path);
			
			this.sprite = new Sprite(spriteTexture);

			SetScale(Vector2.one);
			offset = Vector2.zero;
			align = new Vector2(sprite.GetGlobalBounds().Width / 2, sprite.GetGlobalBounds().Height / 2);
		}

		public void SetPosition(Vector2 pos)
		{
			sprite.Position = pos + offset - align;
			position = pos + offset - align;
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
			sprite.Color = color;
			Internal.context.MainWindow.Draw(sprite);
		}
	}

	public struct DrawCall
    {
		public int priority;
		public Drawable drawable;

        public DrawCall(int priority, Drawable drawable)
        {
            this.priority = priority;
            this.drawable = drawable;
        }
    }

    //made specifically to allow ordered rendering
    public class Renderer
    {
		public static List<DrawCall> DrawCalls = new();

		public static bool ListContains(Drawable d)
        {
			bool r = false;

            for (int i = 0; i < DrawCalls.Count; i++)
            {
				if (DrawCalls[i].drawable == d)
                {
					r = true;
					break;
                }
            }

			return r;
        }

		public static void Draw(Drawable d, int order = -1)
        {
			if (order != -1)
            {
				if(!ListContains(d))DrawCalls.Insert(order, new(order, d));
			}
			else
            {
				DrawCalls.Add(new(DrawCalls.Count, d));
            }
        }

		public static void Draw(S2Sprite d, int order = -1)
        {
			Draw(d.sprite, order);
        }



		public static void RenderAll()
        {
			DrawCalls.Sort(compareCalls);
            for (int i = 0; i < DrawCalls.Count; i++)
            {
				Internal.context.MainWindow.Draw(DrawCalls[i].drawable);
				DrawCalls.RemoveAt(i);
            }
        }

        private static int compareCalls(DrawCall a, DrawCall b)
        {
			if (a.priority > b.priority) return 1;
			if (a.priority < b.priority) return -1;
			return 0;
		}
    }

    public struct Color
	{
		public float
			r, g, b, a;

		public static Color white =		new(1, 1, 1, 1);
		public static Color clear =		new(0, 0, 0, 0);
		public static Color semiclear =		new(1, 1, 1, 0.5f);
		public static Color red =			new(1, 0, 0, 1);
		public static Color green =		new(0, 1, 0, 1);
		public static Color blue =		new(0, 0, 1, 1);

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

		#region Casts
		public static implicit operator SFML.Graphics.Color(Color c)
		{
			return new((byte)c.r, (byte)c.g, (byte)c.b, (byte)c.a);
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

	public struct Vector3
	{
		public float x, y, z;
		public static Vector3 zero = new Vector3(0);
		public static Vector3 one = new Vector3(1, 1, 1);

		static float sqrt(float num)
		{
			return (float)Math.Sqrt((double)num);
		}

		static float pow(float num, float num2)
		{
			return (float)Math.Pow((double)num, (double)num2);
		}

		public override string ToString()
		{
			return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
		}

		public static Vector3 Random(float limit)
		{
			return new Vector3(S2Random.Range(-limit, limit), S2Random.Range(-limit, limit));
		}

		public static Vector3 Parse(string from)
		{
			Vector3 result = new();
			string[] split = from.Split('(', ' ', ',', ')');

			result.x = (float)double.Parse(split[1]);
			result.y = (float)double.Parse(split[split.Length - 2]);

			return result;
		}

		#region Class Methods
		static float Distance(Vector3 a, Vector3 b)
		{
			return sqrt(pow(a.x - b.x, 2) + pow(a.y - b.y, 2) + pow(a.z - b.z, 2));
		}

		float Distance(Vector3 b)
		{
			return sqrt(pow(this.x - b.x, 2) + pow(this.y - b.y, 2) + pow(this.z - b.z, 2));
		}

		void Zero()
		{
			x = 0;
			y = 0;
			z = 0;
		}

		#endregion

		#region Casts
		public static implicit operator Vector3(Box2DX.Common.Vec3 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static implicit operator Vector3(SFML.Graphics.Glsl.Vec3 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static implicit operator Vector3(Vector2f v)
		{
			return new Vector3(v.X, v.Y, 0);
		}

		public static implicit operator Vector3(Vector3f v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static implicit operator Vector3(Vector2 v)
		{
			return new Vector3(v.y, v.y, 0);
		}

		public static implicit operator Vector3(int v)
		{
			return new Vector3(v, v, v);
		}

		public static implicit operator Vector3(float v)
		{
			return new Vector3(v, v, v);
		}

		public static implicit operator Box2DX.Common.Vec3(Vector3 v)
		{
			return new Box2DX.Common.Vec3(v.x, v.y, v.z);
		}

		public static implicit operator SFML.Graphics.Glsl.Vec3(Vector3 v)
		{
			return new SFML.Graphics.Glsl.Vec3(v.x, v.y, v.z);
		}

		public static implicit operator Vector3f(Vector3 v)
		{
			return new Vector3f(v.x, v.y, v.z);
		}
		#endregion

		#region Constructors
		public Vector3(float x = 0, float y = 0, float z = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(Vector3 d)
		{
			this.x = d.x;
			this.y = d.y;
			this.z = d.z;
		}

		public Vector3(Vector3f d)
		{
			this.x = d.X;
			this.y = d.Y;
			this.z = d.Z;
		}

		public Vector3(Box2DX.Common.Vec3 d)
		{
			this.x = d.X;
			this.y = d.Y;
			this.z = d.Z;
		}

		public Vector3(SFML.Graphics.Glsl.Vec3 d)
		{
			this.x = d.X;
			this.y = d.Y;
			this.z = d.Z;
		}

		#endregion

		#region Operators
		public static Vector3 operator *(Vector3 f, float n)
		{
			Vector3 r = new Vector3(f.x, f.y, f.z);
			r.x *= n;
			r.y *= n;
			r.z *= n;
			return r;
		}

		public static Vector3 operator /(Vector3 f, float n)
		{
			Vector3 r = new Vector3(f.x, f.y, f.z);
			r.x /= n;
			r.y /= n;
			r.z /= n;
			return r;
		}

		public static Vector3 operator +(Vector3 n, Vector3 t)
		{
			Vector3 r = new Vector3(n.x, n.y, n.z);
			r.x += t.x;
			r.y += t.y;
			r.z += t.z;
			return r;
		}

		public static Vector3 operator -(Vector3 n, Vector3 t)
		{
			Vector3 r = new Vector3(n.x, n.y, n.z);
			r.x -= t.x;
			r.y -= t.y;
			r.z -= t.z;
			return r;
		}

		public static Vector3 operator *(Vector3 n, Vector3 t)
		{
			Vector3 r = new Vector3(n.x, n.y, n.z);
			r.x *= t.x;
			r.y *= t.y;
			r.z *= t.z;
			return r;
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

		public override string ToString()
		{
			return "(" + x.ToString() + ", " + y.ToString() + ")";
		}

		public static Vector2 Random(float limit)
		{
			return new Vector2(S2Random.Range(-limit, limit), S2Random.Range(-limit, limit));
		}

		public static Vector2 Parse(string from)
		{
			Vector2 result = new();
			string[] split = from.Split('(', ' ', ',', ')');

			result.x = (float)double.Parse(split[1]);
			result.y = (float)double.Parse(split[split.Length - 2]);

			return result;
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
		public static implicit operator Vector2(Box2DX.Common.Vec2 v)
		{
			return new Vector2(v.X, v.Y);
		}

		public static implicit operator Vector2(Vector2f v)
		{
			return new Vector2(v.X, v.Y);
		}

		public static implicit operator Vector2(Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}




		public static implicit operator Box2DX.Common.Vec2(Vector2 v)
		{
			return new Box2DX.Common.Vec2(v.x, v.y);
		}

		public static implicit operator SFML.Graphics.Glsl.Vec2(Vector2 v)
		{
			return new SFML.Graphics.Glsl.Vec2(v.x, v.y);
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

		public Vector2(Box2DX.Common.Vec2 d)
		{
			this.x = d.X;
			this.y = d.Y;
		}

		public Vector2(SFML.Graphics.Glsl.Vec2 d)
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

	//forgive me for the awful, awful things done here
	public class S2DSerializer
	{
		static string FILE_BEGIN = "FILE:\n";
		static string FILE_END = "\nEND";

		static string SCOPE_BEGIN = "\n{";
		static string SCOPE_END = "\n}";

		static string ARR_BEGIN = "[";
		static string ARR_END = "]";

		static string ATTR_BEGIN = "(";
		static string ATTR_END = ")";
		static string LINE_END = ";";

		static string OBJECT_SCOPE = "\nOBJ";

		static string ind = "\t";

		static string name = "level";
		static string path;
		static string filename;
		static string ext = ".s2";

		static string PARSER_QUOTE = "P_QUOTE";

		class serialize_rec_test_b
        {
			string myValue;

			public void init()
            {
				myValue = "this is a class";
            }
        }

		struct serialize_rec_test_a
        {
			string myValue;

			public void init()
            {
				myValue = "this is a struct";
            }
        }



        public class serialize_rec_test
        {
			serialize_rec_test_b funny_class;
			serialize_rec_test_a funny_struct;
			bool funny;
			int funny_number;
			string funny_str;
			int[] funny_arr;
			List<int> funny_list;

			public void init()
			{
				funny_str = "this class has classes in it";
				funny = false;
				funny_number = 69;
				funny_arr = new int[]{
					4, 2, 0, 6, 9,
                };
				funny_list = new();
				funny_list.Add(1);
				funny_list.Add(3);
				funny_list.Add(3);
				funny_list.Add(7);

				funny_class = new();
				funny_class.init();
				funny_struct = new();
				funny_struct.init();
			}
		}

		public static string SerializeObject(object obj, string ref_name, int depth = 0)
        {
			if (obj == null)
            {
				log("Cannot serialize a null object.");
				return "";
            }

			//iterate through every field in the object
			string result = "";
			string tabs = ""; //add tabs according to "depth" in the object
            for (int i = 0; i <= depth; i++)
            {

				tabs += "\t";
            }

			void add(string v)
			{
				result += tabs + "\n" + v;
			}

			void log(object j)
			{
				Console.WriteLine(j);
			}

			void nop() { };

			result += obj.GetType().Name + " " + ref_name + '\n' + tabs + "{";

            foreach (var field in obj.GetType().GetRuntimeFields())
            {
				if (field.IsStatic) continue;

				bool isStruct =
						field.FieldType.IsValueType &&
						!field.FieldType.IsEnum &&
						!field.FieldType.IsClass &&
						!field.FieldType.IsPrimitive;
				
				bool isString = field.FieldType == typeof(string);
				bool isArray = field.FieldType.IsArray;
				bool isClass = field.FieldType.IsClass;

				if(!isStruct && !isArray && !isClass)add(tabs + field.FieldType + " " + field.Name + " = " + field.GetValue(obj));

				if (isStruct)
                {
					add(tabs + SerializeObject(field.GetValue(obj), field.Name, depth + 1));
                }

				if (isClass && !isString)
                {
					add(tabs + SerializeObject(field.GetValue(obj), field.Name, depth + 1));
                }

				if (isString)
                {
					string final = "";
					add(tabs + field.FieldType + " " + field.Name + " = \"" + field.GetValue(obj) + "\"");
				}

				if (isArray)
                {
					add("\t" + field.FieldType + " " + field.Name + " = [");
					object field_arr_conv = field.GetValue(obj);

					//check if it can be converted to both an array or list
					if (field_arr_conv as Array != null) field_arr_conv = field_arr_conv as Array;
					//if (field_arr_conv as List<object> != null) field_arr_conv = field_arr_conv as List<object>;
					foreach (object item in field.GetValue(obj) as Array) // I fucking hate this
                    {
						isStruct =
								item.GetType().IsValueType &&
								!item.GetType().IsEnum &&
								!item.GetType().IsClass &&
								!item.GetType().IsPrimitive;

						isString = item.GetType() == typeof(string);
						isArray = item.GetType().IsArray;
						isClass = item.GetType().IsClass;

						if (!isStruct && !isArray && !isClass) add("\t\t" + item + ",");
					}
					add("\t]");
				}
            }

			result += "\n}";

            //prettyprint


			
            if (depth == 0)
            {
				var conv = result.ToList();
                bool found_brace = false; //because we don't want the first brace to be indented
				int iterations = conv.Count;  //cache list size beforehand because this can change in the loop
				string final_str = ""; 

				void str_update()
                {
					final_str = "";
					for (int i = 0; i < conv.Count; i++)
					{
						final_str += conv[i];
					}
				}

				for (int i = 0; i < iterations; i++)
                {
                    bool opening_brace = conv[i] == '{';
                    bool closing_brace = conv[i] == '}';

                    if (!found_brace)
                    {
                        found_brace = opening_brace;
                    }
                    else
                    {
                        if (opening_brace && conv[i - 1] == '\n')
                        {
							conv.Insert(i, '\t');
							str_update();
                        }

                        if (closing_brace && conv[i - 1] == '\n')
                        {
							conv.Insert(i, '\t');
							str_update();
						}
                    }
                }
				
				return final_str;
            }


            nop();
			return tabs + result;
        }

		public static void SerializeScene(Scene scene)
		{
			string result = scene.name + ":\n";
			result += FILE_BEGIN;
			
			void add(string v)
			{
				result += "\n" + v;
			}

			void log(object j)
			{
				Console.WriteLine(j);
			}
			static void nop() { }; //this is here specifically for breakpoints

			List<Component> actor_components = new();
			Actor act;

			//first, stuff all of the Actors into our will-be string
			for (int i = 0; i < scene.actors.Count; i++)
			{
				//then let's ALSO record all of the components on the scene's Actors

				act = scene.actors[i];
				for (int b = 0; b < act.components.Count; b++)
				{
					actor_components.Add(act.components[b]);
				}
				
				add("define_actor " + act.name);
				result += SCOPE_BEGIN;
				
				add("\tposition " + act.position.ToString() +		LINE_END);
				add("\trotation " + act.rotation.ToString() +		LINE_END);
				add("\tscale " + act.scale.ToString() +				LINE_END);
				add("\tactive " + act.active.ToString() +				LINE_END);
				if (act.parent != null)
				{
					add("\tparent " + act.parent.instanceID);
				}
				else
				{
					add("\tparent null");
				}

				if (act.scene != null)
				{
					add("\tscene " + act.scene.instanceOf.name + LINE_END);
				}
				else
				{
					add("\tscene null");
				}

				add("\tid " + act.instanceID.ToString() +			LINE_END);
				add("\tlayer " + act.layer.ToString() +					LINE_END);
				add("\tupdate " + act.updateMode.ToString() +	LINE_END);
				add("\ttags: ");

				if (act.Tags.Count != 0)
				{
					for (int b = 0; b < act.Tags.Count; b++)
					{
						add("\t\t" + act.Tags[b]);
					}
				}

				add("\tend_tags");

				result += SCOPE_END + "\n";
			}

			result += "\nCOMPONENTS:\n\n";
			Component cmp;

			for (int i = 0; i < actor_components.Count; i++)
			{
				cmp = actor_components[i];
				result += SerializeObject(cmp, "ActorComponent") + "\n";

				//because obviously it should be a component, so it'll have an actor
				//which means we can simply use the actor's ID as the actor it belongs to, and if the
				//field is a component (which it SHOULD be), we can use that component's instance ID, which
				//we can use to link together components and actors in deserialization

				/*foreach (var field in cmp.GetType().GetRuntimeFields())
				{
					//compare both the base and normal types to Actor and Component, 
					//because the field may just be a bare Component or Actor
					bool isActorOrComponent =
						field.FieldType == typeof(Component) ||
						field.FieldType.BaseType == typeof(Component) ||
						field.FieldType == typeof(Actor) ||
						field.FieldType.BaseType == typeof(Actor);

					bool isActor = field.FieldType.BaseType == typeof(Actor) ||
						field.FieldType == typeof(Actor);

					bool isComponent = field.FieldType.BaseType == typeof(Component) ||
						field.FieldType == typeof(Component);

					bool isStruct =
						field.FieldType.IsValueType &&
						!field.FieldType.IsEnum &&
						!field.FieldType.IsClass &&
						!field.FieldType.IsPrimitive;

					bool isString = field.FieldType == typeof(string);
					bool isArray = field.FieldType.IsArray;
					bool isClass = field.FieldType.IsClass && !isComponent && !isActor;
					
					log(field.FieldType.Name);
					if (isComponent)
					{
						//super awful dangerous cast magic, this is some of of the weirdest C# I've written
						bool successfulcast = (field.GetValue(cmp) as Component) != null;
						if (successfulcast)
						{
							//Console.WriteLine("OTHER COMPONENT'S NAME AND FIELD NAME: " + (field.GetValue(cmp) as Component).GetType().Name + ", " + field.Name);
							add("\t" + field.FieldType.FullName + " " + field.Name + " " + (field.GetValue(cmp) as Component).instanceID);
						}
					}

					//I don't know what'll happen if there's a struct IN the struct but we'll figure that out too I guess
					if (isStruct)
					{
						result += "\n\t" + field.FieldType.FullName + " " + field.Name + " {";
						Type s = field.GetValue(cmp).GetType();
						foreach (var struct_field in field.GetValue(cmp).GetType().GetFields())
						{
							result += "\n\t\t" + struct_field.Name + ": " + struct_field.
								GetValue(field.GetValue(cmp)).ToString();
						}

						result += "\n\t}\n";
					}

					if (isString)
					{
						result += "\n\t" + field.FieldType.FullName + " " + field.Name + " " + "\"" + field.GetValue(cmp) + "\"";
					}

					if (isClass && !isString)
					{
						result += "\n\t" + field.FieldType.FullName + " " + field.Name + ": {";
						Type s = field.GetValue(cmp).GetType();
						foreach (var class_field in field.GetValue(cmp).GetType().GetFields())
						{
							result += "\n\t\t" + class_field.Name + ": " + class_field.
								GetValue(field.GetValue(cmp)).ToString();
						}

						result += "\n\t}\n";
					}

					if (isActor)
					{
						add("\t" + field.FieldType.Name + " " + field.Name + ": " + (field.GetValue(cmp) as Actor).instanceID);
					}

					if(!isActorOrComponent && !isStruct && !isClass)
					{
						add("\t" + field.FieldType.Name + " " + field.Name + " " + field.GetValue(cmp));
					}
				}*/

				result += "\n\n";
			}

			string path = Internal.GetCWD() +
				Constants.ResourcesPath + "\\levels\\";

			if (!Directory.Exists(path)) Directory.CreateDirectory(path);

			result += FILE_END;

			
			File.WriteAllText(path + scene.name + ext, result);
			nop();
		}

		//todo: after reconstructing actors we'll also need to reconstruct components, then 
		//re-attach them to their actors or parent components using given IDs
		public static void LoadScene(string path)
		{
			string file = File.ReadAllText(path);
			//split the file's contents
			string[] split = file.Split(
				':', 
				' ', 
				';', 
				'[', 
				']', 
				'\n', 
				'\t'
				);

			//trim the excess

			List<string> non_whitespace = new();
			foreach (var item in split)
			{
				if(!string.IsNullOrWhiteSpace(item))non_whitespace.Add(item);
			}

			static void nop() { }; //this is here specifically for breakpoints
			static void emit(object j) { Console.WriteLine(j); };

			string[] filesplit = non_whitespace.ToArray();

			string actor_name = "";
			string position_s = "";
			Actor new_actor = new Actor();
			new_actor.name = "";
			Component new_component = new Component();
			Assembly current_assembly = Assembly.GetExecutingAssembly();
			Assembly other_assembly = null; //we'll figure this out when we get there

			List<string> contents = new();
			List<Actor> actors = new();
			List<Component> components = new();

			bool inActorDefinition = false;
			bool inActorNameDefinition = false;
			bool inComponentDefinition = false;
			bool inComponentsDefinition = false;
			bool inComponentFieldDefinition = false;
			bool inComponentStringDefinition = false;

			bool position = false, 
				scale = false, 
				rotation = false;

			string[] keywords = {
				"define_actor",
				"component",
				};

			string current_field = "";

			Type newType = null;
			object newComponent = null;

			void resetActor()
			{
				new_actor = new();
				new_actor.name = "";
			}

			Scene reconstructedScene = new();

			//really awful spaghetti deserialization code with silly hacks
			for (int i = 1; i < filesplit.Length; i++) //it's safe enough to start from i = 1 instead of 0 since that's literally just the filename
			{
				string item = filesplit[i];
				int next = i + 1;
				int last = i - 1;
				next = Math.Clamp(next, 0, filesplit.Length - 1);
				string item_next = filesplit[next];
				string item_last = filesplit[i - 1];
				
				if (item.Contains(keywords[0]))
				{
					inActorDefinition = true;
					inActorNameDefinition = true;
					continue;
				}

				if (inActorDefinition)
				{
					if (item == "{")
					{
						inActorNameDefinition = false;
					}

					if (item == "}")
					{
						actors.Add(new_actor);
						resetActor();

						inActorDefinition = false;
						inActorNameDefinition = false;
					}

					if (item == "position")
					{
						position = true;
						continue;
					}

					if (item == "rotation")
					{
						rotation = true;
						continue;
					}

					if (item == "scale")
					{
						scale = true;
						continue;
					}

					if (item == "active")
					{
						new_actor.active = bool.Parse(item_next);
					}

					if (item == "id")
					{
						new_actor.instanceID = int.Parse(item_next);
					}

					if (item == "update")
					{
						if (item_next == "WhenLevelActive")
						{
							new_actor.updateMode = UpdateMode.WhenLevelActive;
						}

						if (item_next == "Always")
						{
							new_actor.updateMode = UpdateMode.Always;
						}
					}

					//well - obviously if it's in this file it's in this scene, so we'll just link these up last, after the scene has been
					//reconstructed
					//if (item == "scene")
					
					if (position)
					{
						string pos_string = item + item_next;
						Vector2 parsed = Vector3.Parse(pos_string);
						new_actor.position = parsed;
						position = false;
					}

					if (rotation)
					{
						new_actor.rotation = Vector3.Parse(item);
						rotation = false;
					}
					
					if (scale)
					{
						string pos_string = item + item_next;
						Vector3 parsed = Vector3.Parse(pos_string);
						new_actor.scale = parsed;
						scale = false;
					}

					if (inActorNameDefinition)
					{
						new_actor.name += item + " ";
					}
				}

				if (item == "COMPONENTS:")
				{
					inActorDefinition = false;
					inActorNameDefinition = false;
					inComponentsDefinition = true;
				}

				if (item.Contains(keywords[1]))
				{
					newType = Type.GetType(item_next);
					if(newType != null)newComponent = Activator.CreateInstance(newType) as Component;
					continue;
				}

				if (item == "{")
				{
					inComponentDefinition = true;
					continue;
				}

				if (inComponentDefinition)
				{
					Type field_type = Type.GetType(item);
					if (item.StartsWith('\"'))
					{
						inComponentStringDefinition = true;
					}

					if (newComponent != null)
					{
						if (newComponent.GetType().GetField(item) != null)
						{
							current_field = item;
							inComponentFieldDefinition = true;
							if (field_type == typeof(string) || field_type == typeof(System.String))
                            {
								inComponentStringDefinition = true;
								nop();
                            }
						}
					}
				}

				if (inComponentStringDefinition)
                {
					nop();
					string str_field = "";
					char chr;
                    for (int b = 1; b < item.Length; b++)
                    {
						chr = item[b];
						if (chr != '\"') str_field += chr;
						if (b == item.Length - 1)
						{
							if (newComponent != null)
							{
								if (newComponent.GetType().GetField(current_field) != null) emit("found field with name " + current_field + " and assigned value " + str_field);
								newComponent.GetType().GetField(current_field).SetValue(newComponent, str_field);
							}

							inComponentStringDefinition = false;
							//break;
							//I don't know if this breaks out of both this loop and the one all this is in so I'll do this instead
							goto confuse;
						}
					}

					confuse:
					continue;
                }
			}

			reconstructedScene.actors = actors;

			GC.Collect();
			nop();
		}
	}

	public class VertexBuffer
	{
		public Vector3[] data;
		public VertexBuffer()
		{
			
		}
	}

	[System.Serializable]
	public class Actor
	{
		public Actor parent = null;
		public bool active;
		public string name = "New Actor";
		public Vector3
			position, scale = Vector3.one;

		public Vector3 rotation = Vector3.zero;
		public int instanceID;
		public SceneInstance scene;
		public UpdateMode updateMode = UpdateMode.WhenLevelActive;

		public int layer;
		public List<string> Tags = new List<string>();
		public List<Component> components = new();


		public static Actor Create(Vector3 position, Vector3 scale, Vector3 rotation, string name = "new Actor")
		{
			Actor actor = new();
			actor.active = true;
			actor.scale = scale;
			actor.position = position;
			actor.rotation = rotation;
			actor.name = name;
			actor.instanceID = S2Random.Range(0, int.MaxValue);
			actor.SetScene(SceneManager.CurrentScene);
			return actor;
		}

		public static Actor Copy(Actor a)
		{
			Actor clone = new Actor();
            /*			clone.position = a.position;
                        clone.rotation = a.rotation;
                        clone.scale = a.scale;

                        clone.components = a.components;
                        clone.Tags = a.Tags;

                        clone.instanceID = a.instanceID;
                        clone.layer = a.layer;

                        clone.active = a.active;
                        clone.name = a.name;

                        clone.SetScene(a.scene);
                        clone.updateMode = a.updateMode;
            */
            foreach (var field in clone.GetType().GetFields())
            {
				clone.GetType()
					.GetField(field.Name)
					.SetValue(clone, 
					a.GetType()
					.GetField(field.Name)
					.GetValue(a));
            }

			return clone;
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
			throw new System.NotImplementedException();
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
		public int instanceID;
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
			cmp.actor.components.AddOnce(cmp);
			cmp.enabled = true;
			cmp.Start();
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
			instanceID = S2Random.Range(0, int.MaxValue);
			UpdateManager.ActiveComponents.AddOnce(cmp);
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

	public class Time
    {
		public static float deltaTime, deltaTimeUnscaled, physicsDelta;
		public static float Timescale;
		public static float Current;
		
		public static Time Instance { get; private set; }
		protected SFML.System.Time timer;
		static float last;
		
		protected Time()
        {

        }
		
		static bool initialized;
		public static void Initialize()
        {
			if (initialized) return;

			Time tm = new();
			tm.timer = new();
			Instance = tm;
			initialized = true;
        }

		public void update()
        {
			Current = timer.AsSeconds();
			deltaTime = Current - last;
			deltaTimeUnscaled = deltaTime;

			deltaTime /= 1000;
			deltaTimeUnscaled /= 1000;

			deltaTime *= Timescale;
			last = Current;
		}

		public static void Update()
        {
			Instance.update();
        }
    }

    public static class UpdateManager
	{
		public static List<Component> InvalidComponents = new();
		public static List<Component> ActiveComponents = new();
		public static List<Actor> ActiveActors = new();

		public static void UpdateEngine()
		{
			Time.Update();
			Component cmp = null;
			for (int i = 0; i < ActiveComponents.Count; i++)
			{
				cmp = ActiveComponents[i];
				if (cmp == null) continue;
				if (cmp.actor == null)
                {
					Internal.Log("Component with ID " + cmp.instanceID + " does not have an Actor.");
					InvalidComponents.AddOnce(cmp);
					ActiveComponents.RemoveAt(i);
					continue;
                }

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

			for (int i = 0; i < S2GUI.ActiveElements.Count; i++)
			{
				if (!S2GUI.ActiveElements[i].active) continue;
				S2GUI.ActiveElements[i].Update();
			}

			Renderer.RenderAll();

            for (int i = 0; i < InvalidComponents.Count; i++)
            {
				

			}
		}
	}
}
