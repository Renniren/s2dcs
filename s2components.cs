﻿using System;
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
		public S2Sprite CreateSprite(string path)
		{

			return new S2Sprite("", Constants.NullTextureID);
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

	public class SpriteRenderer : Component
    {
		public S2Sprite? sprite;

		public SpriteRenderer()
        {
			InitializeComponent(this);
        }

        public override void Update()
        {
			if (sprite == null) return;

        }
    }
}