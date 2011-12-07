/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.IO;
using System.Xml;
using System.Text;

namespace gbrainy.Core.Main
{
	public static class Preferences
	{
		static string file, config_path;
		static PreferencesStorage <string, string> storage;
		static Dictionary <string, string> defaults;

		public const string MemQuestionWarnKey = "MemQuestionWarn";
		public const string MemQuestionTimeKey = "MemQuestionTime";
		public const string DifficultyKey = "Difficulty";
		public const string MinPlayedGamesKey = "MinPlayedGames";
		public const string MaxStoredGamesKey = "MaxStoredGames";
		public const string ToolbarShowKey = "ToolbarShow";
		public const string ToolbarOrientationKey = "ToolbarOrientation";
		public const string ColorBlindKey = "ColorBlind";
		public const string ThemeKey = "Theme";
		public const string EnglishKey = "English";
		public const string EnglishVersionKey = "EnglishVersion";

		static Preferences ()
		{
			storage = new PreferencesStorage <string, string> ();
			defaults = new Dictionary <string, string> ();
			ConfigPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData),
				 Defines.CONFIG_DIR);

			LoadDefaultValues ();
			Load ();
		}

		static public string ConfigPath {
			set {
				config_path = value;
				file = Path.Combine (config_path, "Preferences.xml");
			}
		}

		public static void Save ()
		{
			try {
				if (!Directory.Exists (config_path))
					Directory.CreateDirectory (config_path);

				XmlTextWriter writer = new XmlTextWriter (file, Encoding.UTF8);
				writer.Formatting = Formatting.Indented;

				storage.WriteXml (writer);
				writer.Close ();
			}
			catch (Exception e)
			{
				Console.WriteLine ("Preferences.Save. Could not save file {0}. Error {1}", file, e);
			}
		}


		public static T Get<T> (string key)
		{
			string val;

			if (storage.TryGetValue (key, out val) == true)
			{
				return storage.Convert <T> (val);
			}
			return storage.Convert <T> (defaults [key]);
		}

		public static void Set<T> (string key, object val)
		{
			string v;
			T current;
			
			if (val is string)
				v = (string) val;
			else
				v = val.ToString ();

			current = Get <T> (key);

			// It is the same that the current stored
			if (current.Equals (val))
				return;

			// The new setting is equal to default
			if (v.Equals (defaults[key]) == true)
			{
				// Remove any previous value if existed
				storage.Remove (key);
				return;
			}

			storage [key] = v;
		}

		static public void Clear ()
		{
			storage.Clear ();
		}

		static void LoadDefaultValues ()
		{
			if (defaults.Count > 0)
				throw new InvalidOperationException ("The defaults should only be loaded once");

			defaults.Add (MemQuestionWarnKey, true.ToString ());
			defaults.Add (MemQuestionTimeKey, "6");
			defaults.Add (DifficultyKey, ((int)(GameDifficulty.Medium)).ToString ());
			defaults.Add (MinPlayedGamesKey, "5");
			defaults.Add (MaxStoredGamesKey, "20");
			defaults.Add (ToolbarShowKey, true.ToString ());
			defaults.Add (ToolbarOrientationKey, "1");
			defaults.Add (ColorBlindKey, false.ToString ());
			defaults.Add (ThemeKey, "notebook");
			defaults.Add (EnglishVersionKey, string.Empty);
			defaults.Add (EnglishKey, false.ToString ());
		}

		public static void Load ()
		{
			XmlTextReader reader = null;

			try {
				if (File.Exists (file) == false)
					return;

				reader = new XmlTextReader (file);
				storage.ReadXml (reader);
			}
			catch (Exception e)
			{
				Console.WriteLine ("Preferences.Load. Could not load file {0}. Error {1}", file, e);
			}
			finally
			{
				if (reader != null)
					reader.Close ();
			}
		}
	}
}
