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
using Gtk;
using Mono.Unix;

using gbrainy.Core.Main;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class PreferencesDialog : BuilderDialog
	{
		[GtkBeans.Builder.Object] Gtk.SpinButton prefspinbutton;
		[GtkBeans.Builder.Object] Gtk.SpinButton maxstoredspinbutton;
		[GtkBeans.Builder.Object] Gtk.SpinButton minplayedspinbutton;
		[GtkBeans.Builder.Object] Gtk.CheckButton prefcheckbutton;
		[GtkBeans.Builder.Object] Gtk.CheckButton colorblindcheckbutton;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_easy;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_medium;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_master;
		[GtkBeans.Builder.Object] Gtk.ComboBox themes_combobox;
		[GtkBeans.Builder.Object] Gtk.CheckButton english_checkbutton;

		const int COLUMN_VALUE = 1;
		PlayerHistory history;

		public PreferencesDialog (PlayerHistory history) : base ("PreferencesDialog.ui", "preferences")
		{
			this.history = history;
			prefspinbutton.Value = Preferences.Get <int> (Preferences.MemQuestionTimeKey);
			prefcheckbutton.Active = Preferences.Get <bool> (Preferences.MemQuestionWarnKey);
			maxstoredspinbutton.Value = Preferences.Get <int> (Preferences.MaxStoredGamesKey);
			minplayedspinbutton.Value = Preferences.Get <int> (Preferences.MinPlayedGamesKey);
			colorblindcheckbutton.Active = Preferences.Get <bool> (Preferences.ColorBlindKey);
			english_checkbutton.Active = Preferences.Get <bool> (Preferences.EnglishKey);

			switch ((GameDifficulty) Preferences.Get <int> (Preferences.DifficultyKey)) {
			case GameDifficulty.Easy:
				rb_easy.Active = rb_easy.HasFocus = true;
				break;
			case GameDifficulty.Medium:
				rb_medium.Active = rb_medium.HasFocus = true;
				break;
			case GameDifficulty.Master:
				rb_master.Active = rb_master.HasFocus = true;
				break;
			}

			ListStore store = new ListStore (typeof (string), typeof (Theme)); // DisplayName, theme reference
			CellRenderer layout_cell = new CellRendererText ();
			themes_combobox.Model = store;
			themes_combobox.PackStart (layout_cell, true);
			themes_combobox.SetCellDataFunc (layout_cell, ComboBoxCellFunc);

			foreach (Theme theme in ThemeManager.Themes)
				store.AppendValues (Catalog.GetString(theme.LocalizedName), theme);

			// Default value
			TreeIter iter;
			bool more = store.GetIterFirst (out iter);
			while (more)
			{
				Theme theme = (Theme) store.GetValue (iter, COLUMN_VALUE);

				if (String.Compare (theme.Name, Preferences.Get <string> (Preferences.ThemeKey), true) == 0)
				{
					themes_combobox.SetActiveIter (iter);
					break;
				}
				more = store.IterNext (ref iter);
			}
		}

		private GameDifficulty Difficulty {
			get {
				if (rb_easy.Active)
					return GameDifficulty.Easy;

				if (rb_master.Active)
					return GameDifficulty.Master;

				return GameDifficulty.Medium;
			}
		}

		public bool NewThemeSet {set; get; }

		private void OnCleanHistory (object sender, EventArgs args)
		{
			if (ResponseType.Ok == HigMessageDialog.RunHigConfirmation (
				this,
				Gtk.DialogFlags.DestroyWithParent,
				Gtk.MessageType.Warning,
				Catalog.GetString ("You are about to delete the player's game session history."),
				Catalog.GetString ("If you proceed, you will lose the history of the previous game sessions. Do you want to continue?"),
				Catalog.GetString ("_Delete")))
			{
				history.Clean ();
			}
		}

		private void OnOK (object sender, EventArgs args)
		{
			Preferences.Set <int>  (Preferences.MemQuestionTimeKey, (int) prefspinbutton.Value);
			Preferences.Set <bool> (Preferences.MemQuestionWarnKey, prefcheckbutton.Active);
			Preferences.Set <int>  (Preferences.DifficultyKey, (int) Difficulty);
			Preferences.Set <int>  (Preferences.MaxStoredGamesKey, (int) maxstoredspinbutton.Value);
			Preferences.Set <int>  (Preferences.MinPlayedGamesKey, (int) minplayedspinbutton.Value);
			Preferences.Set <bool> (Preferences.ColorBlindKey, colorblindcheckbutton.Active);
			Preferences.Set <bool> (Preferences.EnglishKey, english_checkbutton.Active);

			TreeIter iter;
			themes_combobox.GetActiveIter (out iter);
			Theme theme = (Theme) themes_combobox.Model.GetValue (iter, COLUMN_VALUE);

			NewThemeSet = String.Compare (theme.Name, Preferences.Get <string> (Preferences.ThemeKey), true) != 0;
			Preferences.Set <string> (Preferences.ThemeKey, theme.Name);

			Preferences.Save ();
		}

		static public void ComboBoxCellFunc (CellLayout cell_layout, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			string name = (string)tree_model.GetValue (iter, 0);
			(cell as CellRendererText).Text = name;
		}
	}
}
