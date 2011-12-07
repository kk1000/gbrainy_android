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

namespace gbrainy.Core.Main
{
	// Events Args used to communicate an state change in the UI
	public class UpdateUIStateEventArgs : EventArgs
	{
		public enum EventUIType
		{
			QuestionText,	// A game needs to update question text
			Time		// Game time ticks and needs to be updated (e.g. status bar)
		}

		object data;
		EventUIType etype;

		public UpdateUIStateEventArgs (EventUIType etype, object data)
		{
			switch (etype) {
				case EventUIType.QuestionText: {
					if (data.GetType () != typeof (string))
						throw new InvalidOperationException ("Invalid object type");

					break;
				}
				default:
					break;
			}

			this.etype = etype;
			this.data = data;
		}

		public object Data {
			get { return data; }
		}
		
		public EventUIType EventType {
			get { return etype; }
		}
	}
}
