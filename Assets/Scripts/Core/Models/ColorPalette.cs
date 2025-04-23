using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColonyDefender.Core
{    
    public enum ColorType
    {
        Background = 0,
        Primary = 1 ,
        Secondary = 2,
        Tertiary = 3,
        Quaternary = 4,
        Text = 5,
        Highlight = 6,
        Enemy = 7,
    }
    
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "ColonyDefender/Color Palette", order = 1)]
    public class ColorPalette : ScriptableObject
    {
        [Serializable]
        public class ColorItem
        {
            public ColorType type;
            public Color color;
        }

        [Header("Colors")]
        [SerializeField] private List<ColorItem> colors = new List<ColorItem>();

        // get a color by its type
        public Color GetColor(ColorType colorType)
        {
            ColorItem item = colors.Find(c => c.type == colorType);
            if (item != null)
                return item.color;
            
            Debug.LogWarning($"Color with type '{colorType}' not found in palette {name}");
            return Color.magenta;
        }

        // get color by index
        public Color GetColorByIndex(int index)
        {
            if (index >= 0 && index < colors.Count)
                return colors[index].color;
            
            Debug.LogWarning($"Color index {index} out of range in palette {name}");
            return Color.magenta;
        }

        // get all color types in the palette
        public IEnumerable<ColorType> GetColorTypes()
        {
            foreach (var item in colors)
            {
                yield return item.type;
            }
        }

        // check if palette contains a specific color type
        public bool HasColorType(ColorType colorType)
        {
            return colors.Exists(c => c.type == colorType);
        }

        // count of colors in palette
        public int Count => colors.Count;
    }
} 