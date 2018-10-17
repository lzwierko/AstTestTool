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
        private bool _isRunning;

        public TestRunner(SynchronizationContext sc, PriTestViewModel vm, AsteriskService ast)
        {
            _sc = sc;
            _vm = vm;
            _ast = ast;
            _logger = LogManager.GetLogger($"TestRunner.{_vm.Id}");
        }

        public void Start()
        {
            _timer = new Timer(Callback, null, 100, 2000);
        }

        public void Stop()
        {
            _timer.Dispose();
            _timer = null;
        }

        private void Callback(object state)
        {
            if (_isRunning) return;
            Task.Run(async () =>
            {
                try
                {
                    if (_isRunning) return;
                    _isRunning = true;

                    _logger.Trace("callback run");
                    var channels = (await _ast.PriGetChannels());
                    channels = channels.Where(a => a.SpanId == _vm.SpanId).ToList();
                    var inPriCallCnt = channels.Count(c => c.IsPriCall);
                    _logger.Trace($"in call before: {inPriCallCnt}");
                    var newCalls = 0;
                    if (inPriCallCnt < _vm.ChannelsCnt)
                    {
                        var diff = (_vm.ChannelsCnt - inPriCallCnt) / 2;
                        if (diff < 1)
                            diff = 1;
                        for (var i = 0; i < diff; i++)
                        {
                            _logger.Info($"make next call: {i}");
                            _ast.OriginateExt(_vm.Id, _vm.NumberToCall, _vm.Extension);
                            _logger.Info($"make next call: {i} completed");
                            newCalls++;
                        }
                    }

                    channels = (await _ast.PriGetChannels()).Where(a => a.SpanId == _vm.SpanId).ToList();
                    _logger.Trace($"in call after: {channels.Count(c => c.IsPriCall)}");
                    inPriCallCnt = channels.Count(c => c.IsPriCall);
                    _sc.Post(o =>
                    {
                        if (newCalls != 0)
                        {
                            _vm.TotalCalls += newCalls;
                        }

                        _vm.ChannelsUsedCnt = inPriCallCnt;
                    }, null);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                finally
                {
                    _logger.Trace("callback end");
                    _isRunning = false;
                }
            });
        }
    }
}