using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SheTaChong.Unity
{
    public interface IGridInput
    {
        Vector2Int position { get; }
        Vector2Int direction { get; }
        void Use();
    }
}
