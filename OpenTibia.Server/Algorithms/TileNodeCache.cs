// <copyright file="TileNodeCache.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Algorithms
{
    using System.Collections.Generic;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    internal static class TileNodeCache
    {
        private static readonly Dictionary<string, Dictionary<Location, TileNode>> NodeDic = new Dictionary<string, Dictionary<Location, TileNode>>();
        private static readonly object SetsLock = new object();

        public static TileNode Create(string aStarSearchId, ITile tileBase)
        {
            aStarSearchId.ThrowIfNullOrWhiteSpace(nameof(aStarSearchId));
            tileBase.ThrowIfNull(nameof(tileBase));

            var locToSearch = tileBase.Location;

            lock (SetsLock)
            {
                if (!NodeDic.ContainsKey(aStarSearchId))
                {
                    NodeDic.Add(aStarSearchId, new Dictionary<Location, TileNode>());
                }

                if (!NodeDic[aStarSearchId].ContainsKey(locToSearch))
                {
                    NodeDic[aStarSearchId].Add(locToSearch, new TileNode(aStarSearchId, tileBase));
                }

                return NodeDic[aStarSearchId][locToSearch];
            }
        }

        public static void CleanUp(string aStarSearchId)
        {
            lock (SetsLock)
            {
                if (NodeDic.ContainsKey(aStarSearchId))
                {
                    NodeDic[aStarSearchId].Clear();
                    NodeDic.Remove(aStarSearchId);
                }
            }
        }
    }
}
