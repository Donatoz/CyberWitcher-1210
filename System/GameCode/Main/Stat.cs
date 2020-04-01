// All code below belongs to BOB[A]H Technologies.
// BOB[A]H Technologies 2020. All rights reserved.
//-----------------------------------------------
//Stat class.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech
{
    /// <summary>
    /// Basic stat which has min and max values.
    /// </summary>
    public class Stat
    {
        public delegate void StateChange(Stat changingStat);
        public delegate void ValueChange(float affectorValue);
        /// <summary>
        /// Stat base value.
        /// </summary>
        public float BaseValue;
        /// <summary>
        /// Stat maximum value.
        /// </summary>
        public float MaxValue;
        /// <summary>
        /// Stat minimum value.
        /// </summary>
        public float MinValue;
        /// <summary>
        /// Modifiers list.
        /// </summary>
        public List<Modifier> modifiers;

        public event StateChange OnValueChange;
        public event ValueChange OnModifierAdded;

        /// <summary>
        /// BaseValue + all modifiers.
        /// </summary>
        public float EffectiveValue
        {
            get
            {
                float result = BaseValue;
                for(int i = 0; i < modifiers.Count; i++)
                {
                    if(result + modifiers[i].Value <= MaxValue)
                        result += modifiers[i].Value;
                    else
                    {
                        result = MaxValue;
                        break;
                    }
                }
                return result;
            }
        }

        public Stat(float BaseValue, float MaxValue = Int32.MaxValue, float MinValue = Int32.MinValue)
        {
            this.BaseValue = BaseValue;
            this.MaxValue = MaxValue;
            this.MinValue = MinValue;
            modifiers = new List<Modifier>();
        }

        /// <summary>
        /// Add modifier with given value (and id).
        /// </summary>
        /// <param name="value">Modifier value</param>
        /// <param name="id">Modifier id(optional)</param>
        public void AddModifier(float value, string id = "modifier")
        {
            if (EffectiveValue + value > MaxValue && EffectiveValue + value < MinValue) return;
            modifiers.Add(new Modifier(value, id));
            OnValueChange?.Invoke(this);
        }

        /// <summary>
        /// Add given modifier to modifiers list.
        /// </summary>
        /// <param name="modifier">Modifier to add</param>
        public void AddModifier(Modifier modifier)
        {
            if (EffectiveValue + modifier.Value > MaxValue && EffectiveValue + modifier.Value < MinValue) return;
            modifiers.Add(modifier);
            OnValueChange?.Invoke(this);
        }

        /// <summary>
        /// Remove modifier by id.
        /// </summary>
        /// <param name="id">Modifier id</param>
        /// <returns>Modifier removed or not</returns>
        public bool RemoveModifier(string id)
        {
            try
            {
                modifiers.Remove(modifiers.Find(x => x.Id == id));
                OnValueChange?.Invoke(this);
            } catch
            {
                return false;
            }
            return true;
        }

        public void Set(float val)
        {
            Clear();
            AddModifier(val);
        }

        public void Clear()
        {
            modifiers.Clear();
        }
    }

    public struct ValueAffector
    {
        public float Value;
        public Func<float, float> Affector;

        public ValueAffector(float value, Func<float, float> affector)
        {
            Value = value;
            Affector = affector;
        }
    }
}
