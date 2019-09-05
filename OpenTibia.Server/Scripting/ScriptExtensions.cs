// -----------------------------------------------------------------
// <copyright file="ScriptExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Scripting
{
    using System;
    using System.Collections;
    using System.ComponentModel;

    public static class ScriptExtensions
    {
        public static object ConvertSingleItem(this string value, Type newType)
        {
            if (typeof(IConvertible).IsAssignableFrom(newType))
            {
                return Convert.ChangeType(value, newType);
            }

            var converter = CustomConvertersFactory.GetConverter(newType);

            if (converter == null)
            {
                throw new InvalidOperationException($"No suitable Converter found for type {newType}.");
            }

            return converter.Convert(value);
        }

        public static object ConvertStringToNewNonNullableType(this string value, Type newType)
        {
            // Do conversion form string to array - not sure how array will be stored in string
            if (newType.IsArray)
            {
                // For comma separated list
                var singleItemType = newType.GetElementType();

                var elements = new ArrayList();

                foreach (var element in value.Split(','))
                {
                    var convertedSingleItem = ConvertSingleItem(element, singleItemType);
                    elements.Add(convertedSingleItem);
                }

                return elements.ToArray(singleItemType);
            }

            return ConvertSingleItem(value, newType);
        }

        public static object ConvertStringToNewType(this string value, Type newType)
        {
            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType
            if (newType.IsGenericType && newType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return null;
                }

                return ConvertStringToNewNonNullableType(value, new NullableConverter(newType).UnderlyingType);
            }

            return ConvertStringToNewNonNullableType(value, newType);
        }
    }
}
