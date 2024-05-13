using System.Collections.Generic;

namespace BeABachelor.Networking.Interface
{
    public interface ISynchronizationController
    {
        void Register(MonoSynchronization monoSynchronization);
        public Dictionary<int, MonoSynchronization> MonoSynchronizations { get; }
    }
}