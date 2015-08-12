using System.ComponentModel;

namespace SyncPaintBoard.Replicator
{
    public class AddMessage
    {
        public string ObjectId
        {
            get; set;
        }
        public INotifyPropertyChanged Object
        {
            get; set;
        }
    }
}