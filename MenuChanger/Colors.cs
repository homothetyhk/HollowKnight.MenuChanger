using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuChanger
{
    public static class Colors
    {
        public static readonly Color DEFAULT_COLOR = Color.white;
        public static readonly Color INVALID_INPUT_COLOR = Color.Lerp(Color.white, Color.red, 0.5f);
        public static readonly Color TRUE_COLOR = Color.Lerp(Color.white, Color.yellow, 0.5f);
        public static readonly Color FALSE_COLOR = Color.grey;
        public static readonly Color LOCKED_TRUE_COLOR = Color.Lerp(Color.grey, Color.yellow, 0.5f);
        public static readonly Color LOCKED_FALSE_COLOR = Color.Lerp(Color.grey, Color.black, 0.5f);
    }
}
