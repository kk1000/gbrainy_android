/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;
using System.Collections;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleMostInCommon : Game
	{
		public enum Element
		{
			SmallCircle,
			MediumCircleWithChild,
			MediumCircle,
			LargeCircle,
		}

		struct FigureElement
		{
			public double x;
			public double y;
			public Element element;

			public FigureElement (double _x, double _y, Element _element)
			{
				x = _x;
				y = _y;
				element = _element;
			}
		}

		private double small_size = 0.01;
		private double medium_size = 0.02;
	  	private ArrayList questions;
	  	private ArrayList answers;
		private ArrayListIndicesRandom random_indices_answers;
		private const double pos1_x = 0.03;
		private const double pos2_x = 0.06;
		private const double pos3_x = 0.09;
		private const double pos4_x = 0.07;
		private const double pos5_x = 0.10;
		private const double pos6_x = 0.02;
		private const double pos7_x = 0.05;
		private const double pos1_y = 0.03;
		private const double pos2_y = 0.06;
		private const double pos3_y = 0.09;
		private const double pos4_y = 0.02;
		private const double pos5_y = 0.05;
		private const double pos6_y = 0.07;
		private const double pos7_y = 0.11;

		public override string Name {
			get {return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Most in common");}
		}

		public override string Question {
			get {return String.Format (
				ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Which of the possible answers have the most in common with the four given figures? Answer {0}, {1}, {2} or {3}."),
					Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));}
		}

		public override string Tip {
			get { return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Think of the common elements that the given figures have inside them.");}
		}

		public override string Rationale {
			get {
				if (CurrentDifficulty ==  GameDifficulty.Easy)
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("It has the same number of elements inside the figure as the given figures.");
				else
					return ServiceLocator.Instance.GetService <ITranslations> ().GetString ("It is the figure with the most elements in common compared to the given figures.");
			}
		}

		protected override void Initialize ()
		{
			// Question
			ArrayList array_good = new ArrayList ();
			array_good.AddRange (new Element [] {Element.SmallCircle, Element.SmallCircle, Element.SmallCircle,
				Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;

			// Four random samples with equal elements
			questions = new ArrayList ();
			for (int i = 0; i < 4; i++) {
				questions.Add (BuildFigure (array_good, questions));
			}

			ArrayList array = new ArrayList ();
			answers = new ArrayList ();
			random_indices_answers = new ArrayListIndicesRandom (4);
			random_indices_answers.Initialize ();

			for (int i = 0; i < random_indices_answers.Count; i++) {
				if ((int) random_indices_answers [i] == 0) {
					Answer.SetMultiOptionAnswer (i, Answer.GetFigureName (i));
					break;
				}
			}

			if (CurrentDifficulty ==  GameDifficulty.Easy) {
				// Answer 1 (good)
				array.AddRange (new Element [] {Element.SmallCircle, Element.SmallCircle, Element.SmallCircle,
				Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));

				// Answer 2
				array.Clear ();
				array.AddRange (new Element [] {Element.SmallCircle, Element.SmallCircle, Element.MediumCircle,
				Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));

				// Answer 3
				array.Clear ();
				array.AddRange (new Element [] {Element.SmallCircle, Element.SmallCircle, Element.MediumCircle,
				Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));

				// Answer 4
				array.Clear ();
				array.AddRange (new Element [] {Element.SmallCircle, Element.SmallCircle, Element.MediumCircle,
				Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));
			}
			else  // Medium or Master
			{

				// Answer 1 (good)
				array.AddRange (new Element [] {Element.SmallCircle, Element.SmallCircle, Element.MediumCircleWithChild,
					Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));

				// Answer 2
				array.Clear ();
				array.AddRange (new Element [] {Element.SmallCircle, Element.MediumCircle, Element.MediumCircle,
					Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));

				// Answer 3
				array.Clear ();
				array.AddRange (new Element [] {Element.SmallCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild,
					Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));

				// Answer 4
				array.Clear ();
				array.AddRange (new Element [] {Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild,
					Element.MediumCircle,Element.MediumCircle, Element.MediumCircleWithChild, Element.MediumCircleWithChild});
				answers.Add (BuildFigure (array, answers));
			}

			double figure_size = 0.22;
			double x = DrawAreaX - 0.05, y = DrawAreaY + 0.45;

			HorizontalContainer container = new HorizontalContainer (x, y, random_indices_answers.Count * figure_size, 0.3);
			DrawableArea drawable_area;

			AddWidget (container);

			for (int i = 0; i < random_indices_answers.Count; i++)
			{
				drawable_area = new DrawableArea (figure_size, figure_size + 0.05);
				drawable_area.SelectedArea = new Rectangle (0.05, 0.05, 0.15, 0.15);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;

					DrawFigure (e.Context, 0.05, 0.05, (FigureElement []) answers[random_indices_answers[n]]);
					e.Context.MoveTo (0.05, 0.22);
					e.Context.ShowPangoText (Answer.GetFigureName (n));
				};

				container.AddChild (drawable_area);
			}
		}

		private ArrayListIndicesRandom RandomizeFromArray (ArrayList ar)
		{
			int index;
			object []array = (object []) ar.ToArray (typeof (object));
			ArrayListIndicesRandom elements = new ArrayListIndicesRandom (ar.Count);
			int left = ar.Count;
			elements.Clear ();

			// Generate a random number that can be as big as the maximum -1
			// Add the random element picked up element in the list
			// The element just randomized gets out of pending list and replaced by the maximum -1 element
			for (int i = 0; i < ar.Count; i++, left--) {
				index = random.Next (left);
				elements.Add ((int) array[index]);
				array[index] = array[left - 1];
			}
			return elements;
		}

		// Generates a new figure that was not generated before
		private FigureElement [] BuildFigure (ArrayList array, ArrayList figures)
		{
			bool done = false;
			FigureElement [] element = null;
			ArrayListIndicesRandom elements = new ArrayListIndicesRandom (array.Count);
			bool element_exists = false;

			while (done == false) {

				elements = RandomizeFromArray (array);
				element = new FigureElement []
				{
					new FigureElement (pos1_x, pos1_y, (Element) elements[0]),
					new FigureElement (pos2_x, pos2_y, (Element) elements[1]),
					new FigureElement (pos3_x, pos3_y, (Element) elements[2]),
					new FigureElement (pos4_x, pos4_y, (Element) elements[3]),
					new FigureElement (pos5_x, pos5_y, (Element) elements[4]),
					new FigureElement (pos6_x, pos6_y, (Element) elements[5]),
					new FigureElement (pos7_x, pos7_y, (Element) elements[6]),
				};

				for (int i = 0; i < figures.Count; i++) {
					FigureElement [] element2 = (FigureElement []) figures[i];

					if (element.Length != element2.Length)
						continue;

					element_exists = true;
					for (int n = 0; n < element.Length; n++) {
						if (element[n].element != element2[n].element) {
							element_exists = false;
							break;
						}
					}
					if (element_exists == true)
						break;
				}

				if (element_exists == false)
					done = true;
			}

			return element;
		}

		private void DrawFigureElement (CairoContextEx gr, double x, double y, FigureElement figure)
		{
			switch (figure.element) {
			case Element.SmallCircle:
				gr.Arc (x + figure.x + small_size / 2, y + figure.y + small_size / 2, small_size, 0, 2 * Math.PI);
				break;
			case Element.MediumCircle:
				gr.Arc (x + figure.x + medium_size / 2, y + figure.y + medium_size / 2, medium_size, 0, 2 * Math.PI);
				break;
			case Element.MediumCircleWithChild:
				gr.Arc (x + figure.x + medium_size / 2, y + figure.y + medium_size / 2, medium_size, 0, 2 * Math.PI);
				gr.Stroke ();
				gr.Arc (x + figure.x + medium_size / 2, y + figure.y + medium_size / 2, small_size, 0, 2 * Math.PI);
				break;
			}
			gr.Stroke ();
		}

		private void DrawFigure (CairoContextEx gr, double x, double y, FigureElement[] figure)
		{
			const double cercle_size = 0.15;
			gr.Stroke ();
			gr.Arc (x + cercle_size / 2, y + cercle_size / 2, cercle_size / 2, 0, 2 * Math.PI);
			gr.Stroke ();

			for (int i = 0; i < figure.Length; i++)
				DrawFigureElement (gr, x, y,  figure[i]);

			gr.Stroke ();
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			double x = DrawAreaX, y = DrawAreaY + 0.1;

			base.Draw (gr, area_width, area_height, rtl);

			for (int i = 0; i < questions.Count; i++) {
				DrawFigure (gr, x, y, (FigureElement []) questions[i]);
				 x+= 0.22;
			}

			y += 0.28;
			x = DrawAreaX;
			gr.MoveTo (x - 0.06, y);
			gr.ShowPangoText (ServiceLocator.Instance.GetService <ITranslations> ().GetString ("Choose one of the following:"));
			gr.Stroke ();
		}
	}
}
