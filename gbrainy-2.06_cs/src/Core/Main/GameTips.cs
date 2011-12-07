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

using gbrainy.Core.Services;

namespace gbrainy.Core.Main
{
	static public class GameTips
	{
		static ArrayListIndicesRandom random_indices;
		static int idx = 0;
	
		static public int Count {
			get { return 15; }
		}

		// Gets a random tip from the list
		static public string Tip {
			get {
				if (idx + 1 >= Count || random_indices == null) {
					random_indices = new ArrayListIndicesRandom (Count);
					random_indices.Initialize ();
					idx = 0;
				}
				return GetTip (random_indices [idx++]);
			}
		}

		static public string GetTip (int tip)
		{
			switch (tip) {
			case 0:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Read the instructions carefully and identify the data and given clues.");
			case 1:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("To score the player gbrainy uses the time and tips needed to complete each game.");
			case 2:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("In logic games, elements that may seem irrelevant can be very important.");
			case 3:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Try to approach a problem from different angles.");
			case 4:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Do not be afraid of making mistakes, they are part of the learning process.");
			case 5:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Do all the problems, even the difficult ones. Improvement comes from challeging yourself.");
			case 6:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Play on a daily basis, you will notice progress soon.");
			case 7: // Translators: Custom Game Selection is a menu option
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Use the 'Custom Game Selection' to choose exactly which games you want to play.");
			case 8:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Use the Settings to adjust the difficulty level of the game.");
			case 9:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Association of elements is a common technique for remembering things.");
			case 10:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Grouping elements into categories is a common technique for remembering things.");
			case 11:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Build acronyms using the first letter of each fact to be remembered.");
			case 12:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("The enjoyment obtained from a puzzle is proportional to the time spent on it.");
			case 13:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Think of breaking down every problem into simpler components.");
			case 14:
				return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("When answering verbal analogies pay attention to the verb tense.");
			default:
				throw new InvalidOperationException ();
			}
		}
	}
}
