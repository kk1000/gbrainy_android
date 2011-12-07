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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Core.Main.Xml
{
	public class GameXml : Game
	{
		// Every GameXml instance is capable of locating any XML defined game
		// This struct translates from a Variant that is global to all games
		// to a specific game + variant
		struct DefinitionLocator
		{
			public int Game { get; set; }
			public int Variant { get; set; }

			public DefinitionLocator (int game, int variant) : this ()
			{
				Game = game;
				Variant = variant;
			}
		};

		// Shared with all instances
		static List <GameXmlDefinition> games;
		static List <DefinitionLocator> locators;

		static string option_prefix = "[option_prefix]";
		static string option_answers = "[option_answers]";

		DefinitionLocator current;
		GameXmlDefinition game;
		string question, answer, rationale, answer_show;
		List <OptionDrawingObject> options;
		ICSharpCompiler compiler;

		
		static public List <GameXmlDefinition> Definitions {
			set {
				games = value;
				locators = new List <DefinitionLocator> ();
			}
		}

		public override GameTypes Type {
			get { return game.Type; }
		}

		public override string Name {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString (game.Name); }
		}

		public override string Question {
			get { return question; }
		}

		public override string Rationale {
			get { return rationale; }
		}

		public override string Tip {
			get {
				if (game.Variants.Count > 0 && game.Variants[current.Variant].Tip != null)
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString (game.Variants[current.Variant].Tip);
				else
					if (String.IsNullOrEmpty (game.Tip) == false)
						return ServiceLocator.Instance.GetService <ITranslations> ().GetString (game.Tip);
					else
						return null;
			}
		}

		void SetAnswerCorrectShow ()
		{		
			if (String.IsNullOrEmpty (answer_show))
				return;

			Answer.CorrectShow = answer_show;
		}

		void SetCheckExpression ()
		{
			string expression;

			if (game.Variants.Count > 0 && String.IsNullOrEmpty (game.Variants[current.Variant].AnswerCheckExpression) == false)
				expression = game.Variants[current.Variant].AnswerCheckExpression;
			else
				expression = game.AnswerCheckExpression;

			if (String.IsNullOrEmpty (expression))
				return;

			Answer.CheckExpression = expression;
		}
		
		void SetCheckAttributes ()
		{
			GameAnswerCheckAttributes attrib;

			if (game.Variants.Count > 0 && game.Variants[current.Variant].CheckAttributes != GameAnswerCheckAttributes.None)
				attrib = game.Variants[current.Variant].CheckAttributes;
			else
				attrib = game.CheckAttributes;

			if (attrib == GameAnswerCheckAttributes.None)
				return;

			Answer.CheckAttributes |= attrib;
		}

		protected override void Initialize ()
		{
			string variables;
			bool variants;
			LocalizableString localizable_question, localizable_rationale;

			compiler = ServiceLocator.Instance.GetService <ICSharpCompiler> ();

			variants = game.Variants.Count > 0;
			SetCheckAttributes ();

			if (variants && game.Variants[current.Variant].Variables != null)
				variables = game.Variants[current.Variant].Variables;
			else
				variables = game.Variables;

			if (variants && game.Variants[current.Variant].Question != null)
				localizable_question = game.Variants[current.Variant].Question;
			else
				localizable_question = game.Question;

			if (variants && game.Variants[current.Variant].Rationale != null)
				localizable_rationale = game.Variants[current.Variant].Rationale;
			else
				localizable_rationale = game.Rationale;

			if (String.IsNullOrEmpty (variables) == false)
			{
				compiler.EvaluateCode (variables);

				try {

					if (String.IsNullOrEmpty (localizable_question.Value) == false)
						localizable_question.ValueComputed = Int32.Parse (ReplaceVariables (localizable_question.Value));

					if (localizable_rationale != null && String.IsNullOrEmpty (localizable_rationale.Value) == false)
						localizable_rationale.ValueComputed = Int32.Parse (ReplaceVariables (localizable_rationale.Value));
				}
				catch (Exception e)
				{
					Console.WriteLine ("GameXml.Initialize. Error {0}", e);
				}
			}

			if (variants && game.Variants[current.Variant].Question != null)
				question = CatalogGetString (game.Variants[current.Variant].Question);
			else
				question = CatalogGetString (game.Question);

			if (variants && game.Variants[current.Variant].AnswerText != null)
				answer = CatalogGetString (game.Variants[current.Variant].AnswerText);
			else
				answer = CatalogGetString (game.AnswerText);

			if (variants && game.Variants[current.Variant].Rationale != null)
				rationale = CatalogGetString (game.Variants[current.Variant].Rationale);
			else
				rationale = CatalogGetString (game.Rationale);

			if (variants && game.Variants[current.Variant].AnswerShow != null)
				answer_show = CatalogGetString (game.Variants[current.Variant].AnswerShow);
			else
				answer_show = CatalogGetString (game.AnswerShow);

			if (String.IsNullOrEmpty (variables) == false)
			{
				question = ReplaceVariables (question);
				rationale = ReplaceVariables (rationale);
				answer = ReplaceVariables (answer);
				answer_show = ReplaceVariables (answer_show);
			}

			if (options != null && options.Count > 0)
			{
				for (int i = 0; i < options.Count; i++)
				{
					OptionDrawingObject option = options [i];
					if (option.Correct == true)
					{
						Answer.SetMultiOptionAnswer (i, answer);
						break;
					}
				}

				// Translators {0}: list of options (A, B, C)
				string answers = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Answer {0}."), 
					Answer.GetMultiOptionsPossibleAnswers (options.Count));
				question = question.Replace (option_answers, answers);					
			}
			else
			{
				Answer.Correct = answer;
			}

			SetCheckExpression ();
			SetAnswerCorrectShow ();
		}

		void CreateDrawingObjects (GameXmlDefinitionVariant game)
		{
			OptionDrawingObject option;
			double x = 0, y = 0, width = 0, height = 0;
			bool first = true;
			int randomized_options = 0;

			if (game.DrawingObjects == null)
				return;

			// Calculate the size of container from the options and count the number of random options
			foreach (DrawingObject draw_object in game.DrawingObjects)
			{
				option = draw_object as OptionDrawingObject;

				if (option == null)
					continue;

				if (option.RandomizedOrder)
					randomized_options++;

				if (first == true)
				{
					x = option.X;
					y = option.Y;
					width = option.Width;
					height = option.Height;
					first = false;
					continue;
				}

				if (option.X < x) 
					x = option.X;

				if (option.Y < y) 
					y = option.Y;

				if (option.X + option.Width > width)
					width = option.X + option.Width;

				if (option.Y + option.Height > height) 
					height = option.Y + option.Height;
			}

			if (first == true)
				return;

			// Randomize the order of the options
			if (randomized_options > 0)
			{
				OptionDrawingObject [] originals;
				ArrayListIndicesRandom random_indices;
				int index = 0;

				random_indices = new ArrayListIndicesRandom (randomized_options);
				originals = new OptionDrawingObject [randomized_options];
				random_indices.Initialize ();

				// Backup originals
				for (int i = 0; i < game.DrawingObjects.Length; i++)
				{
					option = game.DrawingObjects[i] as OptionDrawingObject;

					if (option == null)
						continue;

					originals[index] = option.Copy ();
					index++;
				}

				// Swap
				index = 0;
				for (int i = 0; i < game.DrawingObjects.Length; i++)
				{
					option = game.DrawingObjects[i] as OptionDrawingObject;

					if (option == null)
						continue;

					option.CopyRandomizedProperties (originals [random_indices [index]]);
					index++;
				}
			}

			Container container = new Container (x, y, width - x, height - y);
			AddWidget (container);

			if (options == null)
				options = new List <OptionDrawingObject> ();

			int idx = 0;

			// Create drawing widgets objects
			foreach (DrawingObject draw_object in game.DrawingObjects)
			{
				option = draw_object as OptionDrawingObject;

				if (option == null)
					continue;

				DrawableArea drawable_area = new DrawableArea (option.Width, option.Height);
				drawable_area.X = option.X;
				drawable_area.Y = option.Y; // + i * 0.15;
				container.AddChild (drawable_area);
				
				drawable_area.Data = idx;
				drawable_area.DataEx = Answer.GetMultiOption (idx);
				options.Add (option);

				idx++;
				drawable_area.DrawEventHandler += DrawOption;
			}
		}

		void DrawOption (object sender, DrawEventArgs e)
		{
			int idx = (int) e.Data;

			if (options.Count == 0)
				return;

			OptionDrawingObject _option = options [idx];
			e.Context.SetPangoLargeFontSize ();

			DrawObjects (e.Context, _option.DrawingObjects, idx);
		}

		public override int Variants {
			get {
				if (locators.Count == 0)
					BuildLocationList ();

				return locators.Count;
			}
		}

		public override int Variant {
			set {
				base.Variant = value;

				DefinitionLocator locator;

				locator = locators [Variant];
				current.Game = locator.Game;
				current.Variant = locator.Variant;
				game = games [locator.Game];
				SetCheckAttributes ();

				CreateDrawingObjects (game); // Draw objects shared by all variants

				if (game.Variants.Count > 0)
					CreateDrawingObjects (game.Variants[current.Variant]); // Draw variant specific objects

				SetCheckExpression ();
				SetAnswerCorrectShow ();
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			DrawObjects (gr, game.DrawingObjects, null); // Draw objects shared by all variants

			if (game.Variants.Count > 0)
				DrawObjects (gr, game.Variants[current.Variant].DrawingObjects, null); // Draw variant specific objects
		}

		void DrawObjects (CairoContextEx gr, DrawingObject [] drawing_objects, int? option)
		{
			if (drawing_objects == null)
				return;

			foreach (DrawingObject draw_object in drawing_objects)
			{
				if (draw_object is OptionDrawingObject)
					continue;

				if (draw_object is TextDrawingObject)
				{
					string text;
					TextDrawingObject draw_string = draw_object as TextDrawingObject;

					text = CatalogGetString (draw_string.Text);
					text = ReplaceVariables (text);

					switch (draw_string.Size) {
					case TextDrawingObject.Sizes.Small:
						gr.SetPangoFontSize (0.018);
						break;
					case TextDrawingObject.Sizes.Medium:
						gr.SetPangoNormalFontSize (); // 0.022
						break;
					case TextDrawingObject.Sizes.Large:
						gr.SetPangoLargeFontSize (); // 0.0325
						break;
					case TextDrawingObject.Sizes.XLarge:
						gr.SetPangoFontSize (0.06);
						break;
					case TextDrawingObject.Sizes.XXLarge:
						gr.SetPangoFontSize (0.08);
						break;
					default:
						throw new InvalidOperationException ("Invalid value");
					}

					if (draw_string.Centered) {
						gr.DrawTextCentered (draw_string.X, draw_string.Y, text);
					} else {
						gr.MoveTo (draw_string.X, draw_string.Y);
						if (option == null)
							gr.ShowPangoText (text);
						else
							gr.ShowPangoText (GetOptionPrefix (text, (int) option));

						gr.Stroke ();
					}
				}
				else if (draw_object is ImageDrawingObject)
				{
					ImageDrawingObject image = draw_object as ImageDrawingObject;

					if (String.IsNullOrEmpty (image.Filename) == false)
					{
						string dir;
						IConfiguration config;

						config = ServiceLocator.Instance.GetService <IConfiguration> ();
						dir = config.Get <string> (ConfigurationKeys.GamesGraphics);

						gr.DrawImageFromFile (Path.Combine (dir, image.Filename),
							image.X, image.Y, image.Width, image.Height);
					}
				}
			}
		}

		string GetOptionPrefix (string str, int option)
		{
			string answer;
			
			answer = String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}) "), Answer.GetMultiOption (option));
			return str.Replace (option_prefix, answer);
		}

		static void BuildLocationList ()
		{
			locators.Clear ();

			for (int game = 0; game < games.Count; game++)
			{
				locators.Add (new DefinitionLocator (game, 0));
				for (int variant = 1; variant < games[game].Variants.Count; variant++)
					locators.Add (new DefinitionLocator (game, variant));
			}
		}

		// Protect from calling with null (exception)
		static string CatalogGetString (string str)
		{
			if (String.IsNullOrEmpty (str))
				return str;

			return ServiceLocator.Instance.GetService <ITranslations> ().GetString (str);
		}

		// Protect from calling with null + resolve plurals
		static string CatalogGetString (LocalizableString localizable)
		{
			if (localizable == null)
				return string.Empty;

			if (localizable.IsPlural () == false)
				return CatalogGetString (localizable.String);

			return ServiceLocator.Instance.GetService <ITranslations> ().GetPluralString (localizable.String, localizable.PluralString, localizable.ValueComputed);
		}

		// Replace compiler service variables
		string ReplaceVariables (string str)
		{
			const string exp = "\\[[a-z_]+\\]+";
			string var, var_value, all_vars;
			Regex regex;
			Match match;

			all_vars = compiler.GetAllVariables ();
			if (String.IsNullOrEmpty (str) ||
				String.IsNullOrEmpty (all_vars))
				return str;

			regex = new Regex (exp, RegexOptions.IgnoreCase);
			match = regex.Match (str);

			while (String.IsNullOrEmpty (match.Value) == false)
			{
				var = match.Value.Substring (1, match.Value.Length - 2);
				var_value = compiler.GetVariableValue (var);

				if (String.IsNullOrEmpty (var_value) == false)
					str = str.Replace (match.Value, var_value);

				match = match.NextMatch ();
			}
			return str;
		}
	}
}
