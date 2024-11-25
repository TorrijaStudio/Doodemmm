using System.Collections.Generic;
using UnityEngine;

namespace Animals.Interfaces
{
    public interface IAnimalPart
    {
        public List<float> AssignValuesToResources(List<recurso> resources);
        public List<float> AssignValuesToEnemies(IList<Transform> enemies);

        float Speed { get; set; }
        float Health { get; set; }
        float Damage { get; set; }
    }
}