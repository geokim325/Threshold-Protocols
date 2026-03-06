using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent için gerekli

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;
    private Gamemanager gameManager;
    private FirstPersonLook firstPersonLook;

    [Header("Inspect Ayarlarý")]
    [Tooltip("Crosshair ile inceleme esnasýnda düþmanýn dönme hýzý.")]
    public float inspectRotateSpeed = 2f;

    [Header("Saldýrý Ayarlarý")]
    public float attackJumpForce = 5f;
    public float attackDuration = 2f;
    public float attackAnimStartTime = 2.5f;

    [HideInInspector] public bool reachedControlPoint = false;

    private bool awaitingDecision = false;
    private float decisionWaitTimer = 0f;
    private Transform cameraTransform;
    private Rigidbody rb;
    private BoxCollider scollider;
    private MeshRenderer meshRenderer;

    // --- ANÝMASYON DURUM DEÐÝÞKENLERÝ ---
    [HideInInspector] public bool isInjured = false;
    private string walkAnimName;
    private string idleAnimName;

    // GameManager ayarlarý
    private float stopDistance;
    private float waitBeforeDecision;
    private float injuredChance;
    private float normalSpeed;
    private float injuredSpeed;
    private bool lightClosed = false;

    // GC OPTÝMÝZASYONU
    private NavMeshPath cachedPath;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        scollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        gameManager = Gamemanager.Instance;
        firstPersonLook = FirstPersonLook.Instance;

        if (firstPersonLook != null)
            cameraTransform = firstPersonLook.transform;

        cachedPath = new NavMeshPath();

        stopDistance = gameManager.stopDistance;
        waitBeforeDecision = gameManager.waitBeforeDecision;
        injuredChance = gameManager.injuredChance;
        normalSpeed = gameManager.normalSpeed;
        injuredSpeed = gameManager.injuredSpeed;

        agent.stoppingDistance = stopDistance;

        DetermineCharacterState();
    }

    void DetermineCharacterState()
    {
        isInjured = Random.Range(0f, 100f) < injuredChance;

        if (isInjured)
        {
            agent.speed = injuredSpeed;
            idleAnimName = "Injured Idle";
            walkAnimName = Random.Range(0, 2) == 0 ? "Injured Walk" : "Injured Walk 2";
        }
        else
        {
            agent.speed = normalSpeed;
            idleAnimName = "Normal Idle";
            walkAnimName = Random.Range(0, 2) == 0 ? "Normal Walk" : "Normal Walk 2";
        }

        PlayIdleAnimation();
    }

    void PlayWalkAnimation()
    {
        animator.Play(walkAnimName);
    }

    void PlayIdleAnimation()
    {
        animator.Play(idleAnimName);
    }

    void Update()
    {
        if (currentTarget != null && !agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            if (currentTarget == gameManager.controlPoint && !reachedControlPoint)
            {
                reachedControlPoint = true;
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                PlayIdleAnimation();
                awaitingDecision = true;
            }
            else if (currentTarget == gameManager.approvedPoint || currentTarget == gameManager.notApprovedPoint)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                PlayIdleAnimation();
                Destroy(gameObject, gameManager.disappearDelay);
            }
        }

        if (awaitingDecision)
        {
            decisionWaitTimer += Time.deltaTime;
            LookAtCamera();

            if (decisionWaitTimer >= waitBeforeDecision)
            {
                awaitingDecision = false;
                gameManager.StartSorgu();
            }
        }

        if (!reachedControlPoint && !lightClosed && firstPersonLook != null)
        {
            firstPersonLook.currentTarget = null;
            firstPersonLook.TurnOffLight();
        }
    }

    public void SuretAttack()
    {
        firstPersonLook.canLook = false;

        if (cameraTransform != null)
        {
            Vector3 dir = cameraTransform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        if (rb != null)
            rb.useGravity = false;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        scollider.enabled = false;

        animator.Play("Attack");

        StartCoroutine(MeshDelay());
        StartCoroutine(JumpForward(attackDuration, attackJumpForce));

        firstPersonLook.DestroyReflectedCopy();
        gameManager.sorguCanvas.SetActive(false);
    }

    IEnumerator MeshDelay()
    {
        yield return new WaitForSeconds(1f);
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    IEnumerator JumpForward(float duration, float force)
    {
        firstPersonLook.lightObject.SetActive(false);
        firstPersonLook.thermometerCanvas.SetActive(false);

        float time = 0f;
        Vector3 horizontal = transform.forward * force;
        float vertical = force * 6f;

        while (time < duration)
        {
            horizontal = Vector3.Lerp(horizontal, Vector3.zero, Time.deltaTime * 3f);
            vertical -= 20f * Time.deltaTime;
            transform.position += (horizontal + Vector3.up * vertical) * Time.deltaTime;

            time += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSecondsRealtime(1f);
        gameManager.SuretAttackGameOver();
    }
    private void EnsureAgent()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    public void RotateCharacter(float scaledDelta)
    {
        if (!reachedControlPoint) return;
        transform.Rotate(Vector3.up, scaledDelta * -1f, Space.Self);
    }

    void LookAtCamera()
    {
        if (cameraTransform == null || !reachedControlPoint) return;

        Vector3 lookPos = cameraTransform.position;
        lookPos.y = transform.position.y;

        Quaternion targetRot = Quaternion.LookRotation(lookPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
    }

    public void StartMovementToControlPoint()
    {
        FirstPersonLook.Instance.UnlockTools();
        agent.isStopped = false;
        agent.velocity = Vector3.zero;
        SetDestination(gameManager.controlPoint);
        PlayWalkAnimation();
    }

    public void SetDestination(Transform target)
    {
        currentTarget = target;

        agent.CalculatePath(target.position, cachedPath);

        if (cachedPath.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(cachedPath);
            agent.isStopped = false;
        }
    }

    public void GoToFinalPoint(bool isApproved)
    {
        reachedControlPoint = false;
        decisionWaitTimer = 0f;

        agent.isStopped = false;
        agent.velocity = Vector3.zero;

        SetDestination(isApproved ? gameManager.approvedPoint : gameManager.notApprovedPoint);

        gameManager.Invoke(nameof(gameManager.HideSorgu), 4f);

        PlayWalkAnimation();
    }
}
