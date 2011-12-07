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

#if CSHARP_STATIC

using System;
using Mono.CSharp;

using gbrainy.Core.Services;

namespace gbrainy.Core.Libraries
{
	// This class proxys data from one domain to another
	internal class CSharpCompilerStaticDomainProxy : MarshalByRefObject
	{
		string code;

		public void SetCode (string c)
		{
			code = c;
		}

		public string GetVars ()
		{
			return Evaluator.GetVars ();
		}

		public void EvaluateVariables ()
		{
			string eval;

			try
			{
				// Using's for the variables section
				// We need to evaluate either declarations (like using) or expression/statements separately
				eval = "using System;\n";
				Mono.CSharp.Evaluator.Run (eval);

				// Infrastructure for the user available
				eval = "Random random = new Random ();\n";

				// As Mono 2.4.4 this call is killing in terms of memory leaking
				Mono.CSharp.Evaluator.Run (eval + code);
			}

			catch (Exception e)
			{
				Console.WriteLine ("CSharpCompilerStaticDomainProxy. Error {0} when evaluating variable definition [{1}]", e.Message, code);
			}
		}
	}
}

#endif
