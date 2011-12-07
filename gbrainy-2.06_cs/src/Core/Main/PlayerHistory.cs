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
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace gbrainy.Core.Main
{
	public class PlayerHistory
	{
		string file, config_path;
		List <GameSessionHistory> games;
		int last_game;
	

		public PlayerHistory ()
		{
			ConfigPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData),
				 Defines.CONFIG_DIR);
			last_game = -1;
		}

		public string ConfigPath {
			set { 
				config_path = value;
				file = Path.Combine (config_path, "PlayerHistory.xml");
			}
		}

		public List <GameSessionHistory> Games {
			get {
				if (games == null)
				{
					Load ();
					if (games == null) 
						games = new List <GameSessionHistory> ();
				}
				return games; 
			}
		}

		public void Clean ()
		{
			Games.Clear ();
			Save ();
		}

		public void SaveGameSession (GameSessionHistory score)
		{
			if (score.GamesPlayed < Preferences.Get <int> (Preferences.MinPlayedGamesKey)) {
				last_game = -1;
				return;
			}

			if (Games.Count >= Preferences.Get <int> (Preferences.MaxStoredGamesKey))
				Games.RemoveAt (0);

			// Storing a copy to allow the input object to be modified
			Games.Add (score.Copy ());
			last_game = Games.Count - 1;
			Save ();
		}

		// Check if the last recorded game has been a personal record
		public List <PlayerPersonalRecord> GetLastGameRecords ()
		{
			return PlayerPersonalRecord.GetLastGameRecords (games, last_game);
		}

		public void Save ()
		{
			try {
				if (!Directory.Exists (config_path))
					Directory.CreateDirectory (config_path);

				using (FileStream str = File.Create (file))
				{
					XmlSerializer bf = new XmlSerializer (typeof (List <GameSessionHistory>));
					bf.Serialize (str, Games);
				}
			}
		
			catch (Exception e)
			{
				Console.WriteLine ("PlayerHistory.Save. Could not save file {0}. Error {1}", file, e);
			}
		}

		public void Load ()
		{
			try {				
				if (File.Exists (file) == false)
					return;

				using (FileStream str = File.OpenRead (file))
				{
					XmlSerializer bf = new XmlSerializer (typeof (List <GameSessionHistory>));
				    	games = (List <GameSessionHistory>) bf.Deserialize(str);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine ("PlayerHistory.Load. Could not load file {0}. Error {1}", file, e);
			}
		}	
	}
}
