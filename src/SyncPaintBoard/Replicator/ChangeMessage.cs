namespace SyncPaintBoard.Replicator
{
    public class ChangeMessage
    {
        public string ObjectId { get;  set; }
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
    }
}