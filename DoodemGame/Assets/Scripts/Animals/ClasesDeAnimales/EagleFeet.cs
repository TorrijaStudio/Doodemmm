using System.Collections.Generic;
using System.Linq;
using Animals.Interfaces;
using Totems;
using UnityEngine;

namespace Animals.ClasesDeAnimales
{
    public class EagleFeet :MonoBehaviour,  IAnimalFeet
    {
        private Entity _entity;
        [SerializeField] private Recursos resource;
        [SerializeField] private int resourceQuantity;
        private bool _isSubscribed;
        
        private void Start()
        {
            _entity = transform.GetComponentInParent<Entity>();
            _entity.OnResourcesChanged += HasResourcesForAttack;
        }

        private void HasResourcesForAttack(Recursos resources, int number)
        {
            if(resources != resource)   return;

            if (number >= resourceQuantity)
            {
                if(!_isSubscribed){
                    Debug.Log("Ataque aguila suscrito");
                    _entity.SubscribeAttack(TotemPiece.Type.Feet, new Entity.AttackStruct(TotemStats.attackDistance, AttackEagleFeet));
                    _isSubscribed = true;
                }
            }
            else
            {
                if (_isSubscribed)
                {
                    Debug.Log("Ataque aguila desuscrito");
                    _entity.UnsubscribeAttack(TotemPiece.Type.Feet);
                    _isSubscribed = false;
                }
            }
        }

        private void EagleAttack()
        {
            var colliders = Physics.OverlapSphere(transform.position, 0.25f, LayerMask.GetMask(_entity.layerEnemy));
            switch (colliders.Length)
            {
                case 0:
                    return;
                case 1:
                    var enemy = colliders[0].GetComponent<Entity>();
                    enemy.Attacked(TotemStats.damage * _entity.DamageModifier);
                    enemy.Attacked(TotemStats.damage);
                    break;
                case 2:
                    foreach (var colliderEnemy in colliders)
                    {
                        colliderEnemy.GetComponent<Entity>().Attacked(TotemStats.damage * _entity.DamageModifier);
                    }
                    break;
            }
        }
        
        private void AttackEagleFeet()
        {
            // var entityDamage = _entity.damage;
            // _entity.damage = Damage;
            // _entity.Attack();
            // _entity.damage = entityDamage;
            EagleAttack();
            _entity.AddOrTakeResources(resource, resourceQuantity);
        }
        
        public List<float> AssignValuesToResources(List<recurso> resources)
        {
            var a = new float[resources.Count];
            for (var i = 0; i < resources.Count; i++)
            {
                if(resources[i]._typeRecurso == resource)
                {
                    var dist = transform.position - resources[i].transform.position;
                    a[i] = Mathf.Pow(dist.magnitude / GameManager.Instance.MaxDistance, 8);
                }
            }
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

        [field: SerializeField] public TotemStats TotemStats { get; set; }

    }
}