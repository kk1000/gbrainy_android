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
using Mono.Unix;

// This is an auto-generated file GameXmlToGetString tool. Do not edit manually
public class GameXmlStringFactory
{
	void LoadStrings ()
	{
		int variable = 0;
		Catalog.GetPluralString ("How many degrees rotates the minute hand of a clock in 2 hours [num] minute?",
			"How many degrees rotates the minute hand of a clock in 2 hours [num] minutes?",
			variable);

		Catalog.GetPluralString ("John is 46 years old. His son is [difference] year younger than half of John's age. How old is John's son?",
			"John is 46 years old. His son is [difference] years younger than half of John's age. How old is John's son?",
			variable);

		Catalog.GetPluralString ("John's age is nowadays 2 times his son's age. [ago] year ago, John was [proportion] times older than his son. How old is John's son nowadays?",
			"John's age is nowadays 2 times his son's age. [ago] years ago, John was [proportion] times older than his son. How old is John's son nowadays?",
			variable);

		Catalog.GetPluralString ("John's age (variable x) is nowadays 2 times his son's age (variable y), that is x = 2y, and [ago] year ago, John was [proportion] times older than his son: x - [ago] = (y - [ago]) * [proportion].",
			"John's age (variable x) is nowadays 2 times his son's age (variable y), that is x = 2y, and [ago] years ago, John was [proportion] times older than his son: x - [ago] = (y - [ago]) * [proportion].",
			variable);

		Catalog.GetPluralString ("A file is protected by a password formed by a [digits] digit number represented in base 10 (ranging from 0 to 9). How many different passwords can you have?",
			"A file is protected by a password formed by a [digits] digits number represented in base 10 (ranging from 0 to 9). How many different passwords can you have?",
			variable);

		Catalog.GetPluralString ("A file is protected by a password formed by a [digits] digit represented in base 8 (ranging from 0 to 7). How many different passwords can you have?",
			"A file is protected by a password formed by a [digits] digits represented in base 8 (ranging from 0 to 7). How many different passwords can you have?",
			variable);

		Catalog.GetPluralString ("There is [games] tennis game played simultaneously. How many different forecasts are possible?",
			"There are [games] tennis games played simultaneously. How many different forecasts are possible?",
			variable);

		Catalog.GetPluralString ("In a tennis tournament, in every match a player is eliminated after losing to a single opponent. How many matches does it take to determine the winner of a tennis tournament that starts with [players] player?",
			"In a tennis tournament, in every match a player is eliminated after losing to a single opponent. How many matches does it take to determine the winner of a tennis tournament that starts with [players] players?",
			variable);

		Catalog.GetPluralString ("You have [money] monetary unit in your bank account at 10% interest compounded annually. How much money will you have at the end of 2 years?",
			"You have [money] monetary units in your bank account at 10% interest compounded annually. How much money will you have at the end of 2 years?",
			variable);

		Catalog.GetPluralString ("In a horse race there are people and horses. You count [eyes] eye and [legs] leg. How many horses are present?",
			"In a horse race there are people and horses. You count [eyes] eyes and [legs] legs. How many horses are present?",
			variable);

		Catalog.GetPluralString ("John cleans at the speed of 1 / [john_time] per hour and his friend at 1 / [friend]. Together they will need [answer_a] hour.",
			"John cleans at the speed of 1 / [john_time] per hour and his friend at 1 / [friend]. Together they will need [answer_a] hours.",
			variable);

		Catalog.GetPluralString ("John needs [john_time] hour to clean a warehouse and his friend needs half as many. How many hours would it take them to clean up the warehouse if they worked together? [option_answers]",
			"John needs [john_time] hours to clean a warehouse and his friend needs half as many. How many hours would it take them to clean up the warehouse if they worked together? [option_answers]",
			variable);

		Catalog.GetPluralString ("John needs [john_time] hour to clean a warehouse and his friend needs twice as many. How many hours would it take them to clean up the warehouse if they worked together? [option_answers]",
			"John needs [john_time] hours to clean a warehouse and his friend needs twice as many. How many hours would it take them to clean up the warehouse if they worked together? [option_answers]",
			variable);

		Catalog.GetPluralString ("You have two trucks that have a total weight of [add] unit. If the lighter truck weights 15 units less that half of the weight of the heavier truck, what is the weight of the lighter truck? [option_answers]",
			"You have two trucks that have a total weight of [add] units. If the lighter truck weights 15 units less that half of the weight of the heavier truck, what is the weight of the lighter truck? [option_answers]",
			variable);


	}
}


