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
using System.IO;
using Gtk;
using Mono.Unix;

using gbrainy.Core.Main;
using gbrainy.Clients.Classical.Widgets;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class PdfExportDialog : BuilderDialog
	{
		[GtkBeans.Builder.Object] Gtk.HBox hbox_file;
		[GtkBeans.Builder.Object] Gtk.SpinButton games_spinbutton;
		[GtkBeans.Builder.Object] Gtk.CheckButton colorblindcheckbutton;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_easy;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_medium;
		[GtkBeans.Builder.Object] Gtk.RadioButton rb_master;
		[GtkBeans.Builder.Object] Gtk.CheckButton checkbox_logic;
		[GtkBeans.Builder.Object] Gtk.CheckButton checkbox_calculation;
		[GtkBeans.Builder.Object] Gtk.CheckButton checkbox_verbal;
		[GtkBeans.Builder.Object] Gtk.ComboBox layout_combo;

		BrowseFile file;
		const int COLUMN_VALUE = 1;
		const int DEF_SIDEVALUE = 4;

		public PdfExportDialog () : base ("PdfExportDialog.ui", "pdfexportbox")
		{
			games_spinbutton.Value = 10;
			checkbox_logic.Active = checkbox_calculation.Active = checkbox_verbal.Active = true;

			// Use defaults from Preferences
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
			// File selection
			string def_file;
			def_file = System.IO.Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments),
				// Translators: default file name used when exporting PDF files (keep the pdf extension please)
				Catalog.GetString ("games.pdf"));

			file = new BrowseFile (hbox_file, def_file, true);

			FileFilter[] filters = new FileFilter [2];
			filters[0] = new FileFilter ();
			filters[0].AddPattern ("*.pdf");
			filters[0].Name = Catalog.GetString ("PDF files");

			filters[1] = new FileFilter ();
			filters[1].AddPattern ("*.*");
			filters[1].Name = Catalog.GetString ("All files");

			file.Filters = filters;

			ListStore layout_store = new ListStore (typeof (string), typeof (int)); // DisplayName, index to array
			CellRenderer layout_cell = new CellRendererText ();
			layout_combo.Model = layout_store;
			layout_combo.PackStart (layout_cell, true);
			layout_combo.SetCellDataFunc (layout_cell, ComboBoxCellFunc);

			int [] per_side = PdfExporter.PagesPerSide;

			for (int i = 0; i < per_side.Length; i++)
				layout_store.AppendValues (per_side[i].ToString (), per_side[i]);

			// Default value
			TreeIter iter;
			bool more = layout_store.GetIterFirst (out iter);
			while (more)
			{
				if ((int) layout_store.GetValue (iter, COLUMN_VALUE) == DEF_SIDEVALUE) {
					layout_combo.SetActiveIter (iter);
					break;
				}
				more = layout_store.IterNext (ref iter);
			}
		}

		GameDifficulty Difficulty {
			get {
				if (rb_easy.Active)
					return GameDifficulty.Easy;

				if (rb_master.Active)
					return GameDifficulty.Master;

				return GameDifficulty.Medium;
			}
		}

		void OnOK (object sender, EventArgs args)
		{
			GameSession.Types types = GameSession.Types.None;

			if (checkbox_logic.Active)
				types |= GameSession.Types.LogicPuzzles;

			if (checkbox_calculation.Active)
				types |= GameSession.Types.Calculation;

			if (checkbox_verbal.Active)
				types |= GameSession.Types.VerbalAnalogies;


			TreeIter iter;

			layout_combo.GetActiveIter (out iter);
			int sides = (int) layout_combo.Model.GetValue (iter, COLUMN_VALUE);

			GeneratePdf (types,
				(int) games_spinbutton.Value,
				sides,
				Difficulty,
				colorblindcheckbutton.Active,
				file.Filename);
		}

		void GeneratePdf (GameSession.Types types, int num_games, int gamespage, GameDifficulty difficulty, bool colorblind, string filename)
		{
			Game [] games;
			GameManager gm;
			string msg;
			MessageType msg_type;

			games = new Game [num_games];
			gm = new GameManager ();
			gm.ColorBlind = colorblind;
			gm.Difficulty = difficulty;
			GtkClient.GameManagerPreload (gm);
			gm.GameType = types;

			for (int n = 0; n < num_games; n++)
			{
				 games [n] = gm.GetPuzzle ();
			}

			if (PdfExporter.GeneratePdf (games, gamespage, filename) == true) {
				msg = Catalog.GetString ("The PDF file has been exported correctly.");
				msg_type = MessageType.Info;
			} else {
				msg = Catalog.GetString ("There was a problem generating the PDF file. The file has not been created.");
				msg_type = MessageType.Error;
			}

			// Notify operation result
			MessageDialog md = new MessageDialog (this, DialogFlags.Modal, msg_type, ButtonsType.Ok, msg);
			md.Run ();
			md.Destroy ();
		}

		static public void ComboBoxCellFunc (CellLayout cell_layout, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			string name = (string)tree_model.GetValue (iter, 0);
			(cell as CellRendererText).Text = name;
		}
	}
}
