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
        void Change(ref IThing thing, ushort toItemId, byte unknown);

        void ChangeOnMap(Location location, ushort fromItemId, ushort toItemId, byte unknown);

        void ChangeRel(IThing fromThing, Location locationOffset, ushort fromItemId, ushort toItemId, byte unknown);

        bool CountObjects(IThing thingAt, string comparer, ushort value);

        void Create(IThing atThing, ushort itemId, byte unknown);

        void CreateOnMap(Location location, ushort itemId, byte unknown);

        void Damage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue);

        void Delete(IThing thing);

        void DeleteOnMap(Location location, ushort itemId);

        void Effect(IThing thing, byte effectByte);

        void EffectOnMap(Location location, byte effectByte);

        bool HasFlag(IThing itemThing, string flagStr);

        bool HasInstanceAttribute(IThing thing, string attributeStr, string comparer, ushort value);

        bool HasProfession(IThing thing, byte profesionId);

        bool HasRight(IPlayer user, string rightStr);

        void InvokeAction(ref IThing obj1, ref IThing obj2, ref IPlayer user, string methodName, params object[] parameters);

        bool InvokeCondition(IThing obj1, IThing obj2, IPlayer user, string methodName, params object[] parameters);

        bool IsCreature(IThing thing);

        bool IsHouse(IThing thing);

        bool IsHouseOwner(IThing thing, IPlayer user);

        bool IsObjectThere(Location location, ushort typeId);

        bool IsPlayer(IThing thing);

        bool IsPosition(IThing thing, Location location);

        bool IsType(IThing thing, ushort typeId);

        void Logout(IPlayer user);

        bool MayLogout(IPlayer user);

        void MonsterOnMap(Location location, ushort monsterId);

        void Move(IThing thingToMove, Location targetLocation);

        void MoveRel(ICreature user, IThing objectUsed, Location locationOffset);

        void MoveTop(IThing fromThing, Location targetLocation);

        void MoveTopOnMap(Location fromLocation, ushort itemId, Location toLocation);

        void MoveTopRel(IThing fromThing, Location locationOffset);

        bool Random(byte value);

        void SetStart(IThing thing, Location location);

        void Text(IThing fromThing, string text, byte textType);

        void WriteName(IPlayer user, string format, IThing targetThing);
    }
}