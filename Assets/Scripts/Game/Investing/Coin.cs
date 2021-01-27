using UnityEngine;

namespace EQx.Game.Investing {
    public class Coin : MonoBehaviour {
        [SerializeField]
        Rigidbody attachedRigidbody = default;
        [SerializeField]
        BoxCollider attachedCollider = default;
        [SerializeField]
        Renderer coinRenderer = default;
        [SerializeField]
        Color highlightColor;

        [SerializeField]
        Vector3 movingPlaneNormal = Vector3.forward;

        public bool highlighted;

        Color startingColor;
        public bool dragging = false;
        Plane movingPlane = default;

        public float height => attachedCollider.size.y * transform.localScale.y;
        public float radius => attachedCollider.size.x / 2 * transform.localScale.x;
        public bool isKinematic => attachedRigidbody.isKinematic;

        Material coinMaterial;

        // Start is called before the first frame update
        void Start() {
            if (coinRenderer) {
                coinMaterial = coinRenderer.material;
                startingColor = coinMaterial.color;
            }
        }

        // Update is called once per frame
        void Update() {
            if (dragging) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                movingPlane.Raycast(ray, out float enter);
                transform.position = ray.GetPoint(enter);
            }
        }

        private void OnMouseEnter() {
            if (coinMaterial) {
                coinMaterial.color = highlightColor;
                highlighted = true;
            }
        }

        private void OnMouseExit() {
            if (coinMaterial) {
                coinMaterial.color = startingColor;
                highlighted = false;
            }
        }

        private void OnMouseDown() {
            movingPlane = new Plane(movingPlaneNormal, transform.position);
            attachedRigidbody.isKinematic = true;
            dragging = true;
            highlighted = true;
        }

        private void OnMouseUp() {
            dragging = false;
            highlighted = false;
            Destroy(gameObject);
        }

        private void OnDestroy() {
            Destroy(coinMaterial);
        }
    }
}
