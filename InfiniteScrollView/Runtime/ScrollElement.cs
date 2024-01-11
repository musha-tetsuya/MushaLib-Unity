using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.InfiniteScrollView
{
    /// <summary>
    /// �X�N���[���v�f���
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ScrollElement : MonoBehaviour, IScrollElement
    {
        /// <summary>
        /// RectTransform
        /// </summary>
        private RectTransform m_RectTransform;

        /// <summary>
        /// RectTransform
        /// </summary>
        RectTransform IScrollElement.RectTransform => m_RectTransform ?? (m_RectTransform = transform as RectTransform);

        /// <summary>
        /// Viewport�����[�J�����W�i�����j
        /// </summary>
        Vector2 IScrollElement.LocalPosition { get; set; }

        /// <summary>
        /// ���Ԗڂ̗v�f��
        /// </summary>
        int IScrollElement.Index { get; set; }

        /// <summary>
        /// ����ڂ̗v�f��
        /// </summary>
        int IScrollElement.Column { get; set; }

        /// <summary>
        /// ���s�ڂ̗v�f��
        /// </summary>
        int IScrollElement.Row { get; set; }

        /// <summary>
        /// �y�[�W���ŉ��Ԗڂ̗v�f��
        /// </summary>
        int IScrollElement.LocalIndex { get; set; }

        /// <summary>
        /// �y�[�W���ŉ���ڂ̗v�f��
        /// </summary>
        int IScrollElement.LocalColumn { get; set; }

        /// <summary>
        /// �y�[�W���ŉ��s�ڂ̗v�f��
        /// </summary>
        int IScrollElement.LocalRow { get; set; }

        /// <summary>
        /// ���Ԗڂ̃y�[�W��
        /// </summary>
        int IScrollElement.PageIndex { get; set; }

        /// <summary>
        /// ����ڂ̃y�[�W��
        /// </summary>
        int IScrollElement.PageColumn { get; set; }

        /// <summary>
        /// ���s�ڂ̃y�[�W��
        /// </summary>
        int IScrollElement.PageRow { get; set; }
    }
}
