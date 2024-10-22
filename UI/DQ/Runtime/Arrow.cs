using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ
{
    /// <summary>
    /// 矢印
    /// </summary>
    public class Arrow : MonoBehaviour
    {
        /// <summary>
        /// アニメーションタイプ
        /// </summary>
        public enum AnimationType
        {
            Hide,
            Show,
            Blink
        }

        /// <summary>
        /// AnimationTypeハッシュ値
        /// </summary>
        private static readonly int AnimationHash_AnimationType = Animator.StringToHash("AnimationType");

        /// <summary>
        /// アニメーター
        /// </summary>
        [SerializeField]
        private Animator m_Animator;

        /// <summary>
        /// 初期アニメーションタイプ
        /// </summary>
        [SerializeField]
        private AnimationType m_InitialAnimationType = AnimationType.Hide;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            m_Animator.SetInteger(AnimationHash_AnimationType, (int)m_InitialAnimationType);
        }

        /// <summary>
        /// アニメーションタイプ切り替え
        /// </summary>
        public void SetAnimationType(AnimationType animationType)
        {
            m_Animator.SetInteger(AnimationHash_AnimationType, (int)animationType);
        }
    }
}