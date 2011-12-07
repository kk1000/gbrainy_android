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
using System.Collections.Generic;

using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Verbal
{
	public class AnalogiesQuestionAnswer : Analogies
	{
		static protected Dictionary <int, Analogy> analogies;

		public AnalogiesQuestionAnswer ()
		{
			if (analogies == null)
				analogies = AnalogiesFactory.Get (Analogy.Type.QuestionAnswer);
		}

		public override string Name {
			get { return String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Question and answer #{0}"), Variant);}
		}

		public override Dictionary <int, Analogy> List {
			get { return analogies; }
		}

		protected override void Initialize ()
		{
			Current = GetNext ();

			if (Current == null)
				return;

			if (Current.answers != null) 
				Answer.Correct = Current.answers [Current.right];
			
			SetAnswerCorrectShow ();
		}
	}
}
