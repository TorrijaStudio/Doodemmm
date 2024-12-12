using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Animals.Interfaces;
using Totems;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Entity : NetworkBehaviour ,IAtackable
{
    private NavMeshAgent agente;
    private bool isOnGround;
    public string layer;
    private playerInfo _playerInfo;
    
    public NetworkVariable<int> _idPlayer = new NetworkVariable<int>(writePerm:NetworkVariableWritePermission.Server);
    public string layerEnemy;
    private int currentAreaIndex;
    [SerializeField] private float damageModifier;

    public float DamageModifier
    {
        get => damageModifier;
        set => damageModifier = value;
    }
    private bool isEnemy;
    private Coroutine _followCoroutine;
    public bool isFlying;
    [Header("Status bools")]
    public bool isPoisoned;
    public bool canRevive;
    public bool canBeAttacked;
    // public bool isBleeding;
    // public int PlayerId
    // {
    //     get => _idPlayer.Value;
    //     set => _idPlayer.Value = value;
    // }
    private Dictionary<Recursos, int> _resources;
    
    private IAnimalHead _head;
    private Transform _headTransform;
    private AnimalBody _body;
    private Transform _bodyTransform;
    private IAnimalFeet _feet;
    private Transform _feetTransform;
        
    public Transform objetive;
    public float speed;
    public float maxAttackDistance;

    private float _speedModifier;
    public float SpeedModifier
    {
        get => _speedModifier;
        set
        {
            _speedModifier = value;
            if(agente)
            {
                agente.speed = Math.Max(speed + _speedModifier, 1) / 5f;
                Debug.Log(agente.speed);
            }
        }
    }

    public float health;
    public float maxHealth;
    public float damage;
    public float attackDistance;
    public float attackSpeed;

    private Coroutine _brainCoroutine;
    private void SetLayer(int oldId, int id)
    {
        Debug.LogWarning("Set layer");
        gameObject.layer = LayerMask.NameToLayer(id == 0 ? "Rojo" : "Azul");
        layerEnemy = id == 0 ? "Azul" : "Rojo";
        layer = id == 0 ? "Rojo" : "Azul";
        // var meshes = transform.GetComponentsInChildren<MeshRenderer>().ToList();
        // meshes.ForEach(mesh =>
        // {
        //     var materials = mesh.materials.ToList();
        //     materials.ForEach(mat => mat.color = id == 0 ? new Color(1f, 0.76f, 0.75f) : new Color(0.81f, 0.83f, 1f));
        //     mesh.SetMaterials(materials);
        // });
        // transform.rotation = Quaternion.Euler(id == 0 ? Vector3.forward : Vector3.back);
        Debug.Log(id);
        // objetive = GameManager.Instance.Bases[id];

        transform.LookAt(GameManager.Instance.Bases[id]);
        name = layer + " " + name;
        
        SetOutline(_headTransform, id == 0 ? new Color(0.44f, 0.09f, 0.08f) : new Color(0.07f, 0.15f, 0.43f));
        SetOutline(_feetTransform, id == 0 ? new Color(0.44f, 0.09f, 0.08f) : new Color(0.07f, 0.15f, 0.43f));
        SetOutline(_bodyTransform, id == 0 ? new Color(0.44f, 0.09f, 0.08f) : new Color(0.07f, 0.15f, 0.43f));
    }

    private static void SetOutline(Object tr, Color color)
    {
        
        var outline = tr.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = color;
        outline.OutlineWidth = 7f;
    }
    
    public void SetAnimalParts(GameObject head, GameObject body, GameObject feet)
    {
        _bodyTransform = Instantiate(body, transform).transform;
        _body = _bodyTransform.GetComponent<AnimalBody>();
        SetHealthAndSpeed(_body.TotemStats.health,_body.TotemStats.speed);

        _headTransform = Instantiate(head, _body.GetHeadAttachmentPoint().position,
            _body.GetHeadAttachmentPoint().rotation,
            transform).transform;
        _head = _headTransform.GetComponent<IAnimalHead>();
        SetHealthAndSpeed(_head.TotemStats.health,_head.TotemStats.speed);
        damage = _head.TotemStats.damage;

        _feetTransform = Instantiate(feet, _body.GetFeetAttachmentPoint().position, _body.GetFeetAttachmentPoint().rotation,
            transform).transform;
        _feet = _feetTransform.GetComponent<IAnimalFeet>();
        SetHealthAndSpeed(_feet.TotemStats.health,_feet.TotemStats.speed);
        var a = transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        // var possibleNames = new string[]{ "Pedro", "Francisco" };
        name = $"{body.name}_{head.name}_{feet.name}";
        //Turn off the totem mesh renderers
        foreach (var meshRenderer in a)
        {
            meshRenderer.enabled = false;
        }
        transform.rotation = Quaternion.Euler(Seleccionable.ClientID == 1 ? Vector3.forward : Vector3.back);
    }

    private void SetHealthAndSpeed(float h, float s)
    {
        health += h;
        maxHealth = health;
        speed += s;
        attackSpeed = speed/6f;
        if (agente)
            agente.speed = Mathf.Max(speed + _speedModifier, 1) / 5f;
    }

    
    public void SetSpeedModifier(float speedChange)
    {
        _speedModifier = speedChange;
    }
    public void Attack()
    {
        // Debug.Log(name + " attacking!");
        if (Time.time - timeLastHit >= 1f / attackSpeed)
        {
            float aux = 0;
            if (objetive.parent && objetive.parent.TryGetComponent(out IAtackable m))
            {
                aux = m.Attacked(DamageModifier * damage);
            }
            if (aux < 0)
            {
                // Debug.Log(gameObject.name+"  " + objetive.position);
                // currentObjective = objetive;
                // if(agente.enabled)
                //     agente.SetDestination(objetive.position);
                // if (gameObject.TryGetComponent(out Aguila a))
                // {
                //     a.AguilaKill();
                // }
            }
            timeLastHit = Time.time;
        }   
    }
    public void BleedAttack()
    {
        if (objetive.parent && isEnemy && objetive.parent.TryGetComponent<Entity>(out var enemy))
        {
            enemy.ApplyBleeding();
        }
    }
    
    [ClientRpc]
    public void SpawnClientRpc(int h, int b, int f)
    {
         SetAnimalParts(GameManager.Instance._heads[h], 
             GameManager.Instance._body[b], 
             GameManager.Instance._feet[f]);
         GameManager.Instance.playerObjects.Add(gameObject);
         gameObject.SetActive(GameManager.Instance.startedGame);
    }

    // OnDestroyNe
    
    private void Awake()
    {
        //GameManager.Instance.playerObjects.Add(gameObject);
        //gameObject.SetActive(false);
        
        _attacksMap = new Dictionary<TotemPiece.Type, AttackStruct>();
        _speedModifier = 0;
        // maxAttackDistance = 10f;
    }



    void Start()
    {
        canBeAttacked = true;
        DamageModifier = 1.0f;
        _resources = new Dictionary<Recursos, int>();
        SetLayer(0, _idPlayer.Value);
        // currentDamage = damage;
        _idPlayer.OnValueChanged += SetLayer; 
        SetAgent();
        agente.speed = speed;
        // StartCoroutine(SearchResources());
        
        if(IsHost)
            _brainCoroutine = StartCoroutine(Brain());
    }

    private IEnumerator Brain()
    {
        objetive = null;
        // yield return new WaitForSeconds(0.75f);
        while (true)
        {
            // Debug.LogWarning("Thinkings");
            yield return new WaitForSeconds(0.25f);
            Debug.LogWarning($"{name} is evaluating the situation");
            ReevaluateSituationClean();
        }
    }

    private IEnumerator FollowEnemy()
    {
        if (!agente.isOnNavMesh) yield return null;
        Debug.LogWarning("Following enemy");
        //MARIO aqui el animal se mueve
        while (!isFlying && !agente.isStopped && isEnemy && objetive)
        {
            var position = objetive.position;
            agente.SetDestination(position);
            Debug.LogWarning($"My name is {name} and my head is {_headTransform}");
            var distance = Vector3.Distance(_headTransform.position, position);
            if (distance <= maxAttackDistance)
            {
                Debug.LogWarning("El weon sa parao " + distance + "maxDist: " + maxAttackDistance);
                agente.isStopped = true;
                //MARIO aqui el animal se para
                break;
            }
            yield return new WaitForNextFrameUnit();
        }
    }
    
    void Update()
    {
        checkAreaAgent();

        // var d = _attacksMap.Select(a => a.Value.AttackDistance).Max();
        if (objetive && _headTransform)
        {
            if (isFlying)
            {
                flyUpdate();
            }
            if(isEnemy)
            {
                var distance = Vector3.Distance(_headTransform.position, objetive.position);
                // Debug.Log(maxAttackDistance + " ... " + Vector3.Distance(transform.position, objetive.position));
                if (distance <= maxAttackDistance)
                {
                    if (_followCoroutine != null)
                        StopCoroutine(_followCoroutine);
                    agente.isStopped = true;
                    //MARIO aqui el animal se para
                    if (Time.time - timeLastHit >= 1f / attackSpeed)
                    {
                        if (TryAttack(distance))
                        {
                            timeLastHit = Time.time;
                            // agente.isStopped = true;
                        }
                    }
                }
                // else if (isFlying)
                // {
                //     flyUpdate();
                // }
                else if (!isFlying && agente.isStopped)
                {
                    agente.isStopped = false;
                    // if(isFlying)
                    //     flyUpdate();
                    // else
                    {
                        _followCoroutine = StartCoroutine(FollowEnemy());
                    }
                }
            }
            else
            {
                
            }
        }
        else if(!_headTransform)
        {
            Debug.LogWarning($"{name} ha perdido la cabeza badum tss");
        }
    }
    
    public float Attacked(float enemyDamage)
    {
        if (health <= 0) return 0;
        // Debug.Log(gameObject.name + " -- "+health);
        if (isFlying) enemyDamage *= 0.95f;
        if(IsHost)
            GameManager.Instance.InstantiateTextDamageClientRpc(GetComponent<NetworkObject>(),enemyDamage);
        health -= enemyDamage;
        if(IsHost)
            GameManager.Instance.UpdateHealthEntityServerRpc();
        if (health <= 0.01)
        {
            canBeAttacked = false;
            // StopAllCoroutines();
            StopCoroutine(_brainCoroutine);
            AnimationDeathClientRpc(GetComponent<NetworkObject>());

            if (!canRevive)
            {
                Destroy(gameObject,2.1f);
                if (IsHost)
                {
                    Debug.Log("uwulandia");
                    GameManager.Instance.checkIfRoundEnded(layer);
                }
            }
        }
        
            

        return health;
    }

    private Vector3[] _positions;
    private Vector3[] _startPositions;
    private Quaternion[] _rotations;
    private Quaternion[] _startRotations;
    private Transform[] _transforms;
    private IEnumerator DoAnimationRevive()
    {
        Debug.LogWarning("Animation " + "DoAnimation");

        yield return new WaitForSeconds(0.75f);
        StartCoroutine(AnimationRevive());
    }

    private IEnumerator AnimationRevive()
    {
        _startRotations = _transforms.Select(tr => tr.rotation).ToArray();
        _startPositions = _transforms.Select(tr => tr.position).ToArray();
        
        // Debug.LogWarning("Animation " + "AnimationRevive");
        var t = 0f;
        var animationSpeed = 3.5f;
        foreach (var variable in _transforms)
        {
            if (variable.TryGetComponent(out Rigidbody bRigidbody))
            {
                Destroy(bRigidbody);
            }
            if (variable.TryGetComponent(out BoxCollider boxCollider))
            {
                boxCollider.isTrigger = false;
            }
            variable.SetParent(transform);
        }
        do
        {
            // if(Time.deltaTime > 0.5f)
            // {
            //     Debug.LogWarning("Animation menuda frame gordaa " + Time.deltaTime);
            //     continue;
            // }
            t += Time.deltaTime * animationSpeed;
            // Debug.LogWarning("Animation time" + t);
            for (int i = 0; i < 3; i++)
            {
                var pos = Vector3.Slerp(_startPositions[i], _positions[i], t);
                var rot = Quaternion.Slerp(_startRotations[i], _rotations[i], t);
                _transforms[i].SetPositionAndRotation(pos, rot);
            }
            yield return null;
        } while (t < 1);

        // Debug.LogWarning("Animation finish animation " + t);
        canRevive = false;
        canBeAttacked = true;
        if(IsHost)
        {
            health = maxHealth * 0.25f;
            GameManager.Instance.UpdateHealthEntityServerRpc();
            _brainCoroutine = StartCoroutine(Brain());
        }
    }

    [ClientRpc]
    private void AnimationDeathClientRpc(NetworkObjectReference targetObject)
    {
        _positions = new Vector3[3];
        _rotations = new Quaternion[3];
        _transforms = new Transform[3];
        var index = 0;
        if (targetObject.TryGet(out NetworkObject target))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                if(!t.TryGetComponent<IAnimalPart>(out var p))
                    continue;
                
                Debug.LogError(t.name);
                _positions[index] = t.position;
                _rotations[index] = t.rotation;
                _transforms[index] = t;
                index++;
                
                t.parent = null;
                i--;
                if (t.TryGetComponent(out BoxCollider b))
                {
                    b.isTrigger = false;
                }
                else
                {
                    t.AddComponent<BoxCollider>().isTrigger = false;
                }
                t.AddComponent<Rigidbody>();
                if(!canRevive)
                    Destroy(t.gameObject,1.9f);
            }
            if(canRevive)
            {
                StartCoroutine(DoAnimationRevive());
            }
        }
    }

    public void ApplyPoison()
    {
        isPoisoned = true;
    }

    private IEnumerator RemovePoison(float time)
    {
        yield return new WaitForSeconds(time);
        isPoisoned = false;
    }

    public void ApplyBleeding()
    {
        StartCoroutine(ChangeHealthOverTime(-10, 3));
    }

    public void ApplyHealing()
    {
        StartCoroutine(ChangeHealthOverTime(5, 10));
    }
    
    private IEnumerator ChangeHealthOverTime(float healthChange, int seconds)
    {
        for (int i = 0; i < seconds; i++)
        {
            yield return new WaitForSeconds(1.0f);
            // health += healthChange;
            if (health + healthChange > maxHealth) healthChange = maxHealth - health;
            Attacked(-healthChange);
            // if(IsHost)
            //     GameManager.Instance.UpdateHealthEntityServerRpc();
        }
    }
    
    private void SetAgent()
    {
        agente = GetComponent<NavMeshAgent>();
        isOnGround = true;
        // agente.speed = speed;

        // if(!isFlying)
        //     StartCoroutine(SetDestination(objetive));
    }

    private Vector3 lookat;
    private Quaternion _targetRot;
    private void flyUpdate()
    {
        if (transform.position.y < 2.5F)
        {
            gameObject.transform.Translate(Vector3.up* (Time.deltaTime * speed));
            return;
        }

        if (!objetive) return;
        // Debug.Log("Flying");

        var pos = transform.position;
        var objPosition = objetive.transform.position;
        pos.y = objPosition.y;
        _targetRot = Quaternion.LookRotation(objPosition - pos, Vector3.up);
        
        if (!Mathf.Approximately(Quaternion.Angle(transform.rotation, _targetRot), 0))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRot, 150 * Time.deltaTime);
        }
        

        if (Vector3.Distance(_headTransform.position, objetive.position) > maxAttackDistance)
        {
            Vector3 dir = objetive.position - transform.position;
            dir.Normalize();
            transform.Translate(dir* (Time.deltaTime * speed),Space.World);
        }
        else
        {
            
            // Debug.LogWarning("El weon sa parao " + Vector3.Distance(_headTransform.position, objetive.position) + "maxDist: " + maxAttackDistance);
        }
    }
    private IEnumerator SetDestination(Transform d)
    {
        yield return new WaitUntil((() => agente.isOnNavMesh));
        if (TryGetComponent(out IAttack a))
        {
            a.SetCurrentObjetive(d);
        }

        if (isFlying)
        {
            yield break;
        }
        agente.SetDestination(d.position);
    }
    //private IEnumerator AddPosition()
    //{
    //    yield return new WaitForSeconds(1.0f);
    //    GameManager.Instance.AddPositionSomething(transform.position,gameObject);
    //}

    #region getters and setters

    public bool GetIsOnGround()
    {
        return isOnGround;
    }

    public float GetCurrentDamage()
    {
        return DamageModifier * damage;
    }

    // public void SetCurrentDamage(float v)
    // {
    //     damageModifier = v;
    // }

    public void SetSpeed(float s)
    {
        agente.speed = s < 1 ? 1 : s;
    }
    

    public float GetSpeed()
    {
        return agente.speed;
    }
    #endregion
    private void ReevaluateSituationClean()
    {
        //If the agent is not on navmesh AND it's not flying, it means it hasn't been placed yet (sike)
        if(!agente.isOnNavMesh && !isFlying)  return;
        Debug.Log($"{name} is evaluating situation x2");
        if (objetive)
        {
            if (isEnemy)
            {
                //If the enemy is close enough, we need to stop the animal (navmesh agent is stupid and can't stop without our help (sad))
                if (Vector3.Distance(objetive.position, _headTransform.position) > maxAttackDistance)
                {
                    if(!isFlying)
                    {
                        agente.isStopped = false;
                        _followCoroutine = StartCoroutine(FollowEnemy());
                    }
                }
            }else
            if (Vector3.Distance(objetive.position, _headTransform.position) <= 3f)
            {
                //Pick the resource. Since it's been picked, we need a new objective and thus don't return
                if(objetive.TryGetComponent<recurso>(out var res))
                    res.PickRecurso(this);
            }
            else
            {
                //If it's focused on a resource it shouldn't change objective. Resources have priority!
                if(objetive.GetComponent<MeshRenderer>().enabled)
                    return;
                else
                {
                    objetive = null;
                }
            }
        }

        if (EvaluateResources()) return;
        if (EvaluateEnemies()) return;
        objetive = null;
        Debug.LogWarning("No suitable objective was found for " + name);
    }

    private bool EvaluateResources()
    {
        //Get a list of all the resources that haven't been chosen by an animal yet
        var resources = FindObjectsOfType<recurso>().Where(recurso => !recurso.GetSelected() && Vector3.Distance(_headTransform.position, recurso.transform.position) <= GameManager.Instance.MaxDistance + 5.0 && recurso.GetComponent<MeshRenderer>().enabled).ToList();
        
        //Convert the resources into a list of key-values. The key is the resource, and the value it's priority. Each part of the animal will have a different priority
        var resourceValues = resources.Select(transform1 => new KeyValuePair<Transform, float>(transform1.transform, 0f)).ToList();
        Debug.LogWarning($"{name} found {resources.Count}, {resources.Aggregate("", (current, resource) => current + (resource.name + ", "))} resources");
        //We get the priority values each part gives to each resource
        MergeInformation(ref resourceValues, _head.AssignValuesToResources(resources));
        MergeInformation(ref resourceValues, _body.AssignValuesToResources(resources));
        MergeInformation(ref resourceValues, _feet.AssignValuesToResources(resources));
        
        //We remove all unwanted resources from the list (those with zero priority)
        resourceValues = resourceValues.Where(pair => pair.Value > 0).ToList();
        Debug.LogWarning($"Out of those, {name} was interested in {resourceValues.Count}");
        //If there are no resources, we skip to pondering the enemies!
        if (resourceValues.Count == 0) return false;
        
        //Sort the resources, as we want the one with the highest priority
        resourceValues.Sort((kp, kp1) => Mathf.CeilToInt((kp.Value - kp1.Value) * 1000));
        
        objetive = resourceValues.First().Key;
        if (objetive.TryGetComponent(out recurso rec))
        {
            //We dont want any sneaky animal stealing OUR resources >:(
            rec.SetSelected(true);
        }
        agente.SetDestination(objetive.position);
        isEnemy = false;
        Debug.Log($"{name} is going after resource {objetive.name}");
        return true;
    }
    
    private bool EvaluateEnemies()
    {
        //Get a list of all the resources that haven't been chosen by an animal yet
        var enemies = FindObjectsOfType<Entity>().Where((entity, i) => entity.layer == layerEnemy).Select(entity => entity.transform).ToList();
        
        //Convert the enemies into a list of key-values. The key is the enemy, and the value it's priority. In general most animals do have similar priorities
        var resourcesEnemies = enemies.Select(transform1 => new KeyValuePair<Transform, float>(transform1, 0f)).ToList();;
        
        //We get the priority values each part gives to each resource
        MergeInformation(ref resourcesEnemies, _head.AssignValuesToEnemies(enemies));
        MergeInformation(ref resourcesEnemies, _body.AssignValuesToEnemies(enemies));
        MergeInformation(ref resourcesEnemies, _feet.AssignValuesToEnemies(enemies));
        
        //We remove all unwanted enemies from the list (those with zero priority)
        resourcesEnemies = resourcesEnemies.Where(pair => pair.Value > 0).ToList();
        //If there are no enemies, we don't assign any objectives (Is gang alive?)
        if (resourcesEnemies.Count == 0) return false;
        
        //Sort the enemies, as we want the one with the highest priority
        resourcesEnemies.Sort((kp, kp1) => Mathf.CeilToInt((kp.Value - kp1.Value) * 1000));
        
        objetive = resourcesEnemies.First().Key.GetComponent<Entity>()._headTransform;
        if (!objetive) return false;
        isEnemy = true;
        
        Debug.Log($"{name} is going after enemy {objetive.name}");
        return true;
    }
    
    private void ReevaluateSituation()
    {
        if(!agente.isOnNavMesh && !isFlying)  return;
        if (objetive)
        {
            // Debug.LogError(Vector3.Distance(objetive.position, transform.position));
            // agente.isStopped = true;
            if (isEnemy)
            {
                if (Vector3.Distance(objetive.position, _headTransform.position) > maxAttackDistance)
                {
                    if(!isFlying)
                    {
                        // Debug.Log($"Atacando a {objetive} en {timeLastHit}");
                        // Attack();
                        agente.isStopped = false;
                        _followCoroutine = StartCoroutine(FollowEnemy());
                    }
                }
                // return; 
                
                // if(TryAttack(Vector3.Distance(objetive.position, transform.position)))
                // {
                //     agente.isStopped = true;
                //     return;
                // }
                // if(agente.isStopped)    return;
            }else
            if (Vector3.Distance(objetive.position, _headTransform.position) <= 3f)
            {
                if(objetive.TryGetComponent<recurso>(out var res))
                    res.PickRecurso(this);
                // return; 
            }
        }

        Debug.LogWarning("Searching for new objective");
        agente.isStopped = false;
        var enemies = FindObjectsOfType<Entity>().Where((entity, i) => entity.layer == layerEnemy).Select(entity => entity.transform).ToList();
        var resources = FindObjectsOfType<recurso>().Where(recurso => !recurso.GetSelected() && Vector3.Distance(_headTransform.position, recurso.transform.position) <= GameManager.Instance.MaxDistance && recurso.GetComponent<MeshRenderer>().enabled).ToList();
        Debug.LogWarning("MaxDist: " + GameManager.Instance.MaxDistance);
        if(resources.Count == 0 && enemies.Count == 0)  return;

        var values = enemies.Select(transform1 => new KeyValuePair<Transform, float>(transform1, 0f)).ToList();
        values.AddRange(resources.Select(transform1 => new KeyValuePair<Transform, float>(transform1.transform, 0f)));

        
        
        var partRange = _head.AssignValuesToEnemies(enemies);
        partRange.AddRange(_head.AssignValuesToResources(resources));
        MergeInformation(ref values, partRange);
        
        partRange = _body.AssignValuesToEnemies(enemies);
        partRange.AddRange(_body.AssignValuesToResources(resources));
        MergeInformation(ref values, partRange);
        
        partRange = _feet.AssignValuesToEnemies(enemies);
        partRange.AddRange(_feet.AssignValuesToResources(resources));
        MergeInformation(ref values, partRange);
        // var a = from entry in values orderby entry.Value descending select entry;

        if (objetive && objetive.TryGetComponent<recurso>(out var a))
        {
            a.SetSelected(false);
        }
        if (values.Count > enemies.Count)
        {
            Debug.Log("Hay recursos");
            var resourcesValues = values.GetRange(enemies.Count, resources.Count);
            resourcesValues = resourcesValues.Where(pair => pair.Value > 0).ToList();
            if (resourcesValues.Count == 0)
            {
                resourcesValues = values.Where(pair => pair.Value > 0).ToList();
                if(resourcesValues.Count == 0)
                {
                    objetive = null;
                    return;
                }
            }
            resourcesValues.Sort((kp, kp1) => (int)Mathf.CeilToInt((kp.Value - kp1.Value) * 1000));
            objetive = values.First().Key;
        }
        else
        {
            values = values.Where(pair => pair.Value > 0).ToList();
            if (values.Count == 0)
            {
                objetive = null;
                return;
            }
            values.Sort((kp, kp1) => (int)Mathf.CeilToInt((kp.Value - kp1.Value) * 1000));
            // Debug.Log(resources.Count());

            objetive = values.First().Key;
        }
        isEnemy = (objetive.parent && (bool)objetive.parent.GetComponent<Entity>());
        // Debug.LogWarning($"Next objective is {objetive.name} and is {isEnemy} an enemy??");
        // agente.SetDestination(objetive.position);
        if(isEnemy)
        {
            _followCoroutine = StartCoroutine(FollowEnemy());
        }
        else
        {
            objetive.GetComponent<recurso>().SetSelected(true);
            agente.SetDestination(objetive.position);
            Debug.LogWarning("Going to " + objetive.position);
        }
        // Debug.Log(objetive.name);
    }

    private void MergeInformation(ref List<KeyValuePair<Transform, float>> inDic, List<float> info)
    {
        for (var i = 0; i < inDic.Count; i++)
        {
            inDic[i] = new KeyValuePair<Transform, float>(inDic[i].Key, inDic[i].Value + info[i]);
        }
    }

    public override void OnDestroy()
    {
        _idPlayer.OnValueChanged -= SetLayer;
        
        GameManager.Instance.playerObjects.Remove(gameObject);
        base.OnDestroy();
    }
    public delegate void EmptyEvent();
    public delegate void ResourcesEvent(Recursos res, int n);

    
    public EmptyEvent OnDeath;
    public EmptyEvent OnKilledEnemy;
     
    public ResourcesEvent OnResourcesChanged;
    private float timeLastHit;

    public struct AttackStruct
    {
        public AttackStruct(float d, Action a, Dictionary<Recursos, int> nr)
        {
            AttackDistance = d;
            Attack = a;
            necessaryResources = nr;
            // Type = type;
        }

        // public TotemPiece.Type Type;
        public float AttackDistance;
        public Action Attack;
        public Dictionary<Recursos, int> necessaryResources;
    }
    private Dictionary<TotemPiece.Type, AttackStruct> _attacksMap;
    // private List<AttackStruct> aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa;

    public void SubscribeAttack(TotemPiece.Type type, AttackStruct attackStruct)
    {
        _attacksMap.TryAdd(type, attackStruct);
        maxAttackDistance = _attacksMap.Select(a => a.Value.AttackDistance).Max();
    }
    public void UnsubscribeAttack(TotemPiece.Type type)
    {
        _attacksMap.Remove(type);
        maxAttackDistance = _attacksMap.Select(a => a.Value.AttackDistance).Max();
    }
    private bool TryAttack(float distance)
    {
        if (objetive.parent && objetive.parent.TryGetComponent(out Entity enemy))
        {
            if (!enemy.canBeAttacked) return false;
        }
        if (health < 0.01f) return false;
        string blah = string.Join(", ", _attacksMap.Select(v => v.Value.AttackDistance.ToString(CultureInfo.InvariantCulture)).ToArray());
        Debug.LogWarning(blah);
        var possibleAttacks = _attacksMap.ToArray().Where(a => a.Value.AttackDistance >= distance).ToArray();
        if (!possibleAttacks.Any()) return false;
        // Debug.Log("Evaluating attacks");
        possibleAttacks[Random.Range(0, possibleAttacks.Count())].Value.Attack.Invoke();
        return true;
        // possibleAttacks.
    }
    
    public int GetResources(Recursos res)
    {
        return _resources.TryGetValue(res, out var value) ? value : 0;
    }
     public void AddOrTakeResources(Recursos res, int n)
         {
             if (_resources.TryGetValue(res, out var value))
             {
                 if (value + n > 0)
                     _resources[res] = value + n;
                 else
                     _resources.Remove(res);
                 OnResourcesChanged.Invoke(res, Math.Max(n+value, 0));
             }
             else if(n > 0)
             {
                 _resources.Add(res, n);
                 OnResourcesChanged(res, n);
             }
         }
    public IEnumerator SearchResources()
    {
        //seleccion de bioma segun el bicho que seas:
        yield return new WaitUntil((() => agente.isOnNavMesh));
        var biomas = GameManager.Instance.biomasInMatch;
        float minDistance = float.MaxValue;
        Transform o = null;
        foreach (ABiome b in biomas)
        {
            foreach (Transform r in b.GetRecursos())
            {
                float d = Vector3.Distance(_headTransform.position, r.position);
                if (minDistance > d && !r.GetComponent<recurso>().GetSelected())
                {
                    minDistance = d;
                    o = r;
                }
            }
        }

        if (o)
        {
            o.GetComponent<recurso>().SetSelected(true);
            StartCoroutine(SetDestination(o));
        }
    }

    #region biomas

    private void checkAreaAgent()
    {
        NavMeshHit hit;
        
        if (NavMesh.SamplePosition(agente.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            for (int areaIndex = 0; areaIndex < 5; areaIndex++)
            {
                if ((hit.mask & (1 << areaIndex)) != 0)
                {
                    if (currentAreaIndex != areaIndex)
                    {
                        OnEnterArea(areaIndex,currentAreaIndex);
                        currentAreaIndex = areaIndex;
                    }
                    break; 
                }
            }
        }
    }

    private void OnEnterArea(int index,int prevBiome)
    {
        if (prevBiome != 0)//bioma 0 es el walkable, no hay que deshacer efectos
        {
            var prevBioma = GameManager.Instance.biomasGame[prevBiome];
            prevBioma.LeaveBiome(gameObject);
        }
        
        var bioma = GameManager.Instance.biomasGame[index];
        if(bioma)
            bioma.ActionBioma(gameObject);
    }

    #endregion
}
