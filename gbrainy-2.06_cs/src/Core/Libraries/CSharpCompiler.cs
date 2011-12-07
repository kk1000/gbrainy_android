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

#if !CSHARP_STATIC

using System;
using System.Text;
using System.Text.RegularExpressions;

using Mono.CSharp;
using System.Reflection;
using gbrainy.Core.Services;

namespace gbrainy.Core.Libraries
{
	public class NullReportPrinter : ReportPrinter
	{
		public NullReportPrinter ()
		{

		}
	}

	// This encapsulates Mono.CSharp > 2.10
	public class CSharpCompiler : ICSharpCompiler
	{
		Evaluator evaluator;

		public CSharpCompiler ()
		{
			CompilerSettings settings = new CompilerSettings ();
			Report report = new Report (new NullReportPrinter ());

			evaluator = new Evaluator (settings, report);
		}

		public void EvaluateCode (string code)
		{
			string eval;

			try
			{
				// Using's for the variables section
				// We need to evaluate either declarations (like using) or expression/statements separately
				eval = "using System;\n";
				evaluator.Run (eval);

				// Infrastructure for the user available
				eval = "Random random = new Random ();\n";

				// As Mono 2.4.4 this call is killing in terms of memory leaking
				evaluator.Run (eval + code);
			}

			catch (Exception e)
			{
				Console.WriteLine ("CSharpCompiler. Error {0} when evaluating variable definition [{1}]", e.Message, code);
			};
		}

		public string GetAllVariables ()
		{
			return evaluator.GetVars ();
		}

		public string GetVariableValue (string _var)
		{
			const string exp = "([a-z0-9._%+-]+) ([a-z0-9._%+-]+) (=) ([0-9]+)";
			Match match;
			int idx, cur, newline_len;
			string line;
			string vars;

			vars = GetAllVariables ();
			Regex regex = new Regex (exp, RegexOptions.IgnoreCase);

			newline_len = System.Environment.NewLine.Length;
			cur = 0;

			do
			{
				// Process a line
				idx = vars.IndexOf (System.Environment.NewLine, cur);
				if (idx == -1) idx = vars.Length;

				line = vars.Substring (cur, idx - cur);
				cur = idx + newline_len;
				match = regex.Match (line);

				//  "int num = 2";
				//   group 1 -> int,  group 2 -> num,  group 3 -> =, group 4 -> 2
				if (match.Groups.Count == 5)
				{
					if (match.Groups[2].Value == _var)
						return match.Groups[4].Value;
				}

			} while (cur < vars.Length);

			return string.Empty;
		}
	}
}

#endif
