/*
 * Copyright (C) 2008-2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Core.Main
{
	public class GameSessionHistoryExtended : GameSessionHistory
	{
		public int LogicPlayed { get; set; }
		public int LogicWon { get; set; }
		public int LogicRawScore { get; set; }

		public int MathPlayed { get; set; }
		public int MathWon { get; set; }
		public int MathRawScore { get; set; }

		public int MemoryPlayed { get; set; }
		public int MemoryWon { get; set; }
		public int MemoryRawScore { get; set; }

		public int VerbalPlayed { get; set; }
		public int VerbalWon { get; set; }
		public int VerbalRawScore { get; set; }

		public override void Clear ()
		{
			base.Clear ();
			LogicPlayed = LogicWon = LogicRawScore = 0;
			MathPlayed = MathWon = MathRawScore = 0;
			MemoryPlayed = MemoryWon = MemoryRawScore = 0;
			VerbalPlayed = VerbalWon = VerbalRawScore = 0;
		}

		public void UpdateScore (GameTypes type, GameDifficulty difficulty, int game_score)
		{
			GameSessionHistoryExtended history = this;
			Score.SessionUpdateHistoryScore (ref history, type, difficulty, game_score);
		}
	}
}
