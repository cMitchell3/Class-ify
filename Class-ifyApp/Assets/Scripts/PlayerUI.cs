using UnityEngine;
using TMPro;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Public Fields

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        public TextMeshProUGUI playerNameText;

        #endregion

        #region Private Fields

        [Tooltip("Pixel offset from the player target")]
        private Vector3 screenOffset = new Vector3(0f, 50f, -40f);
        private PlayerController target;
        Transform targetTransform;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                this.transform.SetParent(canvas.transform, false);
                this.transform.SetAsFirstSibling();
            }
            
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }


        void Update()
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        void LateUpdate()
        {
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
                this.transform.position = screenPoint;
            }
        }


        #endregion

        #region Public Methods

        public void SetTarget(PlayerController _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> PlayerController target for PlayerUI.SetTarget.", this);
                return;
            }

            this.target = _target;
            targetTransform = this.target.GetComponent<Transform>();

            if (playerNameText != null)
            {
                playerNameText.text = this.target.photonView.Owner.NickName;
            }
        }

        #endregion
    }
}
