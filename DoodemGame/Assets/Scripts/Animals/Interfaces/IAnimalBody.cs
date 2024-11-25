using System.Collections.Generic;
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

        
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public float Health { get; set; }
        [field: SerializeField] public float Damage { get; set; }
    }
}