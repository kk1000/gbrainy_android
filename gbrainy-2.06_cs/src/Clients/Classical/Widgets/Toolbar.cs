/*
 * Copyright (C) 2007-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Gtk;
using Mono.Unix;

namespace gbrainy.Clients.Classical.Widgets
{
	// Build and manages gbrainy's client Toolbar
	public class Toolbar : Gtk.Toolbar
	{
		Gtk.HBox main_hbox;
		Gtk.VBox framework_vbox;

		public ToolButton AllButton {get; private set; }
		public ToolButton CalculationButton {get; private set; }
		public ToolButton FinishButton {get; private set; }
		public ToolButton LogicButton {get; private set; }
		public ToolButton MemoryButton {get; private set; }
		public ToolButton PauseButton {get; private set; }
		public ToolButton VerbalButton {get; private set; }
		public bool InitCompleted {get; private set; }

		public Toolbar (Gtk.HBox main_hbox, Gtk.VBox framework_vbox)
		{
			this.main_hbox = main_hbox;
			this.framework_vbox = framework_vbox;
			ToolbarStyle = ToolbarStyle.Both;
			BuildToolBar ();
		}

		public void Attach (Gtk.Orientation orientation_new)
		{
			Gtk.Box.BoxChild child = null;
			Box box;

			switch (Orientation) {
			case Gtk.Orientation.Vertical:
				box = main_hbox;
				break;
			case Gtk.Orientation.Horizontal:
			{
				box = framework_vbox;
				break;
			}
			default:
				throw new InvalidOperationException ();
			}

			bool contained = false;
			foreach (var ch in box.AllChildren)
			{
				if (ch == this)
				{
					contained = true;
					break;
				}
			}
			if (contained == true)
				box.Remove (this);

			Orientation = (Gtk.Orientation) orientation_new;

			switch (Orientation) {
			case Gtk.Orientation.Vertical:
				main_hbox.Add (this);
				main_hbox.ReorderChild (this, 0);
				child = ((Gtk.Box.BoxChild)(main_hbox[this]));
				break;
			case Gtk.Orientation.Horizontal:
				framework_vbox.Add (this);
				framework_vbox.ReorderChild (this, 1);
				child = ((Gtk.Box.BoxChild)(framework_vbox[this]));
				break;
			default:
				throw new InvalidOperationException ();
			}

			child.Expand = false;
			child.Fill = false;
			ShowAll ();
			InitCompleted = true;
		}

		void BuildToolBar ()
		{
			IconFactory icon_factory = new IconFactory ();
			AddIcon (icon_factory, "logic-games", "logic-games-32.png");
			AddIcon (icon_factory, "math-games", "math-games-32.png");
			AddIcon (icon_factory, "memory-games", "memory-games-32.png");
			AddIcon (icon_factory, "verbal-games", "verbal-games-32.png");
			AddIcon (icon_factory, "pause", "pause-32.png");
			AddIcon (icon_factory, "resume", "resume-32.png");
			AddIcon (icon_factory, "endgame", "endgame-32.png");
			AddIcon (icon_factory, "allgames", "allgames-32.png");
			icon_factory.AddDefault ();

			IconSize = Gtk.IconSize.Dnd;

			AllButton = new ToolButton ("allgames");
			AllButton.TooltipText = Catalog.GetString ("Play all the games");
			AllButton.Label = Catalog.GetString ("All");
			Insert (AllButton, -1);

			LogicButton = new ToolButton ("logic-games");
			LogicButton.TooltipText = Catalog.GetString ("Play games that challenge your reasoning and thinking");
			LogicButton.Label = Catalog.GetString ("Logic");
			Insert (LogicButton, -1);

			CalculationButton = new ToolButton ("math-games");
			CalculationButton.Label = Catalog.GetString ("Calculation");
			CalculationButton.TooltipText = Catalog.GetString ("Play games that challenge your mental calculation skills");
			Insert (CalculationButton, -1);

			MemoryButton = new ToolButton ("memory-games");
			MemoryButton.Label = Catalog.GetString ("Memory");
			MemoryButton.TooltipText = Catalog.GetString ("Play games that challenge your short term memory");
			Insert (MemoryButton, -1);

			VerbalButton = new ToolButton ("verbal-games");
			VerbalButton.Label = Catalog.GetString ("Verbal");
			VerbalButton.TooltipText = Catalog.GetString ("Play games that challenge your verbal aptitude");
			Insert (VerbalButton, -1);

			PauseButton = new ToolButton ("pause");
			PauseButton.Label = Catalog.GetString ("Pause");
			PauseButton.TooltipText = Catalog.GetString ("Pause or resume the game");
			Insert (PauseButton, -1);

			FinishButton = new ToolButton ("endgame");
			FinishButton.TooltipText = Catalog.GetString ("End the game and show score");
			FinishButton.Label = Catalog.GetString ("End");
			Insert (FinishButton, -1);
		}

		static void AddIcon (IconFactory stock, string stockid, string resource)
		{
			Gtk.IconSet iconset = stock.Lookup (stockid);

			if (iconset != null)
				return;

			iconset = new Gtk.IconSet ();
			Gdk.Pixbuf img = Gdk.Pixbuf.LoadFromResource (resource);
			IconSource source = new IconSource ();
			source.Pixbuf = img;
			iconset.AddSource (source);
			stock.Add (stockid, iconset);
		}
	}
}
