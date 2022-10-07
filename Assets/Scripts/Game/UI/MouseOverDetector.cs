﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EQx.Game.UI {
    public class MouseOverDetector : MonoBehaviour {

        [HideInInspector]
        public bool isMouseOver = false;

        private void OnEnable() {
            isMouseOver = false;
        }

        private void Update() {
            bool mo = IsOverThisElement(GetEventSystemRaycastResults());
            isMouseOver = mo;
        }

        private void OnDisable() {
            isMouseOver = false;
        }

        //Returns 'true' if we touched or hovering on Unity UI element.
        private bool IsOverThisElement(List<RaycastResult> eventSystemRaysastResults) {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject == gameObject)
                    return true;
            }
            return false;
        }


        //Gets all event system raycast results of current mouse or touch position.
        static List<RaycastResult> GetEventSystemRaycastResults() {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

    }
}
