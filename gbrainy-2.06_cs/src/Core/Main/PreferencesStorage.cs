/*
 * Copyright (C) 2008-2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.ComponentModel;

namespace gbrainy.Core.Main
{
   	internal class PreferencesStorage <TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		const string element_item = "item";
		const string element_key = "key";
		const string element_value = "value";
		const string element_collection = "collection";

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void ReadXml (System.Xml.XmlReader reader)
		{
			XmlSerializer key_serializer = new XmlSerializer (typeof (TKey));
			XmlSerializer value_serializer = new XmlSerializer (typeof (string));
	 		bool wasEmpty = reader.IsEmptyElement;

		    	reader.Read ();

			if (wasEmpty)
				return;

			reader.ReadStartElement (element_collection);
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement && reader.NodeType != System.Xml.XmlNodeType.None)
			{
				reader.ReadStartElement (element_item);
				reader.ReadStartElement (element_key);
				TKey key = (TKey) key_serializer.Deserialize (reader);
				reader.ReadEndElement ();

				reader.ReadStartElement (element_value);
				TValue val = (TValue) value_serializer.Deserialize (reader);
				reader.ReadEndElement();

				this[key] = val; // already created in DefaultValues
				reader.ReadEndElement();

				reader.MoveToContent();
			}
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			XmlSerializer key_serializer = new XmlSerializer (typeof(TKey));
			XmlSerializer value_serializer = new XmlSerializer (typeof(TValue));

			writer.WriteStartElement (element_collection);

			foreach (TKey key in Keys)
			{
				writer.WriteStartElement (element_item);
				writer.WriteStartElement (element_key);

				key_serializer.Serialize (writer, key);
				writer.WriteEndElement ();
				writer.WriteStartElement (element_value);

				TValue val = this[key];
				value_serializer.Serialize (writer, val);
				writer.WriteEndElement ();
				writer.WriteEndElement ();
			}
			writer.WriteEndElement ();
		}

		public T Convert<T> (string o)
		{
			TypeConverter converter = TypeDescriptor.GetConverter (typeof (T));

			try {
				return (T)converter.ConvertFromInvariantString (o.ToString ());
			} catch (Exception) {
				return default (T);
			}
		}
    	}
}
