using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Services;

namespace Ast
{
    public class PriMonitor
    {
        private readonly SynchronizationContext _sc;
        private readonly PriTestViewModel _vm;
        private readonly AsteriskService _ast;
        private Timer _timer;
        private readonly Logger _logger;

        public PriMonitor(SynchronizationContext sc, PriTestViewModel vm, AsteriskService ast)
        {
            _sc = sc;
            _vm = vm;
            _ast = ast;
            _logger = LogManager.GetLogger($"PriMonitor.{_vm.Id}");
        }

        public void Start()
        {
            _timer = new Timer(Callback, null, 5000, 5000);
        }

        public void Stop()
        {
            _timer.Dispose();
            _timer = null;
        }

        private void Callback(object state)
        {
            Task.Run(async () =>
            {
                try
                {
                    _logger.Trace("callback");
                    var channels = (await _ast.PriGetChannels()).Where(a => a.SpanId == _vm.SpanId).ToList();
                    var priCallCnt = channels.Count(c => c.IsPriCall);
                    _logger.Trace($"in call: {priCallCnt}");
                    var inPriCallCnt = priCallCnt;
                    _sc.Post(o => { _vm.ChannelsUsedCnt = inPriCallCnt; }, null);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            });


        }
    }
}