using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEditor;

namespace VovTech
{
    public static class UnityExtensions
    {

        public static Vector3 Clamp(this Vector3 vec, float min, float max)
        {
            return new Vector3(Mathf.Clamp(vec.x, min, max), Mathf.Clamp(vec.y, min, max), Mathf.Clamp(vec.z, min, max));
        }

        public static Vector3 ClampZ(this Vector3 vec, float min, float max)
        {
            return new Vector3(vec.x, vec.y, Mathf.Clamp(vec.z, min, max));
        }

        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
        {
            float multiplier = 1;
            for (int i = 0; i < decimalPlaces; i++)
            {
                multiplier *= 10f;
            }
            return new Vector3(
                Mathf.Round(vector3.x * multiplier) / multiplier,
                Mathf.Round(vector3.y * multiplier) / multiplier,
                Mathf.Round(vector3.z * multiplier) / multiplier);
        }

        public static bool InDestination(this NavMeshAgent agent)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Color Alpha(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static void ReplaceClip(this Animator animator, string clipNamePart, AnimationClip clip)
        {
            AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
            {
                if (a.name.Contains(clipNamePart))
                {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, clip));
                    break;
                }
            }
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
        }

        public static int DrawBitMaskField(Rect aPosition, int aMask, System.Type aType, GUIContent aLabel)
        {
            var itemNames = Enum.GetNames(aType);
            var itemValues = Enum.GetValues(aType) as int[];

            int val = aMask;
            int maskVal = 0;
            for (int i = 0; i < itemValues.Length; i++)
            {
                if (itemValues[i] != 0)
                {
                    if ((val & itemValues[i]) == itemValues[i])
                        maskVal |= 1 << i;
                }
                else if (val == 0)
                    maskVal |= 1 << i;
            }
            int newMaskVal = EditorGUI.MaskField(aPosition, aLabel, maskVal, itemNames);
            int changes = maskVal ^ newMaskVal;

            for (int i = 0; i < itemValues.Length; i++)
            {
                if ((changes & (1 << i)) != 0)
                {
                    if ((newMaskVal & (1 << i)) != 0)
                    {
                        if (itemValues[i] == 0)
                        {
                            val = 0;
                            break;
                        }
                        else
                            val |= itemValues[i];
                    }
                    else
                    {
                        val &= ~itemValues[i];
                    }
                }
            }
            return val;
        }
    }
}