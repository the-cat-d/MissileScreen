using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace MissileScreen.Source
{
    public static class UIDraw
    {
        public static UIElement lastElement;
        
        public abstract class UIElement
        {
            protected GameObject gameObject;
            protected RectTransform rectTransform;
            protected Image imageComponent;
            private string _name;

            protected UIElement(
                
                string newName,
                Transform uiParent = null

                )
            {

                if (uiParent != null)
                {
                    foreach (Transform child in uiParent)
                    {
                        if (child.name != newName) continue;

                        gameObject = child.gameObject;
                        rectTransform = gameObject.GetComponent<RectTransform>();
                        imageComponent = gameObject.GetComponent<Image>();
                        return;
                    }
                }
                // Create a new GameObject for the element
                gameObject = new GameObject(newName);
                gameObject.transform.SetParent(uiParent, false);
                rectTransform = gameObject.AddComponent<RectTransform>();
                
                _name = newName;

            }

            public void SetPosition(Vector2 position)
            {
                rectTransform.anchoredPosition = position;
            }

            public Vector2 GetPosition()
            {
                return rectTransform.anchoredPosition;
            }

            public virtual void SetColor(Color? color = null)
            {
                imageComponent.color = (color ?? imageComponent.color).WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);
            }

            public void SetActive(bool active)
            {
               
                if (gameObject == null) return;
                gameObject.SetActive(active);
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
            private TextMeshProUGUI _textComponent;
            
            public UILabel(
                string newName,
                Vector2 position,
                Transform uiParent = null,
                string text = "",
                int fontSize = 24,
                TextAlignmentOptions fontAlignment = TextAlignmentOptions.Left,
                Color? textColor = null,

                bool alignElement = true
                ) : base(newName, uiParent)
            {
                float backgroundOpacity = 0.8f * PluginConfig.hmdMissileScreenTransparency.Value;
                
                imageComponent = gameObject.AddComponent<Image>();
                
                rectTransform.anchoredPosition = position;
                rectTransform.sizeDelta = new Vector2(200, 40);

                imageComponent.color = new Color(0, 0, 0, backgroundOpacity);

                GameObject textObj = new GameObject("LabelText");

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
                textComp.color = (textColor ?? Color.white).WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);

                textComp.alignment = fontAlignment;
                textComp.text = text;
                textComp.overflowMode = TextOverflowModes.Overflow;
                
                rectTransform.sizeDelta = new Vector2(textComp.preferredWidth, textComp.fontSize);

                _textComponent = textComp;
                
                
                if (lastElement != null && alignElement)
                {
                    Plugin.logger.LogDebug($"current: {newName} last: {lastElement.GetGameObject().name}");
   

                    RectTransform targetElementRect = lastElement.GetRectTransform();

                    float alignedYPos = targetElementRect.anchoredPosition.y - targetElementRect.rect.height;
                    
                   
                    
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,alignedYPos);
                }
                lastElement = this;
                
            }

            public void SetAnchorPivot(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
            }

            public void SetText(string text)
            {
                if (_textComponent && rectTransform)
                {
                    _textComponent.text = text;
                    rectTransform.sizeDelta = new Vector2(_textComponent.preferredWidth, _textComponent.fontSize);
                }

            }

            public override void SetColor(Color? color = null)
            {
                if (!_textComponent) return;
                UpdateBackgroundTransparency();
                _textComponent.color = (color ?? _textComponent.color).WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);

            }

            private void UpdateBackgroundTransparency()
            {
                
                imageComponent.color = new Color(0,0,0,0.8f * (PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1));
            }
             
            public void SetFontSize(int size) // Unused
            {
                _textComponent.fontSize = size;
                rectTransform.sizeDelta = new Vector2(_textComponent.preferredWidth, _textComponent.preferredHeight);
            }

            public void SetTextAlignment(TextAlignmentOptions alignment) // Unused
            {
                _textComponent.alignment = alignment;
            }



            public Vector2 GetTextSize() // Unused
            {
                return new Vector2(_textComponent.preferredWidth, _textComponent.preferredHeight);
            }

        }

        public class UIText : UIElement
        {
            private TextMeshProUGUI _textComponent;
            
            public UIText(
                string newName,
                Transform uiParent = null,
                string text = "",
                Color? textColor = null,
                int fontSize = 24,
                float outlineThickness = 1,
                Color? outlineColor = null
                
                ) : base(newName, uiParent)
            {
                
                
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(200, 40);
                

                GameObject textObj = new GameObject("Text");

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
                textComp.color = (textColor ?? Color.white).WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);
                
                textComp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth,outlineThickness / 5f);
                textComp.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor,outlineColor ?? Color.black);
                textComp.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate,outlineThickness / 5f);

                Outline outline = gameObject.AddComponent<Outline>();
                outline.effectColor = outlineColor ?? Color.green;
                outline.effectDistance = new (outlineThickness, outlineThickness);
                
                
                textComp.alignment = TextAlignmentOptions.Center;
                textComp.text = text;
                textComp.overflowMode = TextOverflowModes.Overflow;
                
                rectTransform.sizeDelta = new Vector2(textComp.preferredWidth, textComp.fontSize);

                _textComponent = textComp;
                
                
               
                
            }

            public void SetAnchorPivot(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
            }

            public void SetText(string text)
            {
                if (_textComponent && rectTransform)
                {
                    _textComponent.text = text;
                    rectTransform.sizeDelta = new Vector2(_textComponent.preferredWidth, _textComponent.fontSize);
                }

            }

            public override void SetColor(Color? color = null)
            {
                if (!_textComponent) return;
                _textComponent.color = (color ?? _textComponent.color).WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);

            }

          
            public void SetFontSize(int size) // Unused
            {
                _textComponent.fontSize = size;
                rectTransform.sizeDelta = new Vector2(_textComponent.preferredWidth, _textComponent.preferredHeight);
            }

            public void SetTextAlignment(TextAlignmentOptions alignment) // Unused
            {
                _textComponent.alignment = alignment;
            }



            public Vector2 GetTextSize() // Unused
            {
                return new Vector2(_textComponent.preferredWidth, _textComponent.preferredHeight);
            }

        }
        
        public class UILadderRung(float degree, string newName, Transform uiParent = null, string text = "", Color? textColor = null, int fontSize = 24) : UIText(newName, uiParent, text, textColor, fontSize, PluginConfig.pitchIndicatorOutlineThickness.Value, PluginConfig.pitchIndicatorOutlineColor.Value)
        { 
            public readonly float degree = degree;

        }
        
        
        public class UIImage : UIElement
        {
            private Image _iconComponent;
            public UIImage(
                string newName,
                Transform uiParent,
                Sprite sprite,
                Color imageColor,
                Vector2? imageScale,
                Color? outlineColor,
                Vector2? outlineThickness,
                bool createOutline = false

                ) : base(newName, uiParent)
            {
                _iconComponent = gameObject.AddComponent<Image>();
                _iconComponent.transform.parent = uiParent;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = imageScale ?? new Vector2(1, 1);


                _iconComponent.sprite = sprite;
                _iconComponent.color = imageColor.WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);

                if (!createOutline) return;

                Outline imageOutline = gameObject.AddComponent<Outline>();
                imageOutline.effectColor = outlineColor ?? Color.white;
                imageOutline.effectDistance = outlineThickness ?? new(1, 1);


            }

            public override void SetColor(Color? newColor = null)
            {
                _iconComponent.color = (newColor ?? _iconComponent.color).WithAlpha(PluginConfig.hmdMissileScreen.Value ? PluginConfig.hmdMissileScreenTransparency.Value : 1);
            }
        }

        public static TMP_FontAsset GetDefaultFont()
        {
            TextMeshProUGUI weaponText = SceneSingleton<CombatHUD>.i.GetComponentInChildren<TextMeshProUGUI>();
            return weaponText.font;
        }
    }
}
