using System;
using System.Collections.Generic;
using System.Linq;
using Animals.Interfaces;
using Totems;
using UnityEngine;

namespace Animals.ClasesDeAnimales
{
    public class LizardBody : AnimalBody
    {
        private Entity _entity;
        [SerializeField] private Recursos resource;
        [SerializeField] private int resourceQuantity;
        private bool _isSubscribed;
        private const float AttackDistance = 2.5f;
        private const float AttackAngle = 60f;
        private const float AttackDamage = 10f;

        private void Start()
        {
            _entity = transform.GetComponentInParent<Entity>();
            _entity.OnResourcesChanged += HasResourcesForAttack;
            // _entity.SubscribeAttack(TotemPiece.Type.Body, new Entity.AttackStruct(AttackDistance, AreaAttack));
            // _entity.att
        }

        private void HasResourcesForAttack(Recursos resources, int number)
        {
            if(resources != resource)   return;

            if (number >= resourceQuantity)
            {
                if(!_isSubscribed){
                    Debug.Log("Ataque lagarto suscrito");
                    _entity.SubscribeAttack(TotemPiece.Type.Body, new Entity.AttackStruct(AttackDistance, AreaPoison,new Dictionary<Recursos, int>(){
                        { resource, resourceQuantity }}));
                    _isSubscribed = true;
                }
            }
            else
            {
                if (_isSubscribed)
                {
                    Debug.Log("Ataque lagarto desuscrito");
                    _entity.UnsubscribeAttack(TotemPiece.Type.Body);
                    _isSubscribed = false;
                }
            }
        }
        private void AreaPoison()
        {
            // Debug.Log("Area attack :) from " + transform.parent.name);
            Collider[] hitColliders = Physics.OverlapSphere(_entity.transform.position, AttackDistance, LayerMask.GetMask(_entity.layerEnemy));
            foreach (var c in hitColliders)
            {
                if (c.TryGetComponent(out Entity m))
                {
                    m.ApplyPoison();
                }
            }
        }
        
        public override List<float> AssignValuesToResources(List<recurso> resources)
        {
            var a = new float[resources.Count];
            for (var i = 0; i < resources.Count; i++)
            {
                if(resources[i]._typeRecurso == resource)
                {
                    var dist = transform.position - resources[i].transform.position;
                    a[i] = Mathf.Pow(dist.magnitude / GameManager.Instance.MaxDistance, 8f);
                }
                else
                {
                    a[i] = 0;
                }
            }
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