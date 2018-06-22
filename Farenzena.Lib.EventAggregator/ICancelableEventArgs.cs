namespace Farenzena.Lib.EventAggregator
{
    internal interface ICancelableEventArgs
    {
        bool Cancel { get; set; }
    }
}