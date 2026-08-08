using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MissileScreen.Source
{
    public static class UIDraw
    {
        public abstract class UIElement
        {
            protected GameObject gameObject;
            protected RectTransform rectTransform;
            protected Image imageComponent;

            protected UIElement(
                string name,
                Transform uiParent = null

                )
            {

                if (uiParent != null)
                {
                    foreach (Transform child in uiParent)
                    {
                        if (child.name == name)
                        {
                            gameObject = child.gameObject;
                            rectTransform = gameObject.GetComponent<RectTransform>();
                            imageComponent = gameObject.GetComponent<Image>();
                            return;
                        }
                    }
                }
                // Create a new GameObject for the element
                gameObject = new GameObject(name);
                gameObject.transform.SetParent(uiParent, false);
                rectTransform = gameObject.AddComponent<RectTransform>();

                return;
            }

            public virtual void SetPosition(Vector2 position)
            {
                rectTransform.anchoredPosition = position;
            }

            public virtual Vector2 GetPosition()
            {
                return rectTransform.anchoredPosition;
            }

            public virtual void SetColor(Color color)
            {
                imageComponent.color = color;
            }

            public void SetActive(bool active)
            {
                gameObject.gameObject.SetActive(active);
            }



            public GameObject GetGameObject() => gameObject;
            public RectTransform GetRectTransform() => rectTransform;
            public Image GetImageComponent() => imageComponent;

            public void Destroy()
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        public class UILabel : UIElement
        {
            private TextMeshProUGUI textComponent;
            private float backgroundOpacity;

            public UILabel(
                string name,
                Vector2 position,
                Transform uiParent = null,
                string text = "",
                int fontSize = 24,
                TextAlignmentOptions fontAlignment = TextAlignmentOptions.Left ,// .MiddleLeft
                Color? textColor = null

                ) : base(name, uiParent)
            {

                imageComponent = gameObject.AddComponent<Image>();
                this.backgroundOpacity = 0.8f;
                rectTransform.anchoredPosition = position;
                rectTransform.sizeDelta = new Vector2(200, 40);

                imageComponent.color = new Color(0, 0, 0, this.backgroundOpacity);

                GameObject textObj = new("LabelText");

                textObj.transform.SetParent(gameObject.transform, false);
                RectTransform textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                TextMeshProUGUI textComp = textObj.AddComponent<TextMeshProUGUI>();
                textComp.font = GetDefaultFont();
                textComp.fontSize = fontSize;
                textComp.fontStyle = FontStyles.Normal;
                textComp.color = textColor ?? Color.white;

                textComp.alignment = fontAlignment;
                textComp.text = text;
                textComp.overflowMode = TextOverflowModes.Overflow;
                
                rectTransform.sizeDelta = new Vector2(textComp.preferredWidth, textComp.fontSize);

                textComponent = textComp;


            }

            public void SetAnchorPivot(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
            }

            public void SetText(string text)
            {
                textComponent.text = text;
                rectTransform.sizeDelta = new Vector2(textComponent.preferredWidth, textComponent.fontSize);
            }

            public override void SetColor(Color color)
            {
                textComponent.color = color;

            }

            public void SetFontSize(int size)
            {
                textComponent.fontSize = size;
                rectTransform.sizeDelta = new Vector2(textComponent.preferredWidth, textComponent.preferredHeight);
            }

            public void SetTextAlignment(TextAlignmentOptions alignment)
            {
                textComponent.alignment = alignment;
            }



            public Vector2 GetTextSize()
            {
                return new Vector2(textComponent.preferredWidth, textComponent.preferredHeight);
            }

        }


        public class UIImage : UIElement
        {
            private Image _iconComponent;
            public UIImage(
                string name,
                Transform uiParent,
                Sprite sprite,
                Color imageColor,
                Vector2? imageScale,
                Color? outlineColor,
                Vector2? outlineThickness,
                bool createOutline = false

                ) : base(name, uiParent)
            {
                _iconComponent = gameObject.AddComponent<Image>();
                _iconComponent.transform.parent = uiParent;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = imageScale ?? new Vector2(1, 1);


                _iconComponent.sprite = sprite;
                _iconComponent.color = imageColor;

                if (!createOutline) return;

                Outline imageOutline = gameObject.AddComponent<Outline>();
                imageOutline.effectColor = outlineColor ?? Color.white;
                imageOutline.effectDistance = outlineThickness ?? new(1, 1);


            }

            public void ChangeImageColor(Color newColor)
            {
                _iconComponent.color = newColor;
            }
        }

        public static TMP_FontAsset GetDefaultFont()
        {
            TextMeshProUGUI weaponText = SceneSingleton<CombatHUD>.i.GetComponentInChildren<TextMeshProUGUI>();
            return weaponText.font;
        }
    }
}
