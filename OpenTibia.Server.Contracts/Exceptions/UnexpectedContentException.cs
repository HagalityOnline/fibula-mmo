// <copyright file="UnexpectedContentException.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UnexpectedContentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedContentException"/> class.
        /// </summary>
        public UnexpectedContentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedContentException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public UnexpectedContentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedContentException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UnexpectedContentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedContentException"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected UnexpectedContentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}