/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

using gbrainy.Core.Main;

namespace gbrainy.Core.Toolkit
{
	// Horizontal container (supports RTL)
	// Child controls are stacked horizontally one each other (does not uses X, Y child coordinates)
	public class HorizontalContainer : Container
	{
		bool rtl;

		public HorizontalContainer (double x, double y, double width, double height) : base (x, y, width, height)
		{

		}		

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = X, y = Y;
	
			this.rtl = rtl;

#if DESIGN_MODE
			gr.Save ();
			gr.Color = new Cairo.Color (0, 0, 1);
			gr.Rectangle (X, Y, Width, Height);
			gr.Stroke ();
			gr.Restore ();

			double width = 0;

			foreach (Widget child in children)
			{
				width += child.Width;
				
				if (Height < child.Height)
					throw new InvalidOperationException (String.Format ("Container height too small {0} < {1}", Height, child.Height));
			}

			if (Width < width)
				throw new InvalidOperationException (String.Format ("Container witdh too small {0} < {1}", Width, width));
#endif
			//
			// Coordinates are stored right to left
			//
			if (rtl == false) {
				for (int i = 0; i < children.Count; i++)
				{
					gr.Save ();						
					gr.Translate (x, y);

					children[i].Draw (gr, area_width, area_height, rtl);
					gr.Restore ();
					x += children[i].Width;
				}
			} else {
				x += Width;
				for (int i = 0; i < children.Count; i++)
				{
					x -= children[i].Width;
					gr.Save ();
					gr.Translate (x, y);
					children[i].Draw (gr, area_width, area_height, rtl);
					gr.Restore ();
				}
			}
		}

		public override void MouseEvent (object obj, MouseEventArgs args)
		{
			double x = X, y = Y;

			if (rtl == true)
				x += Width;

			foreach (Widget child in Children)
			{
				if (rtl == true)
					x -= child.Width;

				if ((args.X >= x) && (args.X < x + child.Width) && 
					(args.Y >= y) && (args.Y < y + child.Height))
				{
					child.MouseEvent (this, args);
				} else {
					child.MouseEvent (this, new MouseEventArgs (-1, -1, args.EventType));
				}

				if (rtl == false)
					x += child.Width;
			}
		}
	}
}
