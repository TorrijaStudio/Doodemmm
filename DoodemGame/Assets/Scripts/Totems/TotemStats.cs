using UnityEngine;

namespace Totems
{
    [CreateAssetMenu(fileName = "TotemStats", menuName = "ScriptableObjects/Totem/TotemStats", order = 1)]
    public class TotemStats : ScriptableObject
    {
        public int health;
        public int damage;
        public int speed;
    }
}