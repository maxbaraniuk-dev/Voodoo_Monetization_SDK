using UnityEngine;

namespace Runtime.PublicAPI.Internal
{
    public class CloseButtonDrawer : MonoBehaviour
    {
        [Header("Appearance")]
        public Vector2 buttonSize = new(100, 36);
        public float padding = 10f;
        public string label = "Close";
        public int fontSize = 14;
        
        public System.Action onClose;

        private GUIStyle _buttonStyle;
        private Texture2D _normalTex;
        private Texture2D _hoverTex;

        void OnGUI()
        {
            if (_buttonStyle == null)
            {
                CreateStyles();
            }

            Rect safe = Screen.safeArea;
            float topInset = Screen.height - (safe.y + safe.height); // pixels from top edge that are unsafe
            
            float x = Screen.width - buttonSize.x - padding;
            float y = padding + topInset;
            Rect r = new Rect(x, y, buttonSize.x, buttonSize.y);

            if (GUI.Button(r, label, _buttonStyle))
                HandleClose();
        }

        void HandleClose()
        {
            Debug.Log("Close button pressed");
            onClose?.Invoke();
        }

        void CreateStyles()
        {
            _normalTex = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 0.9f));
            _hoverTex  = MakeTex(2, 2, new Color(0.30f, 0.30f, 0.30f, 0.95f));

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                normal =
                {
                    background = _normalTex,
                    textColor = Color.white
                },
                hover =
                {
                    background = _hoverTex
                },
                padding = new RectOffset(6,6,4,4)
            };
        }
        
        Texture2D MakeTex(int width, int height, Color col)
        {
            Texture2D t = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] cols = new Color[width * height];
            for (int i = 0; i < cols.Length; i++) cols[i] = col;
            t.SetPixels(cols);
            t.Apply();
            t.hideFlags = HideFlags.DontSave;
            return t;
        }

        void OnDestroy()
        {
            if (_normalTex != null) DestroyImmediate(_normalTex);
            if (_hoverTex  != null) DestroyImmediate(_hoverTex);
        }
    }
}