using System.Collections.Generic;
using Totems;
using UnityEngine;

namespace Animals.Interfaces
{
    public abstract class AnimalBody : MonoBehaviour, IAnimalPart
    {
        [SerializeField] private Transform headPoint;
        [SerializeField] private Transform feetPoint;
        
        public Transform GetHeadAttachmentPoint()
        {
            return headPoint;
        }

        public Transform GetFeetAttachmentPoint()
        {
            return feetPoint;
        }

        public abstract List<float> AssignValuesToResources(List<recurso> resources);
        public abstract List<float> AssignValuesToEnemies(IList<Transform> enemies);
        [field: SerializeField] public TotemStats TotemStats { get; set; }

        
    }
}