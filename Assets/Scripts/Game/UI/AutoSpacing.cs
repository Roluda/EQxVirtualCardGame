using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.UI {
    public class AutoSpacing : MonoBehaviour {

        [SerializeField]
        float childSize = 32f;
        HorizontalLayoutGroup horlayoutGroup;
        VerticalLayoutGroup verlayoutGroup;

        public RectTransform lengthRectTransform;

        void Update() {
            if (transform.childCount > 0) {
                int dotCount = transform.childCount;
                if (horlayoutGroup) {
                    horlayoutGroup.spacing = (lengthRectTransform.rect.width - (dotCount * childSize)) / (dotCount - 1);
                }
                if (verlayoutGroup) {
                    verlayoutGroup.spacing = (lengthRectTransform.rect.height - (dotCount * childSize)) / (dotCount - 1);
                }
            }

        }

        void OnValidate() {
            horlayoutGroup = GetComponent<HorizontalLayoutGroup>();
            verlayoutGroup = GetComponent<VerticalLayoutGroup>();
            lengthRectTransform = GetComponent<RectTransform>();
        }

    }
}