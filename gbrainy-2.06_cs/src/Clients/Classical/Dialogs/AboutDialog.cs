/*
 * Copyright (C) 2008-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Gdk;
using Mono.Unix;
using System.Text;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class AboutDialog : Gtk.AboutDialog
	{
		public AboutDialog () : base ()
		{
			StringBuilder license = new StringBuilder (256);
			string [] authors = new string [] {
				Catalog.GetString ("Software"),
				"  Jordi Mas i Hernandez <jmas@softcatala.org>",
				"",
				Catalog.GetString ("Based on ideas by"),
				"  Terry Stickels",
				"  Jordi Mas i Hernandez",
				"  Lewis Carroll",
				("  " + Catalog.GetString ("MENSA works")),
				"  mathforum.org",
				"  Teun Spaans"
			};

			string [] artists = new string [] {
				"Anna Barber\u00e0 Mar\u00e9",
				"Carme Cabal Sard\u00e0",
				"Jordi Mas i Hernandez",
				"Felipe Menegaz",
				"John Cliff",
				"Openclipart.org",
			};

			string [] documenters = new string [] {
				"Milo Casagrande <milo@ubuntu.com>",
				"Jordi Mas i Hernandez <jmas@softcatala.org>"
			};

			// Translators: Replace by the name of the people that translated the application
			string translators = Catalog.GetString ("translator-credits");

			if (translators == "translator-credits")
				translators = null;

			license.Append (Catalog.GetString ("This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as  published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.\n\n"));
			license.Append (Catalog.GetString ("This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.\n\n"));
			license.Append (Catalog.GetString ("You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA."));

			ProgramName = "gbrainy";
			Version = Defines.VERSION;
			Authors = authors;
			Documenters = documenters;
			Logo = LoadFromAssembly ("gbrainy.svg");

			Copyright = Defines.COPYRIGHT;

			Comments = Catalog.GetString ("A brain teaser game for fun and to keep your brain trained.");
			Comments += "\n";
			Comments += String.Format (Catalog.GetString ("gbrainy project web site: {0}"), "http://live.gnome.org/gbrainy");
			Website = Defines.WEB_SITE;
			WebsiteLabel = String.Format (Catalog.GetString ("You can also play on-line at {0}"), Defines.WEB_SITE);
			TranslatorCredits = translators;
			Artists = artists;
			IconName = null;
			License = license.ToString ();
			WrapLicense = true;
			Response += delegate (object o, Gtk.ResponseArgs e) {Destroy ();};
		}

		static Pixbuf LoadFromAssembly (string resource)
		{
			try {
				return new Pixbuf (System.Reflection.Assembly.GetEntryAssembly (), resource);
			} 
			catch (Exception e)
			{
				Console.WriteLine ("AboutDialog.LoadFromAssembly. Could not load resource {0}. Error {1}", resource, e);
				return null;
			}
		}
	}
}
