using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Ast
{
    public class PriTestViewModel : ViewModelBase
    {
        private int _channelsCnt;
        private bool _isTestStarted;
        private int _channelsUsedCnt;

        public string Id { get; set; }

        public int ChannelsCnt
        {
            get => _channelsCnt;
            set
            {
                if (value < 1) _channelsCnt = 1;
                else if (value > 30) _channelsCnt = 30;
                else _channelsCnt = value;
                RaisePropertyChanged();
            }
        }

        public string NumberToCall { get; set; }

        public string Extension{ get; set; }

        public ICommand StartCmd => new RelayCommand(() => Start(this));

        public ICommand StopCmd => new RelayCommand(() => Stop(this));

        public bool IsTestStarted
        {
            get => _isTestStarted;
            set { _isTestStarted = value; RaisePropertyChanged(); }
        }

        #region model

        public Action<PriTestViewModel> Start { get; set; }
        public Action<PriTestViewModel> Stop { get; set; }

        public int ChannelsUsedCnt
        {
            get => _channelsUsedCnt;
            set { _channelsUsedCnt = value; RaisePropertyChanged(); }
        }        

        #endregion
    }
}
