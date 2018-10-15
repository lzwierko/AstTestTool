using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Services;

namespace Ast
{
    public class TestRunner
    {
        private readonly SynchronizationContext _sc;
        private readonly PriTestViewModel _vm;
        private readonly AsteriskService _ast;
        private Timer _timer;
        private readonly Logger _logger;


        public TestRunner(SynchronizationContext sc, PriTestViewModel vm, AsteriskService ast)
        {
            _sc = sc;
            _vm = vm;
            _ast = ast;
            _logger = LogManager.GetLogger($"TestRunner.{_vm.Id}");
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
                    var channels = (await _ast.PriGetChannels()).Where(a => a.SpanId == _vm.Id).ToList();
                    _logger.Trace($"in call before: {channels.Count(c => c.IsIdle)}");

                    var inPriCallCnt = channels.Count(c => c.IsPriCall);
                    if (inPriCallCnt < _vm.ChannelsCnt)
                    {
                        var diff = (_vm.ChannelsCnt - inPriCallCnt) / 2;
                        if (diff < 1)
                            diff = 1;
                        for (var i = 0; i < diff; i++)
                        {
                            _logger.Info("make next call");
                            await _ast.OriginateExt(_vm.Id, _vm.NumberToCall, _vm.Extension);
                        }
                    }

                    channels = (await _ast.PriGetChannels()).Where(a => a.SpanId == _vm.Id).ToList();
                    _logger.Trace($"in call after: {channels.Count(c => c.IsIdle)}");
                    inPriCallCnt = channels.Count(c => c.IsPriCall);
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
