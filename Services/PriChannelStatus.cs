namespace Services
{
    public class PriChannelStatus
    {
        public string SpanId { get; set; }

        public string ChanId { get; set; }

        public string ChanB { get; set; }

        public string Idle { get; set; }

        public string CallLevel { get; set; }

        public string PriCall { get; set; }

        public string Channel { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpanId)}: {SpanId}, {nameof(ChanId)}: {ChanId}, {nameof(Idle)}: {Idle}, {nameof(CallLevel)}: {CallLevel}, {nameof(PriCall)}: {PriCall}, {nameof(Channel)}: {Channel}";
        }
    }
}