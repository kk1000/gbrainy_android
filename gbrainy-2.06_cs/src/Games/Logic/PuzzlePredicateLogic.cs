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

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzlePredicateLogic : Game
	{
		int question;
		ArrayListIndicesRandom random_indices;
		const int num_options = 4;

		struct Predicate
		{
			internal string question;
			internal string [] options;
			internal int answer_index;

			internal Predicate (string question, string op1, string op2, string op3, string op4, int answer_index)
			{
				this.question = question;
				this.answer_index = answer_index;

				options = new string [num_options];
				options[0] = op1;
				options[1] = op2;
				options[2] = op3;
				options[3] = op4;
			}
		};

		Predicate [] predicates;

		void LoadPredicates ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;
			predicates = new Predicate []
			{
				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If all painters are artists and some citizens of Barcelona are artists. Which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
						Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some citizens of Barcelona are painters"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All citizens of Barcelona are painters"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("No citizen of Barcelona is a painter"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("None of the other options"),
					3),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If no ill artist is happy and some artists are happy. Which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
						Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some artist are not ill"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some painters are not artists"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All artists are happy"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("None of the other options"),
					0),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("People that travel always buy a map. You are not going to travel. Which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You do not have any map"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You do not buy a map"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All people have a map"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("None of the other options"),
					3),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If you whistle if you are happy and you always smile when you whistle, which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You smile if you are happy"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You are only happy if you whistle"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("You whistle if you are not happy"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("None of the other options"),
					0),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If your course is always honest and your course is always the best policy, which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Honesty is sometimes the best policy"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Honesty is always the best policy"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Honesty is not always the best policy"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some of the best policies are dishonest"),
					0),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If no old misers are cheerful and some old misers are thin, which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some thin people are not cheerful"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Thin people are not cheerful"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Cheerful people are not thin"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some cheerful people are not thin"),
					0),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If all pigs are fat and nothing that is fed on barley-water is fat, which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All animals fed on barley-water are non pigs"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("No pigs are fed on barley-water"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Pigs are not fed on barley-water"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All the other options"),
					3),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If some pictures are first attempts and no first attempts are really good, which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some bad pictures are not first attempts"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some pictures are not really good"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All bad pictures are first attempts"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("All the others"),
					1),

				new Predicate (String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If you have been out for a walk and you are feeling better, which of the following conclusions is correct? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3)),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("To feel better, you must go out for a walk"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("If you go out for a walk, you will feel better"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Some who go out for a walk feel better"),
					ServiceLocator.Instance.GetService <ITranslations> ().GetString ("No one feels better who does not go out for a walk"),
					2),
			};
		}

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Predicate Logic");}
		}

		public override string Question {
			get {return predicates[question].question;}
		}

		protected override void Initialize ()
		{
			int answers;
			int correct_answer;

			if (predicates == null)
				LoadPredicates ();

			question = random.Next (predicates.Length);

			correct_answer = predicates [question].answer_index;
			answers = predicates [question].options.Length;
			random_indices = new ArrayListIndicesRandom (answers - 1);
			random_indices.Initialize ();
			random_indices.Add (answers - 1);

			for (int i = 0; i < answers; i++)
			{
				if (random_indices[i] ==  correct_answer) {
					Answer.Correct = Answer.GetMultiOption (i);
					break;
				}
			}

			Container container = new Container (DrawAreaX, DrawAreaY + 0.15, 0.8, 0.6);
			AddWidget (container);

			for (int i = 0; i <  predicates[question].options.Length; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.8, 0.12);
				drawable_area.X = DrawAreaX;
				drawable_area.Y = DrawAreaY + 0.15 + i * 0.18;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int data = (int) e.Data;
					int option = random_indices [data];

					e.Context.SetPangoNormalFontSize ();
					e.Context.DrawStringWithWrapping (0.05, 0.02, String.Format (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("{0}) {1}"), Answer.GetMultiOption (data),
						predicates[question].options[option].ToString ()), 0.8 - DrawAreaX);
					e.Context.Stroke ();
				};
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.Draw (gr, area_width, area_height, rtl);

			gr.SetPangoLargeFontSize ();
			gr.MoveTo (0.1, DrawAreaY);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
		}
	}
}
