namespace Services
{
    public class CoreChannelStatus
    {
        public string Id { get; set; }

        public string Location { get; set; }

        public string State { get; set; }

        public string Data { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Location)}: {Location}, {nameof(State)}: {State}, {nameof(Data)}: {Data}";
        }
    }
}