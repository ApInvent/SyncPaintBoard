using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using SyncPaintBoard.Transport;

namespace SyncPaintBoard.Replicator
{
    public class Replicator
    {
        private object _changedObject;
        private string _changedProperty;

        private readonly TransportBase _transport;
        protected readonly ObservableCollection<INotifyPropertyChanged> Collection;
        protected readonly Dictionary<INotifyPropertyChanged, string> Objects = new Dictionary<INotifyPropertyChanged, string>();

        public Replicator(ObservableCollection<INotifyPropertyChanged> collection, TransportBase transport)
        {
            Collection = collection;
            Collection.CollectionChanged += ObjectsCollectionChanged;
            _transport = transport;
            transport.Receive += TransportReceive;
        }

        private void TransportReceive(ReceiveData receiveData)
        {
            dynamic message = Json.Deserialize(receiveData.Data);
            Handle(message);
        }

        private void Handle(AddMessage message)
        {
            Add(message.Object, message.ObjectId);
            Collection.Add(message.Object);
        }

        private void Handle(ChangeMessage message)
        {
            var obj = Objects.First(kvp => kvp.Value == message.ObjectId).Key;
            var propertyInfo = obj.GetType().GetProperty(message.PropertyName);

            _changedObject = obj;
            _changedProperty = message.PropertyName;
            propertyInfo.SetValue(obj, message.PropertyValue);
            _changedObject = null;
            _changedProperty = null;
        }

        private void Handle(RemoveMessage removeMessage)
        {
            var objKey = Objects.FirstOrDefault(kvp => kvp.Value == removeMessage.ObjectId).Key;
            if (objKey == null || !Objects.ContainsKey(objKey))
                return;
            Objects.Remove(objKey);
            Collection.Remove(objKey);
        }

        protected void Add(INotifyPropertyChanged obj)
        {
            if (Objects.ContainsKey(obj))
                return;

            var objId = Guid.NewGuid().ToString();
            Add(obj, objId);
            var message = new AddMessage { ObjectId = objId, Object = obj };
            _transport.Send(Json.Serialize(message));
        }

        protected void Add(INotifyPropertyChanged obj, string objId)
        {
            Objects.Add(obj, objId);
            obj.PropertyChanged += (s,e) => PropertyChange((INotifyPropertyChanged)s, e.PropertyName);
        }

        private void PropertyChange(INotifyPropertyChanged obj, string propertyName)
        {
            if (obj == _changedObject && propertyName == _changedProperty)
                return;

            var propertyValue = obj.GetType().GetProperty(propertyName).GetValue(obj);
            var message = new ChangeMessage
            {
                ObjectId = Objects[obj],
                PropertyName = propertyName,
                PropertyValue = propertyValue,
            };

            _transport.Send(Json.Serialize(message));
        }

        public void Remove(INotifyPropertyChanged obj)
        {
            var message = new RemoveMessage { ObjectId = Objects[obj] };
            Objects.Remove(obj);
            _transport.Send(Json.Serialize(message));
        }

        protected void ObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var obj in e.NewItems.Cast<INotifyPropertyChanged>().Where(d => !Objects.ContainsKey(d)))
                        Add(obj);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var obj in e.OldItems.Cast<INotifyPropertyChanged>().Where(d=> Objects.ContainsKey(d)))
                        Remove(obj);
                    break;
            }
        }

        public void SendState(Action<string> transport)
        {
            foreach (var obj in Objects)
            {
                var message = new AddMessage { ObjectId = obj.Value,Object = obj.Key };
                transport(Json.Serialize(message));
            }
        }
    }
}
