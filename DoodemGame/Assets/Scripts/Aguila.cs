using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Aguila : MonoBehaviour, IAttack
{
    public float height;
    public float flySpeed;
    public bool fly;
    public float xTime = 4f;
    public int numRocas;
    public float damageRoca;
    
    private NavMeshAgent agente;
    private Entity _entity;
    private float landHeight;
    private int rocas;
    private float timeLastHit;
    private Transform currentObjective;
    private int aux1;
    private bool attacked = true;
   
   
    // Start is called before the first frame update
    void Start()
    {
        _entity = GetComponent<Entity>();
        agente = GetComponent<NavMeshAgent>();
        landHeight = transform.position.y;
        currentObjective = _entity.objetive;
        aux1 = numRocas;
        //rocas = numRocas;
    }

    // Update is called once per frame
    void Update()
    {
        if (fly)
        {
            agente.enabled = false;
            flyUpdate();
        }
        else
        {
            //land();
        }
        if(_entity.GetIsOnGround())
        {
            AttackUpdate();   
        }
    }

    private void flyUpdate()
    {
        if (transform.position.y < height)
        {
            gameObject.transform.Translate(Vector3.up* (Time.deltaTime * flySpeed));
            return;
        }

        if (!currentObjective) return;
        if ((transform.position - currentObjective.position).magnitude > _entity.attackDistance)
        {
            Vector3 dir = currentObjective.position - transform.position;
            dir.Normalize();
            transform.Translate(dir* (Time.deltaTime * flySpeed),Space.World);
        }
        
    }

    private void land()
    {
        fly = false;
        if (transform.position.y > landHeight)
        {
            Vector3 dir = currentObjective.position -transform.position;
            dir -= new Vector3(dir.x, 0, dir.z)*0.75f;//reducir el tiempo de caida
            dir.Normalize();
            gameObject.transform.Translate(dir* (Time.deltaTime * flySpeed),Space.World);
        }
        else
        {
            fly = false;
            agente.enabled = true;
            if(currentObjective)
            {
                Debug.LogError("objetivo: "+currentObjective.name);
                agente.SetDestination(currentObjective.position);
            }
            
        }
    }

    public void AguilaKill()
    {
        Collider[] hitColliders = Physics.OverlapSphere(agente.transform.position, _entity.attackDistance,LayerMask.GetMask(_entity.layer));
        List<Collider> allys = new List<Collider>();
        foreach (var c in hitColliders)
        {
            if (c.gameObject.layer == gameObject.layer)
            {
                c.GetComponent<Aguila>().fly = true;
                allys.Add(c);
            }
        }
        StartCoroutine(LandAllys(allys));
    }

    private IEnumerator LandAllys(List<Collider> c)
    {
        Debug.LogError("uhdvb");
        yield return new WaitForSeconds(4f);
        foreach (var VARIABLE in c)
        {
            Debug.LogError("wow");
            VARIABLE.GetComponent<Aguila>().land();
        }
    }
    
    public void AttackUpdate()
    {
        if ((currentObjective == _entity.objetive || rocas == numRocas) && attacked)
        { 
            Collider[] hitColliders = Physics.OverlapSphere(agente.transform.position, _entity.attackDistance, LayerMask.GetMask(_entity.layerEnemy));
            if (hitColliders.Length==0) return;
            foreach (var c in hitColliders)
            {
                Debug.Log("current: "+currentObjective.name +" c: "+c.name);
                if (c.gameObject != gameObject && gameObject.layer!=c.gameObject.layer && c.gameObject!=currentObjective.gameObject)
                {
                    currentObjective = c.transform;
                    attacked = false;
                    if(agente.enabled)
                        agente.SetDestination(currentObjective.position);
                    break;
                }
            }
        }
        if (currentObjective)
        {
            if (Time.time - timeLastHit >= 1f / _entity.attackSpeed)
            {
                float aux = 0;
                if (currentObjective.TryGetComponent(out IAtackable m))
                {
                    aux = m.Attacked(rocas == numRocas ? damageRoca : _entity.damage );
                    var a = rocas == numRocas ? damageRoca : _entity.damage;
                    Debug.Log("ataco a : " + currentObjective.name + " dano: " + a);
                    attacked = true;
                    if (numRocas == rocas)
                    {
                        aux1--;
                        if (aux1 == 0)
                        {
                            rocas = 0;
                            aux1 = numRocas;
                        }
                    }
                   
                }
                if (aux < 0)
                {
                    Debug.Log(gameObject.name+"  "+_entity.objetive.position);
                    currentObjective = _entity.objetive;
                    if(agente.enabled)
                        agente.SetDestination(_entity.objetive.position);
                    if (gameObject.TryGetComponent(out Aguila a))
                    {
                        a.AguilaKill();
                    }
                }
                timeLastHit = Time.time;
            }   
        }
        else
        {
            currentObjective = _entity.objetive;
            if(agente.enabled)
                agente.SetDestination(_entity.objetive.position);
        }
    }

    public void SetCurrentObjetive(Transform co)
    {
        currentObjective = co;
    }
}
