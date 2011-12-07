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
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using gbrainy.Core.Libraries;

namespace gbrainy.Core.Main.Xml
{
	public class GamesXmlFactory
	{
		List <GameXmlDefinition> games;
		bool read = false;

		public GamesXmlFactory ()
		{
			games = new List <GameXmlDefinition> ();
		}

		public List <GameXmlDefinition> Definitions {
			get { return games; }
		}

		public void Read (string file)
		{
			GameXmlDefinition game;
			string name, str, plural;
			bool processing_variant = false;
			int variant = 0;
			OptionDrawingObject option = null;
			DrawingObject last_drawing_object = null;
			string last_context = null;

			if (read == true)
				return;

			try
			{
				StreamReader stream = new StreamReader (file);
				XmlTextReaderLiteral reader = new XmlTextReaderLiteral (stream);
				game = null;
		
				while (reader.Read ())
				{
					// Strings are only used because msgctxt requirement
					if (reader.NodeType == XmlNodeType.Text)
					{
						const string CONTEXT_GLUE = "\u0004";
						string text;
						
						text = reader.ReadString ();

						TextDrawingObject drawing_object = (TextDrawingObject) last_drawing_object;
						// GetText processes msgctxt as msgctxt + context_glue + text to retrieve them
						drawing_object.Text = last_context + CONTEXT_GLUE + text;

						// If the string does not exits, return regular English string without context
						if (GetText.StringExists (drawing_object.Text) == false)
							drawing_object.Text = text;

						continue;
					}

					name = reader.Name.ToLower ();
					switch (name) {
					case "games":
						break;
					case "type":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						game.Type = GameTypesDescription.FromString (reader.ReadElementString ());
						break;
					case "game":
						if (reader.NodeType == XmlNodeType.Element) {
							game = new GameXmlDefinition ();
						} else if (reader.NodeType == XmlNodeType.EndElement) {
							games.Add (game);
						}
						break;
					case "_name":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						game.Name = reader.ReadElementString ();
						break;
					case "difficulty":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						str = reader.ReadElementString ();
						game.Difficulty = GameDifficultyDescription.FromString (str);
						break;
					case "svg":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						ImageDrawingObject draw_image = new ImageDrawingObject ();

						if (processing_variant)
							game.Variants[variant].AddDrawingObject (draw_image);
						else
							game.AddDrawingObject (draw_image);

						draw_image.Filename = reader.GetAttribute ("file");

						str = reader.GetAttribute ("x");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.X = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.X = 0.1;

						str = reader.GetAttribute ("y");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.Y = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.Y = 0.1;

						str = reader.GetAttribute ("width");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.Width = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.Width = 0.8;

						str = reader.GetAttribute ("height");
						if (String.IsNullOrEmpty (str) == false)
							draw_image.Height = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_image.Height = 0.8;

						break;
					case "string":
					case "_string":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						TextDrawingObject draw_string = new TextDrawingObject ();
						last_drawing_object = draw_string;

						if (option != null)
						{
							option.AddDrawingObject (draw_string);
						}
						else
						{
							if (processing_variant)
								game.Variants[variant].AddDrawingObject (draw_string);
							else
								game.AddDrawingObject (draw_string);
						}

						last_context = reader.GetAttribute ("msgctxt");

						draw_string.Text = reader.GetAttribute ("text");
	
						if (String.IsNullOrEmpty (draw_string.Text))
							draw_string.Text = reader.GetAttribute ("_text");
						
						str = reader.GetAttribute ("x");
						if (String.IsNullOrEmpty (str) == false)
							draw_string.X = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_string.X = 0.1;

						str = reader.GetAttribute ("y");
						if (String.IsNullOrEmpty (str) == false)
							draw_string.Y = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							draw_string.Y = 0.1;

						str = reader.GetAttribute ("centered");
						if (String.Compare (str, "yes", true) == 0)
							draw_string.Centered = true;
						else
							draw_string.Centered = false;

						str = reader.GetAttribute ("size");

						if (String.IsNullOrEmpty (str) == false)
						{
							switch (str.ToLower ()) {
							case "small":
								draw_string.Size = TextDrawingObject.Sizes.Small;
								break;
							case "medium":
								draw_string.Size = TextDrawingObject.Sizes.Medium;
								break;
							case "large":
								draw_string.Size = TextDrawingObject.Sizes.Large;
								break;
							case "x-large":
								draw_string.Size = TextDrawingObject.Sizes.XLarge;
								break;
							case "xx-large":
								draw_string.Size = TextDrawingObject.Sizes.XXLarge;
								break;
							default:
								Console.WriteLine ("GameXmlFactory. Unsupported value for size attribute: {0}", str);
								break;
							}
						}
						break;
					case "_question":
					case "question":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						// Create object if needed
						if (processing_variant) {
							if (game.Variants[variant].Question == null)
								game.Variants[variant].Question = new LocalizableString ();
						}
						else {
							if (game.Question == null)
								game.Question = new LocalizableString ();
						}

						plural = reader.GetAttribute ("plural");

						if (String.IsNullOrEmpty (plural) == false) { // Plural
							if (processing_variant) {
								game.Variants[variant].Question.PluralString = reader.ReadElementStringAsItIs ();
								game.Variants[variant].Question.Value = plural;
							}
							else {
								game.Question.PluralString = reader.ReadElementStringAsItIs ();
								game.Question.Value = plural;
							}
						}
						else {
							if (processing_variant)
							{
								game.Variants[variant].Question.String = reader.ReadElementStringAsItIs ();
							}
							else
								game.Question.String = reader.ReadElementStringAsItIs ();
						}
						break;
					case "rationale":
					case "_rationale":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						// Create object if needed
						if (processing_variant) {
							if (game.Variants[variant].Rationale == null)
								game.Variants[variant].Rationale = new LocalizableString ();
						}
						else {
							if (game.Rationale == null)
								game.Rationale = new LocalizableString ();
						}

						plural = reader.GetAttribute ("plural");

						if (String.IsNullOrEmpty (plural) == false) { // Plural
							if (processing_variant) {
								game.Variants[variant].Rationale.PluralString = reader.ReadElementStringAsItIs ();
								game.Variants[variant].Rationale.Value = plural;
							}
							else {
								game.Rationale.PluralString = reader.ReadElementStringAsItIs ();
								game.Rationale.Value = plural;
							}
						}
						else {
							if (processing_variant)
								game.Variants[variant].Rationale.String = reader.ReadElementStringAsItIs ();
							else
								game.Rationale.String = reader.ReadElementStringAsItIs ();
						}
						break;
					case "answer":
					case "_answer":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].AnswerText = reader.ReadElementString ();
						else
							game.AnswerText = reader.ReadElementString ();

						break;
					case "_answer_show":
					case "answer_show":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].AnswerShow = reader.ReadElementString ();
						else
							game.AnswerShow = reader.ReadElementString ();

						break;
					case "answer_expression":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].AnswerCheckExpression = reader.ReadElementString ();
						else
							game.AnswerCheckExpression = reader.ReadElementString ();

						break;
					case "answer_checkattributes":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].CheckAttributes = GameAnswerCheckAttributesDescription.FromString (reader.ReadElementString ());
						else
							game.CheckAttributes = GameAnswerCheckAttributesDescription.FromString (reader.ReadElementString ());

