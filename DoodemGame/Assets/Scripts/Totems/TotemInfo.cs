using UnityEngine;

namespace Totems
{
    //Has to be a different class because of translations
    [CreateAssetMenu(fileName = "TotemInfo", menuName = "ScriptableObjects/Totem/TotemInfo", order = 2)]
    public class TotemInfo : ScriptableObject
    {
        public string animal;
        public string bodyPart;
        public string description;
        //Do not show if empty(?)
        public string specialTraits;
        //Add options for damageType(?) or others
    }
}