using MushaLib.UI.InfiniteScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.UI.DQ.MapEditor
{
    public class MapEditorElementView : ScrollElement
    {
        [SerializeField]
        private Image m_Image;

        [SerializeField]
        private Button m_Button;

        public Image Image => m_Image;

        public Button Button => m_Button;
    }
}
