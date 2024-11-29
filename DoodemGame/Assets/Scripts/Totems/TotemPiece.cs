using System;
using System.Collections;
using System.Collections.Generic;
using tienda;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Totems
{
    public class TotemPiece : MonoBehaviour//, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [SerializeField]
        private TotemStats stats;

        public ScriptableObjectTienda scriptableObjectTienda;
        private TotemInfo _info;
        public Totem totem;
        [SerializeField]
        public Type type;
        [SerializeField]
        private string animalName;

        private Vector3 _draggingPlane;
        private State _state;
        private Coroutine _movement;
        private IDragHandler _dragHandlerImplementation;

        private enum State
        {
            Normal,
            Moving,
            Dragging
        }

        private void Start()
        {
            _info = Resources.Load<TotemInfo>( animalName + "/Description/" + "/ES" + TypeToString(type));
            _state = State.Normal;
            totem = transform.parent.GetComponent<Totem>();
        }

        #region typeEnum
            public enum Type
            {
                Head,
                Body,
                Feet,
            }

            private static string TypeToString(Type t)
            {
                switch (t)
                {
                    case Type.Head:
                        return "Head";
                    case Type.Body:
                        return "Body";
                    case Type.Feet:
                        return "Feet";
                    default:
                        return string.Empty;
                }
            }

        #endregion

        // private void OnTriggerEnter(Collider other)
        // {
        //     // Debug.Log(other.name);
        //     if (other.CompareTag("Pointer"))
        //     {
        //         OnSelected();
        //     }
        // }

        public TotemStats GetStats()
        {
            return stats;
        }
        
        public void MoveTo(Vector3 pos, float speed, bool deactivateOnEnd = false)
        {
            if(_state == State.Dragging)
                return;
            if(_state == State.Moving) 
                StopCoroutine(_movement);
            _state = State.Moving;
            _movement = StartCoroutine(MoveToCoroutine(pos, speed, deactivateOnEnd));
        }

        private IEnumerator MoveToCoroutine(Vector3 pos, float speed, bool deactivateOnEnd = false)
        {
            // Debug.Log("Moving to: " + pos);
            do
            {
                var position = transform.position;
                var newPos = Vector3.MoveTowards(position, pos, speed);
                transform.position = newPos;
                yield return new WaitForFixedUpdate();
            } while (Vector3.Distance(pos, transform.position) > 0.000001);

            if (deactivateOnEnd && totem)
            {
                totem.Lock(false);
                totem.Deactivate();
            }
        }
        
        private void OnMouseEnter()
        {
            
        }

        public void OnSelected()
        {
            if(_state != State.Dragging)
            {
                GetComponentInParent<Totem>().Separate(transform.GetSiblingIndex()+1);
                _draggingPlane = new Vector3(0, transform.position.y, transform.position.z + 0.5f);
            }
        }
        
        

        
        private IEnumerator Staadfdsa()
        {
            yield return new WaitForSeconds(0.5f);
            _state = State.Normal;
        }
        
        // private void OnMouseDrag()
        // {
        //     DragHandler();
        // }

        private Vector3 CalculatePosition()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var z = _draggingPlane.z;
            var y = _draggingPlane.y;
            var t = (z - ray.origin.z) / ray.direction.z;
            var pos = ray.origin + t * ray.direction;
            Debug.Log("Dragging to: " + pos);
            // var pos = new Vector3(t*ray.direction.x + ray.origin.x, _draggingPlane.y, z);
            return pos;
        }
        
        private void DragTo(Vector3 pos)
        {
            if(_state == State.Moving)
                return;
            transform.position = pos;
            _state = State.Dragging;
        }

        
        
        public void OnDrag(PointerEventData eventData)
        {
            DragHandler();
        }

        private void DragHandler()
        {
            _state = State.Dragging;
                        
            DragTo(CalculatePosition());
        }
        
        
        // public void OnEndDrag(PointerEventData eventData)
        // {
        //     MoveTo(new Vector3(10,10,10), 0.01f);
        // }

        // private void OnMouseUp()
        // {
        //     if(_state != State.Dragging)    return;
        //     
        //     Debug.Log("Holinchiss");
        //     _state = State.Normal;
        //     var pos = transform.parent.localToWorldMatrix.MultiplyPoint(Vector3.zero);
        //     MoveTo(pos, 0.01f, true);
        // }

        private void PointerPressHandler()
        {
            if(_state == State.Moving)   return;
            
            totem.Lock(true);
            Debug.Log("PointDown");
            if(_state == State.Dragging)    return;

            _draggingPlane = new Vector3(1, transform.position.y, transform.position.z + 0.5f);
            _state = State.Dragging;
        }
        
        // public void OnMouseDown()
        // {
        //     PointerPressHandler();
        // }
        //
        // public void OnPointerDown(PointerEventData eventData)
        // {
        //     PointerPressHandler();
        // }
    }
}