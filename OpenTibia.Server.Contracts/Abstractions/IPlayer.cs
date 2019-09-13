﻿// <copyright file="IPlayer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public interface IPlayer : ICreature
    {
        ushort Level { get; }

        byte LevelPercent { get; }

        uint Experience { get; }

        byte AccessLevel { get; } // TODO: implement.

        byte SoulPoints { get; } // TODO: nobody likes soulpoints... figure out what to do with them :)

        bool IsLogoutAllowed { get; }

        Location LocationInFront { get; }

        IAction PendingAction { get; }

        void SetOutfit(Outfit outfit);

        uint ChooseToRemoveFromKnownSet();

        bool KnowsCreatureWithId(Guid creatureId);

        byte GetSkillInfo(SkillType fist);

        byte GetSkillPercent(SkillType type);

        void AddKnownCreature(uint creatureId);

        void SetPendingAction(IAction action);

        void ClearPendingActions();

        void CheckInventoryContainerProximity(IThing thingChanging, ThingStateChangedEventArgs eventArgs);

        sbyte OpenContainer(IContainer thingAsContainer);

        sbyte GetContainerId(IContainer thingAsContainer);

        void CloseContainerWithId(byte openContainerIds);

        void OpenContainerAt(IContainer thingAsContainer, byte index);

        IContainer GetContainer(byte container);
    }
}
