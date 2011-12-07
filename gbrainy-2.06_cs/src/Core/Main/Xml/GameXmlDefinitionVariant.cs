/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Text;
using System.Collections.Generic;

namespace gbrainy.Core.Main.Xml
{
	public class GameXmlDefinitionVariant
	{
		public LocalizableString Question { get; set; }
		public string Tip { get; set; }
		public LocalizableString Rationale { get; set; }
		public string AnswerText { get; set; }
		public string Variables { get; set; }
		public GameAnswerCheckAttributes CheckAttributes { get; set; }
		public string AnswerCheckExpression  { get; set; }
		public string AnswerShow { get; set; }

		List <DrawingObject> drawing_objects;

		public DrawingObject [] DrawingObjects {
			get {
				if (drawing_objects == null)
					return null;
	
				return drawing_objects.ToArray ();
			}
		}

		public void AddDrawingObject (DrawingObject obj)
		{
			if (drawing_objects == null)
				drawing_objects = new List <DrawingObject> ();

			drawing_objects.Add (obj);
		}

		public override string ToString ()
		{
			StringBuilder str = new StringBuilder ();

			str.AppendLine ("Question: " + Question);
			str.AppendLine ("Tip: " + Tip);
			str.AppendLine ("Rationale: " + Rationale);
			str.AppendLine ("Answer: " + AnswerText);
			str.AppendLine ("CheckAttributes: " + CheckAttributes);
			str.AppendLine ("Answer.CheckExpression: " + AnswerCheckExpression);

			return str.ToString ();
		}
	}
}
