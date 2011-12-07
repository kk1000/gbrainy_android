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
using Gtk;

namespace gbrainy.Clients.Classical
{
	public class GtkSynchronize :System.ComponentModel.ISynchronizeInvoke
	{
		public GtkSynchronize ()
		{

		}
	
		// true if the caller must call Invoke; otherwise, false.
		public bool InvokeRequired {
			get {
				return true;
			}
		}
	
		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			Application.Invoke (delegate {
				method.DynamicInvoke (args);
			});

			return null;
		}

		public object EndInvoke (IAsyncResult result)
		{
			return null;
		}

		// Use Invoke when the control's main thread is different from the calling thread to marshal the call to the proper thread.
		public object Invoke (Delegate method, object[] args)
		{
			return null;
		}
	}	
}
