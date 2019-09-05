// <copyright file="LocationConverter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Scripting
{
    using System;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Structs;

    internal class LocationConverter : IConverter
    {
        public object Convert(string value)
        {
            value.ThrowIfNullOrWhiteSpace(nameof(value));

            var coordsArray = value.TrimStart('[').TrimEnd(']').Split(',');

            if (coordsArray.Length != 3)
            {
                throw new ArgumentException("Invalid location string.");
            }

            return new Location
            {
                X = System.Convert.ToInt32(coordsArray[0]),
                Y = System.Convert.ToInt32(coordsArray[1]),
                Z = System.Convert.ToSByte(coordsArray[2]),
            };
        }
    }
}