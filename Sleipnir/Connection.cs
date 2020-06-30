namespace Sleipnir
{
    public struct Connection
    {
#if UNITY_EDITOR
        public readonly Slot OutputSlot;
        public readonly Slot InputSlot;

        public Connection(Slot outputSlot, Slot inputSlot)
        {
            OutputSlot = outputSlot;
            InputSlot = inputSlot;
        }
#endif
    }
}