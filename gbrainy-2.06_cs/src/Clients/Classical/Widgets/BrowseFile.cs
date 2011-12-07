/*
 * Copyright (C) 2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
	// Adds a text box + browse button into a given hbox parent configuring
	// the standard browsedirectory widget for the application
	public class BrowseFile
	{
		Entry filename;
		Button browse;
		bool browse_file;
		Gtk.FileFilter[] filters;
		string default_dir;

		public virtual event EventHandler FileSelectedChanged;

		public BrowseFile (HBox parent, string file, bool browse_file)
		{
			this.browse_file = browse_file;
			filename = new Entry ();
			browse = new Button (Catalog.GetString ("Browse..."));
			Filename = file;

			browse.Clicked += new EventHandler (OnBrowse);

			parent.Add (filename);
			parent.Add (browse);

			Gtk.Box.BoxChild box = (Gtk.Box.BoxChild) parent[browse];
			box.Expand = false;
			box.Fill = false;

			parent.ShowAll ();
		}

		public string Filename {
			get { return filename.Text; }
			set { 
				if (value == null)
					filename.Text = string.Empty;
				else
					filename.Text = value;
			}
		}

		public string DefaultDirectory {
			set {
				if (browse_file == false)
					throw new InvalidOperationException ("Default directory can only be set when browsing files");
				
				default_dir = value;
			}
		}

		public Gtk.FileFilter[] Filters {
			set { filters = value; }
		}

		void OnBrowse (object o, EventArgs args)
		{
			FileChooserDialog chooser_dialog = new FileChooserDialog (
				Catalog.GetString ("Open Location") , null,
				browse_file ? FileChooserAction.Open : FileChooserAction.SelectFolder);


			if (browse_file) {
				if (default_dir != null)
					chooser_dialog.SetCurrentFolder (default_dir);
			}
			else {
				chooser_dialog.SetCurrentFolder (filename.Text);
			}

			chooser_dialog.AddButton (Stock.Cancel, ResponseType.Cancel);
			chooser_dialog.AddButton (Stock.Open, ResponseType.Ok);
			chooser_dialog.DefaultResponse = ResponseType.Ok;
			chooser_dialog.LocalOnly = false;

			if (filters != null) {
				foreach (Gtk.FileFilter filter in filters)
					chooser_dialog.AddFilter (filter);
			}

			if (chooser_dialog.Run () == (int) ResponseType.Ok) {
				filename.Text = chooser_dialog.Filename;

				if (FileSelectedChanged != null)
					FileSelectedChanged (this, EventArgs.Empty);
			}

			chooser_dialog.Destroy ();
		}
	}
}
