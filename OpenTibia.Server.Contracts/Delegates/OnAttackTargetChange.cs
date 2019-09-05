﻿// -----------------------------------------------------------------
// <copyright file="OnAttackTargetChange.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    public delegate void OnAttackTargetChange(uint oldTargetId, uint newTargetId);
}