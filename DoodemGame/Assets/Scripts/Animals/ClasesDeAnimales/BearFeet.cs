using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animals.Interfaces;
using Totems;
using UnityEngine;

namespace Animals.ClasesDeAnimales
{
    public class BearFeet :MonoBehaviour,  IAnimalFeet
    {
        [SerializeField] private float range;
        [SerializeField] private float bonus;
        private bool _isBonusActive;
        private Entity _entity;
        
        private void Start()
        {
            _entity = transform.GetComponentInParent<Entity>();
            StartCoroutine(Enhancer());
        }

        private IEnumerator Enhancer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                if (AreAlliesInRange())
                {
                    //Si hay aliados en el rango...
                    if (_isBonusActive)
                    {
                        //... y el bonus esta activado... desactivalo!
                        _entity.DamageModifier += 0.10F;
                        _isBonusActive = false;
                    }
                }else if (!_isBonusActive)
                {
                    //Si NO hay aliados en el rango y el bonus NO esta activado... activalo!
                    _entity.DamageModifier -= 0.10F;
                    _isBonusActive = true;
                }
            }
        }

        private bool AreAlliesInRange()
        {
            return FindObjectsOfType<Entity>().Where(entity => entity.layerEnemy == _entity.layerEnemy && _entity!= entity)
                .Any(entity => Distance(entity) <= range);
            
        }

        private float Distance(Entity entity)
        {
            return Vector3.Distance(entity.transform.position, transform.position);
        }
        
        public List<float> AssignValuesToResources(List<recurso> resources)
        {
            var a = new float[resources.Count];
            return a.ToList();
        }

        public List<float> AssignValuesToEnemies(IList<Transform> enemies)
        {
            var a = new float[enemies.Count];
            for (var i = 0; i < enemies.Count; i++)
            {
                var dist = transform.position - enemies[i].position;
                a[i] = (dist.magnitude / GameManager.Instance.MaxDistance);
            }
            return a.ToList();
        }

        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public float Health { get; set; }
        [field: SerializeField] public float Damage { get; set; }
    }
}