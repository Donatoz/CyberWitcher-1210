using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public static class VovTechExtensions
    {
        public static Character AsCharacter(this Actor actor)
        {
            try
            {
                return actor as Character;
            }
            catch
            {
                return null;
            }
        }

        public static NPC AsNPC(this Actor actor)
        {
            try
            {
                return actor as NPC;
            }
            catch
            {
                return null;
            }
        }

        public static Actor AsActor(this Entity e)
        {
            try
            {
                return e as Actor;
            }
            catch
            {
                return null;
            }
        }

        public static Item AsItem(this Entity e)
        {
            try
            {
                return e as Item;
            }
            catch
            {
                return null;
            }
        }

        public static Weapon AsWeapon(this Item item)
        {
            try
            {
                return item as Weapon;
            }
            catch
            {
                return null;
            }
        }
    }
}