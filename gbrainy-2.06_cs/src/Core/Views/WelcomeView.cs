/*
 * Copyright (C) 2007-2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */


using System;
using Cairo;
using System.Collections.Generic;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Core.Views
{
	public class WelcomeView : IDrawable
	{
		List <Toolkit.Container> containers;
		const double space = 0.17;
		const double image_size = 0.14;

		public WelcomeView ()
		{
			Container container;
			DrawableArea drawable_area;
			double y = 0.22;

			containers = new List <Toolkit.Container> ();
	
			/* Logic */
			container = new HorizontalContainer (0.05, y, 0.95, space);
			containers.Add (container);

			drawable_area = new DrawableArea (0.17, image_size);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawImageFromAssembly ("logic-games.svg", 0, 0, image_size, image_size);
			};

			drawable_area = new DrawableArea (0.75, space);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawStringWithWrapping (0, 0,
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Logic puzzles. Challenge your reasoning and thinking skills."), 
					e.Width);
			};

			/* Math */
			y += space;
			container = new HorizontalContainer (0.05, y, 0.95, space);
			containers.Add (container);

			drawable_area = new DrawableArea (0.17, image_size);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawImageFromAssembly ("math-games.svg", 0, 0, image_size, image_size);
			};

			drawable_area = new DrawableArea (0.75, space);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawStringWithWrapping (0, 0,
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Mental calculation. Arithmetical operations that test your mental calculation abilities."),
					e.Width);
			};

			/* Memory */
			y += space;
			container = new HorizontalContainer (0.05, y, 0.95, space);
			containers.Add (container);

			drawable_area = new DrawableArea (0.17, image_size);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawImageFromAssembly ("memory-games.svg", 0, 0, image_size, image_size);
			};

			drawable_area = new DrawableArea (0.75, space);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawStringWithWrapping (0, 0,
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Memory trainers. To prove your short term memory."),
					e.Width);
			};

			/* Verbal */
			y += space;
			container = new HorizontalContainer (0.05, y, 0.95, space);
			containers.Add (container);

			drawable_area = new DrawableArea (0.17, image_size);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawImageFromAssembly ("verbal-games.svg", 0, 0, image_size, image_size);
			};

			drawable_area = new DrawableArea (0.75, space);
			container.AddChild (drawable_area);
			drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
			{
				e.Context.DrawStringWithWrapping (0, 0,
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Verbal analogies. Challenge your verbal aptitude."),
					e.Width);
			};
		}

		public void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double y = 0.03;

			gr.Scale (area_width, area_height);
			gr.LineWidth = 0.005;

			gr.Color = new Cairo.Color (0, 0, 0, 1);

			gr.MoveTo (0.05, y);
			// Translators: {0} is the version number of the program
			gr.ShowPangoText (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Welcome to gbrainy {0}"), Defines.VERSION), true, -1, 0);
			gr.Stroke ();

			gr.DrawStringWithWrapping (0.05, y + 0.07, 
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained. It includes:"),
				1 - 0.05);

			y = 0.22 + space * 3;
			gr.DrawStringWithWrapping (0.05, y + 0.17,  ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Use the Settings to adjust the difficulty level of the game."),
				1 - 0.05);
			gr.Stroke ();

			foreach (Toolkit.Container container in containers)
				container.Draw (gr, area_width, area_height, rtl);
		}
	}
}
