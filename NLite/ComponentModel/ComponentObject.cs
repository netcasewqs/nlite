using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using NLite.Reflection;

namespace NLite.ComponentModel
{
    /// <summary>
    /// 
    /// </summary>
    #if !SILVERLIGHT
    [DefaultEvent("PropertyChanged")]
    public class ComponentObject : LocalizedObject, IEditableObject, IChangeTracking,INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, object> ordinalState = new Dictionary<string, object>();
        private Dictionary<string, object> beginEditOrdinalState = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public bool IsChanged { get; private set; }

        /// <summary>
        /// 是否在还原状态
        /// </summary>
        private bool IsRestore;

        /// <summary>
        /// Accept change
        /// </summary>
        public virtual void AcceptChanges()
        {
            IsChanged = false;
            ordinalState.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void RejectChanges()
        {
            IsChanged = false;
            Restore(ordinalState);
            Restore(beginEditOrdinalState);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        protected virtual void OnPropertyChanged(string propertyName, object newValue, object oldValue)
        {
            if (IsRestore)
                return;

            IsChanged = true;

            if (IsEdit)
            {
                if (!beginEditOrdinalState.ContainsKey(propertyName))
                    beginEditOrdinalState[propertyName] = oldValue;
            }
            else 
            {
                if (!ordinalState.ContainsKey(propertyName))
                    ordinalState[propertyName] = oldValue;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName, oldValue, newValue));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        protected T PopulateValue<T>(string propertyName,T newValue,T oldValue)
        {
            if (!object.Equals(newValue,oldValue))
            {
                OnPropertyChanged(propertyName, newValue, oldValue);
                return newValue;
            }

            return oldValue;
        }

        [XmlIgnore]
        [NonSerialized]
        private int _editLevel;

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public bool IsEdit
        {
            get { return _editLevel > 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void BeginEdit()
        {
            _editLevel++;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CancelEdit()
        {
            if (_editLevel > 0)
            {
                _editLevel--;
                if (!IsEdit)
                    Restore(beginEditOrdinalState);
            }
        }

        private void Restore(Dictionary<string,object> state)
        {
            lock (this)
            {
                if (state.Count == 0)
                    return;

                IsRestore = true;

                foreach (var item in state)
                    this.SetProperty(item.Key, item.Value);

                state.Clear();

                IsRestore = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void EndEdit()
        {
            if (_editLevel > 0)
            {
                _editLevel--;

                if (!IsEdit)
                {
                    foreach (var item in beginEditOrdinalState)
                    {
                        if (!ordinalState.ContainsKey(item.Key))
                            ordinalState.Add(item.Key, item.Value);
                    }

                    beginEditOrdinalState.Clear();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PropertyChanged = null;
                base.Dispose(disposing);
            }
        }

      
    }
#endif
}
