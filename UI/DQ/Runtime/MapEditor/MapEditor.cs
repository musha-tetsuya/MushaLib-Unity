using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor
{
    public class MapEditor : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int m_Size = new(16, 16);

        [SerializeField]
        private InfiniteScrollView.InfiniteScrollView m_ScrollView;
    }
}
