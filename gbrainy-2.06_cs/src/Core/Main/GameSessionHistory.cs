/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Xml.Serialization;

namespace gbrainy.Core.Main
{
	[Serializable]
	// Old class name, to keep compatibility when serializing with previous PlayerHistory files
	[XmlType("GameHistory")]
	public class GameSessionHistory
	{
		[XmlElementAttribute ("games_played")]
		public int GamesPlayed { get; set; }	

		[XmlElementAttribute ("games_won")]
		public int GamesWon { get; set; }

		[XmlElementAttribute ("total_score")]
		public int TotalScore { get; set; }

		[XmlElementAttribute ("math_score")]
		public int MathScore { get; set; }

		[XmlElementAttribute ("logic_score")]
		public int LogicScore { get; set; }

		[XmlElementAttribute ("memory_score")]
		public int MemoryScore { get; set; }

		[XmlElementAttribute ("verbal_score")]
		public int VerbalScore { get; set; }

		public virtual void Clear ()
		{	
			GamesPlayed = GamesWon = TotalScore = MathScore = LogicScore = MemoryScore = VerbalScore = 0;
		}

		// Deep copy
		public GameSessionHistory Copy ()
		{
			GameSessionHistory history = new GameSessionHistory ();

			history.GamesPlayed = GamesPlayed;
			history.GamesWon = GamesWon;
			history.TotalScore = TotalScore;
			history.MathScore = MathScore;
			history.LogicScore = LogicScore;
			history.MemoryScore = MemoryScore;
			history.VerbalScore = VerbalScore;
			return history;
		}
	}
}

