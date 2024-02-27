using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace UniFramework.Localization
{
    public abstract class LocalizedBehaviour : MonoBehaviour
    {
        private Type _translationType;

        [SerializeField]
        [TableName]
        private string _dataTableName;

        [SerializeField]
        [TranslationKey]
        private string _translationKey;

        /// <summary>
        /// 数据表名称
        /// </summary>
        public string DataTableName
        {
            set
            {
                _dataTableName = value;
            }

            get
            {
                return _dataTableName;
            }
        }

        /// <summary>
        /// 翻译KEY
        /// </summary>
        public string TranslationKey
        {
            set
            {
                _translationKey = value;
            }

            get
            {
                return _translationKey;
            }
        }

        protected virtual void Awake()
        {
            var attribute = this.GetType().GetCustomAttribute<LocalizedBehaviourAttribute>();
            if (attribute == null)
                throw new Exception($"Not found {nameof(LocalizedBehaviourAttribute)} in class : {this.GetType().FullName}");
            _translationType = attribute.TranslationType;
        }
        protected virtual void OnEnable()
        {
            UniLocalization.OnLocalizationChanged += TranslationInternal;
            TranslationInternal();
        }
        protected virtual void OnDisable()
        {
            UniLocalization.OnLocalizationChanged -= TranslationInternal;
        }
        private void TranslationInternal()
        {
            var translation = UniLocalization.GetOrCreateTranslation(_translationType);
            if (translation != null)
            {
                OnTranslation(translation);
            }
        }

        /// <summary>
        /// 刷新组件
        /// </summary>
        public void Refresh()
        {
            TranslationInternal();
        }

        /// <summary>
        /// 执行翻译
        /// </summary>
        protected abstract void OnTranslation(ITranslation translation);

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (isActiveAndEnabled == true)
            {
                TranslationInternal();
            }
        }
#endif
    }
}