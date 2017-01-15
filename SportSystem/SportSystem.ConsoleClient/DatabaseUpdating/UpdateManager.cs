namespace SportSystem.ConsoleClient.DatabaseUpdating
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Xml;

    class UpdateManager
    {
        private readonly int _coresCount;
        private readonly List<Thread> _threads;
        private readonly List<UpdateProcessor> _seedProcessors;

        public UpdateManager()
        {
            this._coresCount = Environment.ProcessorCount;
            this._threads = new List<Thread>(_coresCount);
            this._seedProcessors = new List<UpdateProcessor>(_coresCount);
        }

        public object UpdateData(XmlNodeList data, Type type)
        {
            var elementsPerCore = data.Count / _coresCount;
            var elementsLeftOver = data.Count % _coresCount;

            for (int i = 0; i < _coresCount; i++)
            {
                var startIndex = i * elementsPerCore;
                var elementsToProcessCount = elementsPerCore;

                if (i == _coresCount - 1)
                {
                    elementsToProcessCount += elementsLeftOver;
                }

                var seedProcessor = new UpdateProcessor(data, type, startIndex, elementsToProcessCount);
                _seedProcessors.Add(seedProcessor);

                var thread = new Thread(seedProcessor.UpdateData);
                _threads.Add(thread);
                thread.Start();
            }

            var dataToReturn = new List<object>();

            for (int i = 0; i < _threads.Count; i++)
            {
                _threads[i].Join();
                dataToReturn.AddRange(_seedProcessors[i].Data as IEnumerable<object>);
            }

            return dataToReturn;
        }
    }
}
