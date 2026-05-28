using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Unity.Mathematics;

public class DiveController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;

    [Header("HDRP Water")]
    public WaterSurface targetSurface;
    public bool includeDeformers = true;

    [Header("Jump Dive")]
    public float forcedDiveDuration = 1.5f;
    private float forcedDiveTimer = 0f;

    [Header("Water Detection")]
    public float enterWaterOffset = 0.2f;
    public float surfaceBand = 0.8f;

    [Header("Surface Floating")]
    public float surfaceRootOffset = 0.4f;
    public float surfaceFollowSpeed = 6f;

    [Header("State")]
    public bool inWater = false;
    public bool isAtSurface = false;
    public bool isUnderwater = false;
    public float currentWaterY;
    private bool waitingForJumpWaterEntry = false;

    private WaterSearchParameters searchParameters = new WaterSearchParameters();
    private WaterSearchResult searchResult = new WaterSearchResult();

    private void Update()
    {
        UpdateWaterState();
    }

    public bool TryGetWaterHeight(Vector3 worldPosition, out float waterY)
    {
        waterY = 0f;

        if (targetSurface == null)
            return false;

        searchParameters.startPositionWS = (float3)worldPosition;
        searchParameters.targetPositionWS = (float3)worldPosition;
        searchParameters.error = 0.01f;
        searchParameters.maxIterations = 8;
        searchParameters.includeDeformation = includeDeformers;
        searchParameters.excludeSimulation = false;

        if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
        {
            waterY = ((Vector3)searchResult.projectedPositionWS).y;
            currentWaterY = waterY;
            return true;
        }

        return false;
    }

    private void UpdateWaterState()
    {
        if (!TryGetWaterHeight(transform.position, out currentWaterY))
            return;

        float playerY = transform.position.y;
        float surfaceTargetY = currentWaterY + surfaceRootOffset;

        bool touchingWater = playerY <= surfaceTargetY + 0.8f;

        // If we jumped from boat, only start forced dive AFTER touching water.
        if (waitingForJumpWaterEntry && touchingWater)
        {
            waitingForJumpWaterEntry = false;
            StartForcedDive();
            return;
        }

        if (forcedDiveTimer > 0f)
        {
            forcedDiveTimer -= Time.deltaTime;

            inWater = true;
            isAtSurface = false;
            isUnderwater = true;

            UpdateAnimator();
            return;
        }

        inWater = touchingWater;

        if (inWater)
        {
            isAtSurface = Mathf.Abs(playerY - surfaceTargetY) <= surfaceBand;

            if (playerY >= surfaceTargetY - surfaceBand)
                isAtSurface = true;

            isUnderwater = !isAtSurface;
        }
        else
        {
            isAtSurface = false;
            isUnderwater = false;
        }

        UpdateAnimator();
    }


    public void StartForcedDive()
    {
        forcedDiveTimer = forcedDiveDuration;
        inWater = true;
        isAtSurface = false;
        isUnderwater = true;
        UpdateAnimator();
    }

    public void ForceDiveDown()
    {
        inWater = true;
        isAtSurface = false;
        isUnderwater = true;

        UpdateAnimator();
    }

    public void MarkJumpedFromBoat()
    {
        waitingForJumpWaterEntry = true;
    }

    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        animator.SetBool("IsUnderwater", inWater);
        animator.SetBool("IsSurface", isAtSurface);
    }

    public void ExitWater()
    {
        inWater = false;
        isAtSurface = false;
        isUnderwater = false;

        UpdateAnimator();
    }
}