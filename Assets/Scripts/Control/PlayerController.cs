using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.EventSystems;
using System;
using UnityEngine.AI;
using GameDevTV.Inventories;

namespace RPG.Control
{
    public class PlayerController : Controller
    {
        [SerializeField] CursorMapping[] cursorMappings;
        [SerializeField] float maxNavMeshProjectionDistance = 40;
        [SerializeField] float raycastRadius = 1;
        bool isDraggingUI = false;
        Health health;
        Mover mover;
        ActionStore actionStore;
        InputReader inputReader;
        const int actionSlots = 6;

        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public KeyCode GetInteractionKey()
        {
            return inputReader.GetKeyCode(PlayerAction.Interaction);
        }

        public KeyCode GetCancelKey()
        {
            return inputReader.GetKeyCode(PlayerAction.Cancel);
        }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotSpot;
        }

        void Awake()
        {
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            actionStore = GetComponent<ActionStore>();
            inputReader = GetComponent<InputReader>();
        }

        void Update()
        {
            if(health.IsDead()) 
            {
                SetCursor(CursorType.None);
                return;
            }

            UseAbilites();

            if(InteractWithUI()) 
            {
                return;
            }

            if(InteractWithMovement()) 
            {
                return;
            }

            if(InteractWithComponent()) 
            {
                return;
            }

            SetCursor(CursorType.None);
        }

        bool InteractWithUI()
        {
            if(Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }

            if(EventSystem.current.IsPointerOverGameObject())
            {
                if(Input.GetMouseButton(0))
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

        void UseAbilites()
        {
            for(int i = 0; i < actionSlots; i++)
            {
                if(Input.GetKeyDown(inputReader.GetKeyCode(PlayerAction.Ability1) + i))
                {
                    actionStore.Use(i, gameObject);
                }
            }
        }

        bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();

            foreach(RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

                foreach(IRaycastable raycastable in raycastables)
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

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        bool InteractWithMovement()
        {
            bool hasHit = RaycastNavMesh(out Vector3 target);

            if(hasHit)
            {
                if(!mover.CanMoveTo(target)) 
                {
                    return false;
                }

                if(Input.GetKey(inputReader.GetKeyCode(PlayerAction.Interaction)))
                {
                    mover.StartMoveAction(target, 1f);
                }

                SetCursor(CursorType.Movement);

                return true;
            }

            return false;
        }

        bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit raycastHit);

            if(!hasHit) 
            {
                return false;
            }

            bool hasCastToNavMesh = NavMesh.SamplePosition
            (
                    raycastHit.point,
                    out NavMeshHit navMeshHit,
                    maxNavMeshProjectionDistance,
                    NavMesh.AllAreas
            );

            if(!hasCastToNavMesh) 
            {
                return false;
            }
            
            target = navMeshHit.position;

            return true;
        }

        void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotSpot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping cursorMap in cursorMappings)
            {
                if(cursorMap.type == type)
                {
                    return cursorMap;
                }
            }

            return cursorMappings[0];
        }
    }
}
