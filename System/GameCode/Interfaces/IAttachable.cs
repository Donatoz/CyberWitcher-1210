using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VovTech
{
    public interface IAttachable<T>
    {
        T AttachedObject { get; }
        Dictionary<string, float> StatsToAffect { get; }
        void Attach(T target);
        void DeAttach();
    }
}