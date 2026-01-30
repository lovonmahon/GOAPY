using System;
using UnityEngine.UI;
using UnityEngine;

namespace RiseReign
{
    /*
     * Sight
     * ------------------------------------------------------------
     * This script lets an AI "see" a target (player/enemy).
     *
     * IMPORTANT:
     * - Sight does NOT decide what the AI should do.
     * - Sight does NOT interrupt or replan.
     * - Sight ONLY reports facts to the Worker.
     *
     * Think of this like the AI's eyes.
	 *
	What happens when the AI sees the enemy?

	Eyes see something
	
	Eyes tell the brain where it was
	
	Eyes say: “This is dangerous!”
	
	That’s it.
	
	The eyes do not say:
	
	“Attack!”
	
	“Hide!”
	
	“Run!”
	
	They only say:
	
	“I see something scary, and it was there.”
     */
    public class Sight : MonoBehaviour
    {
        #region variables

        // The thing we are looking for (player, enemy, etc.)
        public Transform player;

        // How wide the vision cone is
        public float maxAngle;

        // How far the AI can see
        public float maxRadius;

        // True if the target is currently visible
        public bool isInFOV = false;

        // -------------------- NEW --------------------
        // Reference to Worker so we can report danger
        Worker worker;
        // ------------------------------------------------

        #endregion

        #region gizmos
        // Draw vision cone in the editor (debug only)
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxRadius);

            Vector3 fovLine1 =
                Quaternion.AngleAxis(maxAngle, transform.up) *
                transform.forward * maxRadius;

            Vector3 fovLine2 =
                Quaternion.AngleAxis(-maxAngle, transform.up) *
                transform.forward * maxRadius;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);

            // Line to the player
            Gizmos.color = isInFOV ? Color.green : Color.red;
            if (player != null)
            {
                Gizmos.DrawRay(
                    transform.position,
                    (player.position - transform.position).normalized * maxRadius
                );
            }

            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
        }
        #endregion

        #region FOV check
        /*
         * Checks if a target is inside the vision cone
         * and not blocked by walls.
         */
        public static bool inFOV(
            Transform checkingObject,
            Transform target,
            float maxAngle,
            float maxRadius)
        {
            Collider[] overlaps = new Collider[10];

            int count = Physics.OverlapSphereNonAlloc(
                checkingObject.position,
                maxRadius,
                overlaps
            );

            for (int i = 0; i < count; i++)
            {
                if (overlaps[i] == null)
                    continue;

                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween =
                        (target.position - checkingObject.position).normalized;

                    // Ignore vertical difference
                    directionBetween.y = 0;

                    float angle =
                        Vector3.Angle(checkingObject.forward, directionBetween);

                    if (angle <= maxAngle)
                    {
                        Ray ray = new Ray(
                            checkingObject.position,
                            target.position - checkingObject.position
                        );

                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == target)
                                return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        // -------------------- NEW --------------------
        // Get the Worker component once
        void Start()
        {
            worker = GetComponent<Worker>();
        }
        // ------------------------------------------------

        void Update()
        {
            // Check if the target is visible
            isInFOV = inFOV(transform, player, maxAngle, maxRadius);

            // -------------------- NEW --------------------
            // If we can see the target:
            if (isInFOV && worker != null)
            {
                // Remember where the threat was last seen
                worker.SetLastKnownThreatPosition(player.position);

                // Tell the world: "I am NOT safe"
                worker.SetHide(true);
            }
            // ------------------------------------------------
        }
    }
}
