using GalaSoft.MvvmLight;

namespace Ast.ViewModels
{
    public class PriChannelViewModel : ViewModelBase
    {
        private string _idle;
        private string _callLevel;
        private string _priCall;
        private string _channel;

        public string SpanId { get; set; }

        public string ChanId { get; set; }

        public string ChanB { get; set; }

        public string Idle
        {
            get => _idle;
            set { _idle = value; RaisePropertyChanged(); }
        }

        public string CallLevel
        {
            get => _callLevel;
            set { _callLevel = value; RaisePropertyChanged(); }
        }

        public string PriCall
        {
            get => _priCall;
            set { _priCall = value; RaisePropertyChanged(); }
        }

        public string Channel
        {
            get => _channel;
            set { _channel = value; RaisePropertyChanged(); }
        }
    }
}
