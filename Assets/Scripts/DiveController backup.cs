// using UnityEngine;
// using UnityEngine.Rendering.HighDefinition;
// using Unity.Mathematics;

// public class DiveController : MonoBehaviour
// {
//     [Header("References")]
//     public Animator animator;
//     public Transform cameraTransform;

//     [Header("HDRP Water")]
//     public WaterSurface targetSurface;
//     public bool includeDeformers = true;

//     [Header("Surface Floating Settings")]
//     public float surfaceRootOffset = 0.8f;
//     public float surfaceSnapDistance = 1f;

//     [Header("State")]
//     public bool inWater = false;
//     public bool isAtSurface = false;
//     public bool isUnderwater = false;

//     private WaterSearchParameters searchParameters = new WaterSearchParameters();
//     private WaterSearchResult searchResult = new WaterSearchResult();

//     public bool TryGetWaterHeight(Vector3 worldPosition, out float waterY)
//     {
//         waterY = 0f;

//         if (targetSurface == null)
//             return false;

//         searchParameters.startPositionWS = (float3)worldPosition;
//         searchParameters.targetPositionWS = (float3)worldPosition;
//         searchParameters.error = 0.01f;
//         searchParameters.maxIterations = 8;
//         searchParameters.includeDeformation = includeDeformers;
//         searchParameters.excludeSimulation = false;

//         if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
//         {
//             waterY = ((Vector3)searchResult.projectedPositionWS).y;
//             return true;
//         }

//         return false;
//     }

//     public void EnterWater()
//     {
//         inWater = true;
//         isUnderwater = true;
//         isAtSurface = false;
//         UpdateAnimator();
//     }

//     public void EnterSurfaceMode()
//     {
//         inWater = true;
//         isUnderwater = true;
//         isAtSurface = true;
//         UpdateAnimator();
//     }

//     public void DiveDown()
//     {
//         inWater = true;
//         isUnderwater = true;
//         isAtSurface = false;
//         UpdateAnimator();
//     }

//     public void ExitWater()
//     {
//         inWater = false;
//         isUnderwater = false;
//         isAtSurface = false;
//         UpdateAnimator();
//     }

//     public void UpdateAnimator()
//     {
//         if (animator == null) return;

//         animator.SetBool("IsUnderwater", isUnderwater);
//         animator.SetBool("IsSurface", isAtSurface);
//     }
// }