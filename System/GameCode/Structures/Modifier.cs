// All code below belongs to BOB[A]H Technologies.
// BOB[A]H Technologies 2020. All rights reserved.
//-----------------------------------------------
//Modifier structure.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    /// <summary>
    /// Structure which holds value and which has some id (supposed to be used for modifying some value).
    /// </summary>
    public struct Modifier
    {
        /// <summary>
        /// Modifier value.
        /// </summary>
        public float Value;
        /// <summary>
        /// Modifier id.
        /// </summary>
        public string Id;

        public Modifier(float Value, string Id = "modifier")
        {
            this.Value = Value;
            this.Id = Id;
        }
    }
}