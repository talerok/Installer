using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Common
{
    public class Iterator
    {
        private int _iterationsCount;
        private int _iterations;
        private Action _action;

        private static Mutex _mutexObj = new Mutex();

        private Iterator(Action action, int iterations)
        {
            _iterationsCount = iterations;
            _iterations = _iterationsCount;
            _action = action;
        }

        public void Do()
        {
            _mutexObj.WaitOne();
            if (_iterations == 0)
                return;
            _action();
            _iterations--;
            _mutexObj.ReleaseMutex();
        }

        public void Reset()
        {
            _mutexObj.WaitOne();
            _iterations = _iterationsCount;
            _mutexObj.ReleaseMutex();
        }

        public static class Factory
        {
            public static Iterator Create(Action action, int iterations)
            {
                return new Iterator(action, iterations);
            }
        }

    }
}
