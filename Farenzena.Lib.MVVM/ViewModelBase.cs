using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Farenzena.Lib.MVVM
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
        {
            PropertyValues = new Dictionary<string, object>();
            PropertyTypes = new Dictionary<string, Type>();
        }

        public IDictionary<string, object> PropertyValues { get; set; }

        public IDictionary<string, Type> PropertyTypes { get; set; }

        protected T GetPropertyValue<T>(string propertyName)
        {
            return (T)PropertyValues[propertyName];
        }

        protected void SetPropertyValue<T>(string propertyName, T value)
        {
            if (!Compare(value, PropertyValues[propertyName]))//value.Equals(PropertyValues[propertyName]))
            {
                PropertyValues[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
        }

        bool Compare(object obj1, object obj2)
        {
            return obj1 == obj2;
        }

        public object GetPropertyValue(string propertyName)
        {
            return PropertyValues[propertyName];
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            if (PropertyTypes[propertyName] != typeof(object) && value.GetType() != PropertyTypes[propertyName])
                throw new InvalidOperationException("Wrong Type");

            if (!value.Equals(PropertyValues[propertyName]))
            {
                PropertyValues[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected void CreateProperty<T>(string propertyName)
        {
            PropertyTypes[propertyName] = typeof(T);
            PropertyValues[propertyName] = default(T);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
