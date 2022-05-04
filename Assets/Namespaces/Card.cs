using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Card
{
    public enum CardType
    {
        None,
        Tower, 
        Spell,
        Skill
    };

    [Flags]
    public enum TowerSubtype
    {
        None = 0,
        Mana = 1 << 0,
        Damage = 1 << 1
    }

    [Flags]
    public enum SpellSubtype
    {
        None = 0
    }

    [Flags]
    public enum SkillSubtype
    {
        None = 0,
        Injury = 1 << 0
    }

    [Flags]
    public enum TargetType
    {
        None = 0,
        Tiles = 1 << 0,
        Towers = 1 << 1,
        Enemies = 1 << 2,
        Cards = 1 << 3
    }

    [Flags]
    public enum CardZones
    {
        None,
        Deck,
        Hand,
        Discard
    }

    public enum Status
    {
        Burn,
        Frozen,
        Attack_Up,
        Attack_Down,
        Defense_Up,
        Defense_Down
    }
}
