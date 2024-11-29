using System.Collections.Generic;
using Totems;
using UnityEngine;

namespace Animals.Interfaces
{
    public interface IAnimalPart
    {
        public List<float> AssignValuesToResources(List<recurso> resources);
        public List<float> AssignValuesToEnemies(IList<Transform> enemies);

        public TotemStats TotemStats{
            get;
            set;
        }
    }
}