using RPG.Combat;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float raycastInteractRadius = 1f;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] CursorMapping[] mappings = null;

        bool isDraggingUI = false;
       
        Mover mover;

        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        private void Awake()
        {
            mover = GetComponent<Mover>();
        }
        void Update()
        {
            if (InteractWithUI()) return;
            if (GetComponent<Health>().IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;
            target = navMeshHit.position;

            
            
            return true;
        }

        

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastInteractRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i <hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }


        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
                
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }


                SetCursor(CursorType.UI);
                return true;
            }
            if(isDraggingUI)
            {
                return true;
            }
            return false;
        }
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture,mapping.hotspot, CursorMode.Auto); 
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping mapping in mappings)
            {
                if (mapping.type != cursorType) continue;
                return mapping;
            }
            return mappings[0];
        }

        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!mover.CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target,1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
