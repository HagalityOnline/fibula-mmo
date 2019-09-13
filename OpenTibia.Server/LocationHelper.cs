// -----------------------------------------------------------------
// <copyright file="LocationHelper.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Helper class for location related functions.
    /// </summary>
    public class LocationHelper : ILocationHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationHelper"/> class.
        /// </summary>
        /// <param name="tileAccessor">The tile accessor to use.</param>
        public LocationHelper(ITileAccessor tileAccessor)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            this.TileAccessor = tileAccessor;
        }

        /// <summary>
        /// Gets the tile accessor instance to use with this helper.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Checks if a throw can happen between two locations, optionally checking for line of sight.
        /// </summary>
        /// <param name="fromLocation">The location from which the throw originates.</param>
        /// <param name="toLocation">The location to which the throw goes.</param>
        /// <param name="checkLineOfSight">Optional. Applies line of sight validation. Defaults to true.</param>
        /// <returns>True if the throw is valid, false otherwise.</returns>
        public bool CanThrowBetween(Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            if (fromLocation == toLocation)
            {
                return true;
            }

            if ((fromLocation.Z >= 8 && toLocation.Z < 8) || (toLocation.Z >= 8 && fromLocation.Z < 8))
            {
                return false;
            }

            var deltaZ = Math.Abs(fromLocation.Z - toLocation.Z);

            if (deltaZ > 2)
            {
                return false;
            }

            var deltaX = Math.Abs(fromLocation.X - toLocation.X);
            var deltaY = Math.Abs(fromLocation.Y - toLocation.Y);

            // distance checks
            if (deltaX - deltaZ > 8 || deltaY - deltaZ > 6)
            {
                return false;
            }

            return !checkLineOfSight || this.InLineOfSight(fromLocation, toLocation);
        }

        /// <summary>
        /// Checks if a destination location is in line of sight from another location.
        /// </summary>
        /// <param name="fromLocation">The source location.</param>
        /// <param name="toLocation">The destination location.</param>
        /// <returns>True if the destination location is in line of sight, false otherwise.</returns>
        public bool InLineOfSight(Location fromLocation, Location toLocation)
        {
            if (fromLocation == toLocation)
            {
                return true;
            }

            var start = fromLocation.Z > toLocation.Z ? toLocation : fromLocation;
            var destination = fromLocation.Z > toLocation.Z ? fromLocation : toLocation;

            var mx = (sbyte)(start.X < destination.X ? 1 : start.X == destination.X ? 0 : -1);
            var my = (sbyte)(start.Y < destination.Y ? 1 : start.Y == destination.Y ? 0 : -1);

            var a = destination.Y - start.Y;
            var b = start.X - destination.X;
            var c = -((a * destination.X) + (b * destination.Y));

            while ((start - destination).MaxValueIn2D != 0)
            {
                var moveHor = Math.Abs((a * (start.X + mx)) + (b * start.Y) + c);
                var moveVer = Math.Abs((a * start.X) + (b * (start.Y + my)) + c);
                var moveCross = Math.Abs((a * (start.X + mx)) + (b * (start.Y + my)) + c);

                if (start.Y != destination.Y && (start.X == destination.X || moveHor > moveVer || moveHor > moveCross))
                {
                    start.Y += my;
                }

                if (start.X != destination.X && (start.Y == destination.Y || moveVer > moveHor || moveVer > moveCross))
                {
                    start.X += mx;
                }

                var tile = this.TileAccessor.GetTileAt(start);

                if (tile != null && tile.BlocksThrow)
                {
                    return false;
                }
            }

            while (start.Z != destination.Z)
            {
                // now we need to perform a jump between floors to see if everything is clear (literally)
                var tile = this.TileAccessor.GetTileAt(start);

                if (tile?.Ground != null)
                {
                    return false;
                }

                start.Z++;
            }

            return true;
        }
    }
}
