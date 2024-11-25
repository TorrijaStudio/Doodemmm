using UnityEngine;

namespace Totems
{
    [CreateAssetMenu(fileName = "TotemStats", menuName = "ScriptableObjects/Totem/TotemStats", order = 1)]
    public class TotemStats : ScriptableObject
    {
        public float health;
        public float damage;
        public float speed;
    }
}