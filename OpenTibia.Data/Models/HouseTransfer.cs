// <copyright file="HouseTransfer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("house_transfer", Schema = "opentibia_classic")]
    public class HouseTransfer : IHouseTransfer
    {
        [Key]
        public long Id { get; }

        public short HouseId { get; }

        public int TransferTo { get; }

        public long Gold { get; }

        public byte Done { get; }
    }
}
