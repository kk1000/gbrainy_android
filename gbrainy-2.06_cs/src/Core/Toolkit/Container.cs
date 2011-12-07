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
using System.Collections.Generic;

using gbrainy.Core.Main;

namespace gbrainy.Core.Toolkit
{
	//
	// Base Container class
	// Does not support rtl and all the coordinates are drawable context (not to the container)
	// 
	public class Container : Widget
	{
		protected List <Widget> children = new List <Widget> ();

		public Container () : base (0, 0)
		{

		}
		
		public Container (double x, double y, double width, double height) : base (width, height)
		{
			if (x < 0 || x > 1)
				throw new ArgumentOutOfRangeException ("x");

			if (y < 0 || y > 1)
				throw new ArgumentOutOfRangeException ("y");

			X = x;
			Y = y;
		}

		public Widget [] Children {
			get { return children.ToArray (); }
		}

		public void AddChild (Widget widget)
		{
			if (children.Contains (widget))
				throw new InvalidOperationException ("Child already exists in container");

			//
			// Propagate events from child controls to this container (parent)
			//
			widget.DrawRequest += delegate (object sender, EventArgs e)
			{
				OnDrawRequest ();
			};

			widget.SelectedEvent += delegate (object sender, SeletectedEventArgs e)
			{
				OnSelected (e);
			};

			children.Add (widget);
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
#if DESIGN_MODE
			gr.Save ();
			gr.Color = new Cairo.Color (0, 0, 1);
			gr.Rectangle (X, Y, Width, Height);
			gr.Stroke ();
			gr.Restore ();
#endif
			foreach (Widget child in children)
			{
				gr.Save ();						
				gr.Translate (child.X, child.Y);
				child.Draw (gr, area_width, area_height, rtl);
				gr.Restore ();
			}

		}

		public override void MouseEvent (object obj, MouseEventArgs args)
		{
			foreach (Widget child in Children)
			{
				if ((args.X >= child.X) && (args.X < child.X + child.Width) &&
					(args.Y >= child.Y) && (args.Y < child.Y + child.Height))
				{
					child.MouseEvent (this, args);
				} else {
					child.MouseEvent (this, new MouseEventArgs (-1, -1, args.EventType));
				}
			}
		}
	}
}
