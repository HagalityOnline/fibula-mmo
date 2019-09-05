// <copyright file="MoveUseItemEventLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Items;
    using Sprache;
    using static OpenTibia.Server.Parsing.Grammar.EventGrammar;

    public class MoveUseItemEventLoader : IItemEventLoader
    {
        /*
            A move use definition is in the form:

                Type, ContitionFunctions -> ActionFunctions

            such as:

            Use, IsType (Obj1,2487), IsHouse (Obj1), HasRight (User,PREMIUM_ACCOUNT), MayLogout (User) -> MoveRel(User,Obj1,[0,0,0]), Change(Obj1,2495,0), WriteName(User,"%N",Obj1), ChangeRel(Obj1,[0,1,0],2488,2496,0), Logout(User)

         */

        public const char CommentSymbol = '#';

        public const char PropertyValueSeparator = '=';

        public IDictionary<ItemEventType, HashSet<IItemEvent>> Load(string moveUseFileName)
        {
            moveUseFileName.ThrowIfNullOrWhiteSpace(nameof(moveUseFileName));

            var moveUseFilePath = "OpenTibia.Server.Data." + ServerConfiguration.DataFilesDirectory + "." + moveUseFileName;

            var assembly = Assembly.GetExecutingAssembly();

            var eventDictionary = new Dictionary<ItemEventType, HashSet<IItemEvent>>
            {
                { ItemEventType.Use, new HashSet<IItemEvent>() },
                { ItemEventType.MultiUse, new HashSet<IItemEvent>() },
                { ItemEventType.Movement, new HashSet<IItemEvent>() },
                { ItemEventType.Collision, new HashSet<IItemEvent>() },
                { ItemEventType.Separation, new HashSet<IItemEvent>() },
            };

            using (var stream = assembly.GetManifestResourceStream(moveUseFilePath))
            {
                if (stream == null)
                {
                    throw new Exception($"Failed to load {moveUseFilePath}.");
                }

                using (var reader = new StreamReader(stream))
                {
                    foreach (var readLine in reader.ReadToEnd().Split("\r\n".ToCharArray()))
                    {
                        var inLine = readLine?.Split(new[] { ObjectsFileItemLoader.CommentSymbol }, 2).FirstOrDefault();

                        // ignore comments and empty lines.
                        if (string.IsNullOrWhiteSpace(inLine) || inLine.StartsWith("BEGIN") || inLine.StartsWith("END"))
                        {
                            continue;
                        }

                        try
                        {
                            var moveUseEventParsed = Event.Parse(inLine);

                            eventDictionary[moveUseEventParsed.Type].Add(this.ItemEventFactory.Create(moveUseEventParsed));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }

            return eventDictionary;
        }
    }
}
