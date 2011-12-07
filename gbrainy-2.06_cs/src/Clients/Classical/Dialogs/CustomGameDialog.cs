/*
 * Copyright (C) 2007-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Collections.Generic;

using gbrainy.Core.Main;
using gbrainy.Clients.Classical.Widgets;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class CustomGameDialog : BuilderDialog
	{
		// This is static to allow to preserve the selection in different instances
		static ListStore games_store;
		[GtkBeans.Builder.Object] Gtk.TreeView treeview;
		[GtkBeans.Builder.Object] Box preview_vbox;
		GameDrawingArea drawing_area;
		GameManager manager;
		GameManager.GameLocator [] games;
		bool selection_done;

		const int COL_NAME = 0;
		const int COL_TYPE = 1;
		const int COL_ENABLED = 2;
		const int COL_OBJECT = 3;
		const int COL_INDEX = 4;

		public CustomGameDialog (GameManager manager) : base ("CustomGameDialog.ui", "customgame")
		{
			Game game;

			selection_done = false;
			this.manager = manager;
			games = manager.AvailableGames;

			drawing_area = new GameDrawingArea ();
			drawing_area.UseSolutionArea = false;
			preview_vbox.Add (drawing_area);
			drawing_area.Visible = true;
			treeview.HeadersClickable = true;

			// Column: Game Name
			TreeViewColumn name_column = new TreeViewColumn (Catalog.GetString("Game Name"), 
				new CellRendererText(), "text", 0);

			name_column.Expand = true;
			name_column.Clickable = true;
			name_column.Clicked += delegate (object sender, EventArgs args)
			{
				Gtk.SortType order;
				Gtk.TreeViewColumn column = (Gtk.TreeViewColumn) sender;

				if (column.SortOrder == Gtk.SortType.Ascending)
					order = Gtk.SortType.Descending;
				else 
					order = Gtk.SortType.Ascending;

				column.SortOrder = order;
				games_store.SetSortColumnId (COL_NAME, order);
			};

			treeview.AppendColumn (name_column);

			// Column: Type
			TreeViewColumn type_column = new TreeViewColumn (Catalog.GetString("Type"), 
				new CellRendererText(), "text", 1);

			type_column.Expand = true;
			treeview.AppendColumn (type_column);
			type_column.Clickable = true;
			type_column.Clicked += delegate (object sender, EventArgs args)
			{
				Gtk.SortType order;
				Gtk.TreeViewColumn column = (Gtk.TreeViewColumn) sender;

				if (column.SortOrder == Gtk.SortType.Ascending)
					order = Gtk.SortType.Descending;
				else 
					order = Gtk.SortType.Ascending;

				column.SortOrder = order;
				games_store.SetSortColumnId (COL_TYPE, order);
			};

			// Column: Selected
			CellRendererToggle toggle_cell = new CellRendererToggle();
			TreeViewColumn toggle_column = new TreeViewColumn(Catalog.GetString("Selected"),
				toggle_cell, "active", COL_ENABLED);
			toggle_cell.Activatable = true;
			toggle_cell.Toggled += OnActiveToggled;
			toggle_column.Expand = false;
			treeview.CursorChanged += OnCursorChanged;
			treeview.AppendColumn (toggle_column);

			if (games_store == null) {
				games_store = new ListStore (typeof(string), typeof (string), typeof(bool), typeof (Game), typeof (int));

				games_store.SetSortFunc (0, new Gtk.TreeIterCompareFunc (GameSort));
				games_store.SetSortColumnId (COL_TYPE, SortType.Ascending);
					 
				// Data
				string type;
				for (int i = 0; i < games.Length; i++)
				{
					if (games [i].IsGame == false)
						continue;

					game = (Game) Activator.CreateInstance (games [i].TypeOf, true);
					game.Variant = games [i].Variant;
					type = GameTypesDescription.GetLocalized (game.Type);
					games_store.AppendValues (game.Name, type, true, game, i);
				}
			}

			treeview.Model = games_store;
			game = (Game) Activator.CreateInstance (games [0].TypeOf, true);
			game.Variant = 0;
			game.Begin ();
			drawing_area.Drawable = game;
			drawing_area.Question = game.Question;
			treeview.ColumnsAutosize ();
		}

		public bool SelectionDone {
			get { return selection_done;}
		}

		public static void Clear ()
		{
			if (games_store != null)
				games_store.Dispose ();

			games_store = null;
		}

		int GameSort (Gtk.TreeModel model, Gtk.TreeIter a, Gtk.TreeIter b)
		{
			string name_a, name_b;
			int sort_column_id;
			SortType order;
			ListStore store = (ListStore) model;

			store.GetSortColumnId (out sort_column_id, out order);

			name_a = (string) model.GetValue (a, sort_column_id);
			name_b = (string) model.GetValue (b, sort_column_id);
			return String.Compare (name_a, name_b);
		}

		private void OnCursorChanged (object o, EventArgs args) 
		{
			TreeIter iter;
		    
		    	if (!treeview.Selection.GetSelected (out iter)) {
				return;
		    	}

			Game game = games_store.GetValue (iter, COL_OBJECT) as Game;

			// We should not be using IsPreviewMode to know if Initialize has been called
			if (game.IsPreviewMode == false) 
			{
				game.IsPreviewMode = true;
				game.Begin ();
			}

			drawing_area.Drawable = game;
			drawing_area.Question = game.Question;
			drawing_area.QueueDraw ();
		}

		private void OnActiveToggled (object o, ToggledArgs args) 
		{
			TreeIter iter;

			if (!games_store.GetIter (out iter, new TreePath (args.Path)))
				return;

			bool enabled = !(bool) games_store.GetValue (iter, COL_ENABLED);
			games_store.SetValue (iter, COL_ENABLED, enabled);
		}

		void OnSelectAll (object sender, EventArgs args)
		{
			games_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)  {
				games_store.SetValue (iter, COL_ENABLED, true);
				return false;
			});
		}

		void OnUnSelectAll (object sender, EventArgs args)
		{
			games_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)  {
				games_store.SetValue (iter, COL_ENABLED, false);
				return false;
			});
		}

		void OnOK (object sender, EventArgs args)
		{
			List <int> play_list;

			play_list = new List <int> ();

			games_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)  {
				bool enabled = (bool) games_store.GetValue (iter, COL_ENABLED);

				if (enabled == true) {
					selection_done = true;
					int idx = (int) games_store.GetValue (iter, COL_INDEX);
					play_list.Add (idx);
				}
				return false;
			});

			if (selection_done == true)
				manager.PlayList = play_list.ToArray ();
		}
	}
}
