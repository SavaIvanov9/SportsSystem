namespace SportsSystem.Importer.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Xml;

    public class SeedManager
    {
        private readonly int _coresCount;
        private readonly List<Thread> _threads;
        private readonly List<SeedProcessor> _seedProcessors;

        public SeedManager()
        {
            this._coresCount = Environment.ProcessorCount;
            this._threads = new List<Thread>(_coresCount);
            this._seedProcessors = new List<SeedProcessor>(_coresCount);
        }

        public object SeedData(XmlNodeList data, Type type)
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

                var seedProcessor = new SeedProcessor(data, type, startIndex, elementsToProcessCount);
                _seedProcessors.Add(seedProcessor);

                var thread = new Thread(seedProcessor.SeedData);
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
