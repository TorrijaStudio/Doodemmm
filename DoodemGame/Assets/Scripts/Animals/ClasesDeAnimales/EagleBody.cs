using System.Collections.Generic;
using System.Linq;
using Animals.Interfaces;
using Totems;
using UnityEngine;
using UnityEngine.AI;

namespace Animals.ClasesDeAnimales
{
    public class EagleBody : AnimalBody
    {
        private Entity _entity;
        
        private void Start()
        {
            _entity = transform.GetComponentInParent<Entity>();
            _entity.isFlying = true;
            _entity.GetComponent<NavMeshAgent>().enabled = false;
        }
        
        
        
        public override List<float> AssignValuesToResources(List<recurso> resources)
        {
            var a = new float[resources.Count];
            return a.ToList();
        }

        public override List<float> AssignValuesToEnemies(IList<Transform> enemies)
        {
            var a = new float[enemies.Count];
            for (var i = 0; i < enemies.Count; i++)
            {
                var dist = transform.position - enemies[i].position;
                a[i] = (dist.magnitude / GameManager.Instance.MaxDistance);
            }
            return a.ToList();

        }
    }
}