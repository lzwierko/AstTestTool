using System;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Ast
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _sever;
        private int _port;
        private string _user;
        private string _password;
        private PriTestViewModel _pri1;
        private PriTestViewModel _pri2;
        private PriTestViewModel _pri3;
        private PriTestViewModel _pri4;
        private bool _isBusy;
        private string _astStatus;

        #region view - server

        public string Sever
        {
            get => _sever;
            set { _sever = value; RaisePropertyChanged(); }
        }

        public int Port
        {
            get => _port;
            set { _port = value; RaisePropertyChanged(); }
        }

        public string User
        {
            get => _user;
            set { _user = value; RaisePropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; RaisePropertyChanged(); }
        }

        public string AstStatus
        {
            get => _astStatus;
            set { _astStatus = value; RaisePropertyChanged(); }
        }

        public ICommand ConnectCmd => new RelayCommand(() =>
        {
            Connect(this);
        });

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; RaisePropertyChanged(); }
        }

        #endregion

        #region model

        public Action<MainWindowViewModel> Connect { get; set; }

        public PriTestViewModel Pri1
        {
            get => _pri1;
            set { _pri1 = value; RaisePropertyChanged(); }
        }

        public PriTestViewModel Pri2
        {
            get => _pri2;
            set { _pri2 = value; RaisePropertyChanged(); }
        }

        public PriTestViewModel Pri3
        {
            get => _pri3;
            set { _pri3 = value; RaisePropertyChanged(); }
        }

        public PriTestViewModel Pri4
        {
            get => _pri4;
            set { _pri4 = value; RaisePropertyChanged(); }
        }

        public IEnumerable<PriTestViewModel> GetPris()
        {
            return new[] { Pri1, Pri2, Pri3, Pri4 };
        }

        #endregion
    }
}
