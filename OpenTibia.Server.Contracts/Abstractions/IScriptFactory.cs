// -----------------------------------------------------------------
// <copyright file="IScriptFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    public interface IScriptFactory
    {
        void Change(IGame gameInstance, ref IThing thing, ushort toItemId, byte unknown);

        void ChangeOnMap(IGame gameInstance, Location location, ushort fromItemId, ushort toItemId, byte unknown);

        void ChangeRel(IGame gameInstance, IThing fromThing, Location locationOffset, ushort fromItemId, ushort toItemId, byte unknown);

        bool CountObjects(IThing thingAt, string comparer, ushort value);

        void Create(IGame gameInstance, IThing atThing, ushort itemId, byte unknown);

        void CreateOnMap(IGame gameInstance, Location location, ushort itemId, byte unknown);

        void Damage(IGame gameInstance, IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue);

        void Delete(IGame gameInstance, IThing thing);

        void DeleteOnMap(IGame gameInstance, Location location, ushort itemId);

        void Effect(IGame gameInstance, IThing thing, byte effectByte);

        void EffectOnMap(IGame gameInstance, Location location, byte effectByte);

        bool HasFlag(IThing itemThing, string flagStr);

        bool HasInstanceAttribute(IThing thing, string attributeStr, string comparer, ushort value);

        bool HasProfession(IThing thing, byte profesionId);

        bool HasRight(IPlayer user, string rightStr);

        void InvokeAction(IGame gameInstance, ref IThing obj1, ref IThing obj2, ref IPlayer user, string methodName, params object[] parameters);

        bool InvokeCondition(IGame gameInstance, IThing obj1, IThing obj2, IPlayer user, string methodName, params object[] parameters);

        bool IsCreature(IThing thing);

        bool IsHouse(IThing thing);

        bool IsHouseOwner(IThing thing, IPlayer user);

        bool IsObjectThere(IGame gameInstance, Location location, ushort typeId);

        bool IsPlayer(IThing thing);

        bool IsPosition(IThing thing, Location location);

        bool IsType(IThing thing, ushort typeId);

        void Logout(IGame gameInstance, IPlayer user);

        bool MayLogout(IPlayer user);

        void MonsterOnMap(IGame gameInstance, Location location, ushort monsterId);

        void Move(IGame gameInstance, IThing thingToMove, Location targetLocation);

        void MoveRel(IGame gameInstance, ICreature user, IThing objectUsed, Location locationOffset);

        void MoveTop(IGame gameInstance, IThing fromThing, Location targetLocation);

        void MoveTopOnMap(IGame gameInstance, Location fromLocation, ushort itemId, Location toLocation);

        void MoveTopRel(IGame gameInstance, IThing fromThing, Location locationOffset);

        bool Random(byte value);

        void SetStart(IThing thing, Location location);

        void Text(IThing fromThing, string text, byte textType);

        void WriteName(IPlayer user, string format, IThing targetThing);
    }
}