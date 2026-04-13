using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SimpleStateMachine : MonoBehaviour
{
    enum State { Idle, Patrol, Chase, Search, Investigate, Alert, Return, Attack }

    [Header("Scene References")]
    public Transform character;
    public Transform[] patrolWaypoints;
    public TextMeshProUGUI stateText;

    [Header("Config")]
    public float idleTimeThreshold = 2.0f;
    public float waypointThreshold = 0.6f;
    public float rotationSpeed = 1f;
    public float searchTimeThreshold = 5.0f;
    public float playerDistThreshold = 2.0f;
    public float normalSpeed = 3.5f;
    public float chaseSpeed = 5.0f;
    public float investigateThreshold = 10.0f;
    public float investigateDistance = 2.0f;
    public float lookAroundAngle = 45.0f;

    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 60f;

    State state;
    NavMeshAgent agent;
    int patrolIndex = 0;

    float idleTime;
    float searchTime;
    bool canSeePlayer;
    float investigateTime = 0.0f;

    bool soundHeard = false;
    Vector3 soundLocation = Vector3.zero;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            //case State.Patrol:
            //    Patrol();
            //    break;
            case State.Chase:
                Chase();
                break;
            //case State.Search:
            //    Search();
            //    break;
            case State.Investigate:
                Investigate();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Alert:
                Alert();
                break;
            case State.Return:
                ReturnHome();
                break;

        }

        // regardless of state, NPC always looks in the direction they are moving
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookDirection = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * rotationSpeed);
        }

        // show state on screen
        stateText.text = $"State: {state}";

        // if NPC ever gets close to player, end 
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;
        if (distToPlayer < playerDistThreshold && canSeePlayer) SceneManager.LoadScene("END");
    }

    public void SoundRecieve (SoundObject soundObject)
    {
        soundHeard = true;
        soundLocation = soundObject.transform.position;
    }
    void Idle()
    {
        agent.speed = normalSpeed;

        if (soundHeard == true)
        {
            state = State.Alert;
        }


        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {
            soundHeard = false;
            state = State.Chase;
        }

        // during idle, can never see player
        //canSeePlayer = false;

        //float idleTimeElapsed = Time.time - idleTime;
        //if (idleTimeElapsed >= idleTimeThreshold)
        //{
        //    Debug.Log(state);
        //    state = State.Patrol;
        //}
    }

    void Alert()
    {
        agent.SetDestination(soundLocation);

        float distance = Vector3.Distance(transform.position, character.transform.position);

        if (distance <= investigateDistance)
        {
            state = State.Investigate;
            //LookAround();
            //float timeElapsed = Time.time - investigateTime;
            //if (timeElapsed >= investigateThreshold)
            //{
            //    soundHeard = false;
            //    state = State.Investigate;
            //}
        }
        //else
        //{
        //    investigateTime = Time.time;
        //}

        //viewEnabled = true;
        canSeePlayer = IsInViewCone();

        if (canSeePlayer)
        {
            soundHeard = false;
            state = State.Chase;
        }
    }

    //void Patrol()
    //{
    //    agent.speed = normalSpeed;

    //    Transform patrolTransform = patrolWaypoints[patrolIndex];
    //    agent.SetDestination(patrolTransform.position);

    //    Vector3 positionXZ = transform.position;
    //    positionXZ.y = 0.0f;

    //    Vector3 patrolPositionXZ = patrolTransform.position;
    //    patrolPositionXZ.y = 0.0f;

    //    float distance = Vector2.Distance(positionXZ, patrolPositionXZ);
    //    if (distance < waypointThreshold)
    //    {
    //        IncreasePatrolIndex();
    //        Debug.Log(state);
    //        state = State.Idle;
    //        idleTime = Time.time;
    //    }

    //    canSeePlayer = IsInViewCone();
    //    if (canSeePlayer)
    //    {
    //        Debug.Log(state);
    //        state = State.Chase;
    //    }

    //    if (soundHeard)
    //    {
    //        EnterInvestigate();
    //    }
    //}

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(character.position);

        canSeePlayer = IsInViewCone();
        if (!canSeePlayer)
        {
            state = State.Investigate;
        }
    }

    //void EnterInvestigate()
    //{
    //    state = State.Investigate;
    //    investigateTime = Time.time;
    //}
    void Investigate()
    {
        //agent.SetDestination(soundLocation);

        float timeElapsed = Time.time - investigateTime;

        transform.Rotate(Vector3.up, Mathf.Sin(Time.time) * Time.deltaTime * 90f);

        if (timeElapsed >= investigateThreshold)
        {
            soundHeard = false;
            state = State.Return;
        }
        else
        {
            investigateTime = Time.time;
        }
        //float distance = Vector3.Distance(transform.position, character.transform.position);

        //if(distance <= investigateDistance)
        //{
        //    float timeElapsed = Time.time - investigateTime;

        //    transform.Rotate(Vector3.up, Mathf.Sin(Time.time) * Time.deltaTime * 90f);

        //    if (timeElapsed >= investigateThreshold)
        //    {
        //        soundHeard = false;
        //        state = State.Return;
        //    }
        //}

        //viewEnabled = true;
        canSeePlayer = IsInViewCone();

        if (canSeePlayer)
        {
            soundHeard = false;
            state = State.Chase;
        }
    }

    //void Search()
    //{
    //    agent.speed = chaseSpeed;

    //    float searchTimeElapsed = Time.time - searchTime;

    //    agent.SetDestination(transform.forward + transform.right);
    //    canSeePlayer = IsInViewCone();

    //    if (canSeePlayer)
    //    {
    //        state = State.Chase;
    //        Debug.Log(state);
    //    }

    //    if (searchTimeElapsed >= searchTimeThreshold)
    //    {
    //        state = State.Patrol;
    //        Debug.Log(state);
    //    }
    //}

    void Attack()
    {
        //if Predator reaches player, game over

    }

    void ReturnHome()
    {
        //predator returns to idle position after not finding or losing the player

    }



    // --- HELPER FUNCTIONS ---

    bool IsInViewCone()
    {
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;

        // 1. Distance check
        if (distToPlayer > viewRadius) return false;

        // 2. Angle check
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > viewAngle * 0.5f) return false;

        // 3. Raycast
        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewRadius))
        {
            return hit.transform == character.transform;
        }
        return false;
    }

    //void IncreasePatrolIndex()
    //{
    //    patrolIndex++;
    //    if (patrolIndex >= patrolWaypoints.Length) patrolIndex = 0;
    //}

    // --- GIZMO DRAWING FOR DEBUG ---

    private void OnDrawGizmos()
    {
        // draw the waypoints
        Gizmos.color = Color.red;
        foreach (Transform patrolTransform in patrolWaypoints)
        {
            Gizmos.DrawWireSphere(patrolTransform.position, 0.5f);
        }

        // draw the view cone (2D version)
        if (state != State.Idle)
        {
            Handles.color = new Color(0f, 1f, 1f, 0.25f);
            if (canSeePlayer) Handles.color = new Color(1f, 0f, 0f, 0.25f);

            Vector3 forward = transform.forward;
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, viewAngle / 2f, viewRadius);
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, -viewAngle / 2f, viewRadius);
        }
    }
}
