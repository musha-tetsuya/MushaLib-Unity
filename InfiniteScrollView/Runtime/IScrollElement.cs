using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.InfiniteScrollView
{
    /// <summary>
    /// �X�N���[���v�f�C���^�[�t�F�[�X
    /// </summary>
    public interface IScrollElement
    {
        /// <summary>
        /// RectTransform
        /// </summary>
        RectTransform RectTransform { get; }

        /// <summary>
        /// Viewport�����[�J�����W�i�����j
        /// </summary>
        Vector2 LocalPosition { get; set; }

        /// <summary>
        /// ���Ԗڂ̗v�f��
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// ����ڂ̗v�f��
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// ���s�ڂ̗v�f��
        /// </summary>
        int Row { get; set; }

        /// <summary>
        /// �y�[�W���ŉ��Ԗڂ̗v�f��
        /// </summary>
        int LocalIndex { get; set; }
        
        /// <summary>
        /// �y�[�W���ŉ���ڂ̗v�f��
        /// </summary>
        int LocalColumn { get; set; }

        /// <summary>
        /// �y�[�W���ŉ��s�ڂ̗v�f��
        /// </summary>
        int LocalRow { get; set; }

        /// <summary>
        /// ���Ԗڂ̃y�[�W��
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// ����ڂ̃y�[�W��
        /// </summary>
        int PageColumn { get; set; }

        /// <summary>
        /// ���s�ڂ̃y�[�W��
        /// </summary>
        int PageRow { get; set; }
    }
}