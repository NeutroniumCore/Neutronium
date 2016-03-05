using System;
using System.Threading;

namespace MVVM.Cef.Glue.Tests.Infra
{
   public class FakeUIThread 
   {
        private Thread _Thread;
        private readonly Action _Start;
        private readonly Action _End;
        private readonly AutoResetEvent _StartingResetEvent;
        private readonly AutoResetEvent _ContinueResetEvent;

        public FakeUIThread(Action start, Action end) 
        {
            _StartingResetEvent = new AutoResetEvent(false);
            _ContinueResetEvent = new AutoResetEvent(false);
            _Start = start;
            _End = end;
        }

        public void Start() 
        {
            if (_Thread != null)
                return;

            _Thread = new Thread(Run) {
                Name = "Fake UI Thread",
                IsBackground = true
            };
            _Thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            _Thread.Start();

            _StartingResetEvent.WaitOne();
        }

        private void Run() 
        {
            _Start();
            _StartingResetEvent.Set();
            _ContinueResetEvent.WaitOne();
            _End();
        }

        public void Stop() 
        {
            var thread = _Thread;
            if (thread == null)
                return;

            _Thread = null;
            _ContinueResetEvent.Set();
            thread.Join();         
        }
    }
}
