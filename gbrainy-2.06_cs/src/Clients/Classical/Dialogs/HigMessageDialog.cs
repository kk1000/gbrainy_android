/*
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
 *
 *
 * Based largely on Tomboy project source code
 *
 */

using System;
using Gtk;

namespace gbrainy.Clients.Classical.Dialogs
{
	public class HigMessageDialog : Gtk.Dialog
	{
		Gtk.AccelGroup accel_group;
		Gtk.VBox extra_widget_vbox;
		Gtk.Image image;

		public HigMessageDialog (Gtk.Window parent,
			                 Gtk.DialogFlags flags,
			                 Gtk.MessageType type,
			                 Gtk.ButtonsType buttons,
			                 string          header,
			                 string          msg) : base ()
		{
			HasSeparator = false;
			BorderWidth = 5;
			Resizable = false;
			Title = string.Empty;

			VBox.Spacing = 12;
			ActionArea.Layout = Gtk.ButtonBoxStyle.End;

			accel_group = new Gtk.AccelGroup ();
			AddAccelGroup (accel_group);

			Gtk.HBox hbox = new Gtk.HBox (false, 12);
			hbox.BorderWidth = 5;
			hbox.Show ();
			VBox.PackStart (hbox, false, false, 0);

			switch (type) {
			case Gtk.MessageType.Error:
				image = new Gtk.Image (Gtk.Stock.DialogError,
					               Gtk.IconSize.Dialog);
				break;
			case Gtk.MessageType.Question:
				image = new Gtk.Image (Gtk.Stock.DialogQuestion,
					               Gtk.IconSize.Dialog);
				break;
			case Gtk.MessageType.Info:
				image = new Gtk.Image (Gtk.Stock.DialogInfo,
					               Gtk.IconSize.Dialog);
				break;
			case Gtk.MessageType.Warning:
				image = new Gtk.Image (Gtk.Stock.DialogWarning,
					               Gtk.IconSize.Dialog);
				break;
			default:
				image = new Gtk.Image ();
				break;
			}

			if (image != null) {
				image.Show ();
				image.Yalign = 0;
				hbox.PackStart (image, false, false, 0);
			}

			Gtk.VBox label_vbox = new Gtk.VBox (false, 0);
			label_vbox.Show ();
			hbox.PackStart (label_vbox, true, true, 0);

			string title = String.Format ("<span weight='bold' size='larger'>{0}" +
				                      "</span>\n",
				                      header);

			Gtk.Label label;

			label = new Gtk.Label (title);
			label.UseMarkup = true;
			label.UseUnderline = false;
			label.Justify = Gtk.Justification.Left;
			label.LineWrap = true;
			label.SetAlignment (0.0f, 0.5f);
			label.Show ();
			label_vbox.PackStart (label, false, false, 0);

			label = new Gtk.Label (msg);
			label.UseMarkup = true;
			label.UseUnderline = false;
			label.Justify = Gtk.Justification.Left;
			label.LineWrap = true;
			label.SetAlignment (0.0f, 0.5f);
			label.Show ();
			label_vbox.PackStart (label, false, false, 0);

			extra_widget_vbox = new Gtk.VBox (false, 0);
			extra_widget_vbox.Show();
			label_vbox.PackStart (extra_widget_vbox, true, true, 12);

			switch (buttons) {
			case Gtk.ButtonsType.None:
				break;
			case Gtk.ButtonsType.Ok:
				AddButton (Gtk.Stock.Ok, Gtk.ResponseType.Ok, true);
				break;
			case Gtk.ButtonsType.Close:
				AddButton (Gtk.Stock.Close, Gtk.ResponseType.Close, true);
				break;
			case Gtk.ButtonsType.Cancel:
				AddButton (Gtk.Stock.Cancel, Gtk.ResponseType.Cancel, true);
				break;
			case Gtk.ButtonsType.YesNo:
				AddButton (Gtk.Stock.No, Gtk.ResponseType.No, false);
				AddButton (Gtk.Stock.Yes, Gtk.ResponseType.Yes, true);
				break;
			case Gtk.ButtonsType.OkCancel:
				AddButton (Gtk.Stock.Cancel, Gtk.ResponseType.Cancel, false);
				AddButton (Gtk.Stock.Ok, Gtk.ResponseType.Ok, true);
				break;
			}

			if (parent != null)
				TransientFor = parent;

			if ((int) (flags & Gtk.DialogFlags.Modal) != 0)
				Modal = true;

			if ((int) (flags & Gtk.DialogFlags.DestroyWithParent) != 0)
				DestroyWithParent = true;
		}

		// constructor for a HIG confirmation alert with two buttons
		public HigMessageDialog (Gtk.Window parent,
					 Gtk.DialogFlags flags,
					 Gtk.MessageType type,
					 string          header,
					 string          msg,
					 string          ok_caption)
			: this (parent, flags, type, Gtk.ButtonsType.Cancel, header, msg)
		{
			AddButton (ok_caption, Gtk.ResponseType.Ok, false);
		}

		protected void AddButton (string stock_id, Gtk.ResponseType response, bool is_default)
		{
			Gtk.Button button = new Gtk.Button (stock_id);
			button.CanDefault = true;

			AddButton (button, response, is_default);
		}

		protected void AddButton (Gdk.Pixbuf pixbuf, string label_text, Gtk.ResponseType response, bool is_default)
		{
			Gtk.Button button = new Gtk.Button ();
			Gtk.Image image = new Gtk.Image (pixbuf);
			// NOTE: This property is new to GTK+ 2.10, but we don't
			//       really need the line because we're just setting
			//       it to the default value anyway.
			//button.ImagePosition = Gtk.PositionType.Left;
			button.Image = image;
			button.Label = label_text;
			button.UseUnderline = true;
			button.CanDefault = true;

			AddButton (button, response, is_default);
		}

		private void AddButton (Gtk.Button button, Gtk.ResponseType response, bool is_default)
		{
			button.Show ();

			AddActionWidget (button, response);

			if (is_default) {
				DefaultResponse = response;
				button.AddAccelerator ("activate",
					               accel_group,
					               (uint) Gdk.Key.Escape,
					               0,
					               Gtk.AccelFlags.Visible);
			}
		}

		//Run and destroy a standard confirmation dialog
		public static Gtk.ResponseType RunHigConfirmation(Gtk.Window parent,
					 Gtk.DialogFlags flags,
					 Gtk.MessageType type,
					 string          header,
					 string          msg,
					 string          ok_caption)
		{
			HigMessageDialog hmd = new HigMessageDialog(parent, flags, type, header, msg, ok_caption);
	 		try {
	 			return (Gtk.ResponseType)hmd.Run();
	 		} finally {
	 			hmd.Destroy();
	 		}	
	 	}

	}
}