						break;
					case "_tip":
						if (reader.NodeType != XmlNodeType.Element)
							break;

						if (processing_variant)
							game.Variants[variant].Tip = reader.ReadElementStringAsItIs ();
						else
							game.Tip = reader.ReadElementStringAsItIs ();

						break;
					case "variant":
						if (reader.NodeType == XmlNodeType.Element) {
							game.NewVariant ();
							variant = game.Variants.Count - 1;
							processing_variant = true;
						} else if (reader.NodeType == XmlNodeType.EndElement) {
							processing_variant = false;
						}
						break;
					case "variables":
						if (processing_variant)
							game.Variants[variant].Variables = reader.ReadElementString ();
						else
							game.Variables = reader.ReadElementString ();
						break;

					case "option":

						switch (reader.NodeType) {
						case XmlNodeType.Element:
							option = new OptionDrawingObject ();
							break;
						case XmlNodeType.EndElement:
							if (String.IsNullOrEmpty (option.AnswerText) && option.RandomizedOrder == false)
								throw new InvalidOperationException ("If the option is not randomized, you need to define an answer");

							option = null;
							break;
						default: // Do any processing
							break;
						}

						if (option == null)
							break;

						if (processing_variant)
							game.Variants[variant].AddDrawingObject (option);
						else
							game.AddDrawingObject (option);
	
						option.AnswerText = reader.GetAttribute ("answer");
	
						if (String.IsNullOrEmpty (option.AnswerText))
							option.AnswerText = reader.GetAttribute ("_answer");

						str = reader.GetAttribute ("x");
						if (String.IsNullOrEmpty (str) == false)
							option.X = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							option.X = 0.1;

						str = reader.GetAttribute ("y");
						if (String.IsNullOrEmpty (str) == false)
							option.Y = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							option.Y = 0.1;

						str = reader.GetAttribute ("width");
						if (String.IsNullOrEmpty (str) == false)
							option.Width = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							option.Width = 0.1;

						str = reader.GetAttribute ("height");
						if (String.IsNullOrEmpty (str) == false)
							option.Height = Double.Parse (str, CultureInfo.InvariantCulture);
						else
							option.Height = 0.1;

						str = reader.GetAttribute ("order");
						if (String.IsNullOrEmpty (str) == false)
							option.RandomizedOrder = true;

						str = reader.GetAttribute ("correct");
						if (String.Compare (str, "yes", true) == 0)
							option.Correct = true;

						break;
					default:
						if (String.IsNullOrEmpty (name) == false)
							Console.WriteLine ("GameXmlFactory. Unsupported tag: {0}", name);

						break;
					}
				}

				reader.Close ();
				stream.Dispose ();
				read = true;

				GameXml.Definitions = games;
			}

			catch (Exception e)
			{
				read = true;
				Console.WriteLine ("GameXmlFactory. Error loading: {0}", e.Message);
			}
		}
	}

	// XmlTextReader translates the entities like '&gt;' into their correspondent character.
	// Since the string returned is different that the one collected by intltool scripts into
	// the PO files, a call to GetText does not return the localized version.
	// We implement ReadElementStringAsItIs to read the string and it is in the XML without
	// entities translation. Later Pango can render markup directly.
	internal class XmlTextReaderLiteral : XmlTextReader
	{
		const int BUFFER_LEN = 16384;
		char [] buffer;

		public XmlTextReaderLiteral (StreamReader input) : base (input)
		{
			buffer = new char [BUFFER_LEN];
		}

		public string ReadElementStringAsItIs ()
		{
			int read;
			StringBuilder str;

			read = ReadChars (buffer, 0, BUFFER_LEN);
			str = new StringBuilder (read);

			for (int i =0; i < read; i++)
				str.Append (buffer [i]);
			
			return str.ToString ();
		}
	}

}
