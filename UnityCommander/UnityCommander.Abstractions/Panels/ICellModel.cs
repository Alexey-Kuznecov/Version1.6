namespace UnityCommander.Abstractions.Panels
{
    public interface ICellModel
    {
        IDictionary<string, object> Additional {  get; }

        IDictionary<string, int> DynamicColumnIndexes { get; }

        int OutputOrder { get; set; }
    }
}