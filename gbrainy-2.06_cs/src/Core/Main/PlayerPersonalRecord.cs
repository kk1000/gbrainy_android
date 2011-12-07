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

using System.Collections.Generic;

namespace gbrainy.Core.Main
{
	public class PlayerPersonalRecord
	{
		public const int MIN_GAMES_RECORD = 5;

		public GameTypes GameType { get; set; }
		public int PreviousScore { get; set; }
		public int NewScore { get; set; }

		public PlayerPersonalRecord (GameTypes type, int previous_score, int new_score)
		{
			GameType = type;
			PreviousScore = previous_score;
			NewScore = new_score;
		}

		// Check if the last recorded game has been a personal record
		static public List <PlayerPersonalRecord> GetLastGameRecords (List <GameSessionHistory> games, int last_game)
		{
			List <PlayerPersonalRecord> records = new List <PlayerPersonalRecord> ();
			GameSessionHistory higher;

			// We can start to talk about personal records after 5 plays
			if (last_game == -1 || games.Count < MIN_GAMES_RECORD)
				return records;

			higher = new GameSessionHistory ();

			// Find the higher record for every type of game
			for (int i = 0; i < last_game; i++)
			{
				if (games[i].LogicScore > higher.LogicScore) 
					higher.LogicScore = games[i].LogicScore;

				if (games[i].MathScore > higher.MathScore) 
					higher.MathScore = games[i].MathScore;

				if (games[i].MemoryScore > higher.MemoryScore) 
					higher.MemoryScore = games[i].MemoryScore;

				if (games[i].VerbalScore > higher.VerbalScore) 
					higher.VerbalScore = games[i].VerbalScore;				
			}
			
			// It is a record?
			if (games[last_game].LogicScore > higher.LogicScore)
				records.Add (new PlayerPersonalRecord (GameTypes.LogicPuzzle, higher.LogicScore, games[last_game].LogicScore));

			if (games[last_game].MathScore > higher.MathScore)
				records.Add (new PlayerPersonalRecord (GameTypes.Calculation, higher.MathScore, games[last_game].MathScore));

			if (games[last_game].MemoryScore > higher.MemoryScore)
				records.Add (new PlayerPersonalRecord (GameTypes.Memory, higher.MemoryScore, games[last_game].MemoryScore));

			if (games[last_game].VerbalScore > higher.VerbalScore)
				records.Add (new PlayerPersonalRecord (GameTypes.VerbalAnalogy, higher.VerbalScore, games[last_game].VerbalScore));

			return records;
		}
	}
}
