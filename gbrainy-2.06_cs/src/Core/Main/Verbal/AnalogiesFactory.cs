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
using System.Xml;
using System.IO;
using System.Collections.Generic;

using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Verbal
{
	static public class AnalogiesFactory
	{
		static Dictionary <int, Analogy> [] analogies_arrays;
		static readonly string IgnoreAnalogy = "<ignore>";

		public const char Separator = '|';

		static AnalogiesFactory ()
		{
			analogies_arrays = new Dictionary <int, Analogy> [(int) Analogy.Type.Last];

			for (int i = 0; i < (int) Analogy.Type.Last; i++)
				analogies_arrays[i] = new Dictionary <int, Analogy> ();
		}

		static public Dictionary <int, Analogy> Get (Analogy.Type type)
		{
			return analogies_arrays [(int) type];
		}

		static public void Read (string file)
		{
			Analogy analogy;
			string name;
			List <string> answers;

			try
			{
				StreamReader stream = new StreamReader (file);
				XmlTextReader reader = new XmlTextReader (stream);
				answers = new List <string> ();

				for (int i = 0; i < (int) Analogy.Type.Last; i++)
					analogies_arrays[i].Clear ();

				analogy = new Analogy ();
				while (reader.Read ())
				{
					name = reader.Name.ToLower ();
					switch (name) {
					case "analogy":
						if (reader.NodeType == XmlNodeType.Element) {
							analogy = new Analogy ();
							answers.Clear ();
						}
						else {
							if (reader.NodeType == XmlNodeType.EndElement &&
								// Ignores verbal analogies disabled for a specific locale
								ServiceLocator.Instance.GetService <ITranslations> ().GetString (analogy.question) != IgnoreAnalogy) {
								analogy.answers = answers.ToArray ();
								analogies_arrays [(int) analogy.type].Add (analogies_arrays [(int) analogy.type].Count, analogy);
							}
						}
						break;
					case "_question":
						if (reader.NodeType != XmlNodeType.Element)
							return;

						string type;

						type = reader.GetAttribute ("type");
		
						if (String.IsNullOrEmpty (type) == false) {
							switch (type.ToLower ()) {
							case "multipleoptions":
								analogy.type = Analogy.Type.MultipleOptions;
								break;
							case "pairofwordsoptions":
								analogy.type = Analogy.Type.PairOfWordsOptions;
								break;
							case "pairofwordscompare":
								analogy.type = Analogy.Type.PairOfWordsCompare;
								break;
							default:
								analogy.type = Analogy.Type.QuestionAnswer;
								break;
							}
						}

						analogy.question = reader.ReadElementString ();
						break;
					case "_tip":
						if (reader.NodeType == XmlNodeType.Element)
							analogy.tip = reader.ReadElementString ();

						break;
					case "_rationale":
						if (reader.NodeType == XmlNodeType.Element)
							analogy.rationale = reader.ReadElementString ();

						break;
					case "_answer":
						if (reader.NodeType != XmlNodeType.Element)
							break;
	
						string right;

						right = reader.GetAttribute ("correct");
		
						if (String.IsNullOrEmpty (right) == false)
							if (right.ToLower () == "yes")
								analogy.right = answers.Count;
					
						answers.Add (reader.ReadElementString ());
						break;
					}
				}

				reader.Close ();
				stream.Dispose ();
			}

			catch (Exception e)
			{
				Console.WriteLine ("AnalogiesFactory. Error loading file: {0}", e.Message);
			}
		}
	}
}
