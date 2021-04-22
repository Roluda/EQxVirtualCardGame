using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Utility {
    public class MouseOverUtilities : MonoBehaviour {
        static Collider oldBuffer;
        static Collider buffer;

        public static bool MouseOver(Collider collider) {
            return First(buffer, collider);
        }

        public static bool MouseEnter(Collider collider) {
            return First(buffer, collider) && !First(oldBuffer, collider);
        }

        public static bool MouseExit(Collider collider) {
            return !First(buffer, collider) && First(oldBuffer, collider);
        }

        public static bool MouseDown(Collider collider) {
            return Input.GetMouseButtonDown(0) && MouseOver(collider);
        }

        public static bool MouseUp(Collider collider) {
            return Input.GetMouseButtonUp(0) && MouseOver(collider);
        }

        public static bool MouseDrag(Collider collider) {
            return Input.GetMouseButton(0) && MouseOver(collider);
        }

        void Update() {
            oldBuffer = buffer;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);
            buffer = hit.collider;
        }

        static bool First(Collider buffer, Collider collider) {
            return buffer == collider;
        }
    }
}
