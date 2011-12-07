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
	// See: GameTypesDescription.Get
	public enum GameTypes
	{	
		None			= 0,
		LogicPuzzle		= 2,
		Memory			= 4,
		Calculation		= 8,
		VerbalAnalogy		= 16,
	}

	// Since we cannot override ToString in an enum type we use a helper class
	public static class GameTypesDescription
	{
		// Type enum to string representation (locale sensitive)
		static public string GetLocalized (GameTypes type)
		{
			switch (type) 
			{
				case GameTypes.LogicPuzzle:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Logic");
				case GameTypes.Memory:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Memory");
				case GameTypes.Calculation:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Calculation");
				case GameTypes.VerbalAnalogy:
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Verbal");
				default:
					throw new InvalidOperationException ("Unknown game type");
			}
		}

		// string (not localized) to enum representation 
		static public GameTypes FromString (string type)
		{
			switch (type)
			{
				case "Logic":
					return GameTypes.LogicPuzzle;
				case "Memory":
					return GameTypes.Memory;
				case "Calculation":
					return GameTypes.Calculation;
				case "Verbal":
					return GameTypes.VerbalAnalogy;
				default:
					throw new InvalidOperationException ("Unknown game type");
			}
		}
	}
}
