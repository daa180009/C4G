using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mana
{
    [Flags]
    public enum ManaType
    {
        [InspectorName("-")]
        None = 0,
        [InspectorName("♧")]
        Clubs = 1 << 0,
        [InspectorName("♤")]
        Spades = 1 << 1,
        [InspectorName("♡")]
        Hearts = 1 << 2,
        [InspectorName("♢")]
        Diamonds = 1 << 3
    };
}
